namespace UNO_V2
{
    partial class Form1
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
            server2 = new Button();
            Login = new Button();
            SuspendLayout();
            // 
            // server2
            // 
            server2.Location = new Point(26, 22);
            server2.Name = "server2";
            server2.Size = new Size(216, 68);
            server2.TabIndex = 0;
            server2.Text = "server";
            server2.UseVisualStyleBackColor = true;
            server2.Click += server2_Click;
            // 
            // Login
            // 
            Login.Location = new Point(26, 96);
            Login.Name = "Login";
            Login.Size = new Size(216, 72);
            Login.TabIndex = 1;
            Login.Text = "Login";
            Login.UseVisualStyleBackColor = true;
            Login.Click += Login_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(269, 238);
            Controls.Add(Login);
            Controls.Add(server2);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button server2;
        private Button Login;
    }
}
