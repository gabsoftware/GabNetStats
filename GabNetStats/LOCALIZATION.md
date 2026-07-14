# Adding a localization

Use a short, mechanical workflow. Avoid copying whole designer `.resx` files.

## UI resources

1. Pick the culture code, usually neutral: `de`, `es`, `ja`, etc.
2. Create these files by copying the matching French file, then replace values:
   - `strings/Res.<culture>.resx`
   - `Forms/FormAbout.<culture>.resx`
   - `Forms/FormStatsOverlay.<culture>.resx`
   - `Forms/FormSettings.<culture>.resx`
   - `Forms/FormMain.<culture>.resx`
   - `Forms/FormNetworkDetails.<culture>.resx`
3. Starting from `*.fr.resx` saves tokens and preserves the right skeleton/key set. After copying, keep only needed text entries:
   - `str_*` in `strings/Res.resx`
   - `*.Text`
   - `*.HeaderText`
   - `*.BalloonTipText`
   - `*.BalloonTipTitle`
   - `*.ToolTipText`
4. Do not copy designer metadata unless layout truly needs it:
   - no `.Size`, `.Location`, `.Icon`, `.Image`, `.Font`, `.TabIndex`, `.Margin`, `>>*`
   - exception: add a targeted `.Location` or `.Size` only when a translated label clips.
5. Keep intentional fallbacks absent:
   - URLs
   - app/version/branding-only text
   - numeric/default field values
   - `OK`
   - refresh glyphs
   - standalone technical labels like `MTU`, `IPv4`, `IPv6`
6. Keep protocol acronyms unchanged inside translated strings: `TCP`, `UDP`, `IP`, `DNS`, `ICMP`, `MAC`, etc.
7. Keep labels close to English length. Prefer short UI labels over literal prose.
   - Good: `Auto-ping:`, `Icons:`, `Copy selection`
   - Avoid: full sentences in small labels/buttons.
8. Language selector names should be autonyms. Keep `str_LanguageDefault` as `English (default)` in every locale.

## Labels that often clip

Check these manually in every new locale. Prefer short labels immediately:

- `Forms/FormSettings.*.resx`
  - `label5.Text` (`Icon set:`): use `Icons:` or equivalent.
  - `chkSettingsAutoPingEnabled.Text` (`Enable auto-ping this host:`): use `Auto-ping:`.
  - `label2.Text` / `label3.Text` (`Download` / `Upload`): use short inbound/outbound terms if needed.
  - `label7.Text` (`Ping every`): keep it very short.
  - `label1.Text` (`Refresh rate (ms):`): shorten or move `textBoxDuration.Location` right.
- `Forms/FormStatsOverlay.*.resx`
  - `chkAutoClose.Text` (`Auto close after:`): use a short phrase.
  - Dynamic overlay labels come from `strings/Res.*.resx`, not this form.
- `strings/Res.*.resx`
  - `str_RawReceptionSpeed`, `str_RawEmissionSpeed`, `str_AvgReceptionSpeed`, `str_AvgEmissionSpeed`: keep compact.
- `Forms/FormNetworkDetails.*.resx`
  - `label34.Text` (`Incoming packets with unknown protocol`): use `Unknown protocol` or equivalent.
  - TCP/UDP listener tab labels: keep short.
  - ICMP/router/reassembly/timeout labels: abbreviate where needed.
  - Port/address column headers: keep compact.

## License file

1. Add `licenses/License.<culture>.txt` encoded as UTF-8.
2. Translate the license text, but keep these tokens verbatim:
   - `[PRODUCTNAME]`
   - `[PRODUCTHOMEPAGE]`
   - `[CONTACTEMAIL]`
   - `License.txt`
3. No project-file edit is needed for the license file; the build picks up `licenses/License.*.txt` automatically.

## Checks

Run from the `GabNetStats` solution folder:

```powershell
dotnet build -c Release .\GabNetStats.sln
.\scripts\ValidateLocalization.ps1
```

Then confirm:

1. `bin\Release\...\<culture>\GabNetStats.resources.dll` exists.
2. `bin\Release\...\licenses\License.<culture>.txt` exists.
3. No license placeholders remain in the generated file.
4. Resource XML parses.
5. Key diff only shows intentional fallbacks.
6. Long-label scan finds no large expansions compared to English.

The validation script checks items 3-6 mechanically. By default, key drift is reported as warnings because some fallback omissions are intentional. Use `.\scripts\ValidateLocalization.ps1 -StrictKeyDrift` when adding a new language and you expect a complete key set.
