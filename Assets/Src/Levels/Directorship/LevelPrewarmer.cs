using System;
using System.Collections;
using System.Linq;
using System.Threading;
using Levels.Visual;
using UnityEngine;

namespace Levels.Directorship
{
    public class LevelPrewarmer : MonoBehaviour, ILevelScenarioPlayer
    {
        public delegate void NotificationHandler(int stepsCount, int currentStep, Stage stage);

        public event NotificationHandler StepCompleted;

        [SerializeField] private float minimumLoadingTime;

        [Header("Shaders")]
        [SerializeField] private ShaderVariantCollection shaderVariants;

        [Header("Systems")]
        [SerializeField] private TracersManager tracers;
        [SerializeField] private BloodSpatterPool blood;

        private readonly CancellationTokenSource _cts = new();
        private static readonly WaitForFixedUpdate WaitForFixedUpdate = new();

        public IEnumerator GetRoutine()
        {
            Stage stage = 0;

            LevelDirector.ActivityMask = LevelActivityMask.Prewarm;

            var stepI = 0;
            var steps = Enumerable.Empty<Action>()
                .Append(() => stage = Stage.Tracers)
                .Concat(tracers.WarmUp())
                .Append(() => stage = Stage.BloodPools)
                .Concat(blood.WarmUp())
                .Append(() => stage = Stage.Shaders)
                .Append(shaderVariants.WarmUp)
                .ToList();

            var allStart = Time.realtimeSinceStartup;
            var budget = Time.fixedDeltaTime * 0.9;

            while (stepI < steps.Count)
            {
                for
                (
                    var start = Time.realtimeSinceStartup;
                    Time.realtimeSinceStartup - start < budget && stepI < steps.Count;
                )
                {
                    steps[stepI++].Invoke();
                    StepCompleted?.Invoke(steps.Count, stepI, stage);
                }

                yield return WaitForFixedUpdate;
            }

            var loadingTime = Time.realtimeSinceStartup - allStart;
            if (loadingTime < minimumLoadingTime)
            {
                yield return new WaitForSeconds(minimumLoadingTime - loadingTime);
            }
        }

        private void OnDestroy()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public enum Stage { Tracers, BloodPools, Shaders }
    }
}