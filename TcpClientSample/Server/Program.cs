using System;
using ActorModel.Infrastructure.Actors;
using Server.Messages;
using Server.Stats;

namespace Server
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Starting server...");

            using (var system = new ActorsSystem())
            using (StatsService.Run(system.Monitor))                                        
            {
                var listener = QueuedActor.Of(new ConnectionListener(Addresses.Listener, system));
                system.SubscribeByAddress(listener);

                var connectionWorkers = QueuedActor.Of(mailbox1 => new RoundRobinActor(
                    id: Addresses.ConnectionWorkers,
                    workerFactory: () => QueuedActor.Of(mailbox2 => new ClientConnectionWorker(system, mailbox2), system.Monitor),
                    degreeOfParallelism: 2, system: system, mailBox: mailbox1));
                system.SubscribeByAddress(connectionWorkers);
                system.Monitor.MonitorActor(connectionWorkers);
                
                system.Send(new StartListening());

                Console.WriteLine("Server started");
                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }

            Console.WriteLine("Finished");
        }
    }
}
