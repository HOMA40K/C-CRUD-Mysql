namespace SimpleMysqlClient
{
    partial class LoginScreen
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.tbDbHost = new System.Windows.Forms.TextBox();
            this.tbDbPort = new System.Windows.Forms.TextBox();
            this.tbDbUser = new System.Windows.Forms.TextBox();
            this.tbDbPass = new System.Windows.Forms.TextBox();
            this.tbDbBase = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.67033F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.32967F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.tbDbHost, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbDbPort, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tbDbUser, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tbDbPass, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.tbDbBase, 1, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(15, 13);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(273, 155);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Хост";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Порт";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Логин";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Пароль";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 133);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "База данных";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(187, 174);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(101, 23);
            this.btnConnect.TabIndex = 5;
            this.btnConnect.Text = "Подключиться";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // tbDbHost
            // 
            this.tbDbHost.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tbDbHost.Location = new System.Drawing.Point(84, 5);
            this.tbDbHost.Name = "tbDbHost";
            this.tbDbHost.Size = new System.Drawing.Size(186, 20);
            this.tbDbHost.TabIndex = 5;
            // 
            // tbDbPort
            // 
            this.tbDbPort.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tbDbPort.Location = new System.Drawing.Point(84, 36);
            this.tbDbPort.Name = "tbDbPort";
            this.tbDbPort.Size = new System.Drawing.Size(67, 20);
            this.tbDbPort.TabIndex = 6;
            // 
            // tbDbUser
            // 
            this.tbDbUser.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tbDbUser.Location = new System.Drawing.Point(84, 67);
            this.tbDbUser.Name = "tbDbUser";
            this.tbDbUser.Size = new System.Drawing.Size(100, 20);
            this.tbDbUser.TabIndex = 7;
            // 
            // tbDbPass
            // 
            this.tbDbPass.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tbDbPass.Location = new System.Drawing.Point(84, 98);
            this.tbDbPass.Name = "tbDbPass";
            this.tbDbPass.Size = new System.Drawing.Size(100, 20);
            this.tbDbPass.TabIndex = 8;
            this.tbDbPass.UseSystemPasswordChar = true;
            // 
            // tbDbBase
            // 
            this.tbDbBase.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tbDbBase.Location = new System.Drawing.Point(84, 129);
            this.tbDbBase.Name = "tbDbBase";
            this.tbDbBase.Size = new System.Drawing.Size(186, 20);
            this.tbDbBase.TabIndex = 9;
            // 
            // LoginScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 209);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnConnect);
            this.Name = "LoginScreen";
            this.Text = "LoginScreen";
            this.Load += new System.EventHandler(this.LoginScreen_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbDbHost;
        private System.Windows.Forms.TextBox tbDbPort;
        private System.Windows.Forms.TextBox tbDbUser;
        private System.Windows.Forms.TextBox tbDbPass;
        private System.Windows.Forms.TextBox tbDbBase;
        private System.Windows.Forms.Button btnConnect;
    }
}