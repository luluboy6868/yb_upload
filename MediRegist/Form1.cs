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
                txt_bl.Text = (Convert.ToInt32(txt_bl.Text) + 1).ToString();
                return "";
            }
        }

        public bool social_check(string str_social)
        {
            string str_fg1 = "select * from dbo.MediRegist where social = '" + str_social + "' and DATEDIFF(HH,happen_date,getdate())<=24";
            //MyClass.con_open();
            SqlDataReader dr11 = MyClass.getcom(str_fg1);
            bool ifyz1 = dr11.HasRows;
            if (ifyz1)
            {
                //MessageBox.Show("已存在身份证号为 " + str_social + " 的病人信息，请核对！");
                PulicClass.My_con.Close();
                PulicClass.My_con.Dispose();
                return true;
            }
            else
            {
                return false;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel2.Text = PulicClass.user_name;
            toolStripStatusLabel4.Text = PulicClass.user_name;
            toolStripStatusLabel6.Text = PulicClass.user_name;
            dateEdit3.Text = DateTime.Now.ToString("yyyy-MM-dd");
            dateEdit4.Text = DateTime.Now.ToString("yyyy-MM-dd");

            tableLayoutPanel1.Width = this.Size.Width - 60;
            //gridControl1.Width = this.Size.Width - 50;
            groupBox11.Width = this.Size.Width - 60;
            //groupBox11.l = this.Size.Height - 200;
            tableLayoutPanel1.Height = this.Size.Height - 200;
            tableLayoutPanel2.Height = this.Size.Height - 200;
            dataGridView1.Height = this.Size.Height - 280;
            dataGridView1.Width = this.Size.Width - 70;
            //gridControl1.Height = tableLayoutPanel1.Height - 150;
            //tableLayoutPanel1.RowStyles[1].Height = tableLayoutPanel1.Height - 500;
            tabFormControl1.SelectedPage = tabFormPage1;
            txt_inpatient_no.Focus();
            if (Properties.Settings.Default.Interval>0)
            {
                timer1.Enabled = true;
                timer1.Interval = Properties.Settings.Default.Interval;
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tabFormContentContainer1_Click(object sender, EventArgs e)
        {

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
            string str_sql;
            str_sql = "select * from MediRegist where 1=1";
            if (dateEdit1.Text.Trim()!="")
            {
                str_sql = str_sql + " and happen_date>='" + dateEdit1.Text.Trim() + "'";
            }
            if (dateEdit2.Text.Trim() != "")
            {
                str_sql = str_sql + " and happen_date<='" + dateEdit2.Text.Trim() + "'";
            }
            if (textEdit1.Text.Trim()!="")
            {
                str_sql = str_sql + " and name like'%" + textEdit1.Text.Trim() + "%'";
            }
            if (textEdit2.Text.Trim() != "")
            {
                str_sql = str_sql + " and social='" + textEdit2.Text.Trim() + "'";
            }
            if (textEdit3.Text.Trim() != "")
            {
                str_sql = str_sql + " and phone='" + textEdit3.Text.Trim() + "'";
            }
            if (textEdit4.Text.Trim() != "")
            {
                str_sql = str_sql + " and temperature>='" + textEdit4.Text.Trim() + "'";
            }
            if (textEdit5.Text.Trim() != "")
            {
                str_sql = str_sql + " and temperature<='" + textEdit5.Text.Trim() + "'";
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
            string _PrintHeader = "就诊登记查询";
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
            textEdit2.Text = "xxxxxxxx";
            toolStripButton9_Click(null, null);

            dateEdit1.Text = "";
            dateEdit2.Text = "";
            textEdit1.Text = "";
            textEdit2.Text = "";
            textEdit3.Text = "";
            textEdit4.Text = "";
            textEdit5.Text = "";
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            toolStripButton7_Click(null,null);
            string str_sfzc;
            byte[] rbuff = new byte[256 + 1];
            int m = Read_card.iReadIdentityCard(0, rbuff);
            if (m > 0)
            {
                str_sfzc = Encoding.Default.GetString(rbuff);
                string[] array = str_sfzc.Split('|');
                textEdit2.Text = array[5];
                toolStripButton9_Click(null, null);
            }
            else
            {
                MessageBox.Show("读身份证出错，请检查读卡器设备是否正常！");
            }
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

        string flag="";

        private void button1_Click(object sender, EventArgs e)
        {
            string str_fg1 = "select top 20 * from yb_emr_upload where isnull(status,'')=''";
            if (txt_inpatient_no.Text!="")
            {
                str_fg1 = str_fg1 + " and inpatient_no='"+ txt_inpatient_no.Text+"'";
            }
            if (txt_times.Text != "")
            {
                str_fg1 = str_fg1 + " and admiss_times='" + txt_times.Text + "'";
            }
            try
            {
                DataSet ds1 = MyClass.getDataSet(str_fg1, "cx1");
                dataGridView1.DataSource = ds1.Tables[0];
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("病历查询出错：" + ex.Message);
                txt_bl.Text = (Convert.ToInt32(txt_bl.Text) + 1).ToString();
                return;
            }
            flag = "bl";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (flag=="bl")
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        string str_bl2="";
                        string ls_head = "SSSS" + space(16) + "F0009" + space(5) + Properties.Settings.Default.yljgdm + Properties.Settings.Default.sfzddm + space(64) + PulicClass.user_id.PadRight(10) + space(7) + space(100);
                        string ls_body = dataGridView1.Rows[i].Cells["mxzdh"].Value.ToString().PadRight(16) + PadRight_hz_bl(bljx(dataGridView1.Rows[i].Cells["mixxml"].Value.ToString()), 3750,out str_bl2);              //"aadfdsfsdfs".PadRight(3750);
                        string ls_zz = space(100) + "ZZZZ";
                        string sdatainput = ls_head + ls_body + ls_zz;
                        StringBuilder builder = new StringBuilder(ls_head + ls_body + ls_zz);
                        StringBuilder builder1 = new StringBuilder(4096);
                        Log.WriteLogFile("病历上传入参：" + builder.ToString());
                        try
                        {
                            SendRcv(builder, builder1);
                        }
                        catch(Exception ex)
                        {
                            Log.WriteLogFile("病历上传出错："+ex.Message);
                            txt_bl.Text = (Convert.ToInt32(txt_bl.Text) + 1).ToString();
                            continue;
                        }
                        Log.WriteLogFile("病历上传出参：" + builder1.ToString());
                        string str_status = builder1.ToString().Substring(21,5);
                        string str_message;
                        if (str_status!="00000")
                        {
                            str_message = substr(builder1.ToString(), 3992, 100).Trim();
                            txt_fail.Text= (Convert.ToInt32(txt_fail.Text) + 1).ToString();
                        }
                        else
                        {
                            str_message = substr(builder1.ToString(), 226, 16);  //返回值，维护流水号
                            txt_blsc.Text = (Convert.ToInt32(txt_blsc.Text) + 1).ToString();
                        }
                        string str_sql = "update yb_emr_upload set status='"+ str_status +"',message='"+ str_message +"' where patient_id='"+ dataGridView1.Rows[i].Cells["patient_id"].Value.ToString() + "' and admiss_times="+ dataGridView1.Rows[i].Cells["admiss_times"].Value.ToString() + " and mxzdh='"+ dataGridView1.Rows[i].Cells["mxzdh"].Value.ToString()+"'";
                        int i_sql = MyClass.getsqlcom(str_sql);
                        if (i_sql <= 0)
                        {
                            Log.WriteLogFile("更新yb_emr_upload失败"+ "patient_id = '"+ dataGridView1.Rows[i].Cells["patient_id"].Value.ToString() + "' and admiss_times = "+ dataGridView1.Rows[i].Cells["admiss_times"].Value.ToString() + " and mxzdh = '"+ dataGridView1.Rows[i].Cells["mxzdh"].Value.ToString()+"'");
                            txt_bl.Text = (Convert.ToInt32(txt_bl.Text) + 1).ToString();
                        }
                        if (str_bl2!="")
                        {
                            ls_head = "SSSS" + space(16) + "F0010" + space(5) + Properties.Settings.Default.yljgdm + Properties.Settings.Default.sfzddm + space(64) + PulicClass.user_id.PadRight(10) + space(7) + space(100);
                            ls_body = dataGridView1.Rows[i].Cells["mxzdh"].Value.ToString().PadRight(16) + PadRight_hz(str_bl2, 3750);
                            StringBuilder builder2 = new StringBuilder(ls_head + ls_body + ls_zz);
                            StringBuilder builder3 = new StringBuilder(4096);
                            Log.WriteLogFile("病历2上传入参：" + builder2.ToString());
                            try
                            {
                                SendRcv(builder2, builder3);
                            }
                            catch (Exception ex)
                            {
                                Log.WriteLogFile("病历上传出错：" + ex.Message);
                                txt_bl.Text = (Convert.ToInt32(txt_bl.Text) + 1).ToString();
                            }
                            Log.WriteLogFile("病历上传出参：" + builder3.ToString());
                            str_status = builder3.ToString().Substring(21, 5);
                            if (str_status != "00000")
                            {
                                Log.WriteLogFile(substr(builder1.ToString(), 3992, 100).Trim());
                                txt_blsc.Text = (Convert.ToInt32(txt_blsc.Text) + 1).ToString();
                            }
                            else
                            {
                                txt_fail.Text = (Convert.ToInt32(txt_fail.Text) + 1).ToString();
                            }
                        }
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dt1 = Convert.ToDateTime(DateTime.Now.ToShortTimeString());
            DateTime dt2 = Convert.ToDateTime(Properties.Settings.Default.RunTime);
            DateTime dt3 = Convert.ToDateTime(Properties.Settings.Default.RunTime).AddMilliseconds(Properties.Settings.Default.Interval);
            if ( dt1>= dt2 && dt1 < dt3)
            MessageBox.Show("aaa"+ DateTime.Now.ToShortTimeString().ToString());
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DateTime dt1 = Convert.ToDateTime(DateTime.Now.ToShortTimeString());
            DateTime dt2 = Convert.ToDateTime(Properties.Settings.Default.RunTime);
            DateTime dt3 = Convert.ToDateTime(Properties.Settings.Default.RunTime).AddMilliseconds(Properties.Settings.Default.Interval);
            MessageBox.Show("dt1:" + dt1.ToString() + " dt2:" + dt2.ToString() + " dt3:" + dt3.ToString());
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (flag=="bl" && dataGridView1.Rows.Count>0)
            {
                MessageBox.Show(bljx(dataGridView1.CurrentRow.Cells["mixxml"].Value.ToString()));
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string str_fg1 = "select top 20 * from yb_ba1_upload where isnull(status,'')=''";
            if (txt_inpatient_no.Text != "")
            {
                str_fg1 = str_fg1 + " and inpatient_no='" + txt_inpatient_no.Text + "'";
            }
            if (txt_times.Text != "")
            {
                str_fg1 = str_fg1 + " and admiss_times='" + txt_times.Text + "'";
            }
            try
            {
                DataSet ds1 = MyClass.getDataSet(str_fg1, "cx1");
                dataGridView1.DataSource = ds1.Tables[0];
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("病案首页1查询出错：" + ex.Message);
                txt_ba1_yc.Text = (Convert.ToInt32(txt_ba1_yc.Text) + 1).ToString();
                return;
            }
            flag = "ba1";
        }

        /// <summary>
        /// 获取body内容
        /// </summary>
        /// <param name="dg">datagridview</param>
        /// <param name="row">行</param>
        /// <param name="b">开始列</param>
        /// <param name="e">结束列</param>
        /// <returns></returns>
        public string getbody(DataGridView dg,int row,int b,int e)
        {
            string str_body = "";
            for (int i=b;i<=e;i++)
            {
                str_body = str_body + dg.Rows[row].Cells[i].Value.ToString();
            }
            return str_body;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (flag == "ba1")
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        string ls_head = "SSSS" + space(16) + "F0007" + space(5) + Properties.Settings.Default.yljgdm + Properties.Settings.Default.sfzddm + space(64) + PulicClass.user_id.PadRight(10) + space(7) + space(100);
                        string ls_body = PadRight_hz(getbody(dataGridView1,i,0,169),3766);
                        string ls_zz = space(100) + "ZZZZ";
                        string sdatainput = ls_head + ls_body + ls_zz;
                        StringBuilder builder = new StringBuilder(ls_head + ls_body + ls_zz);
                        StringBuilder builder1 = new StringBuilder(4096);
                        Log.WriteLogFile("病案首页1上传入参：" + builder.ToString());
                        try
                        {
                            SendRcv(builder, builder1);
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLogFile("病案首页1上传出错：" + ex.Message);
                            txt_ba1_yc.Text = (Convert.ToInt32(txt_ba1_yc.Text) + 1).ToString();
                            continue;
                        }
                        Log.WriteLogFile("病案首页1上传出参：" + builder1.ToString());
                        string str_status = builder1.ToString().Substring(21, 5);
                        string str_message;
                        if (str_status != "00000")
                        {
                            str_message = substr(builder1.ToString(), 3992, 100).Trim();
                            txt_ba1_fail.Text = (Convert.ToInt32(txt_ba1_fail.Text) + 1).ToString();
                        }
                        else
                        {
                            str_message = substr(builder1.ToString(), 226, 16);  //返回值，维护流水号
                            txt_ba1_suc.Text = (Convert.ToInt32(txt_ba1_suc.Text) + 1).ToString();
                        }
                        string str_sql = "update yb_ba1_upload set status='" + str_status + "',message='" + str_message + "' where patient_id='" + dataGridView1.Rows[i].Cells["patient_id"].Value.ToString() + "' and admiss_times=" + dataGridView1.Rows[i].Cells["admiss_times"].Value.ToString() + " and mxzdh='" + dataGridView1.Rows[i].Cells["mxzdh"].Value.ToString() + "'";
                        int i_sql = MyClass.getsqlcom(str_sql);
                        if (i_sql <= 0)
                        {
                            Log.WriteLogFile("更新yb_ba1_upload失败" + "patient_id = '" + dataGridView1.Rows[i].Cells["patient_id"].Value.ToString() + "' and admiss_times = " + dataGridView1.Rows[i].Cells["admiss_times"].Value.ToString() + " and mxzdh = '" + dataGridView1.Rows[i].Cells["mxzdh"].Value.ToString() + "'");
                            txt_ba1_yc.Text = (Convert.ToInt32(txt_ba1_yc.Text) + 1).ToString();
                        }
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string str_fg1 = "select top 20 * from yb_ba2_upload where isnull(status,'')=''";
            if (txt_inpatient_no.Text != "")
            {
                str_fg1 = str_fg1 + " and inpatient_no='" + txt_inpatient_no.Text + "'";
            }
            if (txt_times.Text != "")
            {
                str_fg1 = str_fg1 + " and admiss_times='" + txt_times.Text + "'";
            }
            try
            {
                DataSet ds1 = MyClass.getDataSet(str_fg1, "cx1");
                dataGridView1.DataSource = ds1.Tables[0];
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("病案首页2查询出错：" + ex.Message);
                txt_ba2_yc.Text = (Convert.ToInt32(txt_ba2_yc.Text) + 1).ToString();
                return;
            }
            flag = "ba2";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (flag == "ba2")
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        string ls_head = "SSSS" + space(16) + "F0007" + space(5) + Properties.Settings.Default.yljgdm + Properties.Settings.Default.sfzddm + space(64) + PulicClass.user_id.PadRight(10) + space(7) + space(100);
                        string ls_body = PadRight_hz(getbody(dataGridView1, i, 0, 161), 3766);
                        string ls_zz = space(100) + "ZZZZ";
                        string sdatainput = ls_head + ls_body + ls_zz;
                        StringBuilder builder = new StringBuilder(ls_head + ls_body + ls_zz);
                        StringBuilder builder1 = new StringBuilder(4096);
                        Log.WriteLogFile("病案首页2上传入参：" + builder.ToString());
                        try
                        {
                            SendRcv(builder, builder1);
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLogFile("病案首页2上传出错：" + ex.Message);
                            txt_ba2_yc.Text = (Convert.ToInt32(txt_ba2_yc.Text) + 1).ToString();
                            continue;
                        }
                        Log.WriteLogFile("病案首页2上传出参：" + builder1.ToString());
                        string str_status = builder1.ToString().Substring(21, 5);
                        string str_message;
                        if (str_status != "00000")
                        {
                            str_message = substr(builder1.ToString(), 3992, 100).Trim();
                            txt_ba2_fail.Text = (Convert.ToInt32(txt_ba2_fail.Text) + 1).ToString();
                        }
                        else
                        {
                            str_message = substr(builder1.ToString(), 226, 16);  //返回值，维护流水号
                            txt_ba2_suc.Text = (Convert.ToInt32(txt_ba2_suc.Text) + 1).ToString();
                        }
                        string str_sql = "update yb_ba2_upload set status='" + str_status + "',message='" + str_message + "' where patient_id='" + dataGridView1.Rows[i].Cells["patient_id"].Value.ToString() + "' and admiss_times=" + dataGridView1.Rows[i].Cells["admiss_times"].Value.ToString() + " and mxzdh='" + dataGridView1.Rows[i].Cells["mxzdh"].Value.ToString() + "'";
                        int i_sql = MyClass.getsqlcom(str_sql);
                        if (i_sql <= 0)
                        {
                            Log.WriteLogFile("更新yb_ba2_upload失败" + "patient_id = '" + dataGridView1.Rows[i].Cells["patient_id"].Value.ToString() + "' and admiss_times = " + dataGridView1.Rows[i].Cells["admiss_times"].Value.ToString() + " and mxzdh = '" + dataGridView1.Rows[i].Cells["mxzdh"].Value.ToString() + "'");
                            txt_ba2_yc.Text = (Convert.ToInt32(txt_ba2_yc.Text) + 1).ToString();
                        }
                    }
                }
            }
        }
    }
}
