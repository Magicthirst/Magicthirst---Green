using System.Collections;
using Levels.Abilities.HitScanShoot;
using Levels.Abilities.ParrySabre;
using Levels.Config;
using Levels.Core.Passives;
using Levels.IntentsImpacts;
using Levels.Tests.Util;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;

namespace Levels.Tests
{
    public class ProjectilesParryingTest
    {
        private ParrySabreConfig _parrySabreConfig;
        private ShootConfig _shootConfig;

        private IParryConfig _ParryConfig => _parrySabreConfig;

        private ProjectilesParrying _parrying;
        private readonly MockImpactConsumer<ParryImpact> _parries = new();

        private GameObject _shooter;
        private GameObject _victim;

        [SetUp]
        public void Init()
        {
            InitConfigs();

            _shooter = new GameObject("shooter");
            _victim = new GameObject("victim");

            var builder = new ContainerBuilder();
            builder.RegisterInstance<IImpactConsumer<ParryImpact>>(_parries);
            builder.RegisterInstance(_ParryConfig);
            builder.RegisterInstance(_victim);
            builder.RegisterInstance(_victim.AddComponent<StubMonoBehaviour>() as MonoBehaviour);

            using var container = builder.Build();

            _parrying = ScriptableObject.CreateInstance<ProjectilesParrying>();
            container.Inject(_parrying);
            _parrying.Init();
        }

        [TearDown]
        public void Cleanup()
        {
            _parrying.Dispose();

            Object.DestroyImmediate(_shooter);
            Object.DestroyImmediate(_victim);

            Object.DestroyImmediate(_parrying);
            Object.DestroyImmediate(_parrySabreConfig);
            Object.DestroyImmediate(_shootConfig);
        }

        [UnityTest]
        public IEnumerator JustAttack()
        {
            SetupPositions(out var intent, out _, out _);

            var impacts = new IImpact[] { };
            var passed = false;
            _parrying.Handle.Passed += _ => passed = true;

            Assert.IsTrue(_parrying.Handle.TryConsume(intent, impacts), "attack should be consumed");

            yield return RunParryFor(_ParryConfig.Leeway * 2);

            Assert.IsTrue(passed, "attack should pass");
        }

        [UnityTest]
        public IEnumerator AttackAfterParryExpired()
        {
            SetupPositions(out var intent, out var blockingParry, out _);

            var impacts = new IImpact[] { };
            var passed = false;
            _parrying.Handle.Passed += _ => passed = true;

            _parries.Receive(blockingParry);
            yield return RunParryFor(_ParryConfig.Duration * 2);

            Assert.IsTrue(_parrying.Handle.TryConsume(intent, impacts), "attack should be consumed");

            yield return RunParryFor(_ParryConfig.Leeway * 2);

            Assert.IsTrue(passed, "attack should pass");
        }

        [UnityTest]
        public IEnumerator AttackOutsideOfParrySector()
        {
            SetupPositions(out var intent, out _, out var missingParry);

            var impacts = new IImpact[] { };
            var passed = false;
            _parrying.Handle.Passed += _ => passed = true;

            _parries.Receive(missingParry);
            Assert.IsTrue(_parrying.Handle.TryConsume(intent, impacts), "attack should be consumed");

            yield return RunParryFor(_ParryConfig.Leeway * 2);

            Assert.IsTrue(passed, "attack should pass");
        }

        [UnityTest]
        public IEnumerator SuccessfulParry_AtInstantOfAttack()
        {
            SetupPositions(out var intent, out var blockingParry, out _);

            var impacts = new IImpact[] { };
            var passed = false;
            _parrying.Handle.Passed += _ => passed = true;

            Assert.IsTrue(_parrying.Handle.TryConsume(intent, impacts), "attack should be consumed");
            _parries.Receive(blockingParry);

            yield return RunParryFor(_ParryConfig.Leeway * 2);

            Assert.IsFalse(passed, "attack should not pass");
        }

        [UnityTest]
        public IEnumerator SuccessfulParry_BeforeAttack()
        {
            SetupPositions(out var intent, out var blockingParry, out _);

            var impacts = new IImpact[] { };
            var passed = false;
            _parrying.Handle.Passed += _ => passed = true;

            _parries.Receive(blockingParry);
            yield return RunParryFor(_ParryConfig.Duration / 2);

            Assert.IsTrue(_parrying.Handle.TryConsume(intent, impacts), "attack should be consumed");

            yield return RunParryFor(_ParryConfig.Leeway * 2);

            Assert.IsFalse(passed, "attack should not pass");
        }

        [UnityTest]
        public IEnumerator SuccessfulParry_InLeewayAfterAttack()
        {
            SetupPositions(out var intent, out var blockingParry, out _);

            var impacts = new IImpact[] { };
            var passed = false;
            _parrying.Handle.Passed += _ => passed = true;

            Assert.IsTrue(_parrying.Handle.TryConsume(intent, impacts), "attack should be consumed");

            yield return RunParryFor(_ParryConfig.Leeway * 0.5f);

            _parries.Receive(blockingParry);

            yield return RunParryFor(_ParryConfig.Leeway * 2);

            Assert.IsFalse(passed, "attack should not pass");
        }

        private void SetupPositions
        (
            out HitScanShootIntent intent,
            out ParryImpact blockingParry,
            out ParryImpact missingParry
        )
        {
            // 'V' -- Victim
            // 'S' -- Shooter
            // '-' is a shot direction
            // ')' is a blocking parry direction
            // 'x' is a missing parry direction
            //
            // x\z 0 1 2
            // 0
            // 1  xV) -S
            // 2
            _shooter.transform.position = new Vector3(1f, 0, 2f);
            _victim.transform.position = new Vector3(1f, 0, 0f);

            intent = new HitScanShootIntent(_shooter, _shooter.transform.position, new Vector3(0f, 0f, -1f), _shootConfig);
            blockingParry = new ParryImpact(_victim, new Vector3(0f, 0f, 1f));
            missingParry = new ParryImpact(_victim, new Vector3(0f, 0f, -1f));
        }

        private IEnumerator RunParryFor(float time)
        {
            for (var t = 0f; t < time; t += Time.fixedDeltaTime)
            {
                _parrying.FixedUpdate();
                yield return new WaitForFixedUpdate();
            }
        }

        private void InitConfigs()
        {
            _parrySabreConfig = ScriptableObject.CreateInstance<ParrySabreConfig>();
            _parrySabreConfig.leeway = 0.1f;
            _parrySabreConfig.duration = 0.2f;
            _parrySabreConfig.angleDegrees = 90f;

            _shootConfig = ScriptableObject.CreateInstance<ShootConfig>();
        }
    }
}
