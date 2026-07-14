param(
    [string]$ProjectDir = (Join-Path $PSScriptRoot '..\GabNetStats'),
    [string]$Configuration = 'Release',
    [string]$TargetFramework = 'net10.0-windows10.0.26100.0',
    [switch]$StrictKeyDrift,
    [double]$LargeExpansionRatio = 1.6,
    [int]$LargeExpansionMinExtra = 12
)

$ErrorActionPreference = 'Stop'

function Get-ResxEntries {
    param([string]$Path)

    [xml]$xml = Get-Content -LiteralPath $Path -Raw -Encoding UTF8
    $entries = @{}

    foreach ($node in $xml.root.data) {
        $name = [string]$node.name

        if ([string]::IsNullOrWhiteSpace($name) -or $null -eq $node.value) {
            continue
        }

        $entries[$name] = [string]$node.value
    }

    return $entries
}

function Test-IsTranslatableKey {
    param([string]$Name)

    if ($Name.StartsWith('str_', [System.StringComparison]::Ordinal)) {
        return $true
    }

    return $Name -match '\.(Text|HeaderText|BalloonTipText|BalloonTipTitle|ToolTipText)$'
}

function Get-ResourceIdentity {
    param([string]$FullName)

    $name = [System.IO.Path]::GetFileNameWithoutExtension($FullName)
    $extension = [System.IO.Path]::GetExtension($FullName)

    if ($name -match '^(?<base>.+)\.(?<culture>[a-z]{2}(?:-[A-Za-z]+)?)$') {
        return [pscustomobject]@{
            BaseName = $Matches.base
            Culture = $Matches.culture
            Extension = $extension
        }
    }

    return [pscustomobject]@{
        BaseName = $name
        Culture = ''
        Extension = $extension
    }
}

function Get-RelativePath {
    param(
        [string]$BasePath,
        [string]$Path
    )

    $baseUri = [System.Uri]::new($BasePath.TrimEnd('\') + '\')
    $pathUri = [System.Uri]::new($Path)
    return [System.Uri]::UnescapeDataString($baseUri.MakeRelativeUri($pathUri).ToString()).Replace('/', '\')
}

$projectPath = Resolve-Path -LiteralPath $ProjectDir
$resxFiles = Get-ChildItem -LiteralPath $projectPath -Recurse -Filter '*.resx' |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }

$errors = [System.Collections.Generic.List[string]]::new()
$warnings = [System.Collections.Generic.List[string]]::new()

$resourcesByGroup = @{}

foreach ($file in $resxFiles) {
    try {
        $entries = Get-ResxEntries -Path $file.FullName
    }
    catch {
        $errors.Add("Resource XML parse failed: $($file.FullName) - $($_.Exception.Message)")
        continue
    }

    $identity = Get-ResourceIdentity -FullName $file.FullName
    $relativeDirectory = Get-RelativePath -BasePath $projectPath -Path $file.DirectoryName
    $groupKey = Join-Path $relativeDirectory $identity.BaseName

    if (-not $resourcesByGroup.ContainsKey($groupKey)) {
        $resourcesByGroup[$groupKey] = @{}
    }

    $resourcesByGroup[$groupKey][$identity.Culture] = [pscustomobject]@{
        File = $file.FullName
        Entries = $entries
    }
}

foreach ($groupKey in ($resourcesByGroup.Keys | Sort-Object)) {
    $group = $resourcesByGroup[$groupKey]

    if (-not $group.ContainsKey('')) {
        $warnings.Add("No neutral resource found for $groupKey")
        continue
    }

    $neutralEntries = $group[''].Entries
    $expectedKeys = @($neutralEntries.Keys | Where-Object { Test-IsTranslatableKey $_ } | Sort-Object)

    foreach ($culture in ($group.Keys | Where-Object { $_ -ne '' } | Sort-Object)) {
        $localized = $group[$culture]
        $localizedKeys = @($localized.Entries.Keys | Where-Object { Test-IsTranslatableKey $_ } | Sort-Object)
        $missingKeys = @($expectedKeys | Where-Object { -not $localized.Entries.ContainsKey($_) })
        $extraKeys = @($localizedKeys | Where-Object { -not $neutralEntries.ContainsKey($_) })

        if ($missingKeys.Count -gt 0) {
            $message = "$groupKey.$culture is missing $($missingKeys.Count) translatable key(s): $($missingKeys -join ', ')"

            if ($StrictKeyDrift) {
                $errors.Add($message)
            }
            else {
                $warnings.Add($message)
            }
        }

        if ($extraKeys.Count -gt 0) {
            $message = "$groupKey.$culture contains $($extraKeys.Count) unknown translatable key(s): $($extraKeys -join ', ')"

            if ($StrictKeyDrift) {
                $errors.Add($message)
            }
            else {
                $warnings.Add($message)
            }
        }

        foreach ($key in $localizedKeys) {
            if (-not $neutralEntries.ContainsKey($key)) {
                continue
            }

            $neutralValue = [string]$neutralEntries[$key]
            $localizedValue = [string]$localized.Entries[$key]

            if ([string]::IsNullOrEmpty($neutralValue)) {
                continue
            }

            $growth = $localizedValue.Length - $neutralValue.Length
            $ratio = $localizedValue.Length / [double]$neutralValue.Length

            if ($growth -ge $LargeExpansionMinExtra -and $ratio -ge $LargeExpansionRatio) {
                $warnings.Add("$groupKey.$culture $key is much longer than neutral text ($($neutralValue.Length) -> $($localizedValue.Length) chars)")
            }
        }
    }
}

$licenseSourceFiles = Get-ChildItem -LiteralPath (Join-Path $projectPath 'licenses') -Filter 'License.*.txt'
$licenseCultures = @{}

foreach ($file in $licenseSourceFiles) {
    $culture = $file.BaseName.Substring('License.'.Length)
    $licenseCultures[$culture] = $true
}

$resourceCultures = @{}

foreach ($group in $resourcesByGroup.Values) {
    foreach ($culture in $group.Keys) {
        if ($culture -ne '') {
            $resourceCultures[$culture] = $true
        }
    }
}

foreach ($culture in ($resourceCultures.Keys | Sort-Object)) {
    if (-not $licenseCultures.ContainsKey($culture)) {
        $warnings.Add("Missing license template for resource culture '$culture'")
    }
}

$outputLicenseDir = Join-Path $projectPath "bin\$Configuration\$TargetFramework\licenses"

if (Test-Path -LiteralPath $outputLicenseDir) {
    foreach ($file in (Get-ChildItem -LiteralPath $outputLicenseDir -Filter 'License.*.txt')) {
        $text = Get-Content -LiteralPath $file.FullName -Raw -Encoding UTF8

        foreach ($placeholder in @('[PRODUCTNAME]', '[PRODUCTHOMEPAGE]', '[CONTACTEMAIL]')) {
            if ($text.Contains($placeholder)) {
                $errors.Add("Generated license still contains $placeholder`: $($file.FullName)")
            }
        }
    }
}
else {
    $warnings.Add("Generated license directory not found. Build first to verify placeholders: $outputLicenseDir")
}

foreach ($warning in $warnings) {
    Write-Warning $warning
}

if ($errors.Count -gt 0) {
    foreach ($errorMessage in $errors) {
        Write-Error $errorMessage
    }

    exit 1
}

Write-Host "Localization validation completed with $($warnings.Count) warning(s) and 0 error(s)."
