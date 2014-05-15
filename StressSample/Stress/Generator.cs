using System.Threading;
using System.Threading.Tasks;
using ActorModel.Infrastructure.Actors;

namespace Stress
{
    public class Generator : Actor
    {
        public Generator(ActorId id, ActorsSystem system) : base(id, system)
        {
        }

        public void Start()
        {
            const int totalMessages = 10000;

            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < totalMessages; i++)
                {
                    System.Send(new SendContent("testMessage", Addresses.TcpWritersDispatcher));
                    Wait();
                }
            });
        }

        private static void Wait()
        {
            //Thread.Sleep(10);
            //Thread.SpinWait(50000);
        }
    }
}