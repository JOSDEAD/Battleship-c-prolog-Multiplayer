using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clientSockets = new List<Socket>();
        private const int BUFFER_SIZE = 2048;
        private const int PORT = 8080;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
        private static Dictionary<string, Socket> conectados = new Dictionary<string, Socket>();

        static void Main()
        {
            Console.Title = "Server";
            SetupServer();
            Console.ReadLine(); // When we press enter close everything
            CloseAllSockets();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Iniciando el servidor...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Servidor en marcha...");
        }

        /// <summary>
        /// Close all connected client (we do not need to shutdown the server socket as its connections
        /// are already closed with the clients).
        /// </summary>
        private static void CloseAllSockets()
        {
            foreach (Socket socket in clientSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            serverSocket.Close();
        }
        public static int[,] FromByteArray(byte[] input)
        {
            using (var stream = new MemoryStream(input))
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                var rows = reader.ReadInt32();
                var cols = reader.ReadInt32();
                var result = new int[rows, cols];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = reader.ReadInt32();
                    }
                }
                return result;
            }
        }


        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }

            clientSockets.Add(socket);
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            Console.WriteLine("Cliente Conectado Esperando Solicitud");
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Cliente se desconecto a la fuerza");
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                current.Close();
                clientSockets.Remove(current);
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            string text = Encoding.ASCII.GetString(recBuf);
            Console.WriteLine("Solicitud recivida");

            if (text.ToLower() == "get time") // Client requested time
            {
                Console.WriteLine("Text is a get time request");
                byte[] data = Encoding.ASCII.GetBytes(DateTime.Now.ToLongTimeString());
                Socket current1 = clientSockets[1];
                current1.Send(data);
                Console.WriteLine("Time sent to client");
            }
            else if (text.ToLower() == "exit") // Client wants to exit gracefully
            {
                // Always Shutdown before closing
                current.Shutdown(SocketShutdown.Both);
                current.Close();
                clientSockets.Remove(current);
                Console.WriteLine("Client disconnected");
                return;
            }
            else if (text.ToLower() == "prueba") // Client wants to exit gracefully
            {
                byte[] data = Encoding.ASCII.GetBytes("exit");
                Socket current1 = clientSockets[1];
                current1.Send(data);
            }
            else if (text.Contains("nombre")) // nombre del cliente
            {
                if (conectados.ContainsKey(text.Remove(0, 6)))
                {
                    byte[] data = Encoding.ASCII.GetBytes("false");
                    current.Send(data);
                }
                else
                {
                    conectados.Add(text.Remove(0, 6), current);
                    Console.WriteLine("Cliente " + text.Remove(0, 6) + " conectado");
                    byte[] data = Encoding.ASCII.GetBytes("ok");
                    current.Send(data);
                }
            }
            else
            {


                /* if ( current.Equals(clientSockets[0])){
                     Socket current1 = clientSockets[1];
                     current1.Send(recBuf);
                     Console.WriteLine("msg enviado to cliento 1");
                 }
                 if (current.Equals(clientSockets[1]))
                 {
                     Socket current1 = clientSockets[0];
                     current1.Send(recBuf);
                     Console.WriteLine("msg enviado to cliento 0");
                 }*/
                int[,] tablero = FromByteArray(recBuf);
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Console.Write(tablero[i, j] + ",");

                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                byte[] data = Encoding.ASCII.GetBytes("Gracias");
                current.Send(data);

            }

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }
    }
}
