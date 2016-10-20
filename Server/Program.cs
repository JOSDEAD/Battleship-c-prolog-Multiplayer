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
        static partida partida;
        private const int BUFFER_SIZE = 2048;
        private const int PORT = 8080;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
        private static Dictionary<string, Socket> conectados = new Dictionary<string, Socket>();
        
        static void Main()
        {
            PlConnection.borrarPl();
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
        private void update()
        {

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

           
             if (text.ToLower() == "exit") // Client wants to exit gracefully
            {
                // Always Shutdown before closing

                current.Shutdown(SocketShutdown.Both);
                current.Close();
                clientSockets.Remove(current);
                Console.WriteLine("Client disconnected");

                return;
            }
            else if (text.Contains("exit")) // Client wants to exit gracefully
            {
                // Always Shutdown before closing
                string nombre = text.Remove(0, 4);
                conectados.Remove(nombre);
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
                    string nombre = text.Remove(0, 6);
                    conectados.Add(nombre, current);
                    Console.WriteLine("Cliente " + nombre + " conectado");
                    byte[] data = Encoding.ASCII.GetBytes("ok");
                    current.Send(data);
                    
                    foreach (KeyValuePair<string, Socket> g in conectados)
                    {
                        string users = "";
                        foreach (string d in conectados.Keys)
                        {
                            if (g.Key != d)
                                users += "|" + d;
                        }
                        if (users != "")
                        {
                            string peticion = "update" + users;
                            g.Value.Send(Encoding.ASCII.GetBytes(peticion));
                        }
                    }
                }
            }
            else if (text.Contains("invitacion"))
            {
                string temp = text.Remove(0, 10);
                string[] lista = temp.Split('|');
                conectados[lista[1]].Send(Encoding.ASCII.GetBytes("invitado"+lista[0]));
            }
            else if (text.Contains("acepto"))
            {
                string temp = text.Remove(0, 6);
                string[] lista = temp.Split('|');
                Jugador jugador1 = new Jugador(lista[0]);
                Jugador jugador2 = new Jugador(lista[1]);
                partida = new partida(jugador1, jugador2);
                
                conectados[partida.getJudador1().getJugador()].Send(Encoding.ASCII.GetBytes("dificultad"));
                conectados[partida.getJudador2().getJugador()].Send(Encoding.ASCII.GetBytes("espere"+partida.getJudador1().getJugador()));
                
            }
            else if (text.Contains("crearMatrix"))
            {
                string temp = text.Remove(0, 11);
                partida.setDificultad(temp);
                conectados[partida.getJudador1().getJugador()].Send(Encoding.ASCII.GetBytes("partida"+temp));
                conectados[partida.getJudador2().getJugador()].Send(Encoding.ASCII.GetBytes("partida" + temp));
            }
            else if (text.Contains("haybarco"))
            {
                string temp = text.Remove(0, 8);
                string[] lista = temp.Split(',');
                string respuesta = "";
                
                if (current == conectados[partida.getJudador1().getJugador()]) {
                    respuesta = (PlConnection.hayBarco(int.Parse(lista[0]), int.Parse(lista[1]), partida.getJudador2().getJugador()+".pl")).ToString();
                    conectados[partida.getJudador1().getJugador()].Send(Encoding.ASCII.GetBytes(respuesta));
                    Console.WriteLine(respuesta);
                    if (respuesta=="True")
                        conectados[partida.getJudador2().getJugador()].Send(Encoding.ASCII.GetBytes("hit"+temp));
                }
                else if (current == conectados[partida.getJudador2().getJugador()])
                {
                    respuesta = (PlConnection.hayBarco(int.Parse(lista[0]), int.Parse(lista[1]), partida.getJudador1().getJugador() + ".pl")).ToString();
                    conectados[partida.getJudador2().getJugador()].Send(Encoding.ASCII.GetBytes(respuesta));
                    Console.WriteLine(respuesta);
                    if (respuesta == "True")
                        conectados[partida.getJudador1().getJugador()].Send(Encoding.ASCII.GetBytes("hit" + temp));
                }
           
            }
            else
            {



                int[,] tablero = FromByteArray(recBuf);
                int[] temp = partida.getSize();
                for (int i = 0; i < temp[0]; i++)
                {
                    for (int j = 0; j < temp[1]; j++)
                    {
                        Console.Write(tablero[i, j] + ",");

                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                string solicitante="";
                foreach(string a in conectados.Keys)
                {
                    if (conectados[a] == current)
                        solicitante = a;
                }
                if (solicitante == partida.getJudador1().getJugador())
                {
                    
                    partida.getJudador1().setStatus("listo");
                    partida.getJudador1().setMatrix(tablero);
                    PlConnection.crearMatriz(tablero,partida.getJudador1().getJugador());
                    if (partida.getJudador2().getStatus() == "listo")
                    {
                        conectados[partida.getJudador1().getJugador()].Send(Encoding.ASCII.GetBytes("partida" ));
                        conectados[partida.getJudador2().getJugador()].Send(Encoding.ASCII.GetBytes("partida" ));
                    }
                    else
                    {
                        conectados[partida.getJudador2().getJugador()].Send(Encoding.ASCII.GetBytes("listo"+ partida.getJudador1().getJugador()));
                    }
                }
                else if (solicitante == partida.getJudador2().getJugador())
                {
                    partida.getJudador2().setStatus("listo");
                    partida.getJudador2().setMatrix(tablero);
                    PlConnection.crearMatriz(tablero, partida.getJudador2().getJugador());
                    if (partida.getJudador1().getStatus() == "listo")
                    {
                        conectados[partida.getJudador1().getJugador()].Send(Encoding.ASCII.GetBytes("partida" ));
                        conectados[partida.getJudador2().getJugador()].Send(Encoding.ASCII.GetBytes("partida" ));
                    }
                    else
                    {
                        conectados[partida.getJudador1().getJugador()].Send(Encoding.ASCII.GetBytes("listo"+ partida.getJudador2().getJugador()));
                    }
                }



            }

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }
    }
}
