using System;
using System.Collections.Generic;
using System.Linq;

namespace ActorModel.Infrastructure.Actors
{
    public class ActorsSystem : IDisposable
    {
        private readonly IList<Actor> _actors;

        private ActorsSystem(params Actor[] actors)
        {
            _actors = actors;
        }

        public ActorsSystem()
        {
            _actors = new List<Actor>();
            Scheduler = new Scheduler(this);
            Monitor = new MailboxMonitor();
        }

        public Scheduler Scheduler { get; private set; }

        public MailboxMonitor Monitor { get; private set; }

        public Actor CreateNewActor(Func<ActorsSystem, Actor> factory)
        {
            var newActor = factory(this);   
            _actors.Add(newActor);
            return newActor;
        }

        public void SubscribeByAddress(Actor actor)
        {
            _actors.Add(actor);
        }

        public static ActorsSystem WithoutQueues(params Actor[] actors)
        {
            return new ActorsSystem(actors);
        }

        public static ActorsSystem WithQueues(params Actor[] actors)
        {
            var queuedActors = actors.Select(QueuedActor.Of);
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
            Scheduler.Stop();

            foreach (var actor in _actors)
                actor.Dispose();
        }
    }

    public class MailboxMonitor
    {
        private readonly Dictionary<ActorId, IMailBox> _map = new Dictionary<ActorId, IMailBox>();

        public void MonitorActor(Actor actor)
        {
            _map[actor.Id] = new DelegatingMailBox(() => actor.MailBox);
        }

        public IEnumerable<IMailBox> GetAllStats()
        {
            return _map.Values;
        } 
    }
}