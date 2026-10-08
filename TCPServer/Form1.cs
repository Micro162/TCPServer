using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TCPServer
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        private Thread serverThread;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serverThread = new Thread(StartServer);
            serverThread.IsBackground = true;
            serverThread.Start();

            button1.Enabled = false;
        }

        private void StartServer()
        {
            server = new TcpListener(IPAddress.Any, 5000);
            server.Start();

            AddText("Сервер запущен.");
            AddText("Ожидание подключения клиента...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                AddText("Клиент подключился.");

                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                while (true)
                {
                    int bytes = stream.Read(buffer, 0, buffer.Length);

                    if (bytes == 0)
                        break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytes).Trim();

                    AddText("Клиент: " + message);

                    string response;

                    if (message.ToLower() == "привет")
                    {
                        int hour = DateTime.Now.Hour;

                        if (hour >= 5 && hour < 12)
                            response = "Доброго ранку!";
                        else if (hour >= 12 && hour < 18)
                            response = "Доброго дня!";
                        else if (hour >= 18 && hour < 23)
                            response = "Доброго вечора!";
                        else
                            response = "Доброї ночі!";
                    }
                    else if (message.ToLower() == "час")
                    {
                        response = "Поточний час: " + DateTime.Now.ToString("HH:mm:ss");
                    }
                    else
                    {
                        response = "Невідомий запит.";
                    }

                    byte[] data = Encoding.UTF8.GetBytes(response);

                    stream.Write(data, 0, data.Length);

                    AddText("Сервер: " + response);
                }

                client.Close();

                AddText("Клиент отключился.");
            }
        }

        private void AddText(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddText(text)));
                return;
            }

            textBox1.AppendText(text + Environment.NewLine);
        }
    }
}