using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace ActorModel.Infrastructure.Actors
{
    public class QueuedActor : Actor
    {
        private Actor _actor;        
        private readonly ConcurrentQueue<Message> _mailBox = new ConcurrentQueue<Message>();
        private readonly ManualResetEventSlim _continueProcessing = new ManualResetEventSlim(false);
        
        private bool _started;
        private Thread _actorThread;
        
        private int _messageProcessed;
        private float _avgProcessingSpeed;

        private QueuedActor() : base(ActorId.GenerateNew())
        {
        }
        
        private QueuedActor(Actor actor)
            : base(actor.Id)
        {
            _actor = actor;
            MailBox = new QueueMailBox();
        }
        
        public static Actor Of(Actor actor)
        {
            var queuedActor = new QueuedActor(actor);
            queuedActor.Start();
            return queuedActor;
        }

        public static Actor Of(Func<IMailBox, Actor> actorBuilder)
        {
            var queuedActor = new QueuedActor();                        
            var internalActor = actorBuilder(new DelegatingMailBox(() => queuedActor.MailBox));

            queuedActor._actor = internalActor;
            queuedActor.Id = internalActor.Id;
            
            queuedActor.Start();
            return queuedActor;
        }

        public static Actor Of(Func<IMailBox, Actor> actorBuilder, MailboxMonitor mailboxMonitor)
        {
            var queuedActor = new QueuedActor();
            var internalActor = actorBuilder(new DelegatingMailBox(() => queuedActor.MailBox));

            queuedActor._actor = internalActor;
            queuedActor.Id = internalActor.Id;
            mailboxMonitor.MonitorActor(queuedActor);

            queuedActor.Start();
            return queuedActor;           
        }

        public override IMailBox MailBox
        {
            get
            {
                return new QueueMailBox
                       {
                           ActorId = Id,
                           QueueLength = _mailBox.Count,
                           MessagesPerSecond = _avgProcessingSpeed,
                           TotalMessagesProcessed = _messageProcessed
                       };
            }
        }

        public void Start()
        {
            _started = true;
            _actorThread = new Thread(Loop) {IsBackground = true, Name = _actor.Id.ToString()};            
            _actorThread.Start();            
        }

        public void Stop()
        {            
            _started = false;
            _actor.Dispose();
            _continueProcessing.Set();
            _actorThread.Join(millisecondsTimeout: 1000);            
        }

        public override void Dispose()
        {
            Stop();
        }

        private void Loop()
        {
            while (_started)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                int batchSize = 0;

                _continueProcessing.Wait();
                _continueProcessing.Reset();
                
                Message message;
                while (_mailBox.TryDequeue(out message))
                {
                    _actor.Handle(message);
                    _messageProcessed++;
                    batchSize++;
                    _avgProcessingSpeed = ((float)batchSize * TimeSpan.TicksPerSecond) / stopwatch.ElapsedTicks;
                }

                stopwatch.Stop();                
            }
        }

        public override void Handle(Message message)
        {
            _mailBox.Enqueue(message);            
            _continueProcessing.Set();
        }        
    }

    public class QueueMailBox : IMailBox
    {        
        public int QueueLength { get; set; }

        public ActorId ActorId { get; set; }
        
        public float MessagesPerSecond { get; set; }

        public int TotalMessagesProcessed { get; set; }
    }

    public class DelegatingMailBox : IMailBox
    {
        private readonly Func<IMailBox> _closure;

        public DelegatingMailBox(Func<IMailBox> closure)
        {
            _closure = closure;
        }

        public int QueueLength
        {
            get { return _closure().QueueLength; }
        }

        public ActorId ActorId
        {
            get { return _closure().ActorId; }
        }

        public float MessagesPerSecond
        {
            get { return _closure().MessagesPerSecond; }
        }

        public int TotalMessagesProcessed
        {
            get { return _closure().TotalMessagesProcessed; }
        }
    }
}