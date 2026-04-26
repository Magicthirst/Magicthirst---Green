using Common;
using Levels.Abilities.KillAndDown;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels
{
    public class LevelDirector : MonoBehaviour
    {
        [SerializeField] private GameObject player;

        private IImpactConsumer<DownedImpact> _playerDied;
        private bool _playerIsDead = false;

        [Inject] private IGameNavigation _navigation;

        [Inject]
        public void Construct(IntentsImpacts.IntentsImpacts intentsImpacts)
        {
            _playerDied = intentsImpacts.GetImpactConsumerFor<DownedImpact>(player, null);
        }

        private void OnEnable()
        {
            _playerDied.Impacted += OnDead;
        }

        private void OnDisable()
        {
            _playerDied.Impacted -= OnDead;
        }

        private void OnDead(DownedImpact _)
        {
            if (_playerIsDead)
            {
                return;
            }

            _playerIsDead = true;
            _navigation.FailLevel();
        }
    }
}