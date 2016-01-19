namespace SketchServer
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lb_ServerPort = new System.Windows.Forms.Label();
            this.lb_MachineIP = new System.Windows.Forms.Label();
            this.lb_MachinePort = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btn_serverClear = new System.Windows.Forms.Button();
            this.rtb_server = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btn_clientClear = new System.Windows.Forms.Button();
            this.rtb_client = new System.Windows.Forms.RichTextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btn_HelloKitty = new System.Windows.Forms.Button();
            this.btn_commit = new System.Windows.Forms.Button();
            this.tb_pointY = new System.Windows.Forms.TextBox();
            this.tb_pointX = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_up = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb_ServerPort
            // 
            this.lb_ServerPort.AutoSize = true;
            this.lb_ServerPort.Location = new System.Drawing.Point(12, 26);
            this.lb_ServerPort.Name = "lb_ServerPort";
            this.lb_ServerPort.Size = new System.Drawing.Size(59, 12);
            this.lb_ServerPort.TabIndex = 0;
            this.lb_ServerPort.Text = "监听端口:";
            // 
            // lb_MachineIP
            // 
            this.lb_MachineIP.AutoSize = true;
            this.lb_MachineIP.Location = new System.Drawing.Point(174, 26);
            this.lb_MachineIP.Name = "lb_MachineIP";
            this.lb_MachineIP.Size = new System.Drawing.Size(59, 12);
            this.lb_MachineIP.TabIndex = 1;
            this.lb_MachineIP.Text = "机械臂IP:";
            // 
            // lb_MachinePort
            // 
            this.lb_MachinePort.AutoSize = true;
            this.lb_MachinePort.Location = new System.Drawing.Point(325, 26);
            this.lb_MachinePort.Name = "lb_MachinePort";
            this.lb_MachinePort.Size = new System.Drawing.Size(71, 12);
            this.lb_MachinePort.TabIndex = 2;
            this.lb_MachinePort.Text = "机械臂端口:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(11, 58);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(535, 382);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btn_serverClear);
            this.tabPage1.Controls.Add(this.rtb_server);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(527, 356);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "服务端通讯";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btn_serverClear
            // 
            this.btn_serverClear.Location = new System.Drawing.Point(446, 17);
            this.btn_serverClear.Name = "btn_serverClear";
            this.btn_serverClear.Size = new System.Drawing.Size(75, 23);
            this.btn_serverClear.TabIndex = 1;
            this.btn_serverClear.Text = "清空";
            this.btn_serverClear.UseVisualStyleBackColor = true;
            this.btn_serverClear.Click += new System.EventHandler(this.btn_serverClear_Click);
            // 
            // rtb_server
            // 
            this.rtb_server.Location = new System.Drawing.Point(4, 52);
            this.rtb_server.Name = "rtb_server";
            this.rtb_server.Size = new System.Drawing.Size(518, 300);
            this.rtb_server.TabIndex = 0;
            this.rtb_server.Text = "";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btn_clientClear);
            this.tabPage2.Controls.Add(this.rtb_client);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(527, 356);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "机械臂通讯";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btn_clientClear
            // 
            this.btn_clientClear.Location = new System.Drawing.Point(446, 17);
            this.btn_clientClear.Name = "btn_clientClear";
            this.btn_clientClear.Size = new System.Drawing.Size(75, 23);
            this.btn_clientClear.TabIndex = 2;
            this.btn_clientClear.Text = "清空";
            this.btn_clientClear.UseVisualStyleBackColor = true;
            this.btn_clientClear.Click += new System.EventHandler(this.btn_clientClear_Click);
            // 
            // rtb_client
            // 
            this.rtb_client.Location = new System.Drawing.Point(4, 52);
            this.rtb_client.Name = "rtb_client";
            this.rtb_client.Size = new System.Drawing.Size(518, 300);
            this.rtb_client.TabIndex = 1;
            this.rtb_client.Text = "";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btn_up);
            this.tabPage3.Controls.Add(this.btn_HelloKitty);
            this.tabPage3.Controls.Add(this.btn_commit);
            this.tabPage3.Controls.Add(this.tb_pointY);
            this.tabPage3.Controls.Add(this.tb_pointX);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(527, 356);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "测试";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btn_HelloKitty
            // 
            this.btn_HelloKitty.Location = new System.Drawing.Point(26, 229);
            this.btn_HelloKitty.Name = "btn_HelloKitty";
            this.btn_HelloKitty.Size = new System.Drawing.Size(129, 23);
            this.btn_HelloKitty.TabIndex = 18;
            this.btn_HelloKitty.Text = "HelloKitty";
            this.btn_HelloKitty.UseVisualStyleBackColor = true;
            this.btn_HelloKitty.Click += new System.EventHandler(this.btn_HelloKitty_Click);
            // 
            // btn_commit
            // 
            this.btn_commit.Location = new System.Drawing.Point(115, 157);
            this.btn_commit.Name = "btn_commit";
            this.btn_commit.Size = new System.Drawing.Size(75, 23);
            this.btn_commit.TabIndex = 17;
            this.btn_commit.Text = "确定";
            this.btn_commit.UseVisualStyleBackColor = true;
            this.btn_commit.Click += new System.EventHandler(this.btn_commit_Click);
            // 
            // tb_pointY
            // 
            this.tb_pointY.Location = new System.Drawing.Point(71, 113);
            this.tb_pointY.Name = "tb_pointY";
            this.tb_pointY.Size = new System.Drawing.Size(119, 21);
            this.tb_pointY.TabIndex = 15;
            this.tb_pointY.Text = "222";
            // 
            // tb_pointX
            // 
            this.tb_pointX.Location = new System.Drawing.Point(71, 76);
            this.tb_pointX.Name = "tb_pointX";
            this.tb_pointX.Size = new System.Drawing.Size(119, 21);
            this.tb_pointX.TabIndex = 14;
            this.tb_pointX.Text = "111";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 12;
            this.label2.Text = "Y坐标:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "X坐标:";
            // 
            // btn_up
            // 
            this.btn_up.Location = new System.Drawing.Point(26, 267);
            this.btn_up.Name = "btn_up";
            this.btn_up.Size = new System.Drawing.Size(75, 23);
            this.btn_up.TabIndex = 19;
            this.btn_up.Text = "抬笔";
            this.btn_up.UseVisualStyleBackColor = true;
            this.btn_up.Click += new System.EventHandler(this.btn_up_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 452);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lb_MachinePort);
            this.Controls.Add(this.lb_MachineIP);
            this.Controls.Add(this.lb_ServerPort);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_ServerPort;
        private System.Windows.Forms.Label lb_MachineIP;
        private System.Windows.Forms.Label lb_MachinePort;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox rtb_server;
        private System.Windows.Forms.RichTextBox rtb_client;
        private System.Windows.Forms.Button btn_serverClear;
        private System.Windows.Forms.Button btn_clientClear;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btn_commit;
        private System.Windows.Forms.TextBox tb_pointY;
        private System.Windows.Forms.TextBox tb_pointX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_HelloKitty;
        private System.Windows.Forms.Button btn_up;
    }
}