using System;
using LAM = Levels.Directorship.LevelActivityMask;

namespace Levels.UI.Tutorials
{
    [Flags]
    public enum TutorialStep
    {
        TutorialMovement = LAM.TutorialMovement & LAM.TutorialSpecificsPart,
        TutorialThrowChip = LAM.TutorialThrowChip & LAM.TutorialSpecificsPart,
        TutorialTeleportToChip = LAM.TutorialTeleportToChip & LAM.TutorialSpecificsPart,
        TutorialUsePrimary = LAM.TutorialUsePrimary & LAM.TutorialSpecificsPart,
        TutorialUseSecondary = LAM.TutorialUseSecondary & LAM.TutorialSpecificsPart,
        TutorialChooseSabre = LAM.TutorialChooseSabre & LAM.TutorialSpecificsPart,
        TutorialChoosePistol = LAM.TutorialChoosePistol & LAM.TutorialSpecificsPart,
        TutorialChooseChaos = LAM.TutorialChooseChaos & LAM.TutorialSpecificsPart,
    }
}