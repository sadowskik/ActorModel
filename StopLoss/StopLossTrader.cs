using System;
using System.Collections.Generic;
using System.Linq;
using ActorModel.Infrastructure.Actors;

namespace StopLoss
{
    public class StopLossTrader : Actor
    {
        private readonly IDictionary<Guid, PriceChanged> _30SecWindow = new Dictionary<Guid, PriceChanged>();
        private readonly IDictionary<Guid, PriceChanged> _15SecWindow = new Dictionary<Guid, PriceChanged>();

        private decimal _latestPrice;

        private bool _alive = true;
        private decimal _price;

        public StopLossTrader(ActorId id, ActorsSystem system) : base(id, system)
        {
        }

        public void On(PriceChanged priceChanged)
        {
            var priceId = priceChanged.PriceId;

            _30SecWindow.Add(priceId, priceChanged);
            _15SecWindow.Add(priceId, priceChanged);

            _latestPrice = priceChanged.NewPrice;

            System.Scheduler.Schedule(new RemoveFrom15(Id, priceId), TimeSpan.FromSeconds(15));
            System.Scheduler.Schedule(new RemoveFrom30(Id, priceId), TimeSpan.FromSeconds(30));
        }

        public void On(RemoveFrom15 remove)
        {            
            if (_15SecWindow.Values.All(x => x.NewPrice > _price*0.95m))
                _price = _15SecWindow.Values.Min(x => x.NewPrice);

            _15SecWindow.Remove(remove.PriceId);
        }

        public void On(RemoveFrom30 remove)
        {
            if (!_alive)
                return;

            _30SecWindow.Remove(remove.PriceId);

            if (_30SecWindow.Values.All(x => x.NewPrice < _price*0.95m))
            {
                _alive = false;
                System.Send(new Sell(Addresses.OrderProcessorAddress, Id.Value, _latestPrice));
            }            
        }
    }
}