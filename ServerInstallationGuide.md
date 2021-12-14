# Installation

## Singleplayer / Local

1. Install the [Plugin Loader](https://steamcommunity.com/sharedfiles/filedetails/?id=2407984968)
2. Start the game
3. Open the Plugins menu
4. Enable the Ship Mass Fix plugin from the GitHub source
5. Click on Save, then Restart

## Dedicated Server

For the dedicated server, you simply download the plugin .dll file and copy it to a location you like.
If you have the configuration manager open, go to the plugins tab, press add and add the plugin .dll.
If that does nothing, please go to ``%appdata%/SpaceEngineersDedicated/'`` and search for the ``SpaceEngineers-Dedicated.cfg`` and follow the same steps listed in the Torch Server section below, to add the plugin to the file. The dedicated server program needs to be closed for this.

## Torch Server

For Torch it is different. You can either copy the configuration of a dedicated server, where you added the plugin into the Instance folder of your Torch server, or you can edit the ``SpaceEngineers-Dedicated.cfg`` file in the instance folder of torch and add 

`<string>Path\To\ShipMassFix.dll</string>`

into the `<Plugins></Plugins>` section of the configuration file. The Torch program and the server needs to be closed for it to apply.

## Additional libraries

To use the plugin, you may put the `0harmony.dll` file in the same folder as the plugin. This is not needed if you are using the PluginLoader.
The .dll file can currently be found [here](https://github.com/rtdian/ShipMassFix/releases/latest).
