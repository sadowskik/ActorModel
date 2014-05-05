using System.Collections.Concurrent;
using System.Threading;

namespace ActorModel.Infrastructure.Actors
{
    public class QueuedActor : Actor
    {
        private readonly Actor _actor;        
        private readonly ConcurrentQueue<Message> _mailBox = new ConcurrentQueue<Message>();

        private bool _started;
        private Thread _actorThread;

        public QueuedActor(Actor actor) : base(actor.Id)
        {
            _actor = actor;
        }

        public void Start()
        {
            _started = true;
            _actorThread = new Thread(Loop);
            _actorThread.Start();
        }

        public void Stop()
        {
            _started = false;
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
                Message message;
                if (_mailBox.TryDequeue(out message))
                    _actor.Handle(message);

                Thread.Sleep(50);
            }
        }

        public override void Handle(Message message)
        {
            _mailBox.Enqueue(message);
        }
    }
}