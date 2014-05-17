using System;
using ActorModel.Infrastructure.Actors;
using Server.Messages;

namespace Server
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Starting server...");

            using (var system = new ActorsSystem())
            {
                var listener = QueuedActor.Of(new ConnectionListener(Addresses.Listener, system));
                system.SubscribeByAddress(listener);

                var connectionWorkers = QueuedActor.Of(new RoundRobinActor(
                    id: Addresses.ConnectionWorkers,
                    workerFactory: () => QueuedActor.Of(new ClientConnectionWorker(system)),
                    degreeOfParallelism: 2));
                system.SubscribeByAddress(connectionWorkers);

                
                system.Send(new StartListening());

                Console.WriteLine("Server started");
                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }

            Console.WriteLine("Finished");
        }
    }
}
