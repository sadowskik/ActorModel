namespace ActorModel.Infrastructure.Actors
{
    public abstract class Message
    {
        public abstract ActorId DestinationActorId { get; }
    }
}