namespace osu_viewer
{
    partial class Form_Server
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
            this.button_serverSwitch = new System.Windows.Forms.Button();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.label_port = new System.Windows.Forms.Label();
            this.label_state = new System.Windows.Forms.Label();
            this.listBox_clients = new System.Windows.Forms.ListBox();
            this.button_publish = new System.Windows.Forms.Button();
            this.groupBox_publish = new System.Windows.Forms.GroupBox();
            this.label_name = new System.Windows.Forms.Label();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.label_password = new System.Windows.Forms.Label();
            this.textBox_password = new System.Windows.Forms.TextBox();
            this.checkBox_password = new System.Windows.Forms.CheckBox();
            this.label_publishStatusTitle = new System.Windows.Forms.Label();
            this.label_publishStatus = new System.Windows.Forms.Label();
            this.groupBox_publish.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_serverSwitch
            // 
            this.button_serverSwitch.Location = new System.Drawing.Point(151, 15);
            this.button_serverSwitch.Name = "button_serverSwitch";
            this.button_serverSwitch.Size = new System.Drawing.Size(50, 22);
            this.button_serverSwitch.TabIndex = 1;
            this.button_serverSwitch.Text = "Start";
            this.button_serverSwitch.UseVisualStyleBackColor = true;
            this.button_serverSwitch.Click += new System.EventHandler(this.button_serverSwitch_Click);
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(45, 15);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(100, 22);
            this.textBox_port.TabIndex = 0;
            // 
            // label_port
            // 
            this.label_port.AutoSize = true;
            this.label_port.Location = new System.Drawing.Point(12, 18);
            this.label_port.Name = "label_port";
            this.label_port.Size = new System.Drawing.Size(27, 12);
            this.label_port.TabIndex = 2;
            this.label_port.Text = "Port:";
            // 
            // label_state
            // 
            this.label_state.AutoSize = true;
            this.label_state.Location = new System.Drawing.Point(12, 43);
            this.label_state.Name = "label_state";
            this.label_state.Size = new System.Drawing.Size(31, 12);
            this.label_state.TabIndex = 3;
            this.label_state.Text = "ready";
            // 
            // listBox_clients
            // 
            this.listBox_clients.FormattingEnabled = true;
            this.listBox_clients.ItemHeight = 12;
            this.listBox_clients.Location = new System.Drawing.Point(12, 185);
            this.listBox_clients.Name = "listBox_clients";
            this.listBox_clients.Size = new System.Drawing.Size(189, 112);
            this.listBox_clients.TabIndex = 4;
            // 
            // button_publish
            // 
            this.button_publish.Location = new System.Drawing.Point(128, 43);
            this.button_publish.Name = "button_publish";
            this.button_publish.Size = new System.Drawing.Size(73, 22);
            this.button_publish.TabIndex = 5;
            this.button_publish.Text = "Publish";
            this.button_publish.UseVisualStyleBackColor = true;
            this.button_publish.Click += new System.EventHandler(this.button_publish_Click);
            // 
            // groupBox_publish
            // 
            this.groupBox_publish.Controls.Add(this.label_publishStatus);
            this.groupBox_publish.Controls.Add(this.label_publishStatusTitle);
            this.groupBox_publish.Controls.Add(this.checkBox_password);
            this.groupBox_publish.Controls.Add(this.label_password);
            this.groupBox_publish.Controls.Add(this.textBox_password);
            this.groupBox_publish.Controls.Add(this.label_name);
            this.groupBox_publish.Controls.Add(this.textBox_name);
            this.groupBox_publish.Location = new System.Drawing.Point(12, 64);
            this.groupBox_publish.Name = "groupBox_publish";
            this.groupBox_publish.Size = new System.Drawing.Size(189, 115);
            this.groupBox_publish.TabIndex = 6;
            this.groupBox_publish.TabStop = false;
            this.groupBox_publish.Text = "Publish Setting";
            // 
            // label_name
            // 
            this.label_name.AutoSize = true;
            this.label_name.Location = new System.Drawing.Point(10, 36);
            this.label_name.Name = "label_name";
            this.label_name.Size = new System.Drawing.Size(35, 12);
            this.label_name.TabIndex = 4;
            this.label_name.Text = "Name:";
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(51, 33);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(132, 22);
            this.textBox_name.TabIndex = 3;
            // 
            // label_password
            // 
            this.label_password.AutoSize = true;
            this.label_password.Location = new System.Drawing.Point(10, 86);
            this.label_password.Name = "label_password";
            this.label_password.Size = new System.Drawing.Size(51, 12);
            this.label_password.TabIndex = 6;
            this.label_password.Text = "Password:";
            // 
            // textBox_password
            // 
            this.textBox_password.Enabled = false;
            this.textBox_password.Location = new System.Drawing.Point(67, 83);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.Size = new System.Drawing.Size(116, 22);
            this.textBox_password.TabIndex = 5;
            // 
            // checkBox_password
            // 
            this.checkBox_password.AutoSize = true;
            this.checkBox_password.Location = new System.Drawing.Point(12, 61);
            this.checkBox_password.Name = "checkBox_password";
            this.checkBox_password.Size = new System.Drawing.Size(67, 16);
            this.checkBox_password.TabIndex = 7;
            this.checkBox_password.Text = "Password";
            this.checkBox_password.UseVisualStyleBackColor = true;
            this.checkBox_password.CheckedChanged += new System.EventHandler(this.checkBox_password_CheckedChanged);
            // 
            // label_publishStatusTitle
            // 
            this.label_publishStatusTitle.AutoSize = true;
            this.label_publishStatusTitle.Location = new System.Drawing.Point(10, 18);
            this.label_publishStatusTitle.Name = "label_publishStatusTitle";
            this.label_publishStatusTitle.Size = new System.Drawing.Size(35, 12);
            this.label_publishStatusTitle.TabIndex = 8;
            this.label_publishStatusTitle.Text = "Status:";
            // 
            // label_publishStatus
            // 
            this.label_publishStatus.AutoSize = true;
            this.label_publishStatus.Location = new System.Drawing.Point(51, 18);
            this.label_publishStatus.Name = "label_publishStatus";
            this.label_publishStatus.Size = new System.Drawing.Size(30, 12);
            this.label_publishStatus.TabIndex = 9;
            this.label_publishStatus.Text = "None";
            // 
            // Form_Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(213, 309);
            this.Controls.Add(this.button_publish);
            this.Controls.Add(this.groupBox_publish);
            this.Controls.Add(this.listBox_clients);
            this.Controls.Add(this.label_state);
            this.Controls.Add(this.label_port);
            this.Controls.Add(this.textBox_port);
            this.Controls.Add(this.button_serverSwitch);
            this.Name = "Form_Server";
            this.Text = "Server";
            this.groupBox_publish.ResumeLayout(false);
            this.groupBox_publish.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_serverSwitch;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.Label label_port;
        private System.Windows.Forms.Label label_state;
        private System.Windows.Forms.ListBox listBox_clients;
        private System.Windows.Forms.Button button_publish;
        private System.Windows.Forms.GroupBox groupBox_publish;
        private System.Windows.Forms.CheckBox checkBox_password;
        private System.Windows.Forms.Label label_password;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.Label label_name;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.Label label_publishStatus;
        private System.Windows.Forms.Label label_publishStatusTitle;
    }
}