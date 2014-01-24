using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ChatMat
{
    class Server
    {
        Socket serverSocket;
        Socket clientSocket;
        byte[] buffer;

        public Server()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            buffer = new byte[1024];
        }

        public void Start()
        {
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4444));
            serverSocket.Listen(5);
            serverSocket.BeginAccept(AcceptConnection, serverSocket);
        }

        public void Connect(string ip, int port)
        {
            //results.Items.Add("Connecting...");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(ip), port);
            clientSocket.BeginConnect(iep, new AsyncCallback(Connected), clientSocket);
        }

        void Connected(IAsyncResult iar)
        {
            try
            {
                clientSocket.EndConnect(iar);
                //results.Items.Add("Connected to: " + client.RemoteEndPoint.ToString());
                Thread receiver = new Thread(new ThreadStart(ReceiveData));
                receiver.Start();
            }
            catch (SocketException)
            {
                //results.Items.Add("Error connecting");
            }
        }

        void AcceptConnection(IAsyncResult iar)
        {
            Socket oldserver = (Socket)iar.AsyncState;
            clientSocket = oldserver.EndAccept(iar);
            clientSocket.Send(Encoding.UTF8.GetBytes("Connection from: " + clientSocket.RemoteEndPoint.ToString()));
            Thread receiver = new Thread(new ThreadStart(ReceiveData));
            receiver.Start();
        }

        public void Send(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            clientSocket.BeginSend(messageBytes, 0, messageBytes.Length, 0, new AsyncCallback(SendData), clientSocket);
        }

        void SendData(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            int sent = remote.EndSend(iar);
        }

        void ReceiveData()
        {
            int len;
            string data;
            while (true)
            {
                try
                {
                    len = clientSocket.Receive(buffer);
                }
                catch (Exception)
                {
                    clientSocket.Close();
                    return;
                }
                data = Encoding.UTF8.GetString(buffer, 0, len);
                if (data == "/quit\n")
                {
                    clientSocket.Send(Encoding.UTF8.GetBytes("Bye"));
                    clientSocket.Close();
                    return;
                }
                clientSocket.Send(Encoding.UTF8.GetBytes("RCVD"));

            }
        }

        public void Stop()
        {
            serverSocket.Close();
        }
    }
}
