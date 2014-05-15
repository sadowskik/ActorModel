using System;
using System.Diagnostics;
using ActorModel.Infrastructure.Actors;

namespace Stress
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var system = new ActorsSystem())
            {
                var generator = new Generator(ActorId.GenerateNew(), system);

                var connectionWorkers = QueuedActor.Of(new RoundRobinActor(
                    id: Addresses.TcpWritersDispatcher,
                    workerFactory: () => QueuedActor.Of(new TcpWriter(ActorId.GenerateNew(), system)),
                    degreeOfParallelism: 2));

                system.SubscribeByAddress(connectionWorkers);

                var diskWriter = new DiskWriter(Addresses.DiskWriter);
                var queuedDiskWriter = QueuedActor.Of(diskWriter);
                system.SubscribeByAddress(queuedDiskWriter);


                var stopwatch = new Stopwatch();
                stopwatch.Start();
                generator.Start();

                Console.WriteLine("Stresser running");
                Console.WriteLine("Press any key to stop...");
                Console.ReadLine();

                stopwatch.Stop();
                Console.WriteLine("Speed: {0} msg/sec", ((float)diskWriter.TotalMessagesProcessed)/stopwatch.ElapsedMilliseconds*1000);
            }

        }
    }
}