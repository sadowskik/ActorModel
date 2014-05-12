using System;
using System.Net;
using System.Net.Sockets;
using ActorModel.Infrastructure.Actors;
using Server.Messages;

namespace Server
{
    public class ConnectionListener : Actor
    {
        private bool _running;
        private TcpListener _tcpListener;

        public ConnectionListener(ActorId id, ActorsSystem system) : base(id, system)
        {
        }

        public void On(StartListening e)
        {                              
            _tcpListener = new TcpListener(IPAddress.Any, 3000);
            _tcpListener.Start();

            _running = true;
            while (_running)
            {
                try
                {
                    var client = _tcpListener.AcceptTcpClient();
                    System.Send(new ClientConnected(Addresses.ConnectionWorkers, client));
                }
                catch (Exception)
                {

                    _running = false;
                }
            }
        }

        public override void Dispose()
        {
            _running = false;
            _tcpListener.Server.Close();
        }
    }
}