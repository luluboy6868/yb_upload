namespace MediRegist
{
    partial class F_jsqd4101
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F_jsqd4101));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btn_clear = new System.Windows.Forms.ToolStripButton();
            this.btn_cx = new System.Windows.Forms.ToolStripButton();
            this.btn_upload = new System.Windows.Forms.ToolStripButton();
            this.btn_close = new System.Windows.Forms.ToolStripButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dgv_detail = new System.Windows.Forms.DataGridView();
            this.dgv_config = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_detail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_config)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_clear,
            this.btn_cx,
            this.btn_upload,
            this.btn_close});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1325, 39);
            this.toolStrip1.TabIndex = 14;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btn_clear
            // 
            this.btn_clear.Image = global::MediRegist.Properties.Resources.basket_close_32;
            this.btn_clear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(75, 36);
            this.btn_clear.Text = "清空";
            // 
            // btn_cx
            // 
            this.btn_cx.Image = global::MediRegist.Properties.Resources.search_32;
            this.btn_cx.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_cx.Name = "btn_cx";
            this.btn_cx.Size = new System.Drawing.Size(75, 36);
            this.btn_cx.Text = "查询";
            this.btn_cx.Click += new System.EventHandler(this.btn_cx_Click);
            // 
            // btn_upload
            // 
            this.btn_upload.Image = global::MediRegist.Properties.Resources.arrow_up_32;
            this.btn_upload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_upload.Name = "btn_upload";
            this.btn_upload.Size = new System.Drawing.Size(75, 36);
            this.btn_upload.Text = "上传";
            this.btn_upload.Click += new System.EventHandler(this.btn_upload_Click);
            // 
            // btn_close
            // 
            this.btn_close.Image = global::MediRegist.Properties.Resources.close_32;
            this.btn_close.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(75, 36);
            this.btn_close.Text = "退出";
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 83);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 27;
            this.dataGridView1.Size = new System.Drawing.Size(1286, 579);
            this.dataGridView1.TabIndex = 15;
            // 
            // dgv_detail
            // 
            this.dgv_detail.AllowUserToAddRows = false;
            this.dgv_detail.AllowUserToDeleteRows = false;
            this.dgv_detail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_detail.Location = new System.Drawing.Point(1152, 42);
            this.dgv_detail.Name = "dgv_detail";
            this.dgv_detail.ReadOnly = true;
            this.dgv_detail.RowHeadersWidth = 51;
            this.dgv_detail.RowTemplate.Height = 27;
            this.dgv_detail.Size = new System.Drawing.Size(39, 33);
            this.dgv_detail.TabIndex = 74;
            this.dgv_detail.Visible = false;
            // 
            // dgv_config
            // 
            this.dgv_config.AllowUserToAddRows = false;
            this.dgv_config.AllowUserToDeleteRows = false;
            this.dgv_config.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_config.Location = new System.Drawing.Point(1094, 42);
            this.dgv_config.Name = "dgv_config";
            this.dgv_config.ReadOnly = true;
            this.dgv_config.RowHeadersWidth = 51;
            this.dgv_config.RowTemplate.Height = 27;
            this.dgv_config.Size = new System.Drawing.Size(39, 33);
            this.dgv_config.TabIndex = 75;
            this.dgv_config.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(493, 40);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 76;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // F_jsqd4101
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1325, 675);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dgv_config);
            this.Controls.Add(this.dgv_detail);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "F_jsqd4101";
            this.Text = "医疗保障基金结算清单(4101)";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.F_jsqd4101_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_detail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_config)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btn_clear;
        private System.Windows.Forms.ToolStripButton btn_cx;
        private System.Windows.Forms.ToolStripButton btn_upload;
        private System.Windows.Forms.ToolStripButton btn_close;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridView dgv_detail;
        private System.Windows.Forms.DataGridView dgv_config;
        private System.Windows.Forms.Button button1;
    }
}