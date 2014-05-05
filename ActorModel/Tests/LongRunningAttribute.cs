using NUnit.Framework;

namespace ActorModel.Tests
{
    public class LongRunningAttribute : CategoryAttribute
    {
        public LongRunningAttribute() : base("Long running test")
        {
        }
    }
}