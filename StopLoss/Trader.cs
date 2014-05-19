using System;
using System.Collections.Generic;
using System.Linq;
using ActorModel.Infrastructure.Actors;

namespace StopLoss
{
    public class Trader : Actor
    {
        private readonly IDictionary<Guid,PriceChanged> _30SecWindow = new Dictionary<Guid, PriceChanged>();
        private readonly IDictionary<Guid, PriceChanged> _15SecWindow = new Dictionary<Guid, PriceChanged>();

        bool _alive = true;
        private decimal _price = 0;

        public Trader(ActorId id, ActorsSystem system) : base(id, system)
        {
        }

        public void On(PriceChanged priceChanged)
        {
            var priceId = Guid.NewGuid();

            _30SecWindow.Add(priceId, priceChanged);
            _15SecWindow.Add(priceId, priceChanged);

            System.Scheduler.Schedule(new RemoveFrom15(Id, priceId), TimeSpan.FromSeconds(15));
            System.Scheduler.Schedule(new RemoveFrom30(Id, priceId), TimeSpan.FromSeconds(30));
        }

        public void On(RemoveFrom15 remove)
        {
            _15SecWindow.Remove(remove.PriceId);
            CheckMove();
        }

        public void On(RemoveFrom30 remove)
        {
            CheckSell();
            _30SecWindow.Remove(remove.PriceId);
        }

        private void CheckSell()
        {
            if (!_alive)
                return;

            if (_30SecWindow.Values.Count(x => x.NewPrice > _price*0.95m) == 0)
            {
                _alive = false;
                //System.Send(new Sell(this.Id));
            }

            
        }

        private void CheckMove()
        {
            if (_15SecWindow.Values.All(x => x.NewPrice > _price*0.95m))
            {
                _price = _15SecWindow.Values.Min(x => x.NewPrice);
            }
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
        private readonly Guid _priceId;

        public Sell(ActorId destinationId, Guid priceId)
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
}