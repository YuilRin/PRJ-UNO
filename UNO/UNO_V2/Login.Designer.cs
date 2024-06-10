namespace UNO_V2
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            LoginTb = new Button();
            NameTb = new TextBox();
            lbName = new Label();
            LbIp = new Label();
            IpTb = new TextBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // LoginTb
            // 
            LoginTb.BackColor = Color.SpringGreen;
            LoginTb.Font = new Font("Impact", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LoginTb.Location = new Point(290, 280);
            LoginTb.Name = "LoginTb";
            LoginTb.Size = new Size(107, 56);
            LoginTb.TabIndex = 0;
            LoginTb.Text = "Login";
            LoginTb.UseVisualStyleBackColor = false;
            LoginTb.Click += Login_Click;
            // 
            // NameTb
            // 
            NameTb.Location = new Point(267, 164);
            NameTb.Name = "NameTb";
            NameTb.Size = new Size(157, 27);
            NameTb.TabIndex = 1;
            // 
            // lbName
            // 
            lbName.AutoSize = true;
            lbName.BackColor = Color.Transparent;
            lbName.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            lbName.Location = new Point(267, 140);
            lbName.Name = "lbName";
            lbName.Size = new Size(57, 23);
            lbName.TabIndex = 2;
            lbName.Text = "Name";
            // 
            // LbIp
            // 
            LbIp.AutoSize = true;
            LbIp.BackColor = Color.Transparent;
            LbIp.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            LbIp.Location = new Point(267, 191);
            LbIp.Name = "LbIp";
            LbIp.Size = new Size(25, 23);
            LbIp.TabIndex = 2;
            LbIp.Text = "Ip";
            // 
            // IpTb
            // 
            IpTb.Location = new Point(267, 217);
            IpTb.Name = "IpTb";
            IpTb.Size = new Size(157, 27);
            IpTb.TabIndex = 1;
            IpTb.Text = "127.0.0.1";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Microsoft Sans Serif", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Maroon;
            label1.Location = new Point(276, 27);
            label1.Name = "label1";
            label1.Size = new Size(148, 46);
            label1.TabIndex = 3;
            label1.Text = "LOGIN";
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(671, 428);
            Controls.Add(label1);
            Controls.Add(LbIp);
            Controls.Add(lbName);
            Controls.Add(IpTb);
            Controls.Add(NameTb);
            Controls.Add(LoginTb);
            Name = "Login";
            Text = "Login";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button LoginTb;
        private TextBox NameTb;
        private Label lbName;
        private Label LbIp;
        private TextBox IpTb;
        private Label label1;
    }
}