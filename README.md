# Orchard.ParkingData

RESTful Web API for the City of Santa Monica's Parking Data, implemented as an Orchard module.

## Development Notes

This module currently targets Orchard 1.9.2

This module has a dependency on the [CSM.WebApi](https://github.com/CityofSantaMonica/Orchard.WebApi) module.

The theme for the site can be found here [CSM.ParkingData.Theme](https://github.com/CityofSantaMonica/Orchard.ParkingData.Theme).

Tests can be found in the [Tests](https://github.com/CityofSantaMonica/Orchard.ParkingData/tree/master/Tests) subproject; NUnit is the testing framework.

To get a local copy running, some initial setup is required. This [Orchard development guide](https://github.com/thekaveman/orchard-dev/) provides an introduction
to a general project structure; the same procedure can be used to get setup for developing the Parking API site.

1. Clone the [Orchard source](https://github.com/OrchardCMS/Orchard) and checkout the 1.9.2 branch
2. Clone this repository (e.g. into a folder called `CSM.ParkingData`)
3. Clone the [CSM.WebApi](https://github.com/CityofSantaMonica/Orchard.WebApi) repository (e.g. into a folder called `CSM.WebApi`)
4. Clone the [CSM.ParkingData.Theme](https://github.com/CityofSantaMonica/Orchard.ParkingData.Theme) repository (e.g. into a folder called `CSM.ParkingData.Theme`)
5. Following the [dev guide on source code setup](https://github.com/thekaveman/orchard-dev/blob/master/Source-Control-Custom-Code.md#working-with-a-cloned-module-theme), create junctions
that point from inside `Modules` (or `Themes`) in the Orchard source, to the repositories you cloned in steps 2 - 4.
6. Following the [dev guide on solution setup](https://github.com/thekaveman/orchard-dev/blob/master/Developing-Debugging.md), add the modules (and theme) to your Orchard solution.
7. Rebuild the solution.

## Publishing Notes

1. Obtain the publish profile from Azure, add to the parent `Orchard.Web` project in the Orchard solution.
2. Ensure the publish profile is configured so that the File Publish Options *Remove extra files at destination* and *Exclude files from the App_Data folder* are checked.
3. Add the `<rewrite>` node from this module's 
[`Web.config`](https://github.com/CityofSantaMonica/Orchard.ParkingData/blob/master/Web.config#L42) to the `<system.webServer>` node in `Orchard.Web`'s `Web.config` file. 
4. Rebuild the site and ensure it runs locally (SqlCE setup will work for simple build/run testing)
5. Select the `Orchard.Web` project, and click `Publish Web` to deploy.
