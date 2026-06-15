using System;
using Levels.Abilities.ChaosArea;
using Levels.Abilities.HitScanShoot;
using Levels.Abilities.ParrySabre;
using Levels.UI.Tutorials;
using static Levels.Directorship.LevelActivityMask;

namespace Levels.Directorship
{
    public static class WeaponryMasks
    {
        public static bool IsPrimaryInvokable =>
            (LevelDirector.ActivityMask & Gameplay) != 0 ||
            (LevelDirector.ActivityMask & TutorialUsePrimary) == TutorialUsePrimary;
        public static bool IsSecondaryInvokable =>
            (LevelDirector.ActivityMask & Gameplay) != 0 ||
            (LevelDirector.ActivityMask & TutorialUseSecondary) == TutorialUseSecondary;

        public static bool IsPlayableNow(this Type abilityType)
        {
            return
                (LevelDirector.ActivityMask & Gameplay) != 0 ||

                #region tutorials

                abilityType == typeof(ParrySabreSwinger) &&
                (LevelDirector.ActivityMask & TutorialChooseSabre) == TutorialChooseSabre ||
                abilityType == typeof(HitScanShooter) &&
                (LevelDirector.ActivityMask & TutorialChoosePistol) == TutorialChoosePistol ||
                abilityType == typeof(InfuseAreaWithChaosCaster) &&
                (LevelDirector.ActivityMask & TutorialChooseChaos) == TutorialChooseChaos

                #endregion

                ;
        }

        public static bool IsPlayableNow(this TutorialStep step)
        {
            return (LevelActivityMask)step == LevelDirector.ActivityMask;
        }
    }
}