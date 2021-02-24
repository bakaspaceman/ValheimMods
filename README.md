# Better Stamina
## Features
### General
- [X] Increased default stamina regeneration. Amount configurable.
- [X] Being encumbered will not drain stamina, slow walking speed is bad enough.
- [ ] \(Optional) Allow walking if at 0 stamina and encumbered.
- [X] Building & repairing does not use stamina.
- [X] Remove stamina cost for using **Cultivator**.
- [X] Remove stamina cost for using a **Hoe**.
- [ ] Investigate **Wet** and **Cold** effects on stamina regeneration.

### Combat
- [X] Reduce dodge roll stamina cost by additional amount based on **JUMP** skill level. See below for details.
- [X] Blocking stamina cost reduced based on **BLOCK** skill level. See below for details.

### Skills
In addition to current benefits, all skills reduce stamina cost for the corresponding activities. All skill based reductions follow EaseOutSine interpolation curve instead of linear interpolation that the original game uses.

* Jump
  - [X] **25%** stamina cost reduction for dodging at max level.
  - [X] **25%** stamina cost reduction for jumping at max level.
* Blocking
  - [X] **25%** stamina cost reduction at max level.
* Sneak
  - [X] **25%** stamina cost reduction at max level.
* Weapons
  - [X] Weapon skills already affect attack stamina cost in vanilla game by **33%** at max level, so this mod only changes it to use EaseOutSine interpolation so the benefits are stronger at the lower end of the skill.
* Bows
  - [ ] Reduce stamina cost based on skill for holding the bow drawn
* Swimming
  - [ ] Change the interpolation type for stamina reduction
* Run
  - [ ] Change the interpolation type for the stamina reduction

## Conflicts
* This mod replaces `Attack.GetStaminaUsage()` function.
