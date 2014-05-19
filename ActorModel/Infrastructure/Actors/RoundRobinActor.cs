using System;

namespace ActorModel.Infrastructure.Actors
{
    public class RoundRobinActor : Actor
    {
        private readonly Actor[] _workers;
        private int _next;

        public RoundRobinActor(ActorId id, Func<Actor> workerFactory, int degreeOfParallelism, ActorsSystem system, IMailBox mailBox)
            : base(id,system, mailBox)
        {            
            _workers = new Actor[degreeOfParallelism];

            for (int i = 0; i < degreeOfParallelism; i++)
                _workers[i] = workerFactory();
        }

        public override void Handle(Message message)
        {
            _workers[_next].Handle(message);
            _next = (_next + 1)%_workers.Length;
        }

        public override void Dispose()
        {
            foreach (var worker in _workers)
                worker.Dispose();
        }
    }
}