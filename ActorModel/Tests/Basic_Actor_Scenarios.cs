using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ActorModel.Infrastructure.Actors;
using NFluent;
using NUnit.Framework;

namespace ActorModel.Tests
{
    public class Basic_Actor_Scenarios
    {
        [Test]
        public void should_handle_messages_of_different_types()
        {
            var testActor = new MyTestActor(ActorId.GenerateNew());

            testActor.Handle(new MyTestMessage1());
            testActor.Handle(new MyTestMessage2());

            Check.That(testActor.HandledMessagesTypes)
                .ContainsExactly(typeof (MyTestMessage1), typeof (MyTestMessage2));
        }

        [Test]
        public void should_send_message_to_the_actor_via_system_in_memory()
        {
            var testActorId = ActorId.GenerateNew();
            var testActor = new MyTestActor(testActorId);

            using (var system = ActorsSystem.WithoutQueues(testActor))
            {
                var messageToSend = new MyTestMessage1(destinationId: testActorId);
                system.Send(messageToSend);

                Check.That(testActor.HandledMessages).Contains(messageToSend);
            }
        }

        [Test]
        [LongRunning]
        public void should_send_message_to_the_actor_via_system_with_queues()
        {
            var testActorId = ActorId.GenerateNew();
            var testActor = new MyTestActor(testActorId);

            using (var system = ActorsSystem.WithQueues(testActor))
            {
                var messageToSend = new MyTestMessage1(destinationId: testActorId);
                system.Send(messageToSend);

                Thread.Sleep(100); //that's because of queues usage. use in memory system to avoid dependency from time
                Check.That(testActor.HandledMessages).Contains(messageToSend);
            }
        }

        [Test]
        public void should_distribute_messages_in_a_round_robin_fashion()
        {            
            //arrange
            var actor1 = new MyTestActor(ActorId.GenerateNew());
            var actor2 = new MyTestActor(ActorId.GenerateNew());

            var factory = From(actor1, actor2);
            var roundRobinActor = new RoundRobinActor(ActorId.GenerateNew(), factory, 2);

            //act
            roundRobinActor.Handle(new MyTestMessage1());
            roundRobinActor.Handle(new MyTestMessage1());
            roundRobinActor.Handle(new MyTestMessage1());

            //assert
            Check.That(actor1.HandledMessages).HasSize(2);
            Check.That(actor2.HandledMessages).HasSize(1);
        }

        [Test]
        [LongRunning]
        public void should_distribute_messages_in_a_round_robin_fashion_with_queues()
        {
            //arrange
            var actor1 = new MyTestActor(ActorId.GenerateNew());
            var actor2 = new MyTestActor(ActorId.GenerateNew());

            var factory = From(QueuedActor.Of(actor1), QueuedActor.Of(actor2));
            var roundRobinActor = QueuedActor.Of(new RoundRobinActor(ActorId.GenerateNew(), factory, 2));

            //act
            roundRobinActor.Handle(new MyTestMessage1());
            roundRobinActor.Handle(new MyTestMessage1());
            roundRobinActor.Handle(new MyTestMessage1());

            //assert
            Thread.Sleep(500);
            Check.That(actor1.HandledMessages).HasSize(2);
            Check.That(actor2.HandledMessages).HasSize(1);
        }

        private static Func<T> From<T>(params T[] workers)
        {
            int invoked = 0;
            return () => workers[invoked++];
        }
    }

    public class MyTestActor : Actor
    {
        public List<Type> HandledMessagesTypes
        {
            get { return HandledMessages.Select(x => x.GetType()).ToList(); }
        }

        public List<Message> HandledMessages = new List<Message>();

        public MyTestActor(ActorId id) : base(id)
        {
        }

        public void On(MyTestMessage1 message)
        {
            HandledMessagesTypes.Add(message.GetType());
            HandledMessages.Add(message);
        }

        public void On(MyTestMessage2 message)
        {
            HandledMessagesTypes.Add(message.GetType());
            HandledMessages.Add(message);
        }
    }

    public class MyTestMessage1 : Message
    {
        private readonly ActorId _destinationId;

        public MyTestMessage1(ActorId destinationId)
        {
            _destinationId = destinationId;
        }

        public MyTestMessage1() : this(ActorId.GenerateNew())
        {
        }

        public override ActorId DestinationActorId
        {
            get { return _destinationId; }
        }
    }

    public class MyTestMessage2 : Message
    {
        private readonly ActorId _destinationId;

        public MyTestMessage2(ActorId destinationId)
        {
            _destinationId = destinationId;
        }

        public MyTestMessage2()
            : this(ActorId.GenerateNew())
        {
        }

        public override ActorId DestinationActorId
        {
            get { return _destinationId; }
        }
    }
}
