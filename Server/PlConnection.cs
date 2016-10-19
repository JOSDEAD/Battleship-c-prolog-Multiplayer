using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SbsSW.SwiPlCs;

namespace Server
{
    class PlConnection
    {

        /// <summary>
        /// Crea la base de conocimientos a partir de una matriz. El nickname NO lleva .pl
        /// </summary>
        /// <param name="matriz de 2 dimensiones"></param>
        /// <param name="nickname"></param>
        public static void crearMatriz(int[,] matriz, string nickname)
        {
            if (!PlEngine.IsInitialized)
            {
                Environment.SetEnvironmentVariable("PATH", @"C:\\Program Files (x86)\\swipl\\bin");
                //Por cada fila
                for (int i = 0; i < matriz.GetLength(0); i++)
                {
                    //Se agregan los datos de la columna
                    string columnas = "[";
                    for(int j = 0; j < matriz.GetLength(1); j++)
                    {
                        columnas += matriz[i, j].ToString();
                        if (j != matriz.GetLength(1) - 1) columnas += ",";
                    }
                    columnas += "]";
                    //Se crea el hecho
                    String[] param = { "-q", "-f", @nickname + ".pl" };
                    PlEngine.Initialize(param);
                    string query = String.Format("assertz(fila({0},{1}))", i.ToString(), columnas) + ".";
                    string[] execute = { query, "tell('" + nickname + ".pl" + "')", "listing(fila)", "told" };
                    foreach (string q in execute)
                    {
                        PlQuery.PlCall(q);
                    }
                    PlEngine.PlCleanup();
                }
            }
        }


        /// <summary>
        /// Indica si existe un barco en una posicion especifica de la matriz. El nombre del archivo lleva .pl
        /// </summary>
        /// <param name="fila"></param>
        /// <param name="columna"></param>
        /// <param name="nombreArchivo"></param>
        /// <returns></returns>
        public static bool hayBarco(int fila, int columna, string nombreArchivo)
        {
            Environment.SetEnvironmentVariable("PATH", @"C:\\Program Files (x86)\\swipl\\bin");
            String[] param = { "-q", @"reglas.pl" };
            PlEngine.Initialize(param);
            string query = String.Format("comprobar({0},{1},{2})", fila.ToString(), columna.ToString(), "'" + nombreArchivo + "'") + ".";

            return PlQuery.PlCall(query);
        }


        /// <summary>
        /// Borra todos los archivos prolog dentro de la carpeta del proyecto excepto el archivo de reglas.
        /// </summary>
        public static  void borrarPl()
        {
            Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = "cmd";
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            //Se posiciona en el directorio actual
            proc.StandardInput.WriteLine(@"cd %~dp0");
           
            proc.StandardInput.WriteLine(@"rename reglas.pl reglas.bak");
            proc.StandardInput.WriteLine(@"DEL *.pl");
            //Se respalda el archivo de reglas
            proc.StandardInput.WriteLine(@"rename reglas.bak reglas.pl");
            proc.StandardInput.Flush();
            
        }
    }
}
