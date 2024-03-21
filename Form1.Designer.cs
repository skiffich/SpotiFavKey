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
            btn_set_hotkey = new Button();
            lbl_hotkey = new Label();
            lbl_status = new Label();
            notifyIcon = new NotifyIcon(components);
            SuspendLayout();
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Padding = new Padding(0, 20, 0, 0);
            label1.Size = new Size(432, 40);
            label1.TabIndex = 1;
            label1.Text = "Press the button below to set your Spotify Favorites Hotkey.";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btn_set_hotkey
            // 
            btn_set_hotkey.Location = new Point(100, 64);
            btn_set_hotkey.Name = "btn_set_hotkey";
            btn_set_hotkey.Size = new Size(100, 30);
            btn_set_hotkey.TabIndex = 0;
            btn_set_hotkey.Text = "Set hotkey";
            btn_set_hotkey.UseVisualStyleBackColor = true;
            btn_set_hotkey.Click += btn_set_hotkey_Click;
            // 
            // lbl_hotkey
            // 
            lbl_hotkey.Location = new Point(200, 51);
            lbl_hotkey.Name = "lbl_hotkey";
            lbl_hotkey.Size = new Size(194, 52);
            lbl_hotkey.TabIndex = 2;
            lbl_hotkey.Text = "No hotkey set.";
            lbl_hotkey.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_status
            // 
            lbl_status.Dock = DockStyle.Bottom;
            lbl_status.Location = new Point(0, 257);
            lbl_status.Name = "lbl_status";
            lbl_status.Size = new Size(432, 46);
            lbl_status.TabIndex = 3;
            lbl_status.Text = "Status will appear here";
            lbl_status.TextAlign = ContentAlignment.MiddleCenter;
            lbl_status.UseMnemonic = false;
            // 
            // notifyIcon
            // 
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.MouseClick += notifyIcon_MouseClick;
            // 
            // SpotiFavKey
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightGreen;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(432, 303);
            Controls.Add(lbl_status);
            Controls.Add(lbl_hotkey);
            Controls.Add(label1);
            Controls.Add(btn_set_hotkey);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new Size(450, 350);
            MinimumSize = new Size(450, 350);
            Name = "SpotiFavKey";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "Spotify Favorites Hotkey Setter";
            Load += SpotiFavKey_Load;
            Shown += SpotiFavKey_Shown;
            Resize += SpotiFavKey_Resize;
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Button btn_set_hotkey;
        private Label lbl_hotkey;
        private Label lbl_status;
        private NotifyIcon notifyIcon;
    }
}
