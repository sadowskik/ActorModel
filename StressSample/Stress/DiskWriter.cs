using System;
using System.IO;
using System.Text;
using ActorModel.Infrastructure.Actors;

namespace Stress
{
    public class DiskWriter : Actor
    {
        private readonly StreamWriter _fileWriter;

        public int TotalMessagesProcessed { get; private set; }

        public DiskWriter(ActorId id) : base(id)
        {
            _fileWriter = new StreamWriter("test.out", true, new UTF8Encoding(false, true), 1024*1024*10);
        }

        public void On(SendContent message)
        {
            _fileWriter.WriteLine("Message: {0}; Success: {1}", message.Content, !message.Failed);
            TotalMessagesProcessed++;

            if (TotalMessagesProcessed%100 == 0 || TotalMessagesProcessed%100 == TotalMessagesProcessed)
                Console.WriteLine("Total messages processed: {0}", TotalMessagesProcessed);
        }

        public override void Dispose()
        {
            _fileWriter.Flush();
            _fileWriter.Close();
        }
    }
}