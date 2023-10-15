# Goopify
### A custom goop editor for Super Mario Sunshine!
Information about how goop works: http://smswiki.shoutwiki.com/wiki/Docs/Pollution

## Usage
### Setup
When you first open the program, it will prompt you for a SuperBMD exe path. You can get the most recent build of it [here](https://github.com/RenolY2/SuperBMD/releases). SuperBMD is used to convert the goop models directly to properly formated bmd models to streamline the goop creation process.

You will also be prompted to select __an extracted iso of Super Mario Sunshine__. This is used to get the textures and particles of the goop in the existing game, allowing you to use them as selectable visual options for the custom goop you create

### Creating new goop


### Adding custom goop visuals

#### Current features:
* Import a .col file to use as a reference for where to create goop layers
* Add, scale, and reposition boxes that are used to automatically create the goop layers that the game will use
* Change settings for the goop visuals and effect types, as well as the initial pollution map that will appear
* All the vanilla goop types are pre-selectable goop visuals that you can pick, with support for adding custom visuals to select from too
* Auto exports all the needed particles, models, textures, and animations directily intro your extracted level for streamlined goop creation!
* Extract the information from an existing ymp file, letting you view both the region heightmap images and the data in the ymp exported as a formated text file

#### Planned future features:
* Support for Goop LayerTypes for walls
* Save progress of goop maps you created with the tool, letting you load, edit, and export them again later
* Support for sink events
* Settings menu to change controls, goop model creation limits, and position snapping
* Visual preview of goop texture in the settings stage
* Guides and tutorials on how to use the tool
* Various other fixes to help make the program more simple and streamlined for usage



---
Big thanks to Yoshi2 (RenolY2) and Sunn for information about goop, as well as the great people working on the SMS Bin Editor that I used as a refrense for the OpenTK code

[Link to the SMS Bin Editor](https://github.com/AugsEU/Bin-editor-improvements)
