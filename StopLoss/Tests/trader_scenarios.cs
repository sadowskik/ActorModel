﻿using System;
using System.Collections.Generic;
using ActorModel.Infrastructure.Actors;
using NFluent;
using NUnit.Framework;

namespace StopLoss.Tests
{
    [TestFixture]
    public class trader_scenarios
    {
        [Test]
        public void a()
        {
            var schedulerMock = new SchedulerMock();

            using (var system = new ActorsSystem(schedulerMock))
            {
                var trader = new Trader(ActorId.GenerateNew(), system);
                system.SubscribeByAddress(trader);

                system.Send(new PriceChanged(trader.Id, 20m, Guid.Parse("00000000-0000-0000-0000-000000000001")));
                system.Send(new PriceChanged(trader.Id, 20m, Guid.Parse("00000000-0000-0000-0000-000000000002")));
                system.Send(new PriceChanged(trader.Id, 20m, Guid.Parse("00000000-0000-0000-0000-000000000003")));

                system.Send(new RemoveFrom15(trader.Id, Guid.Parse("00000000-0000-0000-0000-000000000001")));

                Check.That(system.DeadSink).Contains(new Sell(Addresses.OrderProcessorAddress, trader.Id.Value, 50m));
            }
        }
    }

    public class SchedulerMock : IScheduler
    {
        public Dictionary<TimeSpan, IList<Message>> Scheduled = new Dictionary<TimeSpan, IList<Message>>();

        public void Schedule<TMessage>(TMessage message, TimeSpan after) where TMessage : Message
        {
            IList<Message> toSchedule;
            if (Scheduled.TryGetValue(after, out toSchedule))
            {
                toSchedule.Add(message);
            }
            else
            {
                toSchedule = new List<Message> {message};
                Scheduled[after] = toSchedule;
            }
        }

        public void Dispose()
        {
        }
    }
}