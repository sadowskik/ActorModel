using System;
using System.IO;
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
            string message;
            if (!TryReadMessage(clientStream, out message))
            {

                clientStream.Close();
                tcpClient.Close();
                return false;
            }

            if (!string.IsNullOrEmpty(message))
            {
                Thread.Sleep(250); //simulate some work
                Console.WriteLine("Worker {0}: {1}", Id, message);
            }

            return true;
        }

        private static bool TryReadMessage(NetworkStream clientStream, out string message)
        {
            message = null;
            var buffer = new byte[64];
            int bytesRead;
            try
            {
                clientStream.ReadTimeout = 1000;
                bytesRead = clientStream.Read(buffer, 0, 64);
            }
            catch (IOException)
            {
                //timeout
                return true;
            }
            catch
            {
                return false;
            }

            if (bytesRead == 0)
                return false;

            var encoder = new ASCIIEncoding();
            message = encoder.GetString(buffer, 0, bytesRead);
            return true;
        }
    }
}