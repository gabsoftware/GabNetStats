# GabNetStats
GabNetStats is a handy swiss-knife tool for network administration and statistics.
Featuring an old-school but very missed blinking network status icon in the system tray, network traffic graph, and advanced network statistics.

## Features
* Adds an icon in the statusbar that reflects network traffic
* Detailed statistics about network connections
* Context menu for easy access to network configuration
* Real-time network graph

## Dependencies
GabTracker : https://github.com/gabsoftware/GabTracker

## Building
* Clone this repository in one directory
* Clone the GabTracker repository in the same directory, so that both repo are located in the same directory
* Open the GabNetStats solution in Visual Studio 2017
* If it cannot find the GabTracker project, in the solution, add an existing project and choose the GabTracker project
* Generate the solution
* GabNetStats should build without errors.

## Many thanks to :
- Igor Tolmachev (www.itsamples.com) for the original idea
- Valerij Romanovskij (alias ext5 on GitHub) for the Russian translation