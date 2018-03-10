namespace osu_viewer
{
    partial class clientDialog
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
            this.button_Ok = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_custom = new System.Windows.Forms.CheckBox();
            this.textBox_Port = new System.Windows.Forms.TextBox();
            this.textBox_IP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_refresh = new System.Windows.Forms.Button();
            this.listBox_servers = new System.Windows.Forms.ListBox();
            this.textBox_password = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Ok
            // 
            this.button_Ok.Location = new System.Drawing.Point(12, 305);
            this.button_Ok.Name = "button_Ok";
            this.button_Ok.Size = new System.Drawing.Size(75, 23);
            this.button_Ok.TabIndex = 3;
            this.button_Ok.Text = "Connect";
            this.button_Ok.UseVisualStyleBackColor = true;
            this.button_Ok.Click += new System.EventHandler(this.button_Ok_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(138, 305);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 4;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP Address:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_custom);
            this.groupBox1.Controls.Add(this.textBox_Port);
            this.groupBox1.Controls.Add(this.textBox_IP);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 203);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(201, 96);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // checkBox_custom
            // 
            this.checkBox_custom.AutoSize = true;
            this.checkBox_custom.Location = new System.Drawing.Point(6, 18);
            this.checkBox_custom.Name = "checkBox_custom";
            this.checkBox_custom.Size = new System.Drawing.Size(73, 16);
            this.checkBox_custom.TabIndex = 6;
            this.checkBox_custom.Text = "Custom IP";
            this.checkBox_custom.UseVisualStyleBackColor = true;
            this.checkBox_custom.CheckedChanged += new System.EventHandler(this.checkBox_custom_CheckedChanged);
            // 
            // textBox_Port
            // 
            this.textBox_Port.Enabled = false;
            this.textBox_Port.Location = new System.Drawing.Point(71, 68);
            this.textBox_Port.Name = "textBox_Port";
            this.textBox_Port.Size = new System.Drawing.Size(123, 22);
            this.textBox_Port.TabIndex = 2;
            // 
            // textBox_IP
            // 
            this.textBox_IP.Enabled = false;
            this.textBox_IP.Location = new System.Drawing.Point(71, 40);
            this.textBox_IP.Name = "textBox_IP";
            this.textBox_IP.Size = new System.Drawing.Size(123, 22);
            this.textBox_IP.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port:";
            // 
            // button_refresh
            // 
            this.button_refresh.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_refresh.Location = new System.Drawing.Point(12, 12);
            this.button_refresh.Name = "button_refresh";
            this.button_refresh.Size = new System.Drawing.Size(75, 23);
            this.button_refresh.TabIndex = 6;
            this.button_refresh.Text = "Refresh";
            this.button_refresh.UseVisualStyleBackColor = true;
            this.button_refresh.Click += new System.EventHandler(this.button_refresh_Click);
            // 
            // listBox_servers
            // 
            this.listBox_servers.FormattingEnabled = true;
            this.listBox_servers.HorizontalScrollbar = true;
            this.listBox_servers.ItemHeight = 12;
            this.listBox_servers.Location = new System.Drawing.Point(12, 41);
            this.listBox_servers.Name = "listBox_servers";
            this.listBox_servers.Size = new System.Drawing.Size(201, 136);
            this.listBox_servers.TabIndex = 7;
            this.listBox_servers.SelectedIndexChanged += new System.EventHandler(this.listBox_servers_SelectedIndexChanged);
            // 
            // textBox_password
            // 
            this.textBox_password.Enabled = false;
            this.textBox_password.Location = new System.Drawing.Point(67, 183);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.Size = new System.Drawing.Size(146, 22);
            this.textBox_password.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 188);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "Password:";
            // 
            // clientDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(225, 340);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_password);
            this.Controls.Add(this.listBox_servers);
            this.Controls.Add(this.button_refresh);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_Ok);
            this.Name = "clientDialog";
            this.Text = "Client Setting";
            this.Load += new System.EventHandler(this.clientDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Ok;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_Port;
        private System.Windows.Forms.TextBox textBox_IP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox_custom;
        private System.Windows.Forms.Button button_refresh;
        private System.Windows.Forms.ListBox listBox_servers;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.Label label3;
    }
}