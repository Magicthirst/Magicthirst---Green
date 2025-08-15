using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Common;
using UnityEditor.Callbacks;
using UnityEngine;
using VContainer;

namespace Levels.Sync
{
    public abstract class SyncBehavior : MonoBehaviour
    {
        protected ISyncConnection Connection;
        protected SynchronizationContext MainThreadContext;

        [Inject]
        public void ObserveConnection(IConnectionEstablishedEventHolder eventHolder)
        {
            eventHolder.ConnectionEstablished += connection =>
            {
                Connection = connection;
                MainThreadContext.Post(_ => enabled = true, null);
            };
        }

        private void Awake()
        {
            enabled = false;
            MainThreadContext = SynchronizationContext.Current;
            OnAwake();
        }

        protected virtual void OnAwake() {}

        private void OnEnable()
        {
            OnEnableLocal();
            if (Connection != null)
            {
                OnEnableSync();
            }
        }

        protected virtual void OnEnableLocal() {}

        protected virtual void OnEnableSync() {}

        private void OnDisable()
        {
            OnDisableLocal();
            if (Connection != null)
            {
                OnDisableSync();
            }
        }

        protected virtual void OnDisableLocal() {}

        protected virtual void OnDisableSync() {}
    }

#if UNITY_EDITOR
    public static class ForbidLifecycleOverrides
    {
        private static readonly string[] ForbiddenMethods = { "Awake", "OnEnable", "OnDisable" };

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            var syncBehaviourType = typeof(SyncBehavior);
            var derivedClasses = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(ClassIsSyncBehaviourDerivative)
                .ToArray();

            var violatorsMethods = derivedClasses.SelectMany(clazz =>
                clazz.GetMethods((BindingFlags)(0 - 1))
                    .Where(MethodIsForbidden)
                    .Select(method => (violator: clazz, method))
            ).ToArray();

            foreach (var (violator, method) in violatorsMethods)
            {
                Debug.LogError(
                    $"Class {violator.FullName} overrides {method.Name} " +
                    $"which is forbidden for {syncBehaviourType.FullName} derivatives"
                );
            }

            return;

            bool ClassIsSyncBehaviourDerivative(Type type) =>
                type.IsClass
                && syncBehaviourType.IsAssignableFrom(type)
                && type != syncBehaviourType;

            bool MethodIsForbidden(MethodInfo method) =>
                ForbiddenMethods.Contains(method.Name);
        }
    }
#endif
}
