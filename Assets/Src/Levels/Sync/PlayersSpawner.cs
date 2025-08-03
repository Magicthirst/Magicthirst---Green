using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;
using VContainer;

namespace Levels.Sync
{
    public delegate GameObject InstantiateJoinedPlayer(int playerId, GameObject prefab);

    public class PlayersSpawner : SyncBehavior
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform playersParent;

        [Inject] private IObjectResolver _resolver;
        private IReinitSource _reinitSource;
        private InstantiateJoinedPlayer _instantiate;

        private readonly Dictionary<int, ObjectAndReceivers> _players = new();

        protected override void Awake()
        {
            base.Awake();

            playersParent ??= transform.parent;
        }

        private void OnEnable()
        {
            if (_resolver.TryResolve(out _reinitSource) &&
                _resolver.TryResolve(out _instantiate))
            {
                _reinitSource.Reinited += ReinitPlayers;
            }
            else
            {
                Debug.Log($"Not resolved {nameof(_reinitSource)}");
                Debug.Log($"Not resolved {nameof(_instantiate)}");
            }
        }

        private void ReinitPlayers(Dictionary<int, PlayerState> players)
        {
            // ReSharper disable once InconsistentNaming
            var _keys = _players.Keys;
            var keys = players.Keys;
            Debug.Log($"Reinit existing players={string.Join(",", _players)}");
            Debug.Log($"Reinit updated players={string.Join(",", players)}");

            foreach (var playerId in _keys.Except(keys))
            {
                _players[playerId].Object.SetActive(false);
            }

            foreach (var playerId in keys.Except(_keys))
            {
                var player = new ObjectAndReceivers(_instantiate(playerId, playerPrefab));
                player.Object.transform.SetParent(playersParent);
                _players[playerId] = player;
            }

            foreach (var playerId in _keys.Union(keys))
            {
                var player = _players[playerId];
                player.Object.SetActive(true);
                Debug.Log($"spawned playerId={playerId}");

                foreach (var receiver in player.Receivers)
                {
                    receiver.OnPlayerStateUpdated(players[playerId]);
                }
                Debug.Log($"look for this guy (playerId={playerId}) here: {player.Object.transform.position}");
            }
        }

        private void OnDisable()
        {
            if (_reinitSource != null)
            {
                _reinitSource.Reinited -= ReinitPlayers;
            }
        }

        private struct ObjectAndReceivers
        {
            public readonly GameObject Object;
            public readonly IPlayerStateUpdatedReceiver[] Receivers;

            public ObjectAndReceivers(GameObject o)
            {
                Object = o;
                Receivers = Object.GetComponents<IPlayerStateUpdatedReceiver>();
            }
        }
    }
}