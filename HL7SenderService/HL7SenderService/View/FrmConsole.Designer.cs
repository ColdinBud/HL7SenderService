namespace HL7SenderService.View
{
    partial class FrmConsole
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
            this.components = new System.ComponentModel.Container();
            this.txtMessage = new System.Windows.Forms.RichTextBox();
            this.btnConfig = new System.Windows.Forms.Button();
            this.btnStopSvc = new System.Windows.Forms.Button();
            this.btnStartSvc = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMessage
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtMessage, 4);
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessage.Location = new System.Drawing.Point(0, 73);
            this.txtMessage.Margin = new System.Windows.Forms.Padding(0);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(1083, 608);
            this.txtMessage.TabIndex = 3;
            this.txtMessage.Text = "";
            // 
            // btnConfig
            // 
            this.btnConfig.Location = new System.Drawing.Point(913, 3);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(130, 47);
            this.btnConfig.TabIndex = 2;
            this.btnConfig.Text = "Config Setting";
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // btnStopSvc
            // 
            this.btnStopSvc.Location = new System.Drawing.Point(211, 3);
            this.btnStopSvc.Name = "btnStopSvc";
            this.btnStopSvc.Size = new System.Drawing.Size(130, 47);
            this.btnStopSvc.TabIndex = 1;
            this.btnStopSvc.Text = "Stop Service";
            this.btnStopSvc.UseVisualStyleBackColor = true;
            this.btnStopSvc.Click += new System.EventHandler(this.btnStopSvc_Click);
            // 
            // btnStartSvc
            // 
            this.btnStartSvc.Location = new System.Drawing.Point(3, 3);
            this.btnStartSvc.Name = "btnStartSvc";
            this.btnStartSvc.Size = new System.Drawing.Size(130, 47);
            this.btnStartSvc.TabIndex = 0;
            this.btnStartSvc.Text = "Start Service";
            this.btnStartSvc.UseVisualStyleBackColor = true;
            this.btnStartSvc.Click += new System.EventHandler(this.btnStartSvc_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AllowDrop = true;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.9028F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.16644F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.93076F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 171F));
            this.tableLayoutPanel1.Controls.Add(this.txtMessage, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnStartSvc, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnStopSvc, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnConfig, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.72F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1083, 681);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.TabStop = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FrmConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1083, 681);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FrmConsole";
            this.Text = "FrmConsole";
            this.Load += new System.EventHandler(this.FrmConsole_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.Button btnStopSvc;
        private System.Windows.Forms.Button btnStartSvc;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.RichTextBox txtMessage;
        private System.Windows.Forms.Timer timer1;
    }
}