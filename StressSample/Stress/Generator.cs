using ActorModel.Infrastructure.Actors;

namespace Stress
{
    public class Generator : Actor
    {
        public Generator(ActorId id, ActorsSystem system) : base(id, system)
        {
        }
    }
}