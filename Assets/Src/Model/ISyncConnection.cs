namespace Model
{
    public interface ISyncConnection
    {
        public bool IsReceiving();

        public bool IsPublishingInput();

        public bool IsPublishingUpdates();
    }
}