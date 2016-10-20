using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatallaNaval
{
    public partial class Partida : Form
    {
        Thread th;
        static Cliente cliente;
        int[,] miMatriz;
        TableLayoutPanel tableLayoutPanel1;
        TableLayoutPanel tableLayoutPanel2;
        int columns;
        int rows;
        Button temp;
        List<Button> listaBotones = new List<Button>();
        public Partida(int[,] matriz, int x, int y)
        {
            columns = x;
            rows = y;
            miMatriz = matriz;
            InitializeComponent();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            cargar();
            this.Controls.Add(tableLayoutPanel1);
            this.Controls.Add(tableLayoutPanel2);
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel2.AutoSize = true;
            this.AutoSize = true;
            cliente = Cliente.Instance;
            CheckForIllegalCrossThreadCalls = false;
            th = new Thread(() => WorkThreadFunction(this));
            th.Start();
        }

        private void WorkThreadFunction(Partida partida)
        {
            while (true)
            {
                string respuesta = cliente.ReceiveResponse();
                if (respuesta == "exit")
                    partida.Close();
                else if (respuesta == "True"){
                    if (InvokeRequired)
                    {
                        this.Invoke(new Action(() => hit()));

                    }

                }
                else if (respuesta == "False")
                {
                    if (InvokeRequired)
                    {
                        this.Invoke(new Action(() => miss()));

                    }

                }
                else if (respuesta.Contains("hit"))
                {
                    string temp = respuesta.Remove(0, 3);
                    
                    if (InvokeRequired)
                    {
                        this.Invoke(new Action(() => barcoDestruido(temp)));

                    }

                }


            }
        }

        private void barcoDestruido(string lista)
        {
            
            string[] lista1 = lista.Split(',');
            foreach(Button b in listaBotones)
            {
                if (b.Tag.ToString() == lista)
                {
                    b.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.BattleshipHit));
                    break;
                }
            }
            this.Refresh();
        }

        private void miss()
        {
            temp.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.WaterMiss));
            temp.Enabled = false;
            temp = null;
            this.Refresh();
        }

        private void hit()
        {
            temp.BackgroundImage=((System.Drawing.Image)(Properties.Resources.BattleshipHit));
            temp.Enabled = false;
            temp = null;
            this.Refresh();
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
                    if (miMatriz[i, j] == 0)
                        button.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.Water));
                    else if (miMatriz[i, j] == 1)
                        button.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.Battleship));
                    button.Name = miMatriz[i, j].ToString();
                    tableLayoutPanel1.Controls.Add(button);
                    listaBotones.Add(button);
                }
            }
            tableLayoutPanel2.ColumnCount = columns;
            tableLayoutPanel2.RowCount = rows;
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

                    button.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.Water));

                    button.Name = "0";
                    button.Click += (s, e) =>
                    {
                        buttonAction(button);
                    };
                    tableLayoutPanel2.Controls.Add(button);

                }
            }
            Size p = tableLayoutPanel1.Size;
            tableLayoutPanel2.Location = new Point(p.Width + 500, 0);
        }

        private void buttonAction(Button button)
        {
            string[] c = button.Tag.ToString().Split(',');
            if (button.Name == "0")
            {

                cliente.SendString("haybarco"+button.Tag.ToString());
                temp = button;
              
                
            }

           

        }
    }
}
