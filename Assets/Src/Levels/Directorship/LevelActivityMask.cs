using System;
using UnityEngine;
using LAM = Levels.Directorship.LevelActivityMask;

namespace Levels.Directorship
{
    [Flags]
    public enum LevelActivityMask
    {
        GlobalModsMask = 0b0000_0000_0000_1111,

        Gameplay = 1 << 0,
        Tutorial = 1 << 1,
        Pause = 1 << 2,
        Prewarm = 1 << 3,

        TutorialSpecificsPart = 0b1111_1111_1111_1000, // 3-15
        TutorialMovement = Tutorial | 1 << 3,
        TutorialThrowChip = Tutorial | 1 << 4,
        TutorialTeleportToChip = Tutorial | 1 << 5,
        TutorialWeaponPart = 0b1111_1111_1100_0000, // 6-15
        TutorialWeapon = Tutorial | TutorialWeaponPart,
        TutorialUsePrimary = Tutorial | 1 << 6,
        TutorialUseSecondary = Tutorial | 1 << 7,
        TutorialChooseSabre = Tutorial | 1 << 8,
        TutorialChoosePistol = Tutorial | 1 << 9,
        TutorialChooseChaos = Tutorial | 1 << 10,
    }

    [Flags]
    public enum EditorLevelActivityMask
    {
        Gameplay = LAM.Gameplay,
        Tutorial = LAM.Tutorial,
        Pause = LAM.Pause,
        Prewarm = LAM.Prewarm,

        [InspectorName("Tutorial/Movement")] TutorialMovement = LAM.TutorialMovement,
        [InspectorName("Tutorial/Chip")] TutorialThrowChip = LAM.TutorialThrowChip,
        [InspectorName("Tutorial/Chip")] TutorialTeleportToChip = LAM.TutorialTeleportToChip,

        [InspectorName("Tutorial/UsePrimary")] TutorialUsePrimary = LAM.TutorialUsePrimary,
        [InspectorName("Tutorial/UseSecondary")] TutorialUseSecondary = LAM.TutorialUseSecondary,
        [InspectorName("Tutorial/ChooseSabre")] TutorialChooseSabre = LAM.TutorialChooseSabre,
        [InspectorName("Tutorial/ChoosePistol")] TutorialChoosePistol = LAM.TutorialChoosePistol,
        [InspectorName("Tutorial/ChooseChaos")] TutorialChooseChaos = LAM.TutorialChooseChaos,
    }

    public static class LevelActivityMaskMembers
    {
        public static bool IsRunningDuring(this LAM self, LAM context)
        {
            if ((self & context & LAM.GlobalModsMask) == 0)
            {
                return false;
            }

            if (context.HasFlag(LAM.Tutorial))
            {
                if (!self.HasFlag(LAM.Tutorial))
                {
                    return false;
                }

                var hasSpecifics = (self & LAM.TutorialSpecificsPart) != 0;
                if (!hasSpecifics)
                {
                    return true;
                }

                return (self & context & LAM.TutorialSpecificsPart) != 0;
            }

            return (self & context) != 0;
        }
    }
}