# Contributing

Thanks for helping improve GabNetStats. Keep changes focused, easy to review, and consistent with the existing code style.

## Dependencies
GabTracker : https://github.com/gabsoftware/GabTracker
This is the .NET control of the graph. It does not track you! It is poorly named.

## Building

GabNetStats targets `net10.0-windows10.0.26100.0` so builds require the .NET 10 SDK with the Windows desktop workload and Windows SDK 10.0.26100 or newer. The project metadata keeps the supported Windows API baseline at Windows 7 (`6.1`) for both GabNetStats and GabTracker; the Windows 10 target platform version is the SDK/API reference version, not the declared minimum OS baseline.

### Clone the projects
* Clone this repository in one directory
* Clone the GabTracker repository at the same level, so that both cloned repositories are located in the same directory.

For example:

    C:\mydir\
      |- GabNetStats
      |- GabTracker

### Build from Visual Studio 2026
* Open the GabNetStats solution in Visual Studio 2026
* If it cannot find the GabTracker project, in the solution, add an existing project and choose the GabTracker project
* Generate the solution
* GabNetStats should build without errors.

### Build from command line
Alternatively, you can build GabNetStats from the command line:

```sh
cd C:\mydir\GabNetStats
dotnet build -c Release .\GabNetStats.sln
```

## Test

After building, run:

```sh
cd GabNetStats
dotnet test -c Release ./GabNetStats.sln --no-build
```

If the project has not been built yet, omit `--no-build`.

## Localization

When adding or updating translations, follow `GabNetStats/GabNetStats/LOCALIZATION.md`.

## Version Changes

The application version is defined in `GabNetStats/GabNetStats/GabNetStats.csproj`. When bumping it, update `AssemblyVersion`, `FileVersion`, and `Version` to the same value.

Leave `ApplicationVersion` unchanged unless ClickOnce publishing is intentionally being updated.

## Pull Requests

Before opening a pull request, build the solution, run the relevant tests, and briefly describe the user-facing change or bug fix.
