namespace UNO
{
    partial class Server
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
            this.ALL = new System.Windows.Forms.TextBox();
            this.Member = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ALL
            // 
            this.ALL.BackColor = System.Drawing.SystemColors.HighlightText;
            this.ALL.Enabled = false;
            this.ALL.Location = new System.Drawing.Point(306, 60);
            this.ALL.Multiline = true;
            this.ALL.Name = "ALL";
            this.ALL.Size = new System.Drawing.Size(482, 358);
            this.ALL.TabIndex = 0;
            // 
            // Member
            // 
            this.Member.BackColor = System.Drawing.SystemColors.HighlightText;
            this.Member.Enabled = false;
            this.Member.Location = new System.Drawing.Point(12, 60);
            this.Member.Multiline = true;
            this.Member.Name = "Member";
            this.Member.Size = new System.Drawing.Size(253, 358);
            this.Member.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(113, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "USER";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(576, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "MESS";
            // 
            // Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Member);
            this.Controls.Add(this.ALL);
            this.Name = "Server";
            this.Text = "Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChatRoomServer_FormClosing);
            this.Load += new System.EventHandler(this.ChatRoomServer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ALL;
        private System.Windows.Forms.TextBox Member;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}