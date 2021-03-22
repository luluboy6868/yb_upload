namespace MediRegist
{
    partial class F_main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F_main));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.定点数据采集ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.结算清单ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.三大目录管理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.进销存管理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.定点数据采集ToolStripMenuItem,
            this.三大目录管理ToolStripMenuItem,
            this.进销存管理ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1082, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 定点数据采集ToolStripMenuItem
            // 
            this.定点数据采集ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.结算清单ToolStripMenuItem});
            this.定点数据采集ToolStripMenuItem.Name = "定点数据采集ToolStripMenuItem";
            this.定点数据采集ToolStripMenuItem.Size = new System.Drawing.Size(113, 24);
            this.定点数据采集ToolStripMenuItem.Text = "定点数据采集";
            // 
            // 结算清单ToolStripMenuItem
            // 
            this.结算清单ToolStripMenuItem.Name = "结算清单ToolStripMenuItem";
            this.结算清单ToolStripMenuItem.Size = new System.Drawing.Size(198, 26);
            this.结算清单ToolStripMenuItem.Text = "结算清单(4101)";
            this.结算清单ToolStripMenuItem.Click += new System.EventHandler(this.结算清单ToolStripMenuItem_Click);
            // 
            // 三大目录管理ToolStripMenuItem
            // 
            this.三大目录管理ToolStripMenuItem.Name = "三大目录管理ToolStripMenuItem";
            this.三大目录管理ToolStripMenuItem.Size = new System.Drawing.Size(113, 24);
            this.三大目录管理ToolStripMenuItem.Text = "三大目录管理";
            // 
            // 进销存管理ToolStripMenuItem
            // 
            this.进销存管理ToolStripMenuItem.Name = "进销存管理ToolStripMenuItem";
            this.进销存管理ToolStripMenuItem.Size = new System.Drawing.Size(98, 24);
            this.进销存管理ToolStripMenuItem.Text = "进销存管理";
            // 
            // F_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1082, 660);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "F_main";
            this.Text = "DRGs管理系统";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.F_main_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 定点数据采集ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 结算清单ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 三大目录管理ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 进销存管理ToolStripMenuItem;
    }
}