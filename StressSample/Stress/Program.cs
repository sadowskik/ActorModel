using System;
using System.Diagnostics;
using ActorModel.Infrastructure.Actors;

namespace Stress
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.ReadLine();

            using (var system = new ActorsSystem())
            {
                var generator = new Generator(ActorId.GenerateNew(), system);
                system.SubscribeByAddress(generator);

                var connectionWorkers = QueuedActor.Of(mailbox1 => new RoundRobinActor(
                    id: Addresses.TcpWritersDispatcher,
                    workerFactory:
                        () => QueuedActor.Of(mailbox2 => new TcpWriter(ActorId.GenerateNew(), system, mailbox2), system.Monitor),
                    degreeOfParallelism: 5,
                    system: system,
                    mailBox: mailbox1));

                system.SubscribeByAddress(connectionWorkers);
                system.Monitor.MonitorActor(connectionWorkers);

                var diskWriter = new DiskWriter(Addresses.DiskWriter);
                var queuedDiskWriter = QueuedActor.Of(diskWriter);
                system.SubscribeByAddress(queuedDiskWriter);
                system.Monitor.MonitorActor(queuedDiskWriter);


                var stopwatch = new Stopwatch();
                stopwatch.Start();
                system.Send(new GenerateNextMessage(generator.Id));
                
                Console.WriteLine("Stresser running");
                //Console.WriteLine("Press any key to stop...");
                //Console.ReadLine();

                diskWriter.AllJobDone.Wait();
                stopwatch.Stop();
                Console.WriteLine("Speed: {0} msg/sec", ((float)diskWriter.TotalMessagesProcessed)/stopwatch.ElapsedMilliseconds*1000);
            }

        }
    }
}