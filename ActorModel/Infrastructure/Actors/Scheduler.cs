using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace ActorModel.Infrastructure.Actors
{
    public interface IScheduler : IDisposable
    {
        void Schedule<TMessage>(TMessage message, TimeSpan after) where TMessage : Message;
    }

    public class Scheduler : IScheduler
    {
        private readonly ActorsSystem _system;

        private readonly ConcurrentDictionary<Guid, ScheduledJob> _scheduledJobs =
            new ConcurrentDictionary<Guid, ScheduledJob>();
        
        private readonly Timer _timer;

        public Scheduler(ActorsSystem system)
        {
            _system = system;
            _timer = new Timer(_ => DispatchScheduledMessages(), null, dueTime: 0, period: 1);
        }

        private void DispatchScheduledMessages()
        {
            var keyToRun = _scheduledJobs
                .Where(x => x.Value.RunAt <= DateTime.Now)
                .Select(x => x.Key);

            foreach (var key in keyToRun)
            {
                ScheduledJob job;
                if (_scheduledJobs.TryRemove(key, out job))
                    _system.Send(job.OriginalMessage);
            }
        }

        public void Schedule<TMessage>(TMessage message, TimeSpan after) where TMessage : Message
        {
            _scheduledJobs.TryAdd(Guid.NewGuid(), new ScheduledJob(message, after));
        }
        
        private class ScheduledJob
        {
            public Message OriginalMessage { get; private set; }
            public DateTime RunAt { get; private set; }

            public ScheduledJob(Message originalMessage, TimeSpan afterSpan)
            {
                OriginalMessage = originalMessage;
                RunAt = DateTime.Now + afterSpan;
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
            DispatchScheduledMessages();
        }
    }
}