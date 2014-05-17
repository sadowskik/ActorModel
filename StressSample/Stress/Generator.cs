using System;
using ActorModel.Infrastructure.Actors;

namespace Stress
{
    public class Generator : Actor
    {
        public const int MessagesToBeSent = 5000;

        private int _messagesAlreadySent;
        
        public Generator(ActorId id, ActorsSystem system) : base(id, system)
        {
        }

        public void On(GenerateNextMessage _)
        {
            if (_messagesAlreadySent >= MessagesToBeSent)
                return;

            System.Send(new SendContent("testMessage", Addresses.TcpWritersDispatcher));
            _messagesAlreadySent++;

            System.Scheduler.Schedule(new GenerateNextMessage(Id), TimeSpan.FromMilliseconds(25));
        }

        //public void Start()
        //{
        //    Task.Factory.StartNew(() =>
        //    {
        //        for (int i = 0; i < MessagesToBeSent; i++)
        //        {
        //            System.Send(new SendContent("testMessage", Addresses.TcpWritersDispatcher));
        //            Wait();
        //        }
        //    });
        //}        
    }

    public class GenerateNextMessage : Message
    {
        private readonly ActorId _desinationId;

        public GenerateNextMessage(ActorId desinationId)
        {
            _desinationId = desinationId;
        }

        public override ActorId DestinationActorId
        {
            get { return _desinationId; }
        }
    }
}