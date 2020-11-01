using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tcpserver
{
    public partial class Form1 : Form
    {

        private TcpClient client;
        public StreamReader read;
        public StreamWriter write;
        public string texttosend;
        public string texttorecieve;

        public Form1()
        {
            InitializeComponent();

            IPAddress[] localip = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress address in localip)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    textBox1.Text = address.ToString();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TcpListener listner = new TcpListener(IPAddress.Any, int.Parse(textBox2.Text));
            listner.Start();
            client = listner.AcceptTcpClient();
            read = new StreamReader(client.GetStream());
            write = new StreamWriter(client.GetStream());
            write.AutoFlush = true;
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.WorkerSupportsCancellation = true;

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    texttorecieve = read.ReadLine();
                    this.ChatDisplay.Invoke(new MethodInvoker(delegate ()
                    {
                        ChatDisplay.AppendText("Client: " + texttorecieve + "\n");
                    }));

                    texttorecieve = "";
                }
                catch (Exception er)
                {
                    MessageBox.Show(er.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                write.WriteLine(texttosend);
                this.ChatDisplay.Invoke(new MethodInvoker(delegate ()
                {
                    ChatDisplay.AppendText("Me: " + texttosend + "\n");
                }));
            }
            else
            {
                MessageBox.Show("Sending failed");
            }

            backgroundWorker2.CancelAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox3.Text != "")
            {
                texttosend = textBox3.Text;
                backgroundWorker2.RunWorkerAsync();
            }

            textBox3.Clear();
        }
    }
}
