using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ActorModel.Infrastructure.Actors;
using Server.Messages;

namespace Server
{
    public class ClientConnectionWorker : Actor
    {
        public ClientConnectionWorker(ActorsSystem system)
            : base(ActorId.GenerateNew(), system)
        {
        }

        public void On(ClientConnected connected)
        {
            var tcpClient = connected.Client;
            var tcpStream = tcpClient.GetStream();

            if (!Process(tcpStream, tcpClient))
                return;

            System.Send(new ChunkReaded(Addresses.ConnectionWorkers, tcpStream, connected.Client));
        }

        public void On(ChunkReaded chunk)
        {
            if (!Process(chunk.Stream, chunk.Client))
                return;

            System.Send(new ChunkReaded(Addresses.ConnectionWorkers, chunk.Stream, chunk.Client));
        }

        private bool Process(NetworkStream clientStream, TcpClient tcpClient)
        {
            if (IsDisconected(tcpClient))
                return false;
            
            string message;
            if (!TryReadMessage(clientStream, out message))
            {
                clientStream.Close();
                tcpClient.Close();
                return false;
            }

            if (!string.IsNullOrEmpty(message))
            {
                Thread.Sleep(10); //simulate some work
                Console.WriteLine("Worker {0}: {1}", Id, message);
            }

            return true;
        }

        private static bool TryReadMessage(NetworkStream clientStream, out string message)
        {
            message = null;

            try
            {
                var buffer = new byte[11];
                int bytesRead;
                
                if (clientStream.DataAvailable)
                    bytesRead = clientStream.Read(buffer, 0, 11);
                else return true;  
                
                if (bytesRead == 0)
                    return false;

                var encoder = new ASCIIEncoding();
                message = encoder.GetString(buffer, 0, bytesRead);
                return true;
            }
            catch
            {
                return false;
            }            
        }

        private static bool IsDisconected(TcpClient tcp)
        {            
            if (tcp.Client.Poll(0, SelectMode.SelectRead))
            {
                var buff = new byte[1];
                if (tcp.Client.Receive(buff, SocketFlags.Peek) == 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}