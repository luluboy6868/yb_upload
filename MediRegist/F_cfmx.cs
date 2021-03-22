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
    public partial class F_cfmx : Form
    {
        public F_cfmx()
        {
            InitializeComponent();
        }

        PulicClass MyClass = new PulicClass();

        private void F_cfmx_Load(object sender, EventArgs e)
        {
            string str_fg1 = "select * from yb_cfmx_upload where cfbh='" + PulicClass.gv_cfbh + "'";
            try
            {
                DataSet ds1 = MyClass.getDataSet(str_fg1, "cx1");
                dataGridView1.DataSource = ds1.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开处方明细失败：" + ex.Message);
                return;
            }
        }
    }
}
