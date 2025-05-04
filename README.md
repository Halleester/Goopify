# Goopify
### A custom goop editor for Super Mario Sunshine!
Information about how goop works: http://smswiki.shoutwiki.com/wiki/Docs/Pollution

#### Features:
* Create Goopmaps for your levels
	* Import a .col file to use as a reference for where to create goop layers
	* Add, scale, and reposition boxes that are used to automatically create the goop layers that the game will use
	* Change settings for the goop visuals and effect types, as well as the initial pollution map that will appear
	* All the vanilla goop types are pre-selectable goop visuals that you can pick, with support for adding custom visuals to select from too
	* Auto exports all the needed particles, models, textures, and animations directily intro your extracted level for streamlined goop creation!
* Save/load your custom Goopmaps
	* Toolbar options and hotkeys to quickly save your progress to continue on later
	* Tracks edits and gives you warnings if you try to close with unsaved changes
	* All information about your goopmap is contained within it's .goo file, meaning you can share it with others for them to use too
* Extract information from ymap files
	* Export allows you to view both the region heightmap images and the data in the ymp exported (as a formated text file)

#### Planned future features:
* Support for wall LayerTypes
* Support for sink events
* Settings menu to change controls, goop model creation limits, and position snapping
* Guides and tutorials on how to use the tool

## Usage
### Setup
The tool is now bundled with [SuperBMD v2.4.7.1](http://https://github.com/RenolY2/SuperBMD/releases/tag/v2.4.7.1 "SuperBMD v2.4.7.1"), so the only setup step will be a prompt on first launch that will download the resources you need.

### Creating a new goop
To create goop for a level, you'll start by loading the collision of the map (map.col file). From there, you'll be able to add different goop "regions" over the map. You can reposition and resize these anywhere on the map. The best practice is to create multiple smaller regions to only cover what you need in goop rather than one large one with lots of unused space.

By default, all goop regions will try to snap their height to just below the lowest collision that mario would be able to walk on. You can toggle this off, but make sure that the region is just below the ground that you want the goop to appear on.
Once you're happy with the positioning, the "Cut Regions" button will generate the models for the goop to use and allow you to set up the properties of the goop (ex the visuals and the type of goop).

You can paint and erase where the goop shows up right from the editor, but for more precise controls you can download and upload the coverage image that the goop uses. Note that the image will always be black and white, with white being where goop will show up ingame.

Once you're done with this step, you can then click "Finish" to export your custom goop into a level. Note that you will have to select an extracted level folder (aka not a SZS file).

Congrats, you now have custom goop in your level! At any point you can save your custom goopmap using File->Save or by using the hotkey "Ctrl+S"

###Other Notes
<details>
<summary>Adding custom goop visuals</summary>
### Adding custom goop visuals
After setup, you will now have a "**GoopResources**" folder in the same folder as your exe. Each folder within this is a different visual type that you can select as a dropdown during the painting step of creating a goopmap. To create a brand new goop, the easiest way is to copy an existing folder and to replace the textures that display the goop. These would be:
- h_ma_rak.bti
- visualPreview.png
- The goop textures in the Bmd folder

For more information, below is a list of files that will likely be in a folder:

| File Name | Description | Required? |
| ------------ | ------------ | :------------: |
| h_ma_rak.bti | Image used on characters when they are covered in goop | Yes |
| ms_m_ashios.jpa | The "splat" particle for goop | Yes |
| ms_m_spinos.jpa | The "clean-off" particle for goop | Yes |
| ms_m_tokeos.jpa | The "sink-in" particle for goop | Yes |
| textureAnim.btk | Texture animation for the goop | No |
| visualPreview.png | Image that goopify uses for the preview model | Yes |
| Bmd folder | Assets that SuperBmd will use to build the model | Yes |
JPA files can be edited with [RiiStudio](https://github.com/riidefi/RiiStudio/releases "RiiStudio"), and the BTI file can be edited with [GCFT](https://github.com/LagoLunatic/GCFT/releases "GCFT")

Note that any other .jpa files in the folder are situational to their goop type, ex. ms_newfire_a and ms_newfire_b are used for goop that burns you
</details>

---
Big thanks to Yoshi2 and Sunn for help with information about goop, people helping with testing/feedback such as Portable Productions and MightyMang0o, as well as the great people working on the SMS Bin Editor that I used as a reference for the OpenTK code

[Link to the SMS Bin Editor](https://github.com/AugsEU/Bin-editor-improvements)
