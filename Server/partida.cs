using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class partida
    {
        Jugador jugador1;
        Jugador jugador2;
        string Dificultad;
        public partida(Jugador jugador1,Jugador jugador2)
        {
            this.jugador1 = jugador1;
            this.jugador2 = jugador2;
        }
        public Jugador getJudador1() {
            return jugador1;
        }
        public Jugador getJudador2()
        {
            return jugador2;
        }
        public void setDificultad(string dificultad)
        {
            this.Dificultad = dificultad;
        }
        public int[] getSize()
        {
            int[] temp= new int[2];
            if (Dificultad == "Facil")
            {
                temp[0] = 4;
                temp[1] = 4;  
            }
            if (Dificultad == "Medio")
            {
                temp[0] = 5;
                temp[1] = 5;
            }
            if (Dificultad == "Dificil")
            {
                temp[0] = 6;
                temp[1] = 6;
            }
            return temp;
        }
    }
}
