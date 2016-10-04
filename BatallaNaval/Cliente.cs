using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace BatallaNaval
{
    class Cliente
    {

        private static Cliente instance;

        private Cliente() { }

        public static Cliente Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Cliente();
                    ConnectToServer();
                }
                return instance;
            }
        }


        private static readonly Socket ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int PORT = 8080;



        public static void ConnectToServer()
        {
            int attempts = 0;

            while (!ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt " + attempts);
                    // Change IPAddress.Loopback to a remote IP to connect to a remote host.
                    ClientSocket.Connect("172.24.45.125", PORT);
                }
                catch (SocketException)
                {
                    Console.Clear();
                }
            }

            
            Console.WriteLine("Connected");
        }

        /*private static void RequestLoop()
        {
            Console.WriteLine(@"<Type ""exit"" to properly disconnect client>");

            while (true)
            {
                SendRequest();
                ReceiveResponse();
            }
        }*/

        /// <summary>
        /// Close socket and exit program.
        /// </summary>
        public  void Exit()
        {
            SendString("exit"); // Tell the server we are exiting
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            Environment.Exit(0);
        }

        public  void SendRequest(string request)
        {

            SendString(request);

            if (request.ToLower() == "exit")
            {
                Exit();
            }
        }

        /// <summary>
        /// Sends a string to the server with ASCII encoding.
        /// </summary>
        public  void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }
        public  void SendMatrix(byte [] a)
        {   
            ClientSocket.Send(a, 0, a.Length, SocketFlags.None);
        }

        public  string ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return "";
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            return text;
        }
    }
}
