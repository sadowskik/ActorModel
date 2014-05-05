using System;
using System.Linq;

namespace ActorModel.Infrastructure.Actors
{
    public class ActorsSystem : IDisposable
    {
        private readonly Actor[] _actors;

        private ActorsSystem(params Actor[] actors)
        {
            _actors = actors;
        }

        public static ActorsSystem WithoutQueues(params Actor[] actors)
        {
            return new ActorsSystem(actors);
        }

        public static ActorsSystem WithQueues(params Actor[] actors)
        {
            var queuedActors = actors.Select(actor =>
            {
                var queuedActor = new QueuedActor(actor);
                queuedActor.Start();
                return (Actor) queuedActor;
            });

            return new ActorsSystem(queuedActors.ToArray());
        }

        public void Send(Message message)
        {
            var actor = _actors.FirstOrDefault(x => x.Id == message.DestinationActorId);

            if (actor != null)
                actor.Handle(message);
        }

        public void Dispose()
        {
            foreach (var actor in _actors)
                actor.Dispose();
        }
    }
}