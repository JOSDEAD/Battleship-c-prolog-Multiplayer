using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Jugador
    {
        string jugador;
        string status;
        int[,] matrix;
        public Jugador(string nombre)
        {
            jugador = nombre;
        }
        public string getJugador()
        {
            return jugador;
        }
        public string getStatus()
        {
            return status;
        }
        public int[,] getMatrix()
        {
            return matrix;
        }
        public void setStatus(string status)
        {
            this.status = status;
        }

        public void setMatrix(int [,] matrix)
        {
            this.matrix = matrix;
        }

    }
}
