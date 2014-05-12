using ActorModel.Infrastructure.Actors;

namespace Server
{
    public static class Addresses
    {
        public static readonly ActorId Listener = ActorId.GenerateNew();
        public static readonly ActorId ConnectionWorkers = ActorId.GenerateNew();
    }
}