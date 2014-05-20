using ActorModel.Infrastructure.Actors;

namespace StopLoss
{
    public static class Addresses
    {
        public static readonly ActorId TraderAddress = ActorId.GenerateNew();
        public static readonly ActorId OrderProcessorAddress = ActorId.GenerateNew();
    }
}