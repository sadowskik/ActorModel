using ActorModel.Infrastructure.Actors;

namespace Stress
{
    public static class Addresses
    {
        public static readonly ActorId DiskWriter = ActorId.GenerateNew();
        public static readonly ActorId TcpWritersDispatcher = ActorId.GenerateNew();
    }
}