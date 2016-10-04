using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatallaNaval
{
    public partial class Form1 : Form
    {
        static Cliente cliente;
        public int[,] tablero = new int[3, 3] {
                 
             { 0, 0, 0 },
             { 0, 0, 0},
             { 0, 0, 0},

         };
        public Form1()
        {
           
            InitializeComponent();
            cargar();
            cliente = Cliente.Instance;
        }

        public void cargar()
        {
            
                    for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {

                    Button button = new Button();
                    button.Tag = i.ToString() + "," + j.ToString();
                    button.Height = 80;
                    button.Width = 80;
                    button.Margin = new System.Windows.Forms.Padding(0);
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.Text = tablero[i,j].ToString();
                    button.Click += (s, e) =>
                    {
                        buttonAction(button);
                    };
                        tableLayoutPanel1.Controls.Add(button);

                }
            }
        }

        private void buttonAction(Button button)
        {
            string[] c = button.Tag.ToString().Split(',');
            if (button.Text == "0")
            {

                this.tablero[int.Parse(c[0]), int.Parse(c[1])] = 1;
            }
            else if(button.Text=="1") {
                tablero[int.Parse(c[0]), int.Parse(c[1])] = 0;

            }
            tableLayoutPanel1.Controls.Clear();
            cargar();
        }
        public static byte[] ToByteArray(int[,] input)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                var rows = input.GetLength(0);
                var cols = input.GetLength(1);
                writer.Write(rows);
                writer.Write(cols);
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        writer.Write(input[i, j]);
                    }
                }
                return stream.ToArray();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
           
            byte[] a = ToByteArray(tablero);
            cliente.SendMatrix(a);
        }
    }
}
