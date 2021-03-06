# Last Used Weapons
Adds new shortcut to equip items you last held in your hands. Think 'Q' in CS. Also auto equips weapons when leaving water if they were forcibly holstered on entering it.

[Example Video](https://youtu.be/YzIDt9lUe7A)

## Installation (manual)
- You'll need to download BepInEx for Valheim﻿ if this your first Valheim mod.
- Place the dll file for this mod in **Valheim/BepInEx/plugins/** folder.

## General
- Adds new shortcut ("**T**" by default) which when pressed will holster current items in your hand and switch to last used ones. This works with both single handed and two handed weapons, as well as tools and any mix of all of them.
- Last used weapons will be automatically restored when getting out of water, if they were holstered by getting into it in the first place. This feature can be disabled in the config if you dont want it.

## Notes
- You can rebind the key in the config file (it will appear only after you run the game with the mod at least once): **Valheim/BepInEx/config/bakaSpaceman.LastUsedWeapons.cfg**.

- Or download [BepInEx Config Manger](https://github.com/BepInEx/BepInEx.ConfigurationManager/releases)﻿ plugin to be able to edit them in-game by pressing F1.

## Changelog
### 1.1.0
- Fixed a bug with not properly switching between tools