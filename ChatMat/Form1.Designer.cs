namespace ChatMat
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button_Connect = new System.Windows.Forms.Button();
            this.textBox_Address = new System.Windows.Forms.TextBox();
            this.textBox_Received = new System.Windows.Forms.TextBox();
            this.textBox_Username = new System.Windows.Forms.TextBox();
            this.comboBox_Status = new System.Windows.Forms.ComboBox();
            this.button_Add = new System.Windows.Forms.Button();
            this.textBox_Friend = new System.Windows.Forms.TextBox();
            this.groupBox_FriendList = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList_Status = new System.Windows.Forms.ImageList(this.components);
            this.groupBox_FriendList.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Connect
            // 
            this.button_Connect.Location = new System.Drawing.Point(174, 39);
            this.button_Connect.Name = "button_Connect";
            this.button_Connect.Size = new System.Drawing.Size(121, 23);
            this.button_Connect.TabIndex = 2;
            this.button_Connect.Text = "Connect";
            this.button_Connect.UseVisualStyleBackColor = true;
            this.button_Connect.Click += new System.EventHandler(this.button_Connect_Click);
            // 
            // textBox_Address
            // 
            this.textBox_Address.Location = new System.Drawing.Point(12, 41);
            this.textBox_Address.Name = "textBox_Address";
            this.textBox_Address.Size = new System.Drawing.Size(125, 20);
            this.textBox_Address.TabIndex = 1;
            this.textBox_Address.Text = "127.0.0.1:4444";
            this.textBox_Address.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.textBox_Address_MouseDoubleClick);
            // 
            // textBox_Received
            // 
            this.textBox_Received.AcceptsReturn = true;
            this.textBox_Received.AcceptsTab = true;
            this.textBox_Received.Location = new System.Drawing.Point(12, 97);
            this.textBox_Received.Multiline = true;
            this.textBox_Received.Name = "textBox_Received";
            this.textBox_Received.ReadOnly = true;
            this.textBox_Received.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.textBox_Received.Size = new System.Drawing.Size(283, 236);
            this.textBox_Received.TabIndex = 6;
            // 
            // textBox_Username
            // 
            this.textBox_Username.Location = new System.Drawing.Point(12, 15);
            this.textBox_Username.Name = "textBox_Username";
            this.textBox_Username.Size = new System.Drawing.Size(125, 20);
            this.textBox_Username.TabIndex = 0;
            this.textBox_Username.Text = "Username";
            this.textBox_Username.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.textBox_Username_MouseDoubleClick);
            // 
            // comboBox_Status
            // 
            this.comboBox_Status.FormattingEnabled = true;
            this.comboBox_Status.Items.AddRange(new object[] {
            "Online",
            "Offline"});
            this.comboBox_Status.Location = new System.Drawing.Point(174, 14);
            this.comboBox_Status.Name = "comboBox_Status";
            this.comboBox_Status.Size = new System.Drawing.Size(121, 21);
            this.comboBox_Status.TabIndex = 5;
            this.comboBox_Status.SelectedIndexChanged += new System.EventHandler(this.comboBox_Status_SelectedIndexChanged);
            // 
            // button_Add
            // 
            this.button_Add.Location = new System.Drawing.Point(174, 68);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(121, 23);
            this.button_Add.TabIndex = 4;
            this.button_Add.Text = "Add To Friends";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
            // 
            // textBox_Friend
            // 
            this.textBox_Friend.Location = new System.Drawing.Point(12, 70);
            this.textBox_Friend.Name = "textBox_Friend";
            this.textBox_Friend.Size = new System.Drawing.Size(125, 20);
            this.textBox_Friend.TabIndex = 3;
            this.textBox_Friend.Text = "Name:IP Address:Port";
            this.textBox_Friend.DoubleClick += new System.EventHandler(this.textBox_Friend_DoubleClick);
            // 
            // groupBox_FriendList
            // 
            this.groupBox_FriendList.Controls.Add(this.treeView1);
            this.groupBox_FriendList.Location = new System.Drawing.Point(301, 15);
            this.groupBox_FriendList.Name = "groupBox_FriendList";
            this.groupBox_FriendList.Size = new System.Drawing.Size(200, 318);
            this.groupBox_FriendList.TabIndex = 8;
            this.groupBox_FriendList.TabStop = false;
            this.groupBox_FriendList.Text = "Friend List";
            // 
            // treeView1
            // 
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList_Status;
            this.treeView1.Location = new System.Drawing.Point(6, 26);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(188, 286);
            this.treeView1.TabIndex = 7;
            // 
            // imageList_Status
            // 
            this.imageList_Status.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_Status.ImageStream")));
            this.imageList_Status.TransparentColor = System.Drawing.Color.White;
            this.imageList_Status.Images.SetKeyName(0, "online.png");
            this.imageList_Status.Images.SetKeyName(1, "offline.png");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 345);
            this.Controls.Add(this.groupBox_FriendList);
            this.Controls.Add(this.textBox_Friend);
            this.Controls.Add(this.button_Add);
            this.Controls.Add(this.comboBox_Status);
            this.Controls.Add(this.textBox_Username);
            this.Controls.Add(this.textBox_Received);
            this.Controls.Add(this.textBox_Address);
            this.Controls.Add(this.button_Connect);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "ChatMat";
            this.groupBox_FriendList.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Connect;
        private System.Windows.Forms.TextBox textBox_Address;
        private System.Windows.Forms.TextBox textBox_Received;
        private System.Windows.Forms.TextBox textBox_Username;
        private System.Windows.Forms.ComboBox comboBox_Status;
        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.TextBox textBox_Friend;
        private System.Windows.Forms.GroupBox groupBox_FriendList;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ImageList imageList_Status;
    }
}

