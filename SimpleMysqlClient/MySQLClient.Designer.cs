namespace SimpleMysqlClient
{
    partial class MySQLClient
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lbDbName = new System.Windows.Forms.Label();
            this.dgvRecordView = new System.Windows.Forms.DataGridView();
            this.rtbConsole = new System.Windows.Forms.RichTextBox();
            this.tbCmdInput = new System.Windows.Forms.TextBox();
            this.btnExecuteStatement = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnClearInput = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecordView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbDbName
            // 
            this.lbDbName.AutoSize = true;
            this.lbDbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbDbName.Location = new System.Drawing.Point(506, 9);
            this.lbDbName.Name = "lbDbName";
            this.lbDbName.Size = new System.Drawing.Size(85, 24);
            this.lbDbName.TabIndex = 0;
            this.lbDbName.Text = "DbName";
            // 
            // dgvRecordView
            // 
            this.dgvRecordView.AllowUserToAddRows = false;
            this.dgvRecordView.AllowUserToDeleteRows = false;
            this.dgvRecordView.AllowUserToResizeColumns = false;
            this.dgvRecordView.AllowUserToResizeRows = false;
            this.dgvRecordView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvRecordView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvRecordView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvRecordView.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRecordView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvRecordView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvRecordView.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvRecordView.Location = new System.Drawing.Point(3, 3);
            this.dgvRecordView.MultiSelect = false;
            this.dgvRecordView.Name = "dgvRecordView";
            this.dgvRecordView.ReadOnly = true;
            this.dgvRecordView.RowHeadersVisible = false;
            this.dgvRecordView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvRecordView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvRecordView.Size = new System.Drawing.Size(1123, 462);
            this.dgvRecordView.TabIndex = 15;
            // 
            // rtbConsole
            // 
            this.rtbConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbConsole.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.rtbConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rtbConsole.Location = new System.Drawing.Point(3, 4);
            this.rtbConsole.Name = "rtbConsole";
            this.rtbConsole.ReadOnly = true;
            this.rtbConsole.Size = new System.Drawing.Size(1123, 176);
            this.rtbConsole.TabIndex = 2;
            this.rtbConsole.Text = "";
            // 
            // tbCmdInput
            // 
            this.tbCmdInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCmdInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbCmdInput.Location = new System.Drawing.Point(15, 699);
            this.tbCmdInput.Name = "tbCmdInput";
            this.tbCmdInput.Size = new System.Drawing.Size(945, 22);
            this.tbCmdInput.TabIndex = 3;
            // 
            // btnExecuteStatement
            // 
            this.btnExecuteStatement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecuteStatement.Location = new System.Drawing.Point(966, 699);
            this.btnExecuteStatement.Name = "btnExecuteStatement";
            this.btnExecuteStatement.Size = new System.Drawing.Size(75, 23);
            this.btnExecuteStatement.TabIndex = 4;
            this.btnExecuteStatement.Text = "Выполнить";
            this.btnExecuteStatement.UseVisualStyleBackColor = true;
            this.btnExecuteStatement.Click += new System.EventHandler(this.btnExecuteStatement_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(1076, 699);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(62, 23);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Очистить";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 36);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvRecordView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rtbConsole);
            this.splitContainer1.Size = new System.Drawing.Size(1129, 655);
            this.splitContainer1.SplitterDistance = 468;
            this.splitContainer1.TabIndex = 6;
            // 
            // btnClearInput
            // 
            this.btnClearInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearInput.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClearInput.Location = new System.Drawing.Point(1047, 699);
            this.btnClearInput.Name = "btnClearInput";
            this.btnClearInput.Size = new System.Drawing.Size(23, 23);
            this.btnClearInput.TabIndex = 7;
            this.btnClearInput.Text = "X";
            this.btnClearInput.UseVisualStyleBackColor = true;
            this.btnClearInput.Click += new System.EventHandler(this.btnClearInput_Click);
            // 
            // MySQLClient
            // 
            this.AcceptButton = this.btnExecuteStatement;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClearInput;
            this.ClientSize = new System.Drawing.Size(1153, 731);
            this.Controls.Add(this.btnClearInput);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnExecuteStatement);
            this.Controls.Add(this.tbCmdInput);
            this.Controls.Add(this.lbDbName);
            this.Name = "MySQLClient";
            this.Text = "Simple MySQL Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MySQLClient_FormClosing);
            this.Load += new System.EventHandler(this.MySQLClient_Load);
            this.Resize += new System.EventHandler(this.MySQLClient_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecordView)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbDbName;
        private System.Windows.Forms.DataGridView dgvRecordView;
        private System.Windows.Forms.RichTextBox rtbConsole;
        private System.Windows.Forms.TextBox tbCmdInput;
        private System.Windows.Forms.Button btnExecuteStatement;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnClearInput;
    }
}

