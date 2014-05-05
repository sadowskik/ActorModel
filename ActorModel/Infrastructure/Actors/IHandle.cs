namespace ActorModel.Infrastructure.Actors
{
    public interface IHandle
    {
        void Handle(Message message);
    }
}