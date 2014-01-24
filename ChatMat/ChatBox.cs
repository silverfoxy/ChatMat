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
using System.Collections;
using System.IO;

namespace ChatMat
{
    public partial class ChatBox : Form
    {
        Socket clientSocket;
        Hashtable emoticons;
        public string RIpAddr;
        public int Rport;

        public ChatBox(Socket clientSocket)
        {
            InitializeComponent();
            this.clientSocket = clientSocket;

            emoticons = new Hashtable(7);
            emoticons.Add(":)", ChatMat.Properties.Resources.stunned);
            emoticons.Add("x(", ChatMat.Properties.Resources.angry);
            emoticons.Add(":x", ChatMat.Properties.Resources.flirty);
            emoticons.Add(":d", ChatMat.Properties.Resources.grin);
            emoticons.Add(":))", ChatMat.Properties.Resources.laugh);
            emoticons.Add(":p", ChatMat.Properties.Resources.tongue);
            emoticons.Add(";)", ChatMat.Properties.Resources.wink);
            emoticons.Add(":(", ChatMat.Properties.Resources.sad);
            emoticons.Add("[x]", ChatMat.Properties.Resources.tick);
        }

        public void AddEmoticons()
        {
            foreach (string smiley in emoticons.Keys)
            {
                while (richTextBox_Received.Text.ToLower().Contains(smiley))
                {
                    int indx = richTextBox_Received.Text.ToLower().IndexOf(smiley);
                    richTextBox_Received.Select(indx, smiley.Length);
                    Clipboard.SetImage((Image)emoticons[smiley]);
                    richTextBox_Received.Paste();
                }
            }
            richTextBox_Received.SelectionStart = richTextBox_Received.Text.Length;
            richTextBox_Received.ScrollToCaret();
        }

        private void button_Send_Click(object sender, EventArgs e)
        {
            Send(this, textBox_Send.Text);
            textBox_Send.Clear();
        }

        public void Send(Form cb, string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            clientSocket.BeginSend(messageBytes, 0, messageBytes.Length, 0, new AsyncCallback(SendData), clientSocket);
            AppendTextBox(cb, "You: " + message);
        }

        void SendData(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            int sent = remote.EndSend(iar);
        }

        public void AppendTextBox(Form form, string value)
        {
            if (!this.IsHandleCreated)
            {
                this.CreateHandle();
            }
            if (InvokeRequired)
            {
                this.Invoke(new Action<Form, string>(AppendTextBox), new object[] { form, value });
                return;
            }
            try
            {
                ((RichTextBox)form.Controls.Find("richTextBox_Received", false)[0]).AppendText(value);
                AddEmoticons();
            }
            catch (Exception)
            {
                return;
            }

        }

        private void ChatBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                clientSocket.Send(Encoding.UTF8.GetBytes("/quit\n"));
                clientSocket.Close();
            }
            catch (Exception)
            {
                return;
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
                richTextBox_Received.AppendText("Connected to: " + clientSocket.RemoteEndPoint.ToString() + "\r\n");
                //Thread receiver = new Thread(new ParameterizedThreadStart(ReceiveData));
                //receiver.IsBackground = true;
                //receiver.Start(clientSocket);
            }
            catch (SocketException)
            {
                richTextBox_Received.AppendText("Error connecting\r\n");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Thread fileSender = new Thread(new ParameterizedThreadStart(TransferFile));
                fileSender.IsBackground = true;
                fileSender.Start(openFileDialog1.FileName);
            }
            
        }

        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke((Action)delegate
                {
                    richTextBox_Received.AppendText(value);
                });
            }
        }

        void TransferFile(object file)
        {
            byte[] fileName = Encoding.UTF8.GetBytes((string)file); //file name
            byte[] fileData = File.ReadAllBytes((string)file); //file
            byte[] fileNameLen = BitConverter.GetBytes(fileName.Length); //lenght of file name
            byte[] fileDataLen = BitConverter.GetBytes(fileData.Length); //lenght of file
            byte[] m_clientData = new byte[fileNameLen.Length + fileName.Length + fileDataLen.Length + fileData.Length];

            fileNameLen.CopyTo(m_clientData, 0);
            fileName.CopyTo(m_clientData, fileNameLen.Length);
            fileDataLen.CopyTo(m_clientData, fileNameLen.Length + fileName.Length);
            fileData.CopyTo(m_clientData, fileNameLen.Length + fileName.Length + fileDataLen.Length);

            Socket fileTransferSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            fileTransferSocket.Connect(IPAddress.Parse(RIpAddr), Rport);
            //fileTransferSocket.Send(Encoding.UTF8.GetBytes("/s 127.0.0.1:1234|/name filetransfer"));

            fileTransferSocket.Send(m_clientData);
            byte[] ack = new byte[1024];
            fileTransferSocket.Receive(ack);
            /*if (Encoding.UTF8.GetString(ack).Contains("filereceived")
            {
                AppendTextBox((string)file + "Transferred successfully");       
            }*/
            fileTransferSocket.Send(Encoding.UTF8.GetBytes("/quit"));
            //fileTransferSocket.Close();
            //clientSocket.Send(Encoding.UTF8.GetBytes("/f abc.txt"));
            /*
            NetworkStream ns = new NetworkStream(clientSocket);
            StreamWriter writer = new StreamWriter(ns);
            FileStream fs = File.Open(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader reader = new StreamReader(fs);
            string strReadLine = null;
            do
            {
                strReadLine = reader.ReadLine();
                if (strReadLine != null) writer.WriteLine(strReadLine);
                writer.Flush();
            } while (strReadLine != null);
            writer.Close();*/
        }
    }
}
