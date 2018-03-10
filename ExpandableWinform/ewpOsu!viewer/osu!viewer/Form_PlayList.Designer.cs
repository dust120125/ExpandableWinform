namespace osu_viewer
{
    partial class Form_PlayList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_PlayList));
            this.button_NewList = new System.Windows.Forms.Button();
            this.comboBox_Playlists = new System.Windows.Forms.ComboBox();
            this.listBox_MediaList = new System.Windows.Forms.ListBox();
            this.textBox_RenamePlaylist = new System.Windows.Forms.TextBox();
            this.button_Play = new System.Windows.Forms.Button();
            this.button_Delete = new System.Windows.Forms.Button();
            this.buttonDoubleUp = new System.Windows.Forms.Button();
            this.button_Up = new System.Windows.Forms.Button();
            this.button_Down = new System.Windows.Forms.Button();
            this.button_DoubleDown = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_NewList
            // 
            this.button_NewList.Location = new System.Drawing.Point(12, 11);
            this.button_NewList.Name = "button_NewList";
            this.button_NewList.Size = new System.Drawing.Size(86, 23);
            this.button_NewList.TabIndex = 0;
            this.button_NewList.Text = "New Playlist";
            this.button_NewList.UseVisualStyleBackColor = true;
            this.button_NewList.Click += new System.EventHandler(this.button_NewList_Click);
            // 
            // comboBox_Playlists
            // 
            this.comboBox_Playlists.FormattingEnabled = true;
            this.comboBox_Playlists.Location = new System.Drawing.Point(104, 14);
            this.comboBox_Playlists.Name = "comboBox_Playlists";
            this.comboBox_Playlists.Size = new System.Drawing.Size(108, 20);
            this.comboBox_Playlists.TabIndex = 1;
            this.comboBox_Playlists.SelectedIndexChanged += new System.EventHandler(this.comboBox_Playlists_SelectedIndexChanged);
            // 
            // listBox_MediaList
            // 
            this.listBox_MediaList.FormattingEnabled = true;
            this.listBox_MediaList.ItemHeight = 12;
            this.listBox_MediaList.Location = new System.Drawing.Point(12, 68);
            this.listBox_MediaList.Name = "listBox_MediaList";
            this.listBox_MediaList.Size = new System.Drawing.Size(200, 196);
            this.listBox_MediaList.TabIndex = 2;
            // 
            // textBox_RenamePlaylist
            // 
            this.textBox_RenamePlaylist.Location = new System.Drawing.Point(104, 40);
            this.textBox_RenamePlaylist.Name = "textBox_RenamePlaylist";
            this.textBox_RenamePlaylist.Size = new System.Drawing.Size(108, 22);
            this.textBox_RenamePlaylist.TabIndex = 3;
            this.textBox_RenamePlaylist.TextChanged += new System.EventHandler(this.textBox_RenamePlaylist_TextChanged);
            // 
            // button_Play
            // 
            this.button_Play.Location = new System.Drawing.Point(12, 271);
            this.button_Play.Name = "button_Play";
            this.button_Play.Size = new System.Drawing.Size(75, 23);
            this.button_Play.TabIndex = 4;
            this.button_Play.Text = "Play";
            this.button_Play.UseVisualStyleBackColor = true;
            this.button_Play.Click += new System.EventHandler(this.button_Play_Click);
            // 
            // button_Delete
            // 
            this.button_Delete.Location = new System.Drawing.Point(12, 40);
            this.button_Delete.Name = "button_Delete";
            this.button_Delete.Size = new System.Drawing.Size(86, 22);
            this.button_Delete.TabIndex = 5;
            this.button_Delete.Text = "Delete Playlist";
            this.button_Delete.UseVisualStyleBackColor = true;
            this.button_Delete.Click += new System.EventHandler(this.button_Delete_Click);
            // 
            // buttonDoubleUp
            // 
            this.buttonDoubleUp.BackgroundImage = global::osu_viewer.Properties.Resources._2up;
            this.buttonDoubleUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonDoubleUp.FlatAppearance.BorderSize = 0;
            this.buttonDoubleUp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonDoubleUp.Location = new System.Drawing.Point(102, 271);
            this.buttonDoubleUp.Name = "buttonDoubleUp";
            this.buttonDoubleUp.Size = new System.Drawing.Size(23, 23);
            this.buttonDoubleUp.TabIndex = 6;
            this.buttonDoubleUp.UseVisualStyleBackColor = true;
            this.buttonDoubleUp.Click += new System.EventHandler(this.buttonDoubleUp_Click);
            // 
            // button_Up
            // 
            this.button_Up.BackgroundImage = global::osu_viewer.Properties.Resources.up;
            this.button_Up.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Up.FlatAppearance.BorderSize = 0;
            this.button_Up.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_Up.Location = new System.Drawing.Point(131, 271);
            this.button_Up.Name = "button_Up";
            this.button_Up.Size = new System.Drawing.Size(23, 23);
            this.button_Up.TabIndex = 7;
            this.button_Up.UseVisualStyleBackColor = true;
            this.button_Up.Click += new System.EventHandler(this.button_Up_Click);
            // 
            // button_Down
            // 
            this.button_Down.BackgroundImage = global::osu_viewer.Properties.Resources.dw;
            this.button_Down.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Down.FlatAppearance.BorderSize = 0;
            this.button_Down.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_Down.Location = new System.Drawing.Point(160, 271);
            this.button_Down.Name = "button_Down";
            this.button_Down.Size = new System.Drawing.Size(23, 23);
            this.button_Down.TabIndex = 8;
            this.button_Down.UseVisualStyleBackColor = true;
            this.button_Down.Click += new System.EventHandler(this.button_Down_Click);
            // 
            // button_DoubleDown
            // 
            this.button_DoubleDown.BackgroundImage = global::osu_viewer.Properties.Resources._2dw;
            this.button_DoubleDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_DoubleDown.FlatAppearance.BorderSize = 0;
            this.button_DoubleDown.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_DoubleDown.Location = new System.Drawing.Point(189, 271);
            this.button_DoubleDown.Name = "button_DoubleDown";
            this.button_DoubleDown.Size = new System.Drawing.Size(23, 23);
            this.button_DoubleDown.TabIndex = 9;
            this.button_DoubleDown.UseVisualStyleBackColor = true;
            this.button_DoubleDown.Click += new System.EventHandler(this.button_DoubleDown_Click);
            // 
            // Form_PlayList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 306);
            this.Controls.Add(this.button_DoubleDown);
            this.Controls.Add(this.button_Down);
            this.Controls.Add(this.button_Up);
            this.Controls.Add(this.buttonDoubleUp);
            this.Controls.Add(this.button_Delete);
            this.Controls.Add(this.button_Play);
            this.Controls.Add(this.textBox_RenamePlaylist);
            this.Controls.Add(this.listBox_MediaList);
            this.Controls.Add(this.comboBox_Playlists);
            this.Controls.Add(this.button_NewList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_PlayList";
            this.Text = "PlayList Manager";
            this.Load += new System.EventHandler(this.Form_PlayList_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_NewList;
        private System.Windows.Forms.ComboBox comboBox_Playlists;
        private System.Windows.Forms.ListBox listBox_MediaList;
        private System.Windows.Forms.TextBox textBox_RenamePlaylist;
        private System.Windows.Forms.Button button_Play;
        private System.Windows.Forms.Button button_Delete;
        private System.Windows.Forms.Button buttonDoubleUp;
        private System.Windows.Forms.Button button_Up;
        private System.Windows.Forms.Button button_Down;
        private System.Windows.Forms.Button button_DoubleDown;
    }
}