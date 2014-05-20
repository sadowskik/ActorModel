using System;
using System.Threading;
using ActorModel.Infrastructure.Actors;

namespace StopLoss
{
    public class OrderProcessor : Actor
    {
        public OrderProcessor(ActorId id, ActorsSystem system) : base(id, system)
        {
        }

        public void On(Sell order)
        {
            //do sell            
            Thread.Sleep(250);
            Console.WriteLine("Position {0} sold for {1}", order.PositionId, order.SellingPrice);
        }
    }
}