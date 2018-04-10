# MWinTool
MWinTool is not a functional application
It's a platform that can load additional modules, and then modules provide features.

MWinTool is a Windows Form Application
It's work is load/manage modules, and provide a Global-Hotkey system.

A module just need to let MWinTool know:
What it need to show on menu, then it show up in the menu.
Which configuration it need to save, then it save as a unique ".ini" file.
Which string resource need to read, then it read-in from a unique ".txt" file.
Which function need a hotkey, then it show in hotkeys-settings.

And MWinTool is custom localization supported, it can extract module's string resource to a file
that can edit by users, or add new localization options to make a new localization supported easily.

It's very easy to load a module for MWinTool, just need to put module files into "modules" folder
(you can create one if it doesn't exists), then start up MWinTool, and then you should see the module's title
show up in the "Modules" menu, just click it and it will load well. Same you can click it again to unload a module.
