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
                if (cliente.ReceiveResponse() == "exit")
                    form.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string a = textBox1.Text;
            string nombre = "nombre" + a;
            cliente.SendString(nombre);
            string respuesta = cliente.ReceiveResponse();
            if (respuesta == "ok")
            {


                this.Hide();


                Form1 form1 = new Form1();
                form1.Show();
                form1.Visible = true;



            }
            else
                textBox1.Text="";

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cliente.SendString("prueba");
        }
    }
}
