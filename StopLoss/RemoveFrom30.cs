using System;
using ActorModel.Infrastructure.Actors;

namespace StopLoss
{
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
}