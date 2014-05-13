using ActorModel.Infrastructure.Actors;

namespace Stress
{
    public class SendContent : Message
    {
        public string Content { get; private set; }
        public bool Failed { get; private set; }
        
        private readonly ActorId _destination;

        public SendContent(SendContent message, ActorId newDestination, bool failed = false)
            : this(message.Content, newDestination, failed)
        {
        }

        public SendContent(string content, ActorId destination, bool failed = false)
        {
            Content = content;
            Failed = failed;
            _destination = destination;
        }

        public override ActorId DestinationActorId
        {
            get { return _destination; }
        }
    }
}