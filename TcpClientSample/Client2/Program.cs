using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Client2
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.ReadLine();

            var client = new TcpServiceClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000));

            const int parallelism = 10;
            var awaitingTasks = new Task[parallelism];

            Parallel.For(0, parallelism, i =>
            {
                awaitingTasks[i] = client.Send();
            });

            Thread.Sleep(500);
            Task.WaitAll(awaitingTasks);
            Console.WriteLine("Messages send");
        }
    }
}
