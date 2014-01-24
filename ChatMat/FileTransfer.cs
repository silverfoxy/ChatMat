using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ChatMat
{
    public partial class FileTransfer : Form
    {
        public FileTransfer()
        {
            InitializeComponent();
        }

        public void SetStatus(string status)
        {
            label_Status.Text = status;
        }

        public void SetPBar(int max, int current)
        {
            progressBar1.Maximum = max;
            progressBar1.Value = current;
        }

        public void Completed(string fileName)
        {
            /*saveFileDialog1.FileName = fileName;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.Copy(fileName, saveFileDialog1.FileName);
            }*/
            //this.Close();
        }
    }
}
