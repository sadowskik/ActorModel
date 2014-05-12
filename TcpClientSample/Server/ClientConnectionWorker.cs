using System;
using System.Text;
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
            var clientStream = tcpClient.GetStream();

            var message = new byte[4096];

            while (true)
            {
                int bytesRead;

                try
                {
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                //message has successfully been received
                var encoder = new ASCIIEncoding();
                var decodedMessage = encoder.GetString(message, 0, bytesRead);
                Console.WriteLine("Worker {0}: {1}", Id, decodedMessage);
            }

            tcpClient.Close();
        }
    }
}