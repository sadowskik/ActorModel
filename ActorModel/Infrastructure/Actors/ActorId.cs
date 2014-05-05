using System;

namespace ActorModel.Infrastructure.Actors
{
    public class ActorId
    {
        public ActorId(ActorId id)
        {
            Value = id.Value;
        }

        public static ActorId GenerateNew()
        {
            return new ActorId {Value = Guid.NewGuid()};
        }

        private ActorId()
        {
        }

        public Guid Value { get; private set; }
    }
}