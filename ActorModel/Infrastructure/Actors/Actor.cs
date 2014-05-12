using System;

namespace ActorModel.Infrastructure.Actors
{
    public abstract class Actor : IDisposable
    {
        protected Actor(ActorId id)
        {
            Id = id;
        }

        protected Actor(ActorId id, ActorsSystem system)
        {
            Id = id;
            System = system;
        }

        public ActorId Id { get; private set; }

        public ActorsSystem System { get; set; }

        public virtual void Handle(Message message)
        {
            ((dynamic) this).On((dynamic) message);
        }

        public virtual void Dispose()
        {
        }
    }
}