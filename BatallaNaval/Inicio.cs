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
    public partial class Inicio : Form
    {
        Thread th;
        static Cliente cliente;
        public Inicio()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            cliente = Cliente.Instance;
            th=new Thread(()=>WorkThreadFunction(this));
            th.Start();
        }

        private void WorkThreadFunction(Form form)
        {
            while (true)
            {
                string respuesta = cliente.ReceiveResponse();
                if (respuesta == "exit")
                    form.Close();
                else if (respuesta == "ok") {
                    
                    if (InvokeRequired)
                    {
                        this.Invoke(new Action(() => userHome()));
                        
                    }
                    
                    //th.Abort();
                    
                 
                }
                else if (respuesta.Contains("partida"))
                {
                    int x=0;
                    int y=0;
                    int barcos = 0;
                    string temp = respuesta.Remove(0, 7);
                    if (temp == "Facil")
                    {
                        x = 4; y = 4;   barcos=3;
                    }
                    if (temp == "Medio")
                    {
                        x = 5; y = 5; barcos = 4;
                    }
                    if (temp == "Dificil")
                    {
                        x = 6; y = 6; barcos = 5;
                    }

                    if (InvokeRequired)
                    {
                        this.Invoke(new Action(() => openNextForm(x,y,barcos)));

                    }

                    th.Abort();


                }
                else if (respuesta == "dificultad")
                {

                    if (InvokeRequired)
                    {
                        this.Invoke(new Action(() => dificultad()));

                    }

                    


                }
                else if (respuesta.Contains("espere"))
                {
                    string enemigo=respuesta.Remove(0, 6);
                    if (InvokeRequired)
                    {
                        this.Invoke(new Action(() => espera(enemigo)));

                    }

                    


                }
                else if (respuesta.Contains("invitado"))
                {
                    string temp = respuesta.Remove(0, 8);
                    DialogResult dialogResult = MessageBox.Show("Invitacion a partida de " + temp, "Invitar", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        cliente.SendString("acepto" + temp + "|" + perfil.Instance.getname());
                    }
                    else if (dialogResult == DialogResult.No)
                    {

                    }

                }
                else if (respuesta == "false")
                {
                    textBox1.Text = "";

                    string message = "Un usuario con el mismo nombre se encuentra conectado";
                    string caption = "Un usuario con el mismo nombre se encuentra conectado";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    // Displays the MessageBox.

                    result = MessageBox.Show(message, caption, buttons);
                }
                else if (respuesta.Contains("update"))
                {
                    
                    string temp = respuesta.Remove(0, 7);
                    string[] lista = temp.Split('|');

                    if (InvokeRequired)
                    {
                        this.Invoke(new Action(() => update(lista)));
                       
                    }
                }
                    
            }
        }

        private void dificultad()
        {
            Console.WriteLine("entro");
            panel1.Visible = false;
            panel3.Visible = true;
        }

        private void espera(string enemigo)
        {
            label2.Text = "Espera mientras "+enemigo+" configura la partida";
            panel1.Visible = false;
            panel2.Visible = true;
        }

        private void openNextForm(int x,int y,int barcos)
        {
            this.Hide();
            Form1 form1 = new Form1(x,y,barcos);
            form1.Show();
            form1.Visible = true;
        }

        private void userHome()
        {
            label1.Text = "Bienvenido " + perfil.Instance.getname(); 
           
            panel1.Visible = true;
        }
        private void update(string[] lista)
        {
            listBox1.Items.Clear();
            foreach (string g in lista)
            {
          
                
                listBox1.Items.Add(g);
               
            }
           
            this.Refresh();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string a = textBox1.Text;
            string nombre = "nombre" + a;
            perfil.Instance.setname(a);
            cliente.SendString(nombre);


            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cliente.SendString("prueba");
        }

        private void Inicio_FormClosing(object sender, FormClosingEventArgs e)
        {
            cliente.SendString("exit"+ perfil.Instance.getname());
            th.Abort();
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
         
                
            try
            {
                DialogResult dialogResult = MessageBox.Show("Quieres invitar a " + listBox1.SelectedItem.ToString(), "Invitar", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    cliente.SendString("invitacion" + perfil.Instance.getname() + "|" + listBox1.SelectedItem.ToString());
                }
                else if (dialogResult == DialogResult.No)
                {

                }
            }
            catch {
                MessageBox.Show("Ningun usuario seleccionado");
            }
        }

        private void Facil_Click(object sender, EventArgs e)
        {
            if (((Button)sender).Name == "Facil")
                cliente.SendString("crearMatrixFacil");
            else if (((Button)sender).Name == "Medio")
                cliente.SendString("crearMatrixMedio");
            else if (((Button)sender).Name == "Dificil")
                cliente.SendString("crearMatrixDificil");
        }
    }
}
