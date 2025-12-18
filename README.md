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

## Many thanks to :
- Igor Tolmachev (www.itsamples.com) for the original idea
- Valerij Romanovskij (alias ext5 on GitHub) for the Russian translation