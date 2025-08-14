using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;
using VContainer;

namespace Levels.Sync
{
    public delegate GameObject InstantiateJoinedPlayer(int playerId, GameObject prefab);

    public class Reinit : SyncBehavior
    {
        [SerializeField] private GameObject selfPlayer;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform playersParent;

        [Inject] private IObjectResolver _resolver;
        private IReinitSource _reinitSource;
        private InstantiateJoinedPlayer _instantiate;

        private Dictionary<int, ObjectAndReceivers> _players;

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
                _players = new Dictionary<int, ObjectAndReceivers>
                {
                    [Connection.SelfId] = new(selfPlayer)
                };

                _reinitSource.Reinited += ReinitPlayersOnMainThread;
            }
            else
            {
                Debug.Log($"Not resolved {nameof(_reinitSource)}");
                Debug.Log($"Not resolved {nameof(_instantiate)}");
            }

            return;

            void ReinitPlayersOnMainThread(Dictionary<int, PlayerState> players) =>
                MainThreadContext.Post(_ => ReinitPlayers(players), null);
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

            var newPlayers = keys.Except(_keys).ToArray();
            Debug.Log($"Reinit add players={newPlayers}");
            foreach (var playerId in newPlayers)
            {
                Debug.Log($"spawned playerId={playerId}");
                var player = new ObjectAndReceivers(_instantiate(playerId, playerPrefab));
                player.Object.transform.SetParent(playersParent);
                _players[playerId] = player;
            }

            foreach (var playerId in _keys.Union(keys))
            {
                var player = _players[playerId];
                player.Object.SetActive(true);

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
            public readonly PlayerStateUpdatesReceiver[] Receivers;

            public ObjectAndReceivers(GameObject o)
            {
                Object = o;
                Receivers = Object.GetComponents<PlayerStateUpdatesReceiver>();
            }
        }
    }
}