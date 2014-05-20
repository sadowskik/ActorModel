using System;
using ActorModel.Infrastructure.Actors;

namespace StopLoss
{
    public class PriceChanged : Message
    {
        private readonly ActorId _destinationId;
        private readonly decimal _newPrice;
        private readonly Guid _priceId;

        public PriceChanged(ActorId destinationId, decimal newPrice, Guid priceId)
        {
            _destinationId = destinationId;
            _newPrice = newPrice;
            _priceId = priceId;
        }

        public override ActorId DestinationActorId
        {
            get { return _destinationId; }
        }

        public decimal NewPrice
        {
            get { return _newPrice; }
        }

        public Guid PriceId
        {
            get { return _priceId; }
        }
    }
}