Practice with the guide https://github.com/newman55/unity-mod-manager/wiki/How-to-create-a-mod-for-unity-game
Example of using Harmony patch to add new content.
Load png file as icon.
Load 3D prefab from asset bundle.

Set “Copy Local” to False for references to prevent DLL files output to debug and release folders.
Set “Copy to Output Directory” to “Copy always” for info.json and assets.
Set Build>Advanced>Debugging information to None to disable pdb file output.
Set Build Events Post-build event command line, if the mod is installed in game folder, copy output to overwrite mod files in game folder. Compress output files to zip for release.