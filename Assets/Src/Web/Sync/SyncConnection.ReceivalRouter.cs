using JetBrains.Annotations;
using Riptide;

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
    }
}