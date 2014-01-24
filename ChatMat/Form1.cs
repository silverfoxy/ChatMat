using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace ChatMat
{
    public partial class Form1 : Form
    {
        Socket serverSocket;
        Socket clientSocket;
        string ipAddr;
        int port;
        bool online = true;
        static ManualResetEvent allDone = new ManualResetEvent(false);

        public Form1()
        {
            InitializeComponent();
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            online = true;
            comboBox_Status.SelectedIndex = 0;
            Thread listener = new Thread(new ThreadStart(StartListening));
            listener.IsBackground = true;
            listener.Start();
        }

        private void button_Connect_Click(object sender, EventArgs e)
        {
            Connect(textBox_Address.Text.Split(':')[0], int.Parse(textBox_Address.Text.Split(':')[1]));
        }

        public void StartListening()
        {
            try
            {
                if (File.Exists("settings.txt"))
                {
                    using (StreamReader sr = new StreamReader("settings.txt"))
                    {
                        string addr = sr.ReadLine();
                        ipAddr = addr.Split(':')[0].ToString();
                        port = int.Parse(addr.Split(':')[1].ToString());
                    }
                }
                else
                {
                    ipAddr = "127.0.0.1";
                    port = 8080;
                }
            }
            catch (Exception)
            {
                port = 4444;
            }
            try
            {
                serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddr), port));
            }
            catch (Exception)
            {
                throw;
            }
            
            serverSocket.Listen(10);
            while (true)
            {
                allDone.Reset();
                serverSocket.BeginAccept(new AsyncCallback(AcceptConnection), serverSocket);
                allDone.WaitOne();
            }
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
                AppendTextBox(this, "Connected to: " + clientSocket.RemoteEndPoint.ToString() + "\r\n", false);
                clientSocket.Send(Encoding.UTF8.GetBytes("/s " + ipAddr + ":" + port.ToString() + "|" + "/name " + textBox_Username.Text));
                Thread receiver = new Thread(new ParameterizedThreadStart(ReceiveData));
                receiver.IsBackground = true;
                receiver.Start(new ClientSocketObj(clientSocket, true));
            }
            catch (SocketException)
            {
                AppendTextBox(this, "Error connecting\r\n", false);
            }
        }

        void AcceptConnection(IAsyncResult iar)
        {
            allDone.Set();
            Socket oldserver = (Socket)iar.AsyncState;
            clientSocket = oldserver.EndAccept(iar);
            //clientSocket.Send(Encoding.UTF8.GetBytes("Connection from: " + clientSocket.RemoteEndPoint.ToString() + "\r\n"));
            if (online)
                clientSocket.Send(Encoding.UTF8.GetBytes("/s " + ipAddr + ":" + port.ToString() + "|" + "/name " + textBox_Username.Text));
            AppendTextBox(this, "Connection from: " + clientSocket.RemoteEndPoint.ToString() + "\r\n", false);
            Thread receiver = new Thread(new ParameterizedThreadStart(ReceiveData));
            receiver.IsBackground = true;
            receiver.Start(new ClientSocketObj(clientSocket, false));
        }

        void OpenNewChatBox(ChatBox cb)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke((Action)delegate
                {
                    cb.Show();
                });
            }
        }

        void OpenFileTransferForm(FileTransfer ft)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke((Action)delegate
                {
                    ft.Show();
                });
            }
        }

        void FileTransferSetStatus(FileTransfer ft, string status)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke((Action)delegate
                {
                    ft.SetStatus(status);
                });
            }
        }

        void FileTransferSetPBar(FileTransfer ft, int max, int current)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke((Action)delegate
                {
                    ft.SetPBar(max, current);
                });
            }
        }

        void FileTransferCompleted(FileTransfer ft, string fileName)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke((Action)delegate
                {
                    ft.Completed(fileName);
                });
            }
        }

        public void Send(Form cb, string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            clientSocket.BeginSend(messageBytes, 0, messageBytes.Length, 0, new AsyncCallback(SendData), clientSocket);
            AppendTextBox(cb, "You: " + message, true);
        }

        void SendData(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            int sent = remote.EndSend(iar);
        }

        void ReceiveData(object clientSocketObj)
        {
            ClientSocketObj obj = (ClientSocketObj)clientSocketObj;
            Socket clientSocket = obj.socket;
            ChatBox cb = new ChatBox(clientSocket);
            FileTransfer ft = new FileTransfer();
            bool cbOpen = false;
            int len;
            string data;
            string clientName = string.Empty;
            byte[] buffer = new byte[1024];
            int flag = 0;
            string receivedPath = string.Empty;
            int fileNameLen = 0;
            int fileLen = 0;
            int received = 0;
            bool newClient = true;
            
            while (true)
            {
                try
                {
                    len = clientSocket.Receive(buffer);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    clientSocket.Close();
                    return;
                }
                data = Encoding.UTF8.GetString(buffer, 0, len);
                try
                {
                    if (data != "RCVD" && data != "[X]\r\n")
                    {
                        if (data == "/quit\n" || data == "/quit")
                        {
                            clientSocket.Send(Encoding.UTF8.GetBytes("Bye"));
                            AppendTextBox(cb, clientSocket.RemoteEndPoint.ToString() + " Left\r\n", true);
                            clientSocket.Close();
                            if (cb.IsHandleCreated)
                            {
                                CloseForm(cb);
                            }
                            return;
                        }
                        else if (data == "AreYouThere?")
                        {
                            if (online)
                                clientSocket.Send(Encoding.UTF8.GetBytes("yeah"));
                            else
                                clientSocket.Send(Encoding.UTF8.GetBytes("no"));
                            continue;
                        }
                        else if (data.Length > 2 && data.Substring(0, 2) == "/s")
                        {
                            if (!online && !obj.initiatedByUs)
                            {
                                clientSocket.Close();
                                return;
                            }
                            if (!cbOpen)
                            {
                                cbOpen = true;
                                OpenNewChatBox(cb);
                            }
                            newClient = false;
                            string addr = data.Substring(3, data.Length - 3).Split('|').GetValue(0).ToString();
                            string ip = addr.Split(':')[0].ToString();
                            int port = int.Parse(addr.Split(':')[1].ToString());
                            cb.RIpAddr = ip;
                            cb.Rport = port;

                            string name =data.Split('|').GetValue(1).ToString();
                            clientName = name.Substring(6, name.Length - 6);
                            SetFormText(cb, clientName);
                            continue;
                        }
                        else if (data.Contains("/name"))
                        {

                            clientName = data.Substring(data.IndexOf("/name") + 6, data.Length - 6);
                            SetFormText(cb, clientName);
                            //SetFormText(cb, "Friend");
                            continue;
                        }
                        else if (newClient)
                        {
                            if (len > 0)
                            {

                                if (flag == 0)
                                {
                                    fileNameLen = BitConverter.ToInt32(buffer, 0);
                                    string fileName = Encoding.UTF8.GetString(buffer, 4, fileNameLen);
                                    receivedPath = fileName;
                                    fileLen = BitConverter.ToInt32(buffer, 4 + fileNameLen);
                                    flag++;
                                    ft = new FileTransfer();
                                    OpenFileTransferForm(ft);
                                    FileTransferSetStatus(ft, string.Format("Receiving '{0}'  [{1} Bytes]", Path.GetFileName(receivedPath), fileLen));
                                }
                                if (flag >= 1)
                                {
                                    BinaryWriter writer = new BinaryWriter(File.Open(Path.GetFileName(receivedPath), FileMode.Append));
                                    if (flag == 1)
                                    {
                                        writer.Write(buffer, 8 + fileNameLen, len - (8 + fileNameLen));
                                        flag++;
                                    }
                                    else
                                        writer.Write(buffer, 0, len);
                                    received += len;
                                    if (received > fileLen)
                                    {
                                        received = fileLen;
                                    }
                                    FileTransferSetStatus(ft, string.Format("Receiving '{0}'  [{1}/{2} Bytes]", Path.GetFileName(receivedPath), received, fileLen));
                                    FileTransferSetPBar(ft, fileLen, received);
                                    if (received == fileLen)
                                    {
                                        //clientSocket.Send(Encoding.UTF8.GetBytes("filereceived"));
                                        FileTransferCompleted(ft, receivedPath);
                                    }
                                    writer.Close();
                                }
                            }
                            continue;
                        }
                        clientSocket.Send(Encoding.UTF8.GetBytes("RCVD"));
                        AppendTextBox(cb, clientName + ": " + data + "\r\n", true);
                    }
                    else
                        AppendTextBox(cb, "   [X]\r\n", true); //ACK

                }
                catch (Exception ex)
                {
                    if (cb.IsHandleCreated)
                    {
                        CloseForm(cb);
                    }
                }
            }
        }

        public void CheckFriend(object info)
        {
            FriendInfo fInfo = (FriendInfo)info;
            while (true)
            {
                try
                {
                    Socket onlineChecker = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    onlineChecker.Connect(IPAddress.Parse(fInfo.ipAddr), fInfo.port);
                    byte[] buffer = new byte[1024];
                    if (!onlineChecker.Connected)
                    {
                        onlineChecker = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        onlineChecker.Connect(IPAddress.Parse(fInfo.ipAddr), fInfo.port);
                    }
                    onlineChecker.Send(Encoding.UTF8.GetBytes("AreYouThere?"));
                    onlineChecker.Receive(buffer);
                    string data = Encoding.UTF8.GetString(buffer);
                    if (data.Length > 2 && data.Substring(0, 2) == "/s")
                    {
                        onlineChecker.Receive(buffer);
                        data = Encoding.UTF8.GetString(buffer);
                    }
                    if (data.Length >= 4 && data.Substring(0, 4) == "yeah")
                    {
                        SetFriendList(fInfo.friendName, true);
                    }
                    else
                        SetFriendList(fInfo.friendName, false);
                    onlineChecker.Send(Encoding.UTF8.GetBytes("/quit"));
                }
                catch (Exception ex)
                {
                    SetFriendList(fInfo.friendName, false);
                }

                Thread.Sleep(2000);
            }
        }

        void SetFriendList(string friendName, bool online)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke((Action)delegate
                {
                    int imageIndex;
                    if (online)
		                imageIndex = 0;
                    else
                        imageIndex = 1;
                    if (treeView1.Nodes.ContainsKey(friendName))
                    {
                        treeView1.Nodes[friendName].ImageIndex = imageIndex;
                    }
                    else
                        treeView1.Nodes.Add(friendName, friendName, imageIndex);
                });
            }
        }

        public void Stop()
        {
            serverSocket.Close();
        }

        public void SetFormText(Form form, string text)
        {
            if (!this.IsHandleCreated)
            {
                this.CreateHandle();
            }
            if (InvokeRequired)
            {
                this.Invoke(new Action<Form, string>(SetFormText), new object[] { form, text });
                return;
                /*form.BeginInvoke(new EventHandler(delegate
                {
                    form.Text = "Chatting with " + text;
                }));*/
            }
            else
            {
                form.Text = "Chatting with " + text;
            }
        }

        public void CloseForm(Form form)
        {
            if (!this.IsHandleCreated)
            {
                this.CreateHandle();
            }
            if (InvokeRequired)
            {
                form.BeginInvoke(new EventHandler(delegate
                {
                    form.Close();
                }));
            }
            else
            {
                form.Close();
            }

        }

        public void AppendTextBox(Form form, string value, bool isRichTextBox)
        {
            if (!this.IsHandleCreated)
            {
                this.CreateHandle();
            }
            if (InvokeRequired)
            {
                this.Invoke(new Action<Form, string, bool>(AppendTextBox), new object[] { form, value, isRichTextBox });
                return;
            }
            try
            {
                if (isRichTextBox)
                {
                    ((RichTextBox)form.Controls.Find("richTextBox_Received", false)[0]).AppendText(value);
                    ((ChatBox)form).AddEmoticons();
                }
                else
                    ((TextBox)form.Controls.Find("textBox_Received", false)[0]).AppendText(value);
            }
            catch (Exception)
            {
                return;
            }

        }

        private void textBox_Address_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBox_Address.SelectAll();
        }

        private void textBox_Username_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBox_Username.SelectAll();
        }

        private void comboBox_Status_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_Status.SelectedIndex == 0 && !online) //online
            {
                online = true;
                AppendTextBox(this, "you appear online to your friends.\r\n", false);
            }
            else if (comboBox_Status.SelectedIndex == 1)  //offline
            {
                online = false;
                AppendTextBox(this, "You appear offline to your friends.\r\n", false);
            }
        }

        private void textBox_Friend_DoubleClick(object sender, EventArgs e)
        {
            textBox_Friend.SelectAll();
        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            string friendName = textBox_Friend.Text.Split(':').GetValue(0).ToString();
            string ipAddr = textBox_Friend.Text.Split(':').GetValue(1).ToString();
            int port = int.Parse(textBox_Friend.Text.Split(':').GetValue(2).ToString());
            Thread checkFriend = new Thread(new ParameterizedThreadStart(CheckFriend));
            checkFriend.IsBackground = true;
            checkFriend.Start(new FriendInfo(friendName, ipAddr, port));
        }
    }

    class FriendInfo
    { 
        public string friendName;
        public string ipAddr;
        public int port;
        public FriendInfo(string friendName, string ipAddr, int port)
        {
            this.friendName = friendName;
            this.ipAddr = ipAddr;
            this.port = port;
        }
    }

    class ClientSocketObj
    {
        public Socket socket;
        public bool initiatedByUs;

        public ClientSocketObj(Socket socket, bool initiatedByUs)
        {
            this.socket = socket;
            this.initiatedByUs = initiatedByUs;
        }
    }
}
