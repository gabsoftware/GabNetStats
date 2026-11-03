# GabNetStats
GabNetStats is a handy swiss-knife tool for managing your network interfaces and showing and statistics.
Featuring an old-school but very missed blinking network status icon in the system tray, network traffic graph, and advanced network statistics.

## Features
* Adds an icon in the statusbar for a visual hint of network traffic
* Detailed statistics about network connections
* Context menu for easy access to network configuration
* Real-time network graph

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
  
    cd C:\mydir\GabNetStats
    dotnet build -c Release .\GabNetStats.sln

## License
Copyright (C) GabSoftware

GabNetStats is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. See [LICENSE](LICENSE) (LGPL-3.0) and [COPYING](COPYING) (GPL-3.0, which the LGPL builds on).

GabTracker is distributed under the same license.

## Code signing policy
Free code signing provided by [SignPath.io](https://about.signpath.io/), certificate by [SignPath Foundation](https://signpath.org/).

Release binaries are built from this repository by [GitHub Actions](.github/workflows/dotnet-desktop.yml) and signed through SignPath. Only binaries built from the source code of GabNetStats and [GabTracker](https://github.com/gabsoftware/GabTracker) are signed; third-party dependencies keep their original signatures.

Team roles:
- Committers and reviewers: [Gabriel Hautclocq](https://github.com/gabsoftware)
- Approvers: [Gabriel Hautclocq](https://github.com/gabsoftware)

## Privacy policy
This program will not transfer any information to other networked systems unless specifically requested by the user or the person installing or operating it.

GabNetStats has no telemetry, analytics or automatic update check. Its only network activity is the optional auto-ping feature, which is disabled by default; when enabled in the settings, it periodically pings a host that the user chooses. Links in the About dialog open in your web browser only when clicked.

## Many thanks to :
- Igor Tolmachev (www.itsamples.com) for the original idea
- Valerij Romanovskij (alias ext5 on GitHub) for the Russian translation
- erasmion (https://github.com/erasmion) for providing the erasmion icon set!
