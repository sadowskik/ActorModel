using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static void Main()
        {
            //string message = "test";

            //while (!string.IsNullOrEmpty(message) && message != "exit")
            //{
            //    message = Console.ReadLine();
            //    SendMessage(message);
            //}

            Parallel.For(0, 100, i => SendMessage("test" + i));
        }

        private static void SendMessage(string message)
        {            
            var client = new TcpClient();
            var serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);

            client.Connect(serverEndPoint);

            var clientStream = client.GetStream();

            var encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes(message);

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
            client.Close();            
        }
    }    
}
