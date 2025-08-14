using System;
using JetBrains.Annotations;
using Riptide;
using UnityEngine;

namespace Web.Sync
{
    public partial class SyncConnection
    {
        private void Route()
        {
            _client.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived([CanBeNull] object sender, MessageReceivedEventArgs args)
        {
            var fullMark = (MessageMark)args.MessageId;
            var mark = fullMark & MessageMark.FilterExtras;

            Debug.Log($"received mark: {(ushort)mark}");

            try
            {
                switch (mark)
                {
                    case MessageMark.Reinit:
                        ReceiveReinit(args.Message);
                        break;
                    case MessageMark.Movement:
                        ReceiveMovement(args.Message);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}