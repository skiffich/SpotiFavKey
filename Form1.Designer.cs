namespace SpotiHotKey
{
    partial class SpotiFavKey
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpotiFavKey));
            label1 = new Label();
            btn_set_hotkey_1 = new Button();
            lbl_hotkey_1 = new Label();
            lbl_status = new Label();
            notifyIcon = new NotifyIcon(components);
            cb_saveList_1 = new ComboBox();
            cb_saveList_2 = new ComboBox();
            lbl_hotkey_2 = new Label();
            btn_set_hotkey_2 = new Button();
            cb_saveList_3 = new ComboBox();
            lbl_hotkey_3 = new Label();
            btn_set_hotkey_3 = new Button();
            cb_saveList_4 = new ComboBox();
            lbl_hotkey_4 = new Label();
            btn_set_hotkey_4 = new Button();
            showNotificationsCheck = new CheckBox();
            label2 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Padding = new Padding(0, 20, 0, 0);
            label1.Size = new Size(632, 40);
            label1.TabIndex = 1;
            label1.Text = "Press the button below to set your Spotify Favorites Hotkey.";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btn_set_hotkey_1
            // 
            btn_set_hotkey_1.Location = new Point(332, 51);
            btn_set_hotkey_1.Name = "btn_set_hotkey_1";
            btn_set_hotkey_1.Size = new Size(100, 30);
            btn_set_hotkey_1.TabIndex = 0;
            btn_set_hotkey_1.Text = "Set hotkey";
            btn_set_hotkey_1.UseVisualStyleBackColor = true;
            btn_set_hotkey_1.Click += btn_set_hotkey_Click;
            // 
            // lbl_hotkey_1
            // 
            lbl_hotkey_1.Location = new Point(438, 40);
            lbl_hotkey_1.Name = "lbl_hotkey_1";
            lbl_hotkey_1.Size = new Size(194, 52);
            lbl_hotkey_1.TabIndex = 2;
            lbl_hotkey_1.Text = "No hotkey set.";
            lbl_hotkey_1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_status
            // 
            lbl_status.Dock = DockStyle.Bottom;
            lbl_status.Location = new Point(0, 257);
            lbl_status.Name = "lbl_status";
            lbl_status.Size = new Size(632, 46);
            lbl_status.TabIndex = 3;
            lbl_status.Text = "Status will appear here";
            lbl_status.TextAlign = ContentAlignment.MiddleCenter;
            lbl_status.UseMnemonic = false;
            // 
            // notifyIcon
            // 
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.BalloonTipClicked += notifyIcon_BalloonTipClicked;
            notifyIcon.MouseClick += notifyIcon_MouseClick;
            // 
            // cb_saveList_1
            // 
            cb_saveList_1.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_saveList_1.Enabled = false;
            cb_saveList_1.FormattingEnabled = true;
            cb_saveList_1.Location = new Point(12, 53);
            cb_saveList_1.Name = "cb_saveList_1";
            cb_saveList_1.Size = new Size(314, 28);
            cb_saveList_1.TabIndex = 4;
            // 
            // cb_saveList_2
            // 
            cb_saveList_2.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_saveList_2.FormattingEnabled = true;
            cb_saveList_2.Location = new Point(12, 105);
            cb_saveList_2.Name = "cb_saveList_2";
            cb_saveList_2.Size = new Size(314, 28);
            cb_saveList_2.TabIndex = 7;
            // 
            // lbl_hotkey_2
            // 
            lbl_hotkey_2.Location = new Point(438, 92);
            lbl_hotkey_2.Name = "lbl_hotkey_2";
            lbl_hotkey_2.Size = new Size(194, 52);
            lbl_hotkey_2.TabIndex = 6;
            lbl_hotkey_2.Text = "No hotkey set.";
            lbl_hotkey_2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btn_set_hotkey_2
            // 
            btn_set_hotkey_2.Location = new Point(332, 103);
            btn_set_hotkey_2.Name = "btn_set_hotkey_2";
            btn_set_hotkey_2.Size = new Size(100, 30);
            btn_set_hotkey_2.TabIndex = 5;
            btn_set_hotkey_2.Text = "Set hotkey";
            btn_set_hotkey_2.UseVisualStyleBackColor = true;
            btn_set_hotkey_2.Click += btn_set_hotkey_Click;
            // 
            // cb_saveList_3
            // 
            cb_saveList_3.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_saveList_3.FormattingEnabled = true;
            cb_saveList_3.Location = new Point(12, 157);
            cb_saveList_3.Name = "cb_saveList_3";
            cb_saveList_3.Size = new Size(314, 28);
            cb_saveList_3.TabIndex = 10;
            // 
            // lbl_hotkey_3
            // 
            lbl_hotkey_3.Location = new Point(438, 144);
            lbl_hotkey_3.Name = "lbl_hotkey_3";
            lbl_hotkey_3.Size = new Size(194, 52);
            lbl_hotkey_3.TabIndex = 9;
            lbl_hotkey_3.Text = "No hotkey set.";
            lbl_hotkey_3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btn_set_hotkey_3
            // 
            btn_set_hotkey_3.Location = new Point(332, 155);
            btn_set_hotkey_3.Name = "btn_set_hotkey_3";
            btn_set_hotkey_3.Size = new Size(100, 30);
            btn_set_hotkey_3.TabIndex = 8;
            btn_set_hotkey_3.Text = "Set hotkey";
            btn_set_hotkey_3.UseVisualStyleBackColor = true;
            btn_set_hotkey_3.Click += btn_set_hotkey_Click;
            // 
            // cb_saveList_4
            // 
            cb_saveList_4.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_saveList_4.FormattingEnabled = true;
            cb_saveList_4.Location = new Point(12, 209);
            cb_saveList_4.Name = "cb_saveList_4";
            cb_saveList_4.Size = new Size(314, 28);
            cb_saveList_4.TabIndex = 13;
            // 
            // lbl_hotkey_4
            // 
            lbl_hotkey_4.Location = new Point(438, 196);
            lbl_hotkey_4.Name = "lbl_hotkey_4";
            lbl_hotkey_4.Size = new Size(194, 52);
            lbl_hotkey_4.TabIndex = 12;
            lbl_hotkey_4.Text = "No hotkey set.";
            lbl_hotkey_4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btn_set_hotkey_4
            // 
            btn_set_hotkey_4.Location = new Point(332, 207);
            btn_set_hotkey_4.Name = "btn_set_hotkey_4";
            btn_set_hotkey_4.Size = new Size(100, 30);
            btn_set_hotkey_4.TabIndex = 11;
            btn_set_hotkey_4.Text = "Set hotkey";
            btn_set_hotkey_4.UseVisualStyleBackColor = true;
            btn_set_hotkey_4.Click += btn_set_hotkey_Click;
            // 
            // showNotificationsCheck
            // 
            showNotificationsCheck.AutoSize = true;
            showNotificationsCheck.Location = new Point(613, -1);
            showNotificationsCheck.Name = "showNotificationsCheck";
            showNotificationsCheck.Size = new Size(101, 24);
            showNotificationsCheck.TabIndex = 14;
            showNotificationsCheck.Text = "checkBox1";
            showNotificationsCheck.UseVisualStyleBackColor = true;
            showNotificationsCheck.CheckStateChanged += showNotificationsCheck_CheckStateChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(476, -1);
            label2.Name = "label2";
            label2.Size = new Size(131, 20);
            label2.TabIndex = 15;
            label2.Text = "Show notifications";
            // 
            // SpotiFavKey
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightGreen;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(632, 303);
            Controls.Add(label2);
            Controls.Add(showNotificationsCheck);
            Controls.Add(cb_saveList_4);
            Controls.Add(lbl_hotkey_4);
            Controls.Add(btn_set_hotkey_4);
            Controls.Add(cb_saveList_3);
            Controls.Add(lbl_hotkey_3);
            Controls.Add(btn_set_hotkey_3);
            Controls.Add(cb_saveList_2);
            Controls.Add(lbl_hotkey_2);
            Controls.Add(btn_set_hotkey_2);
            Controls.Add(cb_saveList_1);
            Controls.Add(lbl_status);
            Controls.Add(lbl_hotkey_1);
            Controls.Add(label1);
            Controls.Add(btn_set_hotkey_1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new Size(650, 350);
            MinimumSize = new Size(650, 350);
            Name = "SpotiFavKey";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "Spotify Favorites Hotkey Setter";
            Load += SpotiFavKey_Load;
            Shown += SpotiFavKey_Shown;
            Resize += SpotiFavKey_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button btn_set_hotkey_1;
        private Label lbl_hotkey_1;
        private Label lbl_status;
        private NotifyIcon notifyIcon;
        private ComboBox cb_saveList_1;
        private ComboBox cb_saveList_2;
        private Label lbl_hotkey_2;
        private Button btn_set_hotkey_2;
        private ComboBox cb_saveList_3;
        private Label lbl_hotkey_3;
        private Button btn_set_hotkey_3;
        private ComboBox cb_saveList_4;
        private Label lbl_hotkey_4;
        private Button btn_set_hotkey_4;
        private CheckBox showNotificationsCheck;
        private Label label2;
    }
}
