using System;

namespace ActorModel.Infrastructure.Actors
{
    public abstract class Actor : IDisposable
    {
        protected Actor(ActorId id) : this(id, null, new DummyMailBox())
        {
        }

        protected Actor(ActorId id, ActorsSystem system) : this(id, system, new DummyMailBox())
        {
        }

        protected Actor(ActorId id, ActorsSystem system, IMailBox mailBox)
        {
            Id = id;
            System = system;
            MailBox = mailBox;
        }

        public ActorId Id { get; protected set; }

        public ActorsSystem System { get; protected set; }

        public virtual IMailBox MailBox { get; protected set; }

        public virtual void Handle(Message message)
        {
            ((dynamic) this).On((dynamic) message);
        }

        public virtual void Dispose()
        {
        }
    }
}