using DevExpress.XtraBars;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using DevExpress.ClipboardSource.SpreadsheetML;
using System.Threading;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MediRegist
{
    public partial class Form1 : DevExpress.XtraBars.TabForm
    {
        public Form1()
        {
            InitializeComponent();
        }
        void OnOuterFormCreating(object sender, OuterFormCreatingEventArgs e)
        {
            Form1 form = new Form1();
            form.TabFormControl.Pages.Clear();
            e.Form = form;
            OpenFormCount++;
        }
        static int OpenFormCount = 1;

        PulicClass MyClass = new PulicClass();
        LogClass Log = new LogClass();

        [DllImport("SendRcv.dll")]
        public static extern ulong SendRcv(StringBuilder pdatainput, StringBuilder pdataoutput);

        public string space(int i)
        {
            string str = new string(' ', i);
            return str;
        }

        public string PadRight_hz(string str, int i)
        {
            if (i - Encoding.Default.GetBytes(str).Length >= 0)
            {
                string str1 = space(i - Encoding.Default.GetBytes(str).Length);
                return str + str1;
            }
            else
            {
                Log.WriteLogFile("长度超长，已截取:" + str);
                return substr(str, 0, i);
            }
        }

        public string PadRight_hz_bl(string str, int i,out string str2)
        {
            if (i - Encoding.Default.GetBytes(str).Length >= 0)
            {
                string str1 = space(i - Encoding.Default.GetBytes(str).Length);
                str2 = "";
                return str + str1;
            }
            else
            {
                str2 = substr(str, i, Encoding.Default.GetBytes(str).Length - i);
                return substr(str, 0, i);
            }
        }

        public string substr(string str, int k, int count)
        {
            return System.Text.Encoding.Default.GetString(System.Text.Encoding.Default.GetBytes(str), k, count);
        }

        public string bljx(string str)
        {
            try
            {
                // 获取html元素（htmlContext为html页面字符串）
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(str);  // 加载html页面
                                        //HtmlNode navNode = htmlDoc.GetElementbyId("pageheadszzq");
                                        //richTextBox2.Text = navNode.Attributes["value"].Value;
                HtmlNode navNode = htmlDoc.DocumentNode.SelectSingleNode("/casemodel/table/tbody/tr[2]");
                //MessageBox.Show("InnerText:"+ navNode.InnerText);
                //MessageBox.Show("InnerHtml:" + navNode.InnerHtml);
                //MessageBox.Show("Name:" + navNode.Name);
                return navNode.InnerText.Replace("_tagtemp_nbsp;", " ").Replace("_tagtemp_ldquo;", "\"").Replace("_tagtemp_rdquo;", "\"");
            }
            catch(Exception ex)
            {
                Log.WriteLogFile("病历解析失败：" + ex.Message);
                return "病历解析失败";
            }
        }

        string insCode;
        private void Form1_Load(object sender, EventArgs e)
        {
            string str1 = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            System.IO.FileInfo fi = new System.IO.FileInfo(str1);
            this.Text = this.Text + "    版本号:" + fi.LastWriteTime + "    操作员:" + PulicClass.user_name;
            dateEdit3.Text = DateTime.Now.ToString("yyyy-MM-dd");
            dateEdit4.Text = DateTime.Now.ToString("yyyy-MM-dd");

            tableLayoutPanel1.Width = this.Size.Width - 60;
            //gridControl1.Width = this.Size.Width - 50;
            //groupBox11.Width = this.Size.Width - 60;
            //groupBox11.l = this.Size.Height - 200;
            tableLayoutPanel1.Height = this.Size.Height - 240;
            tableLayoutPanel2.Height = this.Size.Height - 200;
            splitContainer1.Height=this.Size.Height - 200;
            splitContainer1.Width = this.Size.Width - 70;
            dataGridView1.Height = this.Size.Height - 200;
            dataGridView1.Width = this.Size.Width - 70;
            dgv_sh.Height = this.Size.Height - 200;
            dgv_sh.Width = this.Size.Width - 70;
            dataGridView3.Height = this.Size.Height - 200;
            dataGridView3.Width = this.Size.Width - 70;
            label2.Location=new Point(dataGridView1.Location.X, dataGridView1.Location.Y + dataGridView1.Height+5);
            //gridControl1.Height = tableLayoutPanel1.Height - 150;
            //tableLayoutPanel1.RowStyles[1].Height = tableLayoutPanel1.Height - 500;
            tabFormControl1.SelectedPage = tabFormPage1;
            txt_inpatient_no.Focus();
            if (Properties.Settings.Default.Interval>0)
            {
                timer1.Enabled = true;
                timer1.Interval = Properties.Settings.Default.Interval;
            }
            insCode = Properties.Settings.Default.insCode;
            //获取上传列表
            string str_fg1 = "SELECT * FROM dbo.yb_upload_config WHERE status='0'";
            try
            {
                DataSet ds1 = MyClass.getDataSet(str_fg1, "cx1");
                dgv_config.DataSource = ds1.Tables[0];
                if (dgv_config.Rows.Count <= 0)
                {
                    Log.WriteLogFile("上传列表yb_upload_config查询为空！");
                    MessageBox.Show("上传列表yb_upload_config查询为空！");
                    return;
                }
                dgv_config1.DataSource = ds1.Tables[0];
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("yb_upload_config查询出错：" + ex.Message);
                MessageBox.Show("yb_upload_config查询出错：" + ex.Message);
                return;
            }
            for (int i=0;i<dgv_config.Rows.Count;i++)
            {
                comboBox1.Items.Add(dgv_config.Rows[i].Cells["serviceName"].Value + "|" + dgv_config.Rows[i].Cells["serviceType"].Value);
            }
        }

        /// <summary>
        /// 根据生日计算年龄
        /// </summary>
        /// <param name="Birthdate"></param>
        /// <returns></returns>
        public static string GetAgeByBirth(string Birthdate)
        {
            string ages = string.Empty;
            try
            {
                if (Birthdate.Trim().Length==8)
                {
                    Birthdate = Birthdate.Trim().Substring(0, 4) + "-" + Birthdate.Trim().Substring(4, 2) + "-" + Birthdate.Trim().Substring(6, 2);
                }
                //年龄格式化
                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyy-MM-dd";
                DateTime dt = Convert.ToDateTime(Birthdate, dtFormat);
                int age = DateTime.Now.Year - dt.Year;
                //if (DateTime.Now.Month < dt.Month || (DateTime.Now.Month == dt.Month && DateTime.Now.Day < dt.Day)) age--;
                //TimeSpan ts = DateTime.Now - dt;
                //ages = age == 0 ? "-" + ts.Days : age.ToString();
                ages = age.ToString() + "岁";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return ages;
        }

        private void toolStripButton12_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txt_name_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar == '\r')
            //    com_sex.Focus();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            //gridView1.Columns.Clear();
            if (dgv_config.Rows.Count<=0)
            {
                MessageBox.Show("列表数据为空，请核实！");
                return;
            }
            string str_sql="";
            string serverType = comboBox1.Text.Trim();
            if (serverType == "")
            {
                for (int i = 0;i< dgv_config.Rows.Count;i++)
                {
                    if(i>0)
                    {
                        str_sql += "\r" + "union all"+"\r";
                    }
                    str_sql += "select patient_id,admiss_times,status,upload_date,message from " + dgv_config.Rows[i].Cells["serviceType"].Value + "_upload";
                }
                str_sql = "(" + str_sql + ") a";
            }
            else
            {
                int m = serverType.IndexOf("|")+1;
                str_sql = serverType.Substring(m, serverType.Length - m) + "_upload a";
            }
            
            str_sql = "select a.patient_id,b.inpatient_no 住院号,a.admiss_times 住院次数,b.name 姓名,(select name from zd_unit_code where code=b.dept) 科室,(select name from zd_unit_code where code=b.ward) 病区,b.admiss_date 入院日期,b.dis_date 出院日期,status 状态,message 错误信息,upload_date 上传时间 from "+str_sql;
            str_sql = str_sql + ",zy_inactpatient b where a.patient_id=b.patient_id collate Chinese_PRC_CI_AS and a.admiss_times=b.admiss_times and status";
            if (rbtn_all.Checked)
            {
                str_sql = str_sql + " is not null";
            }
            if (rbtn_suc.Checked)
            {
                str_sql = str_sql + "='100000'";
            }
            if (rbtn_error.Checked)
            {
                str_sql = str_sql + "<>'100000'";
            }
            if (rq1.Text.Trim() != "")
            {
                str_sql = str_sql + " and upload_date>='" + rq1.Text.Trim() + "'";
            }
            if (rq2.Text.Trim() != "")
            {
                str_sql = str_sql + " and upload_date<'" + Convert.ToDateTime(rq2.Text).AddDays(1).ToShortDateString() + "'";
            }
            if (txt_zyh.Text.Trim()!="")
            {
                str_sql = str_sql + " and b.inpatient_no ='" + txt_zyh.Text.Trim() + "'";
            }
            if (txt_zycs.Text.Trim() != "")
            {
                str_sql = str_sql + " and b.admiss_times=" + txt_zycs.Text.Trim() + "";
            }
            DataSet ds1 = MyClass.getDataSet(str_sql, "cx1");
            gridControl1.DataSource = ds1.Tables[0];
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            PrintingSystem print = new DevExpress.XtraPrinting.PrintingSystem();
            PrintableComponentLink link = new PrintableComponentLink(print);
            print.Links.Add(link);
            link.Component = gridControl1;//这里可以是可打印的部件
            link.Landscape = true;   //是否横向打印
            link.PaperKind = System.Drawing.Printing.PaperKind.A4;  //纸张类型
            link.PaperKind = System.Drawing.Printing.PaperKind.A4;  //纸张类型
            link.Margins.Left= 6 ;
            link.Margins.Right = 6;
            string _PrintHeader = "医保上传查询";
            PageHeaderFooter phf = link.PageHeaderFooter as PageHeaderFooter;
            phf.Header.Content.Clear();
            phf.Header.Content.AddRange(new string[] { "", _PrintHeader, "" });
            phf.Header.Font = new System.Drawing.Font("宋体", 14, System.Drawing.FontStyle.Bold);
            phf.Header.LineAlignment = BrickAlignment.Center;
            //phf.Footer.Content.Clear();
            //phf.Footer.Content.AddRange(new string[] { "", richTextBox1.Text, "" });
            //phf.Footer.Content.AddRange(new string[] { "", "dddddd", "" });
            //phf.Footer.Font = new System.Drawing.Font("宋体", 9, System.Drawing.FontStyle.Regular);
            //phf.Footer.LineAlignment = BrickAlignment.None;
            link.CreateDocument(); //建立文档
            print.PreviewFormEx.Show();//进行预览
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "导出Excel";
            fileDialog.Filter = "Excel文件(*.xls)|*.xls";
            DialogResult dialogResult = fileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();
                gridControl1.ExportToXls(fileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            //txt_zyh.Text = "xxxxxxxx";
            //toolStripButton9_Click(null, null);
            gridView1.Columns.Clear();
            rq1.Text = "";
            rq2.Text = "";
            txt_zyh.Text = "";
            //textEdit2.Text = "";
            txt_zycs.Text = "";
            //textEdit4.Text = "";
            //textEdit5.Text = "";
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            if (textEdit6.Text=="" & textEdit8.Text=="")
            {
                MessageBox.Show("身份证号和卡号不能同时为空！");
                return;
            }
            //消费记录
            string sql_sql = "SELECT b.name,CASE b.sex WHEN '1' THEN '男' WHEN '2' THEN '女' ELSE '其他' END sex,b.social_no,dbo.ComputeAge(b.birthday,GETDATE()) age,b.home_tel,b.home_street,a.happen_date,c.name charge_name,CAST(a.charge_price AS DECIMAL(12,2)) charge_price,a.charge_amount*a.caoyao_fu amount,CAST(a.charge_price*a.charge_amount*a.caoyao_fu AS DECIMAL(12,2)) je";
            sql_sql = sql_sql + " FROM dbo.view_mz_detail_charge a,dbo.mz_patient_mi b,view_zd_charge c";
            sql_sql = sql_sql + " where a.patient_id=b.patient_id AND a.charge_code=c.code AND a.serial_no=c.serial AND a.charge_status > '1'";
            sql_sql = sql_sql + " and happen_date>='" + dateEdit4.Text + "' AND happen_date<'"+ Convert.ToDateTime(dateEdit3.Text).AddDays(1).ToShortDateString()+"'";
            if (textEdit6.Text!="")
            {
                sql_sql = sql_sql + " and b.p_bar_code='"+textEdit6.Text+"'";
            }
            if (textEdit8.Text != "")
            {
                sql_sql = sql_sql + " and b.social_no='" + textEdit8.Text + "'";
            }
            sql_sql = sql_sql+" order by happen_date desc";
            DataSet ds1 = MyClass.getDataSet(sql_sql, "cx1");
            gridControl2.DataSource = ds1.Tables[0];
            //充值记录
            string sql_sql1 = "SELECT a.card_no,CASE a.depo_status WHEN '2' THEN -a.charge ELSE a.charge END charge,a.depo_date,dbo.GetICRealBalance(a.patient_id) balance";
            sql_sql1 = sql_sql1 + " FROM dbo.ic_deposit a,dbo.mz_patient_mi b";
            sql_sql1 = sql_sql1 + " WHERE a.patient_id=b.patient_id";
            sql_sql1 = sql_sql1 + " and depo_date>='" + dateEdit4.Text + "' AND depo_date<'" + Convert.ToDateTime(dateEdit3.Text).AddDays(1).ToShortDateString() + "'";
            if (textEdit6.Text != "")
            {
                sql_sql1 = sql_sql1 + " and b.p_bar_code='" + textEdit6.Text + "'";
            }
            if (textEdit8.Text != "")
            {
                sql_sql1 = sql_sql1 + " and b.social_no='" + textEdit8.Text + "'";
            }
            sql_sql1 = sql_sql1 + " order by depo_date desc";
            DataSet ds2 = MyClass.getDataSet(sql_sql1, "cx2");
            gridControl3.DataSource = ds2.Tables[0];
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            textEdit6.Text = "xxxxxxxx";
            toolStripButton16_Click(null, null);

            //dateEdit4.Text = DateTime.Now.ToString("yyyy-MM-dd");
            //dateEdit3.Text = DateTime.Now.ToString("yyyy-MM-dd");
            textEdit6.Text = "";
            textEdit8.Text = "";
        }

        private void toolStripButton19_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            toolStripButton14_Click(null, null);
            string parm = "172.28.20.158|1|00000|全院|00000||";
            Boolean b = Read_card.chisHICInit(ref parm);
            StringBuilder builder = new StringBuilder(512);
            StringBuilder builder1 = new StringBuilder(512);
            var builder2 = new StringBuilder(512);
            byte[] b_ret = new byte[16];
            Boolean a = Read_card.chisGetHICNo(builder, builder1);
            Read_card.chisHICRelease();
            textEdit6.Text = builder.ToString();
            if ((textEdit6.Text).Trim().Length > 3)
            {
                toolStripButton16_Click(null, null);
            }
        }

        private void toolStripButton17_Click(object sender, EventArgs e)
        {
            toolStripButton14_Click(null, null);
            string str_sfzc;
            byte[] rbuff = new byte[256 + 1];
            int m = Read_card.iReadIdentityCard(0, rbuff);
            if (m > 0)
            {
                str_sfzc = Encoding.Default.GetString(rbuff);
                string[] array = str_sfzc.Split('|');
                textEdit8.Text = array[5];
                toolStripButton16_Click(null, null);
            }
            else
            {
                MessageBox.Show("读身份证出错，请检查读卡器设备是否正常！");
            }
        }

        string flag="rg";


        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dt1 = Convert.ToDateTime(DateTime.Now.ToShortTimeString());
            DateTime dt2 = Convert.ToDateTime(Properties.Settings.Default.RunTime);
            DateTime dt3 = Convert.ToDateTime(Properties.Settings.Default.RunTime).AddMilliseconds(Properties.Settings.Default.Interval);
            if ( dt1>= dt2 && dt1 < dt3)
            {
                flag = "zd"; //系统自动点击按钮
                //do
                //{
                //    button4_Click(null, null); 
                //    if (dataGridView1.Rows.Count>0)
                //    {
                //        button3_Click(null, null);
                //    }
                //}
                //while (dataGridView1.Rows.Count > 0);
                button3_Click(null, null);
                flag = "rg"; //人工点击按钮
            }
        }
        /// <summary>
        /// 缓冲以使滑动滚轮时不卡
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="setting"></param>
        public static new void DoubleBuffered(DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

            pi.SetValue(dgv, setting, null);
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            //string ss = dataGridView1.CurrentRow.Cells["patient_id"].Value.ToString();
            string ss = RequstEmrData(dataGridView1.CurrentRow.Cells["patient_id"].Value.ToString(), dataGridView1.CurrentRow.Cells["admiss_times"].Value.ToString());
            //dataGridView1.CurrentRow.Cells["basy"].ToolTipText = ss;
            if (ss != "")
            {
                //MessageBox.Show(ss,"tishi", MessageBoxButtons.OK, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                MessageBox.Show(ss);
            }
        }

        private string Computercount()
        {
            if (dataGridView1.Rows.Count>0)
            {
                int check= 0;
                int uncheck= 0;
                for (int i=0;i<dataGridView1.Rows.Count;i++)
                {
                    if (dataGridView1.Rows[i].Cells["check_status"].Value.ToString()=="1")
                    {
                        check = check + 1;
                    }
                    else
                    {
                        uncheck = uncheck + 1;
                    }
                }
                return "共" + dataGridView1.Rows.Count.ToString() + "人，审核通过" + check.ToString() + "人，未通过" + uncheck.ToString() + "人！";
            }
            else
            {
                return "共0人，审核通过0人，未通过0人！";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //dataGridView1.DataSource = null;
            string str_fg1 = "select * from yb_upload_main where upload_status<>'1'";
            if (txt_inpatient_no.Text != "")
            {
                str_fg1 = str_fg1 + " and inpatient_no='" + txt_inpatient_no.Text + "'";
            }
            if (txt_times.Text != "")
            {
                str_fg1 = str_fg1 + " and admiss_times='" + txt_times.Text + "'";
            }
            if (rbtnCheck.Checked)
            {
                str_fg1 = str_fg1 + " and check_status='1'";
            }
            if (rbtnUncheck.Checked)
            {
                str_fg1 = str_fg1 + " and check_status='0'";
            }
            if (date_cash_b.Text != "")
            {
                str_fg1 += " and cash_date>='" + date_cash_b.Text + "'";
            }
            if (date_cash_e.Text != "")
            {
                str_fg1 += " and cash_date<'" + Convert.ToDateTime(date_cash_e.Text).AddDays(1).ToShortDateString() + "'";
            }
            try
            {
                DataSet ds1 = MyClass.getDataSet(str_fg1, "cx1");
                dataGridView1.DataSource = ds1.Tables[0];
                DoubleBuffered(dataGridView1, true);
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("查询出错：" + ex.Message);
                return;
            }
            label1.Text = Computercount();
            //显示背景颜色
            //if (dataGridView1.Rows.Count>0)
            //{
            //    for (int i=0;i<dataGridView1.Rows.Count;i++)
            //    {
            //        if (dataGridView1.Rows[i].Cells["check_status"].Value.ToString() == "0")
            //        {
            //            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Pink;
            //        }
            //        if (dataGridView1.Rows[i].Cells["upload_status"].Value.ToString()!="1" && dataGridView1.Rows[i].Cells["cash_date"].Value.ToString()!="")
            //        {
            //            DateTime d1 = Convert.ToDateTime(dataGridView1.Rows[i].Cells["cash_date"].Value.ToString());
            //            DateTime d2 = DateTime.Now;
            //            TimeSpan ts = d2 - d1;
            //            int differenceInDays = ts.Days;
            //            if (differenceInDays > 10)
            //            {
            //                dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
            //            }
            //        }
            //    }
            //}



            //flag = "ba1";
        }

        public DataTable GetDgvToTable(DataGridView dgv, int m, int n)
        {
            DataTable dt = new DataTable();

            // 列强制转换
            for (int count = m; count < n + m; count++)
            {
                DataColumn dc = new DataColumn(dgv.Columns[count].Name.ToString());
                dt.Columns.Add(dc);
            }

            // 循环行
            for (int count = 0; count < dgv.Rows.Count; count++)
            {
                //if (Convert.ToBoolean(dgv.Rows[count].Cells["colu_sele"].Value))
                //{
                DataRow dr = dt.NewRow();
                for (int countsub = m; countsub < n + m; countsub++)
                {
                    dr[countsub - m] = Convert.ToString(dgv.Rows[count].Cells[countsub].Value);
                }
                dt.Rows.Add(dr);
                //}
            }
            return dt;
        }
        public DataTable GetDgvToTableNew(DataGridView dgv, int m, int n,int startRow,int endRow)
        {
            DataTable dt = new DataTable();

            // 列强制转换
            for (int count = m; count < n + m; count++)
            {
                DataColumn dc = new DataColumn(dgv.Columns[count].Name.ToString());
                dt.Columns.Add(dc);
            }

            // 循环行
            for (int count = startRow; count <= endRow; count++)
            {
                //if (Convert.ToBoolean(dgv.Rows[count].Cells["colu_sele"].Value))
                //{
                DataRow dr = dt.NewRow();
                for (int countsub = m; countsub < n + m; countsub++)
                {
                    dr[countsub - m] = Convert.ToString(dgv.Rows[count].Cells[countsub].Value);
                }
                dt.Rows.Add(dr);
                //}
            }
            return dt;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count <= 0 && flag == "rg")
            {
                Log.WriteLogFile("无上传数据！");
                return;
            }

            if (dgv_config.Rows.Count <= 0)
            {
                Log.WriteLogFile("表单列表为空！");
                return;
            }

            if (flag == "rg")
            {
                //更新yb_upload_main上传状态
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells[0];
                    Boolean flag_c = Convert.ToBoolean(checkCell.Value);
                    string str_sql;
                    if (flag_c)
                    {
                        if (flag == "rg" && dataGridView1.Rows[i].Cells["check_status"].Value.ToString() == "0")
                        {
                            if (MessageBox.Show("您选择上传的病人中存在审核结果未通过的，是否上传？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                string sql_temp = "insert into yb_upload_uncheck(patient_id,admiss_times,opera,happen_date) VALUES('" + dataGridView1.Rows[i].Cells["patient_id"].Value.ToString();
                                sql_temp += "'," + dataGridView1.Rows[i].Cells["admiss_times"].Value.ToString();
                                sql_temp += ",'" + PulicClass.user_id + "',getdate())";
                                int sql = MyClass.getsqlcom(sql_temp);
                                if (sql <= 0)
                                {
                                    MessageBox.Show("记录yb_upload_uncheck失败！");
                                    Log.WriteLogFile("记录yb_upload_uncheck失败！" + sql_temp);
                                    return;
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        str_sql = "UPDATE yb_upload_main SET upload_status='2' WHERE patient_id='" + dataGridView1.Rows[i].Cells["patient_id"].Value + "' AND admiss_times=" + dataGridView1.Rows[i].Cells["admiss_times"].Value.ToString();
                    }
                    else
                    {
                        str_sql = "UPDATE yb_upload_main SET upload_status='3' WHERE patient_id='" + dataGridView1.Rows[i].Cells["patient_id"].Value + "' AND admiss_times=" + dataGridView1.Rows[i].Cells["admiss_times"].Value.ToString();
                    }
                    int i_sql = MyClass.getsqlcom(str_sql);
                    if (i_sql <= 0)
                    {
                        //MessageBox.Show("更新上传状态失败！");
                        txt_yc.Text = (Convert.ToInt32(txt_yc.Text) + 1).ToString();
                        Log.WriteLogFile("更新上传状态失败:" + str_sql);
                        return;
                    }
                }
            }
            else
            {
                string str_sql = "update yb_upload_main set upload_status='2' where check_status='1' and upload_status<>'1'";
                int i_sql = MyClass.getsqlcom(str_sql);
                if (i_sql <= 0)
                {
                    //MessageBox.Show("更新上传状态失败！");
                    txt_yc.Text = (Convert.ToInt32(txt_yc.Text) + 1).ToString();
                    Log.WriteLogFile("更新上传状态失败:" + str_sql);
                    return;
                }
            }

            //判断是否有需要上传数据
            SqlDataReader temDR = MyClass.getcom("select count(1) from yb_upload_main where upload_status='2'");
            bool ifcom = temDR.Read();
            if (ifcom)
            {
                if (temDR.GetInt32(0) == 0)
                {
                    Log.WriteLogFile("无要上传数据！");
                    return;
                }
                else
                {
                    Log.WriteLogFile("需要上传病人数为：" + temDR.GetInt32(0).ToString());
                }
            }

            //开始上传
            for (int i = 0; i < dgv_config.Rows.Count; i++)
            {
                string serviceType = dgv_config.Rows[i].Cells["serviceType"].Value.ToString();
                int columnCount = Convert.ToInt32(dgv_config.Rows[i].Cells["columnCount"].Value);
                string str_sql = "SELECT a.* FROM " + serviceType + "_upload" + " a,yb_upload_main b WHERE a.patient_id=b.patient_id collate Chinese_PRC_CI_AS AND a.admiss_times=b.admiss_times AND b.upload_status='2'";
                try
                {
                    DataSet ds1 = MyClass.getDataSet(str_sql, "cx1");
                    dgv_detail.DataSource = ds1.Tables[0];
                    int hs = dgv_detail.Rows.Count; //行数
                    int forCount = Properties.Settings.Default.forCount;  //多少行一循环
                    if (hs > 0)
                    {
                        int xhcs = (int)Math.Ceiling(Convert.ToDouble(hs) / forCount);
                        int end;
                        string str_sql1 = "";
                        for (int s = 0; s < xhcs; s++)
                        {
                            end = hs < (s + 1) * forCount ? hs : (s + 1) * forCount;
                            Log.WriteLogFile("第" + (s + 1).ToString() + "次循环，上传第" + (s * forCount + 1).ToString() + "至" + end.ToString() + "条");
                            string str_reqSnum = MyClass.GetdateDb(1) + PulicClass.user_id;
                            string json = "{\"reqSnum\":\"" + str_reqSnum + "\",\"reqTime\":\"" + str_reqSnum.Substring(0, 14) + "\",\"insCode\":\"" + insCode + "\",\"serviceType\":\"" + serviceType + "\",\"sign\":\"\",\"data\":" + JsonConvert.SerializeObject(GetDgvToTableNew(dgv_detail, 0, columnCount, s * forCount, end - 1)) + "}";
                            Log.WriteLogFile("入参:" + json);
                            try
                            {
                                DrProcWebServiceImplService ws = new DrProcWebServiceImplService();
                                ws.Timeout = 1800000;
                                string r = ws.collectData(json);
                                Log.WriteLogFile("出参:" + r);
                                JObject obj = JObject.Parse(r);
                                string str_code = obj["code"].ToString();
                                string str_msg = obj["msg"].ToString();
                                str_sql1 = "update a set status='" + str_code + "',upload_date=GETDATE(),upload_opera='" + PulicClass.user_id + "',message='" + str_msg + "' FROM " + serviceType + "_upload" + " a,yb_upload_main b WHERE a.patient_id=b.patient_id AND a.admiss_times=b.admiss_times AND b.upload_status='2'";

                                if (str_code == "100000")
                                {
                                    txt_suc.Text = (Convert.ToInt32(txt_suc.Text) + 1).ToString();
                                }
                                else
                                {
                                    txt_fail.Text = (Convert.ToInt32(txt_fail.Text) + 1).ToString();
                                }
                            }
                            catch (Exception ex)
                            {
                                txt_yc.Text = (Convert.ToInt32(txt_yc.Text) + 1).ToString();
                                Log.WriteLogFile(serviceType + "调用地纬服务出错：" + ex.Message);
                                return;
                            }
                        }
                        int i_sql = MyClass.getsqlcom(str_sql1);
                        if (i_sql <= 0)
                        {
                            txt_yc.Text = (Convert.ToInt32(txt_yc.Text) + 1).ToString();
                            Log.WriteLogFile("更新明细状态失败:" + str_sql1);
                            //return;
                        }
                    }
                    else
                    {
                        txt_yc.Text = (Convert.ToInt32(txt_yc.Text) + 1).ToString();
                        Log.WriteLogFile(serviceType + "查询为空：" + str_sql);
                    }
                }
                catch (Exception ex)
                {
                    txt_yc.Text = (Convert.ToInt32(txt_yc.Text) + 1).ToString();
                    Log.WriteLogFile(serviceType + "查询出错：" + ex.Message);
                    return;
                }
            }
            string opera = "";
            if (flag == "rg")
            {
                opera = PulicClass.user_id;
            }
            else
            {
                opera = "00000";
            }

            string str_sql2 = "UPDATE yb_upload_main SET upload_status='1',upload_date=GETDATE(),upload_opera='"+ opera + "' WHERE upload_status='2'";
            int i_sql1 = MyClass.getsqlcom(str_sql2);
            if (i_sql1 <= 0)
            {
                txt_yc.Text = (Convert.ToInt32(txt_yc.Text) + 1).ToString();
                Log.WriteLogFile("更新上传状态失败1:" + str_sql2);
                //return;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            richTextBox3.Text = substr(richTextBox1.Text, Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string sdatainput = richTextBox1.Text;
            StringBuilder builder = new StringBuilder(sdatainput);
            StringBuilder builder1 = new StringBuilder(4096);
            try
            {
                SendRcv(builder, builder1);
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("上传出错：" + ex.Message);
            }
            Log.WriteLogFile("上传出参：" + builder1.ToString());
            richTextBox3.Text = builder1.ToString();
            string str_status = builder1.ToString().Substring(25, 5);
            string str_message;
            if (str_status != "00000")
            {
                str_message = substr(builder1.ToString(), 3992, 100).Trim();
                MessageBox.Show(str_message);
            }
            else
            {
                //str_message = substr(builder1.ToString(), 226, 16);  //返回值，维护流水号
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //string ls_head = "SSSS" + space(16) + "YD029" + space(5) + Properties.Settings.Default.yljgdm + Properties.Settings.Default.sfzddm + space(64) + PulicClass.user_id.PadRight(10) + space(7) + space(100);
            //string s = PadRight_hz("王洪超", 50) + PadRight_hz("371428198712020579", 50) + space(3666);
            //string ls_zz = space(100) + "ZZZZ";
            //richTextBox1.Text = ls_head + s + ls_zz;
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //if (e.ColumnIndex==0)
            //{
            //    MessageBox.Show(e.Value.ToString());
            //}
            //if (dataGridView1.Columns[e.ColumnIndex].Name == "patient_id")
            //{
            //    MessageBox.Show(e.Value.ToString());
            //}
// patient_id,admiss_times,inpatient_no,name,basy,zyzd,ryjl,zyyz,cyxj,jybg,ssjl,icd10,icd9,check_status,upload_status,input_date,check_times,upload_date,admiss_date,dis_date,cash_date
            if (dataGridView1.Columns[e.ColumnIndex].Name == "basy")  //病案首页
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "未提交";
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "zyzd") //主要诊断
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "未填写";
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "ryjl") //入院记录
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "缺失";
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "zyyz") //住院医嘱
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "缺失";
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "cyxj")  //出院小结
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "缺失";
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "jybg") //检验报告
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "缺失";
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "jcbg")  //检查报告
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "缺失";
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "ssjl") //手术记录
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "缺失";
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "icd10")
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "符合" : "不符合";
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "icd9")
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "符合" : "不符合";
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "check_status") //审核结果
            {
                if(object.Equals(e.Value,"0"))
                {
                    e.Value = "未通过";
                    //e.CellStyle.ForeColor = Color.Red;
                    //dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Pink;
                }
                else
                {
                    e.Value = "通过";
                    if (dataGridView1.Rows[e.RowIndex].Cells["cash_date"].Value.ToString()!="")
                    {
                        DateTime d1 = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["cash_date"].Value.ToString());
                        DateTime d2 = DateTime.Now;
                        TimeSpan ts = d2 - d1;
                        int differenceInDays = ts.Days;
                        if (differenceInDays<=9)
                        {
                            DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[e.RowIndex].Cells[0];
                            checkCell.Value = true;
                        }
                    }
                    
                }
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "upload_status") //上传状态
            {
                //e.Value = Convert.ToInt32(e.Value) == 1 ? "已上传" : "未上传";
                if (object.Equals(e.Value, "0"))
                {
                    e.Value = "未上传";
                    //DateTime d1 = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["cash_date"].Value.ToString());
                    //DateTime d2 = DateTime.Now;
                    //TimeSpan ts = d2 - d1;
                    //int differenceInDays = ts.Days;
                    //if (differenceInDays>10)
                    //{
                    //    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                    //}
                }
                else if (object.Equals(e.Value, "1"))
                {
                    e.Value = "已上传";
                }
                else if (object.Equals(e.Value, "2"))
                {
                    e.Value = "已选中";
                }
                else if (object.Equals(e.Value, "3"))
                {
                    e.Value = "未选中";
                }
                else
                {
                    e.Value = "其他";
                }
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "check_times") //是否审核
            {
                e.Value = Convert.ToInt32(e.Value) == 0 ? "未审核" : "审核";
            }
            //if (e.ColumnIndex == 4)
            //{
            //    if (dataGridView1.Rows[e.RowIndex].Cells["cash_date"].Value.ToString() != "")
            //    {
            //        DateTime d1 = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["cash_date"].Value.ToString());
            //        DateTime d2 = DateTime.Now;
            //        TimeSpan ts = d2 - d1;
            //        int differenceInDays = ts.Days;
            //        if (differenceInDays > 10)
            //        {
            //            dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
            //        }
            //    }
            //}

        }

        string xuan_flag = "0";
        private void button12_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.Rows.Count;
            if (xuan_flag == "0")
            {
                for (int i = 0; i < index; i++)
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells[0];
                    //Boolean flag = Convert.ToBoolean(checkCell.Value);
                    //if (flag == false)
                    //{
                    checkCell.Value = true;
                    //}
                    //else { continue; }
                }
                xuan_flag = "1";
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells[0];
                    //Boolean flag = Convert.ToBoolean(checkCell.Value);
                    //if (flag == false)
                    //{
                    checkCell.Value = false;
                    //}
                    //else { continue; }
                }
                xuan_flag = "0";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count>0)
            {
                try
                {
                    for (int i=0; i<dataGridView1.Rows.Count;i++)
                    {
                        DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells[0];
                        if (Convert.ToBoolean(checkCell.Value))
                        {
                            MyClass.con_open();
                            SqlDataReader temDR = MyClass.getcom("exec proc_yb_reimport '" + dataGridView1.Rows[i].Cells["patient_id"].Value.ToString() + "','" + dataGridView1.Rows[i].Cells["admiss_times"].Value.ToString() + "'");
                            bool ifcom = temDR.Read();
                            if (ifcom)
                            {
                                if (temDR.GetString(0) == "1")
                                {
                                    //MessageBox.Show("导入成功！");
                                }
                                else
                                {
                                    MessageBox.Show("导入失败：" + temDR.GetString(1));
                                    return;
                                }
                                PulicClass.My_con.Close();
                                PulicClass.My_con.Dispose();
                            }
                            else
                            {
                                MessageBox.Show("执行失败！");
                                return;
                            }
                        }
                    }
                    MessageBox.Show("导入成功！");
                }
                catch(Exception ex)
                {
                    MessageBox.Show("重新导入失败："+ex.Message);
                    Log.WriteLogFile("重新导入失败： " + ex.Message);
                }
                
            }
            
        }

        //重传.查询
        private void button6_Click(object sender, EventArgs e)
        {
            string str_fg1 = "select * from yb_upload_main where upload_status='1'";
            if (txt_zyh2.Text != "")
            {
                str_fg1 = str_fg1 + " and inpatient_no='" + txt_zyh2.Text + "'";
            }
            if (txt_zycs2.Text != "")
            {
                str_fg1 = str_fg1 + " and admiss_times='" + txt_zycs2.Text + "'";
            }
            if (date_kssj2.Text!="")
            {
                str_fg1 = str_fg1 + " and upload_date>'" + date_kssj2.Text + "'";
            }
            if (date_jssj2.Text.Trim() != "")
            {
                str_fg1 = str_fg1 + " and upload_date<'" + Convert.ToDateTime(date_jssj2.Text).AddDays(1).ToShortDateString() + "'";
            }
            try
            {
                DataSet ds1 = MyClass.getDataSet(str_fg1, "cx1");
                dataGridView2.DataSource = ds1.Tables[0];
                DoubleBuffered(dataGridView2, true);
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("查询出错：" + ex.Message);
                MessageBox.Show("查询出错：" + ex.Message);
                return;
            }
        }

        //重传.上传
        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count <= 0 && flag == "rg")
            {
                Log.WriteLogFile("无上传数据！");
                MessageBox.Show("无上传数据！");
                return;
            }

            if (dgv_config1.Rows.Count <= 0)
            {
                Log.WriteLogFile("表单列表为空！");
                MessageBox.Show("表单列表为空！");
                return;
            }
            //插入yb_reupload_record表
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView2.Rows[i].Cells[0];
                Boolean flag_c = Convert.ToBoolean(checkCell.Value);
                string  str_sql;
                if (flag_c)
                {
                    for (int i1=0;i1<dgv_config1.Rows.Count;i1++)
                    {
                        DataGridViewCheckBoxCell checkCel2 = (DataGridViewCheckBoxCell)dgv_config1.Rows[i1].Cells[0];
                        Boolean flag = Convert.ToBoolean(checkCel2.Value);
                        if (flag)
                        {
                            str_sql = "insert into yb_reupload_record(patient_id,admiss_times,serviceType,upload_status,upload_date) values('"
                                    + dataGridView2.Rows[i].Cells["patient_id1"].Value + "'," + dataGridView2.Rows[i].Cells["admiss_times1"].Value.ToString() + ",'"
                                    + dgv_config1.Rows[i1].Cells["serviceType"].Value + "','0',getdate())";
                            int i_sql = MyClass.getsqlcom(str_sql);
                            if (i_sql <= 0)
                            {
                                MessageBox.Show("插入yb_reupload_record失败！");
                                Log.WriteLogFile("插入yb_reupload_record失败:" + str_sql);
                                return;
                            }
                        }
                    }
                }
            }
            //判断是否有需要上传数据
            SqlDataReader temDR = MyClass.getcom("select count(1) from yb_reupload_record where upload_status='0'");
            bool ifcom = temDR.Read();
            if (ifcom)
            {
                if (temDR.GetInt32(0) == 0)
                {
                    Log.WriteLogFile("无要上传数据！");
                    return;
                }
                else
                {
                    Log.WriteLogFile("需要上传数为：" + temDR.GetInt32(0).ToString());
                }
            }

            //开始上传
            for (int i = 0; i < dgv_config1.Rows.Count; i++)
            {
                DataGridViewCheckBoxCell checkCel2 = (DataGridViewCheckBoxCell)dgv_config1.Rows[i].Cells[0];
                Boolean flag = Convert.ToBoolean(checkCel2.Value);
                if (flag)
                {
                    string serviceType = dgv_config.Rows[i].Cells["serviceType"].Value.ToString();
                    int columnCount = Convert.ToInt32(dgv_config.Rows[i].Cells["columnCount"].Value);
                    string str_sql = "SELECT a.* FROM " + serviceType + "_upload" + " a,yb_reupload_record b WHERE a.patient_id=b.patient_id AND a.admiss_times=b.admiss_times AND b.upload_status='0' and b.serviceType='" + serviceType+"'";
                    try
                    {
                        DataSet ds1 = MyClass.getDataSet(str_sql, "cx1");
                        dgv_detail.DataSource = ds1.Tables[0];
                        int hs = dgv_detail.Rows.Count; //行数
                        int forCount = Properties.Settings.Default.forCount;  //多少行一循环
                        if (hs > 0)
                        {
                            int xhcs = (int)Math.Ceiling(Convert.ToDouble(hs) / forCount);
                            Log.WriteLogFile(serviceType+"服务需要上传总行数为：" + hs.ToString() + " 每"+ forCount.ToString()+"行上传一次，需要上传"+ xhcs.ToString()+"次");
                            int end;
                            string str_sql1="";
                            for (int s=0;s<xhcs;s++)
                            {
                                int n = (s + 1) * forCount;
                                end = hs < n ? hs : n;
                                Log.WriteLogFile("第" + (s + 1).ToString() + "次循环，上传第"+(s * forCount+1).ToString() +"至" + end.ToString() + "条");
                                string str_reqSnum = MyClass.GetdateDb(1) + PulicClass.user_id;
                                string json = "{\"reqSnum\":\"" + str_reqSnum + "\",\"reqTime\":\"" + str_reqSnum.Substring(0, 14) + "\",\"insCode\":\"" + insCode + "\",\"serviceType\":\"" + serviceType + "\",\"sign\":\"\",\"data\":" + JsonConvert.SerializeObject(GetDgvToTableNew(dgv_detail, 0, columnCount,s* forCount, end-1)) + "}";
                                Log.WriteLogFile("入参:" + json);
                                
                                try
                                {
                                    DrProcWebServiceImplService ws = new DrProcWebServiceImplService();
                                    ws.Timeout = 1800000;
                                    string r = ws.collectData(json);
                                    Log.WriteLogFile("出参:" + r);
                                    JObject obj = JObject.Parse(r);
                                    string str_code = obj["code"].ToString();
                                    string str_msg = obj["msg"].ToString();
                                    str_sql1 = "update yb_reupload_record set upload_status='1',status='" + str_code + "',upload_opera='" + PulicClass.user_id + "',message='" + str_msg + "' WHERE upload_status='0' and serviceType='" + serviceType + "'";
                                }
                                catch (Exception ex)
                                {
                                    Log.WriteLogFile(serviceType + "调用地纬服务出错：" + ex.Message);
                                    MessageBox.Show(serviceType + "调用地纬服务出错：" + ex.Message);
                                    return;
                                }
                            }
                            int i_sql = MyClass.getsqlcom(str_sql1);
                            if (i_sql <= 0)
                            {
                                MessageBox.Show("更新明细状态失败");
                                Log.WriteLogFile("更新明细状态失败:" + str_sql1);
                                return;
                            }
                        }
                        else
                        {
                            Log.WriteLogFile(serviceType + "查询为空：" + str_sql);
                            //MessageBox.Show(serviceType + "查询为空：" + str_sql);  //因为有的表没数据，去掉，不判断
                            //return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLogFile(serviceType + "查询出错：" + ex.Message);
                        MessageBox.Show(serviceType + "查询出错：" + ex.Message);
                        return;
                    }
                }
            }
            MessageBox.Show("上传完成！");
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "未提交";
            }
            if (e.ColumnIndex == 6)
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "未填写";
            }
            if (e.ColumnIndex == 7)
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "缺失";
            }
            if (e.ColumnIndex == 8)
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "缺失";
            }
            if (e.ColumnIndex == 9)
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "缺失";
            }
            if (e.ColumnIndex == 10)
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "缺失";
            }
            if (e.ColumnIndex == 11)
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "缺失";
            }
            if (e.ColumnIndex == 12)
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "正常" : "缺失";
            }
            if (e.ColumnIndex == 13)
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "符合" : "不符合";
            }
            if (e.ColumnIndex == 14)
            {
                e.Value = Convert.ToInt32(e.Value) == 1 ? "符合" : "不符合";
            }
            if (e.ColumnIndex == 15)
            {
                if (object.Equals(e.Value, "0"))
                {
                    e.Value = "未通过";
                    //e.CellStyle.ForeColor = Color.Red;
                    //dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Pink;
                }
                else
                {
                    e.Value = "通过";
                    //DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[e.RowIndex].Cells[0];
                    //checkCell.Value = true;
                }
            }
            if (e.ColumnIndex == 16)
            {
                //e.Value = Convert.ToInt32(e.Value) == 1 ? "已上传" : "未上传";
                if (object.Equals(e.Value, "0"))
                {
                    e.Value = "未上传";
                }
                else if (object.Equals(e.Value, "1"))
                {
                    e.Value = "已上传";
                }
                else if (object.Equals(e.Value, "2"))
                {
                    e.Value = "已选中";
                }
                else if (object.Equals(e.Value, "3"))
                {
                    e.Value = "未选中";
                }
                else
                {
                    e.Value = "其他";
                }
            }
            if (e.ColumnIndex == 18)
            {
                e.Value = Convert.ToInt32(e.Value) == 0 ? "未审核" : "审核";
            }
        }

        string xuan_flag2 = "0";
        //主表全选按钮
        private void button7_Click(object sender, EventArgs e)
        {
            int index = dataGridView2.Rows.Count;
            if (xuan_flag2 == "0")
            {
                for (int i = 0; i < index; i++)
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView2.Rows[i].Cells[0];
                    //Boolean flag = Convert.ToBoolean(checkCell.Value);
                    //if (flag == false)
                    //{
                    checkCell.Value = true;
                    //}
                    //else { continue; }
                }
                xuan_flag2 = "1";
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView2.Rows[i].Cells[0];
                    //Boolean flag = Convert.ToBoolean(checkCell.Value);
                    //if (flag == false)
                    //{
                    checkCell.Value = false;
                    //}
                    //else { continue; }
                }
                xuan_flag2 = "0";
            }
        }

        string xuan_flag1 = "0";
        //服务列表全选按钮
        private void button2_Click(object sender, EventArgs e)
        {
            int index = dgv_config1.Rows.Count;
            if (xuan_flag1 == "0")
            {
                for (int i = 0; i < index; i++)
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dgv_config1.Rows[i].Cells[0];
                    //Boolean flag = Convert.ToBoolean(checkCell.Value);
                    //if (flag == false)
                    //{
                    checkCell.Value = true;
                    //}
                    //else { continue; }
                }
                xuan_flag1 = "1";
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dgv_config1.Rows[i].Cells[0];
                    //Boolean flag = Convert.ToBoolean(checkCell.Value);
                    //if (flag == false)
                    //{
                    checkCell.Value = false;
                    //}
                    //else { continue; }
                }
                xuan_flag1 = "0";
            }
        }

        private void dataGridView2_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.ColumnIndex==2)
            //{
            //    dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "测试内容塑料袋放进塑料袋就翻身解放螺丝钉解放";
            //}
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int hs = dataGridView2.Rows.Count; //行数
            int forCount = 3;  //多少行一循环
            if (hs > 0)
            {
                int xhcs = (int)Math.Ceiling(Convert.ToDouble(hs) / forCount);
                Log.WriteLogFile(serviceType + "服务需要上传总行数为：" + hs.ToString() + " 每" + forCount.ToString() + "行上传一次，需要上传" + xhcs.ToString() + "次");
                int end;
                for (int s = 0; s < xhcs; s++)
                {
                    int ii = (s + 1) * forCount;
                    end = hs < ii ? hs : ii;
                    string str_reqSnum = MyClass.GetdateDb(1) + PulicClass.user_id;
                    string json = "{\"reqSnum\":\"" + str_reqSnum + "\",\"reqTime\":\"" + str_reqSnum.Substring(0, 14) + "\",\"insCode\":\"" + insCode + "\",\"serviceType\":\"" + serviceType + "\",\"sign\":\"\",\"data\":" + JsonConvert.SerializeObject(GetDgvToTableNew(dataGridView2, 1, dataGridView2.Columns.Count-1, s * xhcs, end - 1)) + "}";
                    Log.WriteLogFile("入参:" + json);
                    MessageBox.Show(json);

                }
            }
            //MessageBox.Show(JsonConvert.SerializeObject(GetDgvToTableNew(dataGridView2, 1, dataGridView2.Columns.Count-1, 0, 3)));
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            int hs = dataGridView2.Rows.Count; //行数
            int forCount = 3;  //多少行一循环
            if (hs > 0)
            {
                int xhcs = (int)Math.Ceiling(Convert.ToDouble(hs) / forCount);
                Log.WriteLogFile(serviceType + "服务需要上传总行数为：" + hs.ToString() + " 每" + forCount.ToString() + "行上传一次，需要上传" + xhcs.ToString() + "次");
                int end;
                for (int s = 0; s < xhcs; s++)
                {
                    int ii = (s + 1) * forCount;
                    end = hs < ii ? hs : ii;
                    string str_reqSnum = MyClass.GetdateDb(1) + PulicClass.user_id;
                    string json = "{\"reqSnum\":\"" + str_reqSnum + "\",\"reqTime\":\"" + str_reqSnum.Substring(0, 14) + "\",\"insCode\":\"" + insCode + "\",\"serviceType\":\"" + serviceType + "\",\"sign\":\"\",\"data\":" + JsonConvert.SerializeObject(GetDgvToTableNew(dataGridView2, 1, dataGridView2.Columns.Count - 1, s * forCount, end - 1)) + "}";
                    Log.WriteLogFile("入参:" + json);
                    MessageBox.Show(json);
                }
            }
        }
        public string RequstEmrData(string patient_id,string admiss_times)
        {
            string ss="";
            try
            {
                Log.WriteLogFile("开始调用EmrWebservice（病人id：" + patient_id + "  住院次数：" + admiss_times + ")");
                WebReference.EmrService wr = new WebReference.EmrService();
                wr.Timeout = 300000;
                int times = Convert.ToInt32(admiss_times);
                DataTable dt = wr.GetPATMrStatus(patient_id,times);
                ss = "状态：" + dt.Rows[0][0].ToString() + "\n\r出院时间：" + dt.Rows[0][1].ToString() + "\n\r提交时间：" + dt.Rows[0][2].ToString() + "\n\r编目时间：" + dt.Rows[0][3].ToString();
                Log.WriteLogFile("出参：" + ss);
                Log.WriteLogFile("webservice end");
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("Emr接口调用异常," + ex);
                MessageBox.Show("Emr接口调用异常," + ex);
                return "";
            }
            return ss;
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < dataGridView1.Rows.Count - 1)
            {
                DataGridViewRow dgrSingle = dataGridView1.Rows[e.RowIndex];
                try
                {
                    if (dgrSingle.Cells["check_status"].Value.ToString().Contains("0"))
                    {
                        dgrSingle.DefaultCellStyle.BackColor = Color.Pink;
                    }
                    if (dgrSingle.Cells["cash_date"].Value.ToString() != "")
                    {
                        DateTime d1 = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["cash_date"].Value.ToString());
                        DateTime d2 = DateTime.Now;
                        TimeSpan ts = d2 - d1;
                        int differenceInDays = ts.Days;
                        if (differenceInDays ==10)
                        {
                            dgrSingle.DefaultCellStyle.ForeColor = Color.Yellow;
                        }
                        if (differenceInDays > 10)
                        {
                            dgrSingle.DefaultCellStyle.ForeColor = Color.Red;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void dgv_sh_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv_sh.Columns[e.ColumnIndex].Name=="zyyz_sh")
            {
                string flag;
                if (dgv_sh.Rows[e.RowIndex].Cells["zyyz_sh"].Value.ToString()=="正常")
                {
                    flag = "1";
                }
                else
                {
                    flag = "0";
                }
                string str_sql = "update yb_upload_main_cy set zyyz='" + flag +"',check_opera='"+PulicClass.user_id+"',check_date=getdate()"
                               + " where patient_id='" + dgv_sh.Rows[e.RowIndex].Cells["patient_id_sh"].Value.ToString()
                               + "' and admiss_times=" + dgv_sh.Rows[e.RowIndex].Cells["admiss_times_sh"].Value.ToString();
                int i_sql = MyClass.getsqlcom(str_sql);
                if (i_sql <= 0)
                {
                    MessageBox.Show("更新状态失败:" + str_sql);
                    Log.WriteLogFile("更新状态失败:" + str_sql);
                    return;
                }
            }
            if (dgv_sh.Columns[e.ColumnIndex].Name == "jybg_sh")
            {
                string flag;
                if (dgv_sh.Rows[e.RowIndex].Cells["jybg_sh"].Value.ToString() == "正常")
                {
                    flag = "1";
                }
                else
                {
                    flag = "0";
                }
                string str_sql = "update yb_upload_main_cy set jybg='" + flag + "',check_opera='" + PulicClass.user_id + "',check_date=getdate()"
                               + " where patient_id='" + dgv_sh.Rows[e.RowIndex].Cells["patient_id_sh"].Value.ToString()
                               + "' and admiss_times=" + dgv_sh.Rows[e.RowIndex].Cells["admiss_times_sh"].Value.ToString();
                int i_sql = MyClass.getsqlcom(str_sql);
                if (i_sql <= 0)
                {
                    MessageBox.Show("更新状态失败:" + str_sql);
                    Log.WriteLogFile("更新状态失败:" + str_sql);
                    return;
                }
            }
            if (dgv_sh.Columns[e.ColumnIndex].Name == "jcbg_sh")
            {
                string flag;
                if (dgv_sh.Rows[e.RowIndex].Cells["jcbg_sh"].Value.ToString() == "正常")
                {
                    flag = "1";
                }
                else
                {
                    flag = "0";
                }
                string str_sql = "update yb_upload_main_cy set jcbg='" + flag + "',check_opera='" + PulicClass.user_id + "',check_date=getdate()"
                               + " where patient_id='" + dgv_sh.Rows[e.RowIndex].Cells["patient_id_sh"].Value.ToString()
                               + "' and admiss_times=" + dgv_sh.Rows[e.RowIndex].Cells["admiss_times_sh"].Value.ToString();
                int i_sql = MyClass.getsqlcom(str_sql);
                if (i_sql <= 0)
                {
                    MessageBox.Show("更新状态失败:" + str_sql);
                    Log.WriteLogFile("更新状态失败:" + str_sql);
                    return;
                }
            }
        }

        private void tbtn_close_sh_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbtn_qk_sh_Click(object sender, EventArgs e)
        {
            txt_inpatient_no_sh.Text = "";
            txt_times_sh.Text = "";
            rbtn_all_sh.Checked = true;
            while (this.dgv_sh.Rows.Count != 0)
            {
                this.dgv_sh.Rows.RemoveAt(0);
            }
        }

        private void tbtn_cx_sh_Click(object sender, EventArgs e)
        {
            string str_sql = "select patient_id,admiss_times,inpatient_no,name,admiss_date,dis_date,CASE zyyz WHEN '1' THEN '正常' ELSE '缺失' END zyyz"
                            +", CASE jybg WHEN '1' THEN '正常' ELSE '缺失' END jybg, CASE jcbg WHEN '1' THEN '正常' ELSE '缺失' END jcbg"
                            + " , check_opera, check_date, cash_date, case cash_flag when '1' then '是' else '否' end cash_flag, input_date, check_times  from yb_upload_main_cy where 1 = 1";
            if (date_cysj_b.Text!="")
            {
                str_sql += " and dis_date>='"+date_cysj_b.Text + "'";
            }
            if (date_cysj_e.Text!="")
            {
                str_sql += " and dis_date<'" + Convert.ToDateTime(date_cysj_e.Text).AddDays(1).ToShortDateString() + "'";
            }
            if (txt_inpatient_no_sh.Text!="")
            {
                str_sql += " and inpatient_no='"+txt_inpatient_no_sh.Text+"'";
            }
            if (txt_times_sh.Text!="")
            {
                str_sql += " and admiss_times="+txt_times_sh.Text;
            }
            if (rbtn_wj_sh.Checked)
            {
                str_sql += " and cash_flag=0";
            }
            if (rbtn_yj_sh.Checked)
            {
                str_sql += " and cash_flag=1";
            }
            str_sql += " order by dis_date desc";
            DataSet ds1 = MyClass.getDataSet(str_sql, "cx1");
            dgv_sh.DataSource = ds1.Tables[0];
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count <= 0)
            {
                Log.WriteLogFile("无上传数据！");
                MessageBox.Show("无上传数据！");
                return;
            }
            //更新yb_upload_main表
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView2.Rows[i].Cells[0];
                Boolean flag_c = Convert.ToBoolean(checkCell.Value);
                string str_sql;
                if (flag_c)
                {
                    str_sql = "UPDATE dbo.yb_upload_main SET upload_status='0' where patient_id='"
                            + dataGridView2.Rows[i].Cells["patient_id1"].Value + "' and admiss_times=" + dataGridView2.Rows[i].Cells["admiss_times1"].Value.ToString();
                    int i_sql = MyClass.getsqlcom(str_sql);
                    if (i_sql <= 0)
                    {
                        MessageBox.Show("更新yb_upload_main表失败！");
                        Log.WriteLogFile("更新yb_upload_main表失败:" + str_sql);
                        return;
                    }
                }
            }
            MessageBox.Show("上传状态重置成功");
            button6_Click(null, null);
        }
        //入库统计-清空
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            dateEdit2.Text = "";
            dateEdit1.Text = "";
        }
        //入库统计-查询
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string sql = "select record_date 记录日期,ris_record 检查记录,lis_record 检验记录,basy_record 病案首页记录,cyxj_record 出院小结记录,op_record 手术记录 from yb_in_detl_record where 1=1";
            if (dateEdit2.Text.Trim()!="")
            {
                sql += " and record_date>='"+dateEdit2.Text+"'";
            }
            if (dateEdit1.Text.Trim() != "")
            {
                sql = sql + " and record_date<'" + Convert.ToDateTime(dateEdit1.Text).AddDays(1).ToShortDateString() + "'";
            }
            sql += " order by input_date desc";
            DataSet ds1 = MyClass.getDataSet(sql, "cx1");
            dataGridView3.DataSource = ds1.Tables[0];

        }

        //入库统计-退出
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
