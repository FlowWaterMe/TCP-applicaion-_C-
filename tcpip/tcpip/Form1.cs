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
namespace tcpip
{
    public partial class form1 : Form
    {
        private ThreadStart start;
        private Thread listenThread, client_th;
        static private bool m_bListening = false;
        static private System.Net.IPAddress MyIP = System.Net.IPAddress.Parse("127.0.0.2");//127.0.0.2
        static private TcpListener listener = new TcpListener(MyIP, 5567);

        
        private String msg;
        ArrayList clientArray = new ArrayList();

        public form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            start = new ThreadStart(startListen);//创建Thread类的对象，生成一个子线程
            listenThread = new Thread(start); 
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (m_bListening)
            {
                m_bListening = false;
                label2.Text = "Server Idle...";
            }
            else
            {
                m_bListening = true;
                if (TextIp.Text!="" && textPort.Text != "" )
                {
                    string strip = TextIp.Text;
                    int port = int.Parse(textPort.Text);
                    System.Net.IPAddress MyIP = System.Net.IPAddress.Parse(strip);//127.0.0.2
                    TcpListener listener = new TcpListener(MyIP, port);
                    start = new ThreadStart(startListen);//创建Thread类的对象，生成一个子线程
                    listenThread = new Thread(start); 
                }
                listenThread.Start();
                label2.Text = "Server Listening...";
            }
        }
        private void showmore()
        {
            textBox1.Text = msg;
        }

        private void startListen()
        {
            listener.Start();
            while (m_bListening)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    clientArray.Add(client);
                    ParameterizedThreadStart threadStart = new ParameterizedThreadStart(AcceptMsg);
                    client_th = new Thread(threadStart);
                    client_th.Start(client);

                }
                catch (Exception re)
                {
                    MessageBox.Show(re.Message);
                }
            }
            listener.Stop();
        }

        private void AcceptMsg(object arg)
        {
            TcpClient client = (TcpClient)arg;
            NetworkStream ns = client.GetStream();
            while (true)
            {
                try
                {
                    byte[] bytes = new byte[1024];
                    int bytesread = ns.Read(bytes, 0, bytes.Length);
                    msg = Encoding.Default.GetString(bytes, 0, bytesread);
                    //显示
                    //MessageBox.Show(msg);
                    showmore();
                    ns.Flush();
                    //ns.Close();
                }
                catch
                {
                    MessageBox.Show("与客户端断开连接");
                    break;
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text!="")
            {
                object a = clientArray[0];
                TcpClient client = (TcpClient)clientArray[0];
                if (client == null)
                    return;
                NetworkStream sendStream = client.GetStream();
                String msg = textBox2.Text;
                Byte[] sendBytes = Encoding.Default.GetBytes(msg);
                sendStream.Write(sendBytes, 0, sendBytes.Length);
                sendStream.Flush();
            }
            else{
                MessageBox.Show("please input message to box");
            }
            //sendStream.Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

    }
}
