using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatallaNaval
{
    public partial class Form1 : Form
    {
        Thread th;
        static int rows;
        static int columns;
        static int MaxBarcos;
        static int barcos;
        static Cliente cliente;
        TableLayoutPanel tableLayoutPanel1;
        public int[,] tablero;
        public Form1(int rows1,int columns1,int barcos1)
        {
            
            MaxBarcos = barcos1;
            rows = rows1;
            columns = columns1;
            tablero = new int[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    tablero[i, j] = 0;
                }
            }
            this.AutoSize = true;
            tableLayoutPanel1 = new TableLayoutPanel();

            tableLayoutPanel1.AutoSize = true;
            this.Controls.Add(tableLayoutPanel1);
            
            InitializeComponent();
            cliente = Cliente.Instance;
            CheckForIllegalCrossThreadCalls = false;
            th = new Thread(() => WorkThreadFunction(this));
            th.Start();
            cargar();
            
        }
       private void WorkThreadFunction(Form form)
        {
            while (true)
            {
                string respuesta = cliente.ReceiveResponse();
                if (respuesta == "exit")
                    form.Close();
                else if (respuesta == "partida")
                {

                    if (InvokeRequired)
                    {
                        this.Invoke(new Action(() => prueba()));

                    }

                    //th.Abort();


                }
                else if (respuesta.Contains("listo"))
                {
                    string temp = respuesta.Remove(0, 5);
                    if (InvokeRequired)
                    {
                        this.Invoke(new Action(() => falta(temp)));

                    }

                    //th.Abort();


                }

            }
        }

        private void falta(string temp)
        {
            
            Size p = tableLayoutPanel1.Size;
            label1.Location = new Point(10, p.Height + 40);
            label1.Text = temp+" esta listo";
            label1.Visible = true;
            this.Refresh();
        }

        private void prueba()
        {
            this.Hide();
        }

        public void cargar()
        {

            tableLayoutPanel1.ColumnCount = columns;
            tableLayoutPanel1.RowCount = rows;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {

                    Button button = new Button();
                    button.Tag = i.ToString() + "," + j.ToString();
                    button.Height = 100;
                    button.Width = 100;
                    button.Margin = new System.Windows.Forms.Padding(0);
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    if(tablero[i,j]==0)
                        button.BackgroundImage= ((System.Drawing.Image)(Properties.Resources.Water));
                    else if(tablero[i,j]==1)
                        button.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.Battleship));
                    button.Name = tablero[i, j].ToString();
                    button.Click += (s, e) =>
                    {
                        buttonAction(button);
                    };
                        tableLayoutPanel1.Controls.Add(button);

                }
            }
            Size p = tableLayoutPanel1.Size;
            button1.Location = new Point((p.Width / 2) - 50, p.Height + 10);
        }

        private void buttonAction(Button button)
        {
            string[] c = button.Tag.ToString().Split(',');
            if (button.Name == "0")
            {
                if (barcos < MaxBarcos)
                {
                    this.tablero[int.Parse(c[0]), int.Parse(c[1])] = 1;
                    barcos++;
                    button.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.Battleship));
                    button.Name = "1";
                }
            }
            else if(button.Name == "1") {
                barcos--;
                this.tablero[int.Parse(c[0]), int.Parse(c[1])] = 0;
                button.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.Water));
                button.Name = "0";
            }
            this.Refresh();
            
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
            DialogResult dialogResult = MessageBox.Show("¿Estas Listo?","listo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                byte[] a = ToByteArray(tablero);
                cliente.SendMatrix(a);

            }
            else if (dialogResult == DialogResult.No)
            {

            }
           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cliente.SendString("exit" + perfil.Instance.getname());
            th.Abort();
        }
    }
}
