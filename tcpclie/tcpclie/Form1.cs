using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Collections;
namespace tcpclie
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private Thread client_th;
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
         
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textPort.Text != "" && textIP.Text != "")
            {
                string mystr = textIP.Text;
                string ip = mystr;
                //1.intA =int.Parse(str);  string to int
                //2.int.TryParse(str, out intA);
                //3.intA = Convert.ToInt32(str);
                int port = int.Parse(textPort.Text);
                this.connect_s(ip, port);
            }
             else
            {
                MessageBox.Show("please input ip and port,then connect net");
            }

        }
        public void connect_s(string ip,int port)
        {
            try
            {
                client = new TcpClient(ip, port);
                ThreadStart threadStart = new ThreadStart(AcceptMsg);
                client_th = new Thread(threadStart);
                client_th.Start();
                textBox2.Text = "连接成功";

            }
            catch (System.Exception e)
            {
                textBox2.Text = e.ToString(); 

            }
        }
        private void AcceptMsg()
        {
            NetworkStream ns = client.GetStream();

            //StreamReader sr = new StreamReader(ns);//流读写器
            //字组处理
            while (true)
            {
                try
                {
                    byte[] bytes = new byte[1024];
                    int bytesread = ns.Read(bytes, 0, bytes.Length);
                    string msg = Encoding.Default.GetString(bytes, 0, bytesread);
                    //显示
                    //MessageBox.Show(msg);
                    textBox2.Text = msg;
                    ns.Flush();
                    //ns.Close();
                }
                catch (System.Exception e)
                {
                    textBox2.Text = e.ToString(); 
                    MessageBox.Show("与服务器断开连接了");
                    break;
                }
            }
        }
 
        private void button2_Click(object sender, EventArgs e)
        {
            if (client == null)
            {
                MessageBox.Show("client is disconnect,should connect and input message again");
                return;
            }
            NetworkStream sendStream = client.GetStream();
            String msg = textBox1.Text;
            Byte[] sendBytes = Encoding.Default.GetBytes(msg);
            sendStream.Write(sendBytes, 0, sendBytes.Length);
            sendStream.Flush();
            //sendStream.Close();
            textBox1.Text = "Msg Sent!";
        
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
