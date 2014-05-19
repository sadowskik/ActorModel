using ActorModel.Infrastructure.Actors;

namespace StopLoss
{
    public class PriceChanged : Message
    {
        private readonly ActorId _destinationId;
        private readonly decimal _newPrice;

        public PriceChanged(ActorId destinationId, decimal newPrice)
        {
            _destinationId = destinationId;
            _newPrice = newPrice;
        }

        public override ActorId DestinationActorId
        {
            get { return _destinationId; }
        }

        public decimal NewPrice
        {
            get { return _newPrice; }
        }
    }
}