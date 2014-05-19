using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Server.Stats
{
    public class StatsController : ApiController
    {
        public IEnumerable<MailboxRepresentation> Get()
        {
            var stats = StatsService.Monitor.GetAllStats()
                .Select(x => new MailboxRepresentation
                             {
                                 QueueLength = x.QueueLength,
                                 ActorId = x.ActorId.ToString(),
                                 MessagesPerSecond = x.MessagesPerSecond,
                                 TotalMessages = x.TotalMessagesProcessed
                             })
                .ToList();

            return stats;
        }        
    }

    public class MailboxRepresentation
    {
        public int QueueLength { get; set; }

        public string ActorId { get; set; }

        public float MessagesPerSecond { get; set; }

        public int TotalMessages { get; set; }
    }
}