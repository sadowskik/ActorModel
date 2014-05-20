using System;
using ActorModel.Infrastructure.Actors;

namespace StopLoss
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var system = new ActorsSystem())
            {
                var trader = QueuedActor.Of(new StopLossTrader(Addresses.TraderAddress, system));
                system.SubscribeByAddress(trader);

                var orderProcessor = QueuedActor.Of(new OrderProcessor(Addresses.OrderProcessorAddress, system));
                system.SubscribeByAddress(orderProcessor);

                var market = QueuedActor.Of(new Market(ActorId.GenerateNew(), system));
                system.SubscribeByAddress(market);

                Console.WriteLine("Press any key to stop");
                Console.ReadLine();
            }
        }
    }
}
