using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediRegist
{
    public partial class F_jsqd4101 : Form
    {
        public F_jsqd4101()
        {
            InitializeComponent();
        }

        PulicClass MyClass = new PulicClass();
        LogClass Log = new LogClass();
        DataView dataView;

        public static new void DoubleBuffered(DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

            pi.SetValue(dgv, setting, null);
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void F_jsqd4101_Load(object sender, EventArgs e)
        {
            string str1 = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            System.IO.FileInfo fi = new System.IO.FileInfo(str1);
            this.Text = this.Text + "    版本号:" + fi.LastWriteTime + "    操作员:" + PulicClass.user_name;
            dataGridView1.Height = this.Size.Height - 150;
            dataGridView1.Width = this.Size.Width - 70;

            //获取上传列表
            string str_fg1 = "SELECT * FROM dbo.mihs_upload_config WHERE status='0'";
            try
            {
                DataSet ds1 = MyClass.getDataSet(str_fg1, "cx1");
                dgv_config.DataSource = ds1.Tables[0];
                dataView = ds1.Tables[0].DefaultView;
                if (dgv_config.Rows.Count <= 0)
                {
                    Log.WriteLogFile("上传列表mihs_upload_config查询为空！");
                    MessageBox.Show("上传列表mihs_upload_config查询为空！");
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("mihs_upload_config查询出错：" + ex.Message);
                MessageBox.Show("mihs_upload_config查询出错：" + ex.Message);
                return;
            }
        }

        private void btn_cx_Click(object sender, EventArgs e)
        {
            string str_sql= "select * from mihs_4101_setlinfo";
            DataSet ds1 = MyClass.getDataSet(str_sql, "cx1");
            dataGridView1.DataSource = ds1.Tables[0];
            DoubleBuffered(dataGridView1, true);
        }

        private void btn_upload_Click(object sender, EventArgs e)
        {
            
            string sbjgbh = Properties.Settings.Default.sbjgbh;
            string zcm = Properties.Settings.Default.zcm;
            string yybm = Properties.Settings.Default.yybm;
            for (int i=0;i<dataGridView1.Rows.Count; i++)
            {
                string str_sql = "select * from mihs_4101_setlinfo";
                DataSet ds1 = MyClass.getDataSet(str_sql, "cx1");
                dgv_detail.DataSource = ds1.Tables[0];
                string str_jylsh = "4101" + dataGridView1.Rows[i].Cells["mdtrt_sn"].Value + DateTime.Now.ToString("yyyyMMddHHmmss");
                string json = "{\"p_info\":\"4101\",\"p_input\":{\"setlinfo\":" + MyClass.GetRowJson(dataGridView1, 0, dataGridView1.Columns.Count - 2, i) + "}" + "}";
                string r = MyClass.getweb_new(sbjgbh, zcm, str_jylsh, "mihs_service_invoke", json, yybm);
                JObject obj = JObject.Parse(r);
                if ((int)obj["resultcode"] >= 0)
                {
                    string bb = obj["mzjyxx_ds"].ToString();
                    //var aa = JsonConvert.DeserializeObject(r);
                    //dynamic model = JsonConvert.DeserializeObject(bb);
                    //this.dataGridView1.DataSource = model;
                }
                else
                {
                    MessageBox.Show("医保调用失败：" + r);
                    return;
                }
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataView.RowFilter = "code='4101'";
            string json = "";
            string[] sArray = dataView[0].Row["params"].ToString().Split('|');
            foreach (string i in sArray)
            {
                string node = i.ToString().Split(',')[0].ToString();
                string tableName = "mihs_4101_" + node;
                string rows = i.ToString().Split(',')[1].ToString();
                string sql = "select * from " + tableName + " where " + dataView[0].Row["primarykey"].ToString() + "='1013454-12'";
                DataSet ds1 = MyClass.getDataSet(sql, "detail");
                dgv_detail.DataSource = null;
                dgv_detail.DataSource = ds1.Tables[0];
                if (dgv_detail.Rows.Count>0)
                {
                    if (rows=="1")
                    {
                        json += "\"" + node + "\":" + MyClass.GetRowJson(dgv_detail, 0, dgv_detail.Columns.Count - Convert.ToInt32(dataView[0].Row["AddColumnCount"]), 0) + ",";
                    }
                    else
                    {
                        json += "\"" + node + "\":" + JsonConvert.SerializeObject(MyClass.GetDgvToTable(dgv_detail, 0, dgv_detail.Columns.Count - Convert.ToInt32(dataView[0].Row["AddColumnCount"]))) + ",";
                    }
                }
            }
            json = "{\"p_info\":\"4101\",\"p_input\":{" + json.TrimEnd(',')+"}}";
            MessageBox.Show(json);

            string sbjgbh = Properties.Settings.Default.sbjgbh;
            string zcm = Properties.Settings.Default.zcm;
            string yybm = Properties.Settings.Default.yybm;
            string str_jylsh = "4101" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string r = MyClass.getweb_new("37060201", zcm, str_jylsh, "mihs_service_invoke", json, yybm);
            JObject obj = JObject.Parse(r);
            if ((int)obj["resultcode"] >= 0)
            {
                MessageBox.Show("调用成功！");
                //string bb = obj["mzjyxx_ds"].ToString();
                //var aa = JsonConvert.DeserializeObject(r);
                //dynamic model = JsonConvert.DeserializeObject(bb);
                //this.dataGridView1.DataSource = model;
            }
            else
            {
                MessageBox.Show("医保调用失败：" + r);
                return;
            }
        }
    }
}
