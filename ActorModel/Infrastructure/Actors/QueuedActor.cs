using System.Collections.Concurrent;
using System.Threading;

namespace ActorModel.Infrastructure.Actors
{
    public class QueuedActor : Actor
    {
        private readonly Actor _actor;        
        private readonly ConcurrentQueue<Message> _mailBox = new ConcurrentQueue<Message>();
        private readonly ManualResetEventSlim _continueProcessing = new ManualResetEventSlim(false);

        private bool _started;
        private Thread _actorThread;

        public QueuedActor(Actor actor) : base(actor.Id)
        {
            _actor = actor;
        }

        public static Actor Of(Actor actor)
        {
            var queuedActor = new QueuedActor(actor);
            queuedActor.Start();
            return queuedActor;
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
                _continueProcessing.Wait();
                _continueProcessing.Reset();

                Message message;
                while (_mailBox.TryDequeue(out message))
                    _actor.Handle(message);
            }
        }

        public override void Handle(Message message)
        {
            _mailBox.Enqueue(message);
            _continueProcessing.Set();
        }
    }
}