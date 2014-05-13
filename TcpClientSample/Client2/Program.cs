﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();

            var client = new TcpClient();
            var serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);

            client.Connect(serverEndPoint);
            
            string message = "test";
            while (!string.IsNullOrEmpty(message) && message != "exit")
            {
                message = Console.ReadLine();
                var clientStream = client.GetStream();
                var encoder = new ASCIIEncoding();
                byte[] buffer = encoder.GetBytes(message);
                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();                
            }

            client.Close();      
        }
    }
}