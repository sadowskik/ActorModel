using System;
using ActorModel.Infrastructure.Actors;

namespace StopLoss
{
    public class Market : Actor
    {
        private readonly Random _timeIntervalRandomizer;
        private readonly Random _pricesRandomizer;
        
        public Market(ActorId id, ActorsSystem system) : base(id, system)
        {
            _timeIntervalRandomizer = new Random();
            _pricesRandomizer = new Random();
            GenerateNewPrices();
        }

        public void On(RefreshPrices _)
        {
            System.Send(new PriceChanged(
                destinationId: Addresses.TraderAddress,
                newPrice: (decimal) (_pricesRandomizer.NextDouble()*100),
                priceId: Guid.NewGuid()));

            GenerateNewPrices();
        }

        private void GenerateNewPrices()
        {
            System.Scheduler.Schedule(
                new RefreshPrices(Id),
                TimeSpan.FromMilliseconds(_timeIntervalRandomizer.NextDouble()*1000));
        }
    }
}