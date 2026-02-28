using System.Linq;
using Levels.Abilities.CommonImpacts;
using Levels.Core.Passives;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Core
{
    [CreateAssetMenu(fileName = "Entity", menuName = "Core/Entities/Player", order = 1)]
    public class Player : Entity
    {
        public Health Health { get; private set; }
        public TeleportChip TeleportChip { get; private set; }
        public ProjectilesParrying ProjectilesParrying { get; private set; }

        private bool _inited = false;

        [Inject] private IObjectResolver _resolver;
        [Inject] private PublishIntent<ImpactIntent> _publishParry;

        public override void Init()
        {
            base.Init();

            if (!_inited)
            {
                _inited = true;

                Health = (Health)Components.First(c => c is Health);
                TeleportChip = (TeleportChip)Components.First(c => c is TeleportChip);
                ProjectilesParrying = (ProjectilesParrying)Components.First(c => c is ProjectilesParrying);

                _resolver
                    .Resolve<IntentsImpacts.IntentsImpacts>()
                    .RegisterBroker(ProjectilesParrying.Handle, Owner.GetInstanceID());
            }

            ProjectilesParrying.AttackParried += OnParried;
        }

        private void OnParried(object _)
        {
            _publishParry(ImpactIntent.SelfCast(new CasterParriedEffect(Owner)));
        }

        public override void Dispose()
        {
            base.Dispose();

            ProjectilesParrying.AttackParried -= OnParried;
        }
    }

    public enum TeleportChipState { Ready, Thrown, OnGround }
}