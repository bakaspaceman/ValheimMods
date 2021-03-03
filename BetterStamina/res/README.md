# Better Stamina
This mod rebalances the stamina system to make it a little bit more forgiving and improve the combat flow while keeping to the original survival feeling. Skills will now reduce corresponding stamina costs.

## Installation (manual)
- You'll need to download BepInEx for Valheim﻿ if this your first Valheim mod.
- Place the dll file for this mod in **Valheim/BepInEx/plugins/** folder.

## General
- Increased default stamina regeneration rate by 50%.
- Reduced Rested status effect stamina regen rate from 100% to 50%. As is, this bonus is crucial to have, so this change is trying to make it less punishing to adventure without it. Value can be changed in the config.
- Having weapons equipped will now reduce stamina regen rate by 10% and increase Running stamina costs by 10%. Values can be changed in the config.
- Added config options to override Rested effects duration increase per comfort level, Wet status effects stamina regen rate and Cold satus effects stamina regen rate. These values are same as on vanilla by default, it's up to the user to adjust them if they want it. See notes section below on how to do it.
- Added an option to override stamina regen delay time in the config. It's same as vanilla by default (1 second).
- Being encumbered will not drain stamina, slow walking speed is bad enough.
- Removed stamina cost for building & repairing.
- Removed stamina cost for using a cultivator.
- Removed stamina cost for using a hoe.

## Skills
- In addition to current benefits, all skills will now reduce the stamina cost for their corresponding activities.
- The benefits will increase faster at the early and mid skill levels. Skill factor now uses sin out ease function instead of linear interpolation.

### Jump
- Added 33% stamina cost reduction at max level for dodging.
- Added 33% stamina cost reduction at max level for jumping.
### Blocking
- Added 33% stamina cost reduction at max level.
### Sneak
- Added 33% stamina cost reduction at max level.
### Bows
- Added 33% stamina cost reduction at max level for holding the bow drawn.
### Weapons
- Weapon skills already reduce stamina cost for weapon attacks in vanilla game by 33% at max level, so this mod only changes the benefit increase rate.
### Swimming
- Swimming skill already reduces stamina cost for swimming in vanilla game by roughly 60% at max level, so this mod only changes the benefit increase rate.
### Run
- Run skill already reduces stamina cost for running in vanilla game by roughly 50% at max level, so this mod only changes the benefit increase rate.

## Conflicts
The mod might have issues with any other mod that modifies stamina.

- Replaces `Attack.GetStaminaUsage()`.
- Edits Player.`PlayerAttackInput()` via transpiler patch.
- Edits `Player.CheckRun()` via transpiler patch.
- Edits `Player.OnSwiming()` via transpiler patch.

## Notes
- You can disable/enable certain features and tweak all the mentioned values in the config file (it will appear only after you run the game with the mod at least once): **Valheim/BepInEx/config/bakaSpaceman.BetterStamina.cfg**.

- Or download [BepInEx Config Manger](https://github.com/BepInEx/BepInEx.ConfigurationManager/releases)﻿ plugin to be able to edit them in-game by pressing F1.

## Change Log
### 1.2.0
- Reduced Rested stamina regen bonus from 100% to 50% and increased base stamina regen rate by 50%, to reduce the overall impact and importance of having Rested status effect.
- Added ability to override Cold, Wet and Rested status effect stamina regen rates via config file.
- Added config option to override Rested bonus duration increase per comfort level. See "RestedDurationIncreasePerConfortLevel" in the config.
- Added config option to override base stamina regen rate if player has weapons equipped. See "StaminaRegenRateModWithWeapons" in the config.
- Added config option to override Running stamina cost reduction at max skill level. "RunCostModifierAtMaxSkill" in the config.
- Added config option to override Running stamina cost separately if player has weapons equipped. "RunWithWeaponsCostModifierAtMaxSkill" in the config.
### 1.1.0
- Added NexusID
- Added StaminaRegenDelay variable to the config so users can change it if they want.