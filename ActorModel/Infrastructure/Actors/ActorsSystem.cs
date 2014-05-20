using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ActorModel.Infrastructure.Actors
{
    public class ActorsSystem : IDisposable
    {
        private readonly IList<Actor> _actors;
        private readonly List<Message> _deadSink = new List<Message>();

        private ActorsSystem(IScheduler scheduler = null, params Actor[] actors)
        {
            _actors = actors;
            Scheduler = scheduler;            
        }

        public ActorsSystem(IScheduler scheduler = null)
        {
            _actors = new List<Actor>();
            Monitor = new MailboxMonitor();            
            Scheduler = scheduler ?? new Scheduler(this);
        }

        public IScheduler Scheduler { get; private set; }

        public MailboxMonitor Monitor { get; private set; }

        public IReadOnlyCollection<Message> DeadSink
        {
            get { return new ReadOnlyCollection<Message>(_deadSink); }
        }

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

        public static ActorsSystem WithoutQueues(IScheduler scheduler = null, params Actor[] actors)
        {
            return new ActorsSystem(scheduler,actors);
        }

        public static ActorsSystem WithQueues(IScheduler scheduler = null, params Actor[] actors)
        {
            var queuedActors = actors.Select(QueuedActor.Of);
            return new ActorsSystem(scheduler, queuedActors.ToArray());
        }

        public void Send(Message message)
        {
            var actor = _actors.FirstOrDefault(x => x.Id == message.DestinationActorId);

            if (actor != null)
                actor.Handle(message);
            else
                _deadSink.Add(message);
        }

        public void Dispose()
        {
            if (Scheduler != null)
                Scheduler.Dispose();

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