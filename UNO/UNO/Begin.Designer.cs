namespace UNO
{
    partial class Begin
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
            this.btnRules = new System.Windows.Forms.Button();
            this.btnJoin = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnRules
            // 
            this.btnRules.Location = new System.Drawing.Point(338, 335);
            this.btnRules.Name = "btnRules";
            this.btnRules.Size = new System.Drawing.Size(114, 50);
            this.btnRules.TabIndex = 9;
            this.btnRules.Text = "How 2 Play";
            this.btnRules.UseVisualStyleBackColor = true;
            // 
            // btnJoin
            // 
            this.btnJoin.Location = new System.Drawing.Point(338, 265);
            this.btnJoin.Name = "btnJoin";
            this.btnJoin.Size = new System.Drawing.Size(114, 50);
            this.btnJoin.TabIndex = 8;
            this.btnJoin.Text = "JOIN";
            this.btnJoin.UseVisualStyleBackColor = true;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(338, 195);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(114, 50);
            this.btnCreate.TabIndex = 7;
            this.btnCreate.Text = "CREATE";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(261, 142);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(278, 22);
            this.textBoxIP.TabIndex = 6;
            this.textBoxIP.Text = "127.0.0.1";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(261, 66);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(278, 22);
            this.textBoxName.TabIndex = 5;
            // 
            // Begin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnRules);
            this.Controls.Add(this.btnJoin);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.textBoxIP);
            this.Controls.Add(this.textBoxName);
            this.Name = "Begin";
            this.Text = "Begin";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRules;
        private System.Windows.Forms.Button btnJoin;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.TextBox textBoxName;
    }
}