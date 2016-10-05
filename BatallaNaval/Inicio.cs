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
                    Console.WriteLine("Va a entrar aqui");
                    if (InvokeRequired)
                    {
                        this.Invoke(new Action(() => openNextForm()));
                        return;
                    }
                    
                    th.Abort();
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
                    
            }
        }
        private void openNextForm()
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
            form1.Visible = true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string a = textBox1.Text;
            string nombre = "nombre" + a;
            cliente.SendString(nombre);


            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cliente.SendString("prueba");
        }
    }
}
