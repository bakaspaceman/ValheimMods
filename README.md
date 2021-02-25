# Better Stamina
## Features
### General
- [X] Increased default stamina regeneration. Amount configurable.
- [X] Being encumbered will not drain stamina, slow walking speed is bad enough.
- [ ] \(Optional) Allow walking if at 0 stamina and encumbered.
- [X] Building & repairing does not use stamina.
- [X] Remove stamina cost for using **Cultivator**.
- [X] Remove stamina cost for using a **Hoe**.

### Skills
In addition to current benefits, all skills reduce stamina cost for the corresponding activities. All skill based reductions follow EaseOutSine interpolation curve instead of linear interpolation that the original game uses.

* Jump
  - [X] **33%** stamina cost reduction for dodging at max level.
  - [X] **33%** stamina cost reduction for jumping at max level.
* Blocking
  - [X] **33%** stamina cost reduction at max level.
* Sneak
  - [X] **33%** stamina cost reduction at max level.
* Weapons
  - [X] Weapon skills already reduce stamina cost for weapon attacks in vanilla game by **33%** at max level, so this mod only changes it to use EaseOutSine interpolation so the benefits are stronger at the lower end of the skill.
* Bows
  - [X] **33%** stamina cost reduction based on skill for holding the bow drawn
* Swimming
  - [ ] Change the interpolation type for stamina reduction
* Run
  - [ ] Change the interpolation type for the stamina reduction

## Conflicts
* This mod replaces `Attack.GetStaminaUsage()` function.
* This mod uses Harmony transpiler patch to edit `Player.PlayerAttackInput()`.

# Wet & Cold
Changes health regeneration rates for **Wet** and **Cold** status effects to **-10%** and **-20%** respectively (original values are -25% and -50% respectively)
