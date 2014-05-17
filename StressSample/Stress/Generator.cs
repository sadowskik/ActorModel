using System.Threading.Tasks;
using ActorModel.Infrastructure.Actors;

namespace Stress
{
    public class Generator : Actor
    {
        public const int TotalMessages = 5000;

        public Generator(ActorId id, ActorsSystem system) : base(id, system)
        {
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < TotalMessages; i++)
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