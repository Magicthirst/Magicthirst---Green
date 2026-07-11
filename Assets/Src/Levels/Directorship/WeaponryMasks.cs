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
            var mask = LevelDirector.ActivityMask;
           
            return
                mask.HasFlag(Gameplay) || 
                mask.HasFlag(Tutorial) &&
                (
                    abilityType == typeof(ParrySabreSwinger) ? mask.HasFlag(TutorialChooseSabre) :
                    abilityType == typeof(HitScanShooter) ? mask.HasFlag(TutorialChoosePistol) :
                    abilityType == typeof(InfuseAreaWithChaosCaster) ? mask.HasFlag(TutorialChooseChaos) :
                    false
                )
            ;
        }

        public static bool IsPlayableNow(this TutorialStep step)
        {
            return (LevelActivityMask)step == LevelDirector.ActivityMask;
        }
    }
}