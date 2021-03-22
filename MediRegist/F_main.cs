using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediRegist
{
    public partial class F_main : Form
    {
        public F_main()
        {
            InitializeComponent();
        }

        private void 结算清单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            F_jsqd4101 F_4101 = new F_jsqd4101();
            F_4101.Show();
        }

        private void F_main_Load(object sender, EventArgs e)
        {
            string str1 = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            System.IO.FileInfo fi = new System.IO.FileInfo(str1);
            this.Text = this.Text + "    版本号:" + fi.LastWriteTime + "    操作员:" + PulicClass.user_name;
        }
    }
}
