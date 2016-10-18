using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatallaNaval
{
    class perfil
    {
        private static perfil instance;
        private static string nombre;
        private perfil() { }

        public static perfil Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new perfil();
                }
                return instance;
            }
        }
        public string getname()
        {
            return nombre;
        }
        public void setname(string a)
        {
            nombre=a;
        }
    }
}
