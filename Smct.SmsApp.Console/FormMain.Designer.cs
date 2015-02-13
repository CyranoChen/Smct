namespace Smct.SmsApp.Console
{
    partial class FormMain
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
            this.gvEmployee = new System.Windows.Forms.DataGridView();
            this.btnSendSms = new System.Windows.Forms.Button();
            this.btnSmsConfig = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gvEmployee)).BeginInit();
            this.SuspendLayout();
            // 
            // gvEmployee
            // 
            this.gvEmployee.BackgroundColor = System.Drawing.Color.White;
            this.gvEmployee.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvEmployee.Location = new System.Drawing.Point(0, 45);
            this.gvEmployee.Name = "gvEmployee";
            this.gvEmployee.RowTemplate.Height = 24;
            this.gvEmployee.Size = new System.Drawing.Size(630, 400);
            this.gvEmployee.TabIndex = 0;
            // 
            // btnSendSms
            // 
            this.btnSendSms.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSendSms.Location = new System.Drawing.Point(510, 10);
            this.btnSendSms.Name = "btnSendSms";
            this.btnSendSms.Size = new System.Drawing.Size(120, 30);
            this.btnSendSms.TabIndex = 1;
            this.btnSendSms.Text = "发送生日短信";
            this.btnSendSms.UseVisualStyleBackColor = true;
            this.btnSendSms.Click += new System.EventHandler(this.btnSendSms_Click);
            // 
            // btnSmsConfig
            // 
            this.btnSmsConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSmsConfig.Location = new System.Drawing.Point(0, 10);
            this.btnSmsConfig.Name = "btnSmsConfig";
            this.btnSmsConfig.Size = new System.Drawing.Size(120, 30);
            this.btnSmsConfig.TabIndex = 2;
            this.btnSmsConfig.Text = "配置短信猫";
            this.btnSmsConfig.UseVisualStyleBackColor = true;
            this.btnSmsConfig.Click += new System.EventHandler(this.btnSmsConfig_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(297, 10);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 30);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "打开端口";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(404, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 30);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "关闭端口";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 446);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnSmsConfig);
            this.Controls.Add(this.btnSendSms);
            this.Controls.Add(this.gvEmployee);
            this.Name = "FormMain";
            this.Text = "上海明东短信生日祝福控制面板";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvEmployee)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gvEmployee;
        private System.Windows.Forms.Button btnSendSms;
        private System.Windows.Forms.Button btnSmsConfig;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnClose;
    }
}

