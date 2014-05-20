using System;
using ActorModel.Infrastructure.Actors;

namespace StopLoss
{
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

        protected bool Equals(Sell other)
        {
            return Equals(_destinationId, other._destinationId) && _positionId.Equals(other._positionId) && _sellingPrice == other._sellingPrice;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Sell) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_destinationId != null ? _destinationId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ _positionId.GetHashCode();
                hashCode = (hashCode*397) ^ _sellingPrice.GetHashCode();
                return hashCode;
            }
        }
    }
}