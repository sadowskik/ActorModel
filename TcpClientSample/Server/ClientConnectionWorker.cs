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

            var status = Process(tcpStream, tcpClient);

            if (status == ProcessingStatus.ClientDisconected)
                return;

            System.Send(new ChunkReaded(Addresses.ConnectionWorkers, tcpStream, connected.Client));
        }

        public void On(ChunkReaded chunk)
        {
            var status = Process(chunk.Stream, chunk.Client);

            if (status == ProcessingStatus.ClientDisconected)
                return;

            if (status == ProcessingStatus.MessageReaded)
                System.Send(new ChunkReaded(Addresses.ConnectionWorkers, chunk.Stream, chunk.Client));

            if (status == ProcessingStatus.ClientIsNotSending)
                System.Scheduler.Schedule(
                    new ChunkReaded(Addresses.ConnectionWorkers, chunk.Stream, chunk.Client),
                    TimeSpan.FromMilliseconds(100));
        }

        private ProcessingStatus Process(NetworkStream clientStream, TcpClient tcpClient)
        {
            if (IsDisconected(tcpClient))
                return ProcessingStatus.ClientDisconected;

            string message;
            var messageReadStatus = TryReadMessage(clientStream, out message);
            if (messageReadStatus == ProcessingStatus.ClientDisconected)
            {
                clientStream.Close();
                tcpClient.Close();
                return messageReadStatus;
            }

            if (!string.IsNullOrEmpty(message))
            {
                Thread.Sleep(10); //simulate some work
                Console.WriteLine("Worker {0}: {1}", Id, message);
            }

            return messageReadStatus;
        }

        private enum ProcessingStatus
        {
            MessageReaded,
            ClientDisconected,
            ClientIsNotSending,
        }

        private static ProcessingStatus TryReadMessage(NetworkStream clientStream, out string message)
        {
            message = null;

            try
            {
                var buffer = new byte[11];
                int bytesRead;
                
                if (clientStream.DataAvailable)
                    bytesRead = clientStream.Read(buffer, 0, 11);
                else 
                    return ProcessingStatus.ClientIsNotSending;  
                
                if (bytesRead == 0)
                    return ProcessingStatus.ClientIsNotSending;

                var encoder = new ASCIIEncoding();
                message = encoder.GetString(buffer, 0, bytesRead);
                return ProcessingStatus.MessageReaded;
            }
            catch
            {
                return ProcessingStatus.ClientDisconected;
            }            
        }

        private static bool IsDisconected(TcpClient tcp)
        {
            try
            {
                if (tcp.Client.Poll(0, SelectMode.SelectRead))
                {
                    var buff = new byte[1];
                    if (tcp.Client.Receive(buff, SocketFlags.Peek) == 0)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return true;
            }

            return false;
        }
    }
}