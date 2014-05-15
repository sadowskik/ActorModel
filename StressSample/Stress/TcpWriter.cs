using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ActorModel.Infrastructure.Actors;

namespace Stress
{
    public class TcpWriter : Actor
    {
        private readonly TcpClient _client;

        public TcpWriter(ActorId id, ActorsSystem system) : base(id, system)
        {
            var serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);
            _client = new TcpClient();
            _client.Connect(serverEndPoint);
        }
       
        public void On(SendContent message)
        {
            bool failed = false;

            try
            {
                var clientStream = _client.GetStream();
                var encoder = new ASCIIEncoding();
                byte[] buffer = encoder.GetBytes(message.Content);
                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
            }
            catch (Exception)
            {
                failed = true;
            }
            finally
            {
                System.Send(new SendContent(message, Addresses.DiskWriter, failed));                
            }
        }

        public override void Dispose()
        {
            _client.Close();
        }
    }
}