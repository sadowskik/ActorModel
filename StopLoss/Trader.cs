using System;
using System.Collections.Generic;
using System.Linq;
using ActorModel.Infrastructure.Actors;

namespace StopLoss
{
    public class Trader : Actor
    {
        private readonly IDictionary<Guid, PriceChanged> _30SecWindow = new Dictionary<Guid, PriceChanged>();
        private readonly IDictionary<Guid, PriceChanged> _15SecWindow = new Dictionary<Guid, PriceChanged>();

        private bool _alive = true;
        private decimal _price;

        public Trader(ActorId id, ActorsSystem system) : base(id, system)
        {
        }

        public void On(PriceChanged priceChanged)
        {
            var priceId = priceChanged.PriceId;

            _30SecWindow.Add(priceId, priceChanged);
            _15SecWindow.Add(priceId, priceChanged);

            System.Scheduler.Schedule(new RemoveFrom15(Id, priceId), TimeSpan.FromSeconds(15));
            System.Scheduler.Schedule(new RemoveFrom30(Id, priceId), TimeSpan.FromSeconds(30));
        }

        public void On(RemoveFrom15 remove)
        {
            _15SecWindow.Remove(remove.PriceId);

            if (_15SecWindow.Values.All(x => x.NewPrice > _price*0.95m))
                _price = _15SecWindow.Values.Min(x => x.NewPrice);
        }

        public void On(RemoveFrom30 remove)
        {
            if (!_alive)
                return;

            if (_30SecWindow.Values.All(x => x.NewPrice < _price*0.95m))
            {
                _alive = false;
                System.Send(new Sell(Addresses.OrderProcessorAddress, Id.Value, _price));
            }

            _30SecWindow.Remove(remove.PriceId);
        }
    }

    public class RemoveFrom15 : Message
    {
        private readonly ActorId _destinationId;
        private readonly Guid _priceId;

        public RemoveFrom15(ActorId destinationId, Guid priceId)
        {
            _destinationId = destinationId;
            _priceId = priceId;
        }

        public override ActorId DestinationActorId
        {
            get { return _destinationId; }
        }

        public Guid PriceId
        {
            get { return _priceId; }
        }
    }

    public class RemoveFrom30 : Message
    {
        private readonly ActorId _destinationId;
        private readonly Guid _priceId;

        public RemoveFrom30(ActorId destinationId, Guid priceId)
        {
            _destinationId = destinationId;
            _priceId = priceId;
        }

        public override ActorId DestinationActorId
        {
            get { return _destinationId; }
        }

        public Guid PriceId
        {
            get { return _priceId; }
        }
    }

    public class Sell : Message
    {
        private readonly ActorId _destinationId;
        private readonly Guid _positionId;
        private readonly decimal _sellingPrice;

        public Sell(ActorId destinationId, Guid positionId, decimal sellingPrice)
        {
            _destinationId = destinationId;
            _positionId = positionId;
            _sellingPrice = sellingPrice;
        }

        public override ActorId DestinationActorId
        {
            get { return _destinationId; }
        }

        public Guid PositionId
        {
            get { return _positionId; }
        }

        public decimal SellingPrice
        {
            get { return _sellingPrice; }
        }
    }
}