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
        private readonly NetworkStream _clientStream;

        public TcpWriter(ActorId id, ActorsSystem system) : base(id, system)
        {
            var serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);
            _client = new TcpClient();
            _client.Connect(serverEndPoint);
            _clientStream = _client.GetStream();
        }
       
        public void On(SendContent message)
        {
            bool failed = false;

            try
            {                
                var encoder = new ASCIIEncoding();
                var buffer = encoder.GetBytes(message.Content);                               

                _clientStream.Write(buffer, 0, buffer.Length);
                _clientStream.Flush();
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
            
            _clientStream.Close();
            _clientStream.Dispose();
        }
    }
}