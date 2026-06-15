using System;

namespace Levels.Directorship
{
    [Flags]
    public enum LevelActivityMask
    {
        PartPhase = 0b0000_0000_0000_0011,

        Gameplay = 1 << 0,
        Tutorial = 1 << 1,
        Pause = 1 << 2,

        TutorialSpecificsPart = 0b0000_0011_1111_1000, // 3-9
        TutorialMovement = Tutorial | 1 << 3,
        TutorialChip = Tutorial | 1 << 4,
        TutorialWeaponPart = 0b0000_0011_1110_0000, // 5-9
        TutorialWeapon = Tutorial | TutorialWeaponPart,
        TutorialUsePrimary = Tutorial | 1 << 5,
        TutorialUseSecondary = Tutorial | 1 << 6,
        TutorialChooseSabre = Tutorial | 1 << 7,
        TutorialChoosePistol = Tutorial | 1 << 8,
        TutorialChooseChaos = Tutorial | 1 << 9,
    }
}