using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediRegist
{
    class PulicClass
    {
        #region  全局变量
        public static string login_id = "";  //登录名，比如fzzb
        public static string user_id = ""; //用户id，比如00000
        public static string user_name = "";  //登录人员姓名
        public static string user_group = "";  //登录工号所在组
        public static string Mean_SQL = "", Mean_Table = "", Mean_Field = "";  //定义全局变量，记录“基础信息”各窗体中的表名及SQL语句
        public static SqlConnection My_con;  //定义一个SqlConnection类型的公共变量My_con，用于判断数据库是否连接成功
        public static string M_str_sqlcon = Properties.Settings.Default.chisdb;

        public static int Login_n = 0;  //用户登录与重新登录的标识
        public static string AllSql = "Select * from tb_Stuffbusic";    //存储职工基本信息表中的SQL语句
                                                                        //public static int res = 0;
        public static string gv_cfbh = ""; //处方上传的处方编号


        #endregion

        #region//变量声明区
        public DevExpress.Utils.ToolTipController MyToolTipClt = null;
        DevExpress.Utils.ToolTipControllerShowEventArgs args = null;

        #endregion


        ///
        /// 
        /// 冒泡提示
        ///
        /// System.Windows.Forms的一个控件，在其上面提示显示
        /// 提示的标题默认（温馨提示）
        /// 提示的信息默认（???）
        /// 提示显示等待时间
        /// DevExpress.Utils.ToolTipType 显示的类型
        /// DevExpress.Utils.ToolTipLocation 在控件显示的位置
        /// 是否自动隐藏提示信息
        /// DevExpress.Utils.ToolTipIconType 显示框图表的类型
        /// 一个System.Windows.Forms.ImageList 装载Icon图标的List，显示的ToolTipIconType上，可以为Null
        /// 图标在ImageList上的索引，ImageList为Null时传0进去
        public void NewToolTip(Control ctl, string title, string content, int showTime, DevExpress.Utils.ToolTipType toolTipType, DevExpress.Utils.ToolTipLocation tipLocation, bool isAutoHide, DevExpress.Utils.ToolTipIconType tipIconType, System.Windows.Forms.ImageList imgList, int imgIndex)
        {
            try
            {
                MyToolTipClt = new DevExpress.Utils.ToolTipController();
                args = MyToolTipClt.CreateShowArgs();
                content = (string.IsNullOrEmpty(content) ? "???" : content);
                title = string.IsNullOrEmpty(title) ? "温馨提示" : title;
                MyToolTipClt.ImageList = imgList;
                MyToolTipClt.ImageIndex = (imgList == null ? 0 : imgIndex);
                args.AutoHide = isAutoHide;
                MyToolTipClt.ShowBeak = true;
                MyToolTipClt.ShowShadow = true;
                MyToolTipClt.Rounded = true;
                MyToolTipClt.AutoPopDelay = (showTime == 0 ? 2000 : showTime);
                //MyToolTipClt.SetToolTip(ctl, content);
                //MyToolTipClt.SetTitle(ctl, title);
                //MyToolTipClt.SetToolTipIconType(ctl, tipIconType);
                //MyToolTipClt.SetToolTipIconType(ctl, tipIconType);
                MyToolTipClt.Active = true;
                MyToolTipClt.HideHint();
                MyToolTipClt.ShowHint(content, title, ctl, tipLocation);
            }
            catch (Exception ex)
            {
                //CommonFunctionHeper.CommonFunctionHeper.CreateLogFiles(ex);
                MessageBox.Show(ex.Message);
            }
        }


        #region  建立数据库连接
        /// <summary>
        /// 建立数据库连接.
        /// </summary>
        /// <returns>返回SqlConnection对象</returns>
        public static SqlConnection getcon()
        {
            My_con = new SqlConnection(M_str_sqlcon);   //用SqlConnection对象与指定的数据库相连接
            My_con.Open();  //打开数据库连接
            return My_con;  //返回SqlConnection对象的信息
        }
        #endregion

        #region  测试数据库是否赋加
        /// <summary>
        /// 测试数据库是否赋加
        /// </summary>
        public void con_open()
        {
            getcon();
            //con_close();
        }
        #endregion

        #region  关闭数据库连接
        /// <summary>
        /// 关闭于数据库的连接.
        /// </summary>
        public void con_close()
        {
            if (My_con.State == ConnectionState.Open)   //判断是否打开与数据库的连接
            {
                My_con.Close();   //关闭数据库的连接
                My_con.Dispose();   //释放My_con变量的所有空间
            }
        }
        #endregion

        #region  读取指定表中的信息
        /// <summary>
        /// 读取指定表中的信息.
        /// </summary>
        /// <param name="SQLstr">SQL语句</param>
        /// <returns>返回bool型</returns>
        public SqlDataReader getcom(string SQLstr)
        {
            getcon();   //打开与数据库的连接
            SqlCommand My_com = My_con.CreateCommand(); //创建一个SqlCommand对象，用于执行SQL语句
            My_com.CommandText = SQLstr;    //获取指定的SQL语句
            My_com.CommandTimeout = 0;
            SqlDataReader My_read = My_com.ExecuteReader(); //执行SQL语名句，生成一个SqlDataReader对象
            return My_read;
        }
        #endregion

        #region 执行SqlCommand命令
        /// <summary>
        /// 执行SqlCommand
        /// </summary>
        /// <param name="M_str_sqlstr">SQL语句</param>
        public int getsqlcom(string SQLstr)
        {
            getcon();   //打开与数据库的连接
            SqlCommand SQLcom = new SqlCommand(SQLstr, My_con); //创建一个SqlCommand对象，用于执行SQL语句
            int i = SQLcom.ExecuteNonQuery();   //执行SQL语句
            SQLcom.Dispose();   //释放所有空间
            con_close();    //调用con_close()方法，关闭与数据库的连接
            return i;
        }
        #endregion

        #region  创建DataSet对象
        /// <summary>
        /// 创建一个DataSet对象
        /// </summary>
        /// <param name="M_str_sqlstr">SQL语句</param>
        /// <param name="M_str_table">表名</param>
        /// <returns>返回DataSet对象</returns>
        public DataSet getDataSet(string SQLstr, string tableName)
        {
            getcon();   //打开与数据库的连接
            SqlDataAdapter SQLda = new SqlDataAdapter(SQLstr, My_con);  //创建一个SqlDataAdapter对象，并获取指定数据表的信息
            DataSet My_DataSet = new DataSet(); //创建DataSet对象
            SQLda.Fill(My_DataSet, tableName);  //通过SqlDataAdapter对象的Fill()方法，将数据表信息添加到DataSet对象中
            con_close();    //关闭数据库的连接
            return My_DataSet;  //返回DataSet对象的信息

            //WritePrivateProfileString(string section, string key, string val, string filePath);
        }
        #endregion


        public string getdatestr(string date_str)
        {
            //date_str = "20190628105856";
            date_str = date_str.Substring(0, 4) + "-" + date_str.Substring(4, 2) + "-" + date_str.Substring(6, 2) + " " + date_str.Substring(8, 2) + ":" + date_str.Substring(10, 2);
            return date_str;
        }

        public string GetdateDb(int m)
        {
            string str_date = "";
            con_open();
            SqlDataReader temDR = getcom("select convert(varchar(23),GETDATE(),121),replace(replace(replace(replace(convert(varchar(23),GETDATE(),121),'-',''),' ',''),':',''),'.','')");
            bool ifcom = temDR.Read();
            if (ifcom)
            {
                if (m == 0)
                {
                    str_date = temDR.GetString(0);
                }
                if (m == 1)
                {
                    str_date = temDR.GetString(1);
                }
                My_con.Close();
                My_con.Dispose();
                return str_date;
            }
            return "";
        }


        public void savepdf(string strbase64, string jshid)
        {
            try
            {
                strbase64 = strbase64.Replace(@"\n", "");
                jshid = jshid.Replace("*", "");
                //MessageBox.Show(strbase64);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(Convert.FromBase64String(strbase64));
                System.IO.FileStream fs = new System.IO.FileStream(System.IO.Directory.GetCurrentDirectory() + "\\pdf\\" + jshid + ".pdf", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                byte[] b = stream.ToArray();
                //byte[] b = stream.GetBuffer();
                fs.Write(b, 0, b.Length);
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        //获取某行json
        public string GetRowJson(DataGridView dgv, int m, int n,int Row)
        {
            DataTable dt = new DataTable();

            // 列强制转换
            for (int count = m; count < n + m; count++)
            {
                DataColumn dc = new DataColumn(dgv.Columns[count].Name.ToString());
                dt.Columns.Add(dc);
            }

            //添加列
            DataRow dr = dt.NewRow();
            for (int countsub = m; countsub < n + m; countsub++)
            {
                dr[countsub - m] = Convert.ToString(dgv.Rows[Row].Cells[countsub].Value);
            }
            dt.Rows.Add(dr);

            return JsonConvert.SerializeObject(dt).Replace("[","").Replace("]","");
        }

        public string getweb_new(string sbjgbh, string zcm, string str_hisjyh, string str_method, string str_jsonPara, string yybm)
        {
            LogClass Log = new LogClass();
            string ss = "";
            try
            {
                Log.WriteLogFile("webservice begin（社保机构编号：" + sbjgbh + "  注册码：" + zcm + "  医院编码：" + yybm + ")");
                Log.WriteLogFile("his交易号：" + str_hisjyh + "方法：" + str_method + "入参：" + str_jsonPara);
                WebReference1.Mhs5service wr = new WebReference1.Mhs5service();
                wr.Timeout = 300000;
                ss = wr.pipInvoke(sbjgbh, zcm, str_hisjyh, str_method, str_jsonPara, yybm);
                Log.WriteLogFile("出参：" + ss);
                Log.WriteLogFile("webservice end");
            }
            catch (Exception ex)
            {
                MessageBox.Show("接口调用失败," + ex);
                Log.WriteLogFile("接口调用失败," + ex);
                return "";
            }
            return ss;
        }
    }
}
