# Better Stamina
## General
- [X] Increased default stamina regeneration. Amount configurable.
- [X] Being encumbered will not drain stamina, slow walking speed is bad enough.
- [ ] \(Optional) Allow walking if at 0 stamina and encumbered.
- [X] Building & repairing does not use stamina.
- [X] Remove stamina cost for using **Cultivator**.
- [X] Remove stamina cost for using a **Hoe**.
- [ ] Investigate **Wet** and **Cold** effects on stamina regeneration.

## Combat
- [X] Reduce dodge roll stamina cost by additional amount based on **JUMP** skill level. See below for details.
- [X] Blocking stamina cost reduced based on **BLOCK** skill level. See below for details.

## Skills
In addition to current benefits, all skills reduce stamina cost for the corresponding activities.

* Jump
  - [X] **25%** stamina cost reduction for dodging at max level. EaseOutSine interpolation.
  - [ ] Jumping cost reduction
* Blocking
  - [X] **25%** stamina cost reduction at max level. EaseOutSine interpolation.
