namespace ActorModel.Infrastructure.Actors
{
    public interface IMailBox
    {
        int QueueLength { get;}
        ActorId ActorId { get; }
        float MessagesPerSecond { get;}
        int TotalMessagesProcessed { get; }
    }

    public class DummyMailBox : IMailBox
    {
        public int QueueLength
        {
            get { return 0; }
        }

        public ActorId ActorId
        {
            get { return ActorId.GenerateNew(); }
        }

        public float MessagesPerSecond
        {
            get { return 0; }
        }

        public int TotalMessagesProcessed
        {
            get { return 0; }
        }
    }
}