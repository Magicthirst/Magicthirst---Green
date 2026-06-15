using System;
using Levels.Directorship;

namespace Levels.UI.Tutorials
{
    [Flags]
    public enum TutorialStep
    {
        TutorialMovement = LevelActivityMask.TutorialMovement,
        TutorialChip = LevelActivityMask.TutorialChip,
        TutorialUsePrimary = LevelActivityMask.TutorialUsePrimary,
        TutorialUseSecondary = LevelActivityMask.TutorialUseSecondary,
        TutorialWeapon = LevelActivityMask.TutorialWeapon,
        TutorialChooseSabre = LevelActivityMask.TutorialChooseSabre,
        TutorialChoosePistol = LevelActivityMask.TutorialChoosePistol,
        TutorialChooseChaos = LevelActivityMask.TutorialChooseChaos,
    }
}