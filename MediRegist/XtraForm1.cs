using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data.SqlClient;

namespace MediRegist
{
    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {
        public XtraForm1()
        {
            InitializeComponent();
        }

        PulicClass pulicClass = new PulicClass();

        public static int asc(char c1)
        {
            return (int)c1;
        }

        public string get_pass_new(string pw_string, string user_string)
        {

            string x_temp_str1;
            string x_temp_str2;
            int x_length;
            int x_position1;
            int x_position2;
            int x_temp_int1;
            int x_temp_int2;

            user_string = user_string.Trim();
            user_string = user_string.ToUpper();
            pw_string = pw_string.Trim();
            pw_string = pw_string.ToUpper();
            if (pw_string.Length == 0)
            {
                pw_string = "";
            }
            else
            {
                /*根据user_name的第一个字符变换password的第一个字符*/
                x_length = user_string.Length;
                //x_position1 = 1;
                x_temp_int1 = 0;
                //将所有用户名字符全部转换为asc码，再求和
                for (x_position1 = 0; x_position1 < x_length; x_position1++)
                {
                    x_temp_str1 = user_string.Substring(x_position1, 1); //mid(user_string, x_position1, 1);
                    x_temp_int2 = asc(Convert.ToChar(x_temp_str1));  //asc(x_temp_str1)
                    x_temp_int1 = x_temp_int1 + x_temp_int2;
                }
                x_temp_int1 = x_temp_int1 + x_length;
                //减去75的倍数直到小于122
                while (x_temp_int1 > 122)
                {
                    x_temp_int1 = x_temp_int1 - 75;
                }

                //--------------------------------------------------------------//
                //密码处理

                x_temp_str2 = pw_string.Substring(0, 1);
                x_temp_int2 = asc(Convert.ToChar(x_temp_str2));  // asc(x_temp_str2);

                x_temp_int1 = x_temp_int1 + x_temp_int2; //将用户的asc码+第一位密码的asc码

                if (x_temp_int1 > 122 && x_temp_int1 < 198)
                {
                    x_temp_int1 = x_temp_int1 - 75;
                }

                if (x_temp_int1 > 197)
                {
                    x_temp_int1 = x_temp_int1 - 133;
                }

                if (x_temp_int1 > 57 && x_temp_int1 < 65)
                {
                    x_temp_int1 = x_temp_int1 - 7;
                }

                if (x_temp_int1 > 90 && x_temp_int1 < 97)
                {
                    x_temp_int1 = x_temp_int1 - 6;
                }

                x_length = pw_string.Length; //len(pw_string);
                x_temp_int2 = x_length - 1;
                //将上面求的asc码转换成字符当成第一个字符再加剩余位数字符
                pw_string = (char)(x_temp_int1) + pw_string.Substring(pw_string.Length - x_temp_int2, x_temp_int2);       //char(x_temp_int1) + right(pw_string,x_temp_int2);


                /*变换pass word*/
                x_position1 = 1;
                while (x_position1 <= x_length)
                {
                    x_temp_str1 = pw_string.Substring(x_position1 - 1, 1); //mid(pw_string, x_position1, 1);
                    x_temp_int1 = asc(Convert.ToChar(x_temp_str1)); //Convert.ToInt16(x_temp_str1); //asc(x_temp_str1);

                    x_position2 = x_position1 - 1;
                    if (x_position2 == 0)
                    {
                        x_position2 = x_length;
                    }
                    x_temp_str2 = pw_string.Substring(x_position2 - 1, 1); //mid(pw_string, x_position2, 1);
                    x_temp_int2 = asc(Convert.ToChar(x_temp_str2));  //Convert.ToInt16(x_temp_str2); //asc(x_temp_str2);

                    x_temp_int1 = x_temp_int1 + x_temp_int2;
                    if (x_temp_int1 > 122 && x_temp_int1 < 198)
                    {
                        x_temp_int1 = x_temp_int1 - 75;
                    }
                    if (x_temp_int1 > 197)
                    {
                        x_temp_int1 = x_temp_int1 - 133;
                    }
                    if (x_temp_int1 > 57 && x_temp_int1 < 65)
                    {
                        x_temp_int1 = x_temp_int1 - 7;
                    }
                    if (x_temp_int1 > 90 && x_temp_int1 < 97)
                    {
                        x_temp_int1 = x_temp_int1 - 6;
                    }
                    if (x_position1 == 1)
                    {
                        x_temp_int2 = x_length - 1;
                        pw_string = (char)(x_temp_int1) + pw_string.Substring(pw_string.Length - x_temp_int2, x_temp_int2); //char(x_temp_int1) + right(pw_string,x_temp_int2);
                    }
                    else
                    {
                        x_temp_int2 = x_length - x_position1;
                        x_position1 = x_position1 - 1;
                        //      pw_string = left(pw_string,x_position1) +&
                        //char(x_temp_int1) +&
                        //right(pw_string,x_temp_int2);
                        pw_string = pw_string.Substring(0, x_position1) + (char)(x_temp_int1) + pw_string.Substring(pw_string.Length - x_temp_int2, x_temp_int2);
                        x_position1++;
                    }
                    x_position1++;
                }
            }
            return pw_string;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (textEdit1.Text != "")
            {
                if (Properties.Settings.Default.isSelectSys)
                {
                    if (com_sysid.SelectedIndex == 1)
                    {
                        pulicClass.con_open();
                        SqlDataReader temDR = pulicClass.getcom("select user_mi,user_group,b.name from xt_user a,a_employee_mi b where a.user_mi=b.code and a.user_name='" + textEdit1.Text + "' and pass_word='" + get_pass_new(textEdit2.Text, textEdit1.Text) + "'");
                        bool ifcom = temDR.Read();
                        if (ifcom)
                        {
                            PulicClass.login_id = textEdit1.Text.Trim();
                            PulicClass.user_id = temDR.GetString(0);
                            PulicClass.user_name = temDR.GetString(2);
                            PulicClass.user_group = temDR.GetInt32(1).ToString();
                            PulicClass.My_con.Close();
                            PulicClass.My_con.Dispose();
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("用户名或密码错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        pulicClass.con_close();
                    }
                    else
                    {
                        pulicClass.con_open();
                        byte[] b = System.Text.Encoding.Default.GetBytes(textEdit2.Text);
                        //转成 Base64 形式的 System.String  
                        string str_password = Convert.ToBase64String(b);
                        string cisdb = Properties.Settings.Default.cisdb;
                        SqlDataReader temDR = pulicClass.getcom("select employment_id,'',b.name from "+ cisdb +"..auth_user a,a_employee_mi b where a.employment_id=b.code and login_name='" + textEdit1.Text + "' and active='1' and password='" + str_password + "'");
                        bool ifcom = temDR.Read();
                        if (ifcom)
                        {
                            PulicClass.login_id = textEdit1.Text.Trim();
                            PulicClass.user_id = temDR.GetString(0);
                            PulicClass.user_name = temDR.GetString(2);
                            PulicClass.user_group = "";
                            PulicClass.My_con.Close();
                            PulicClass.My_con.Dispose();
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("用户名或密码错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        pulicClass.con_close();
                    }
                }
                else
                {
                    pulicClass.con_open();
                    SqlDataReader temDR = pulicClass.getcom("select user_mi,user_group,b.name from xt_user a,a_employee_mi b where a.user_mi=b.code and a.user_name='" + textEdit1.Text + "' and a.subsys_id='" + Properties.Settings.Default.subsys_id + "' and pass_word='" + get_pass_new(textEdit2.Text, textEdit1.Text) + "'");
                    bool ifcom = temDR.Read();
                    if (ifcom)
                    {
                        PulicClass.login_id = textEdit1.Text.Trim();
                        PulicClass.user_id = temDR.GetString(0);
                        PulicClass.user_name = temDR.GetString(2);
                        PulicClass.user_group = temDR.GetInt32(1).ToString();
                        PulicClass.My_con.Close();
                        PulicClass.My_con.Dispose();
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("用户名或密码错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    pulicClass.con_close();
                }

            }
            else
            {
                MessageBox.Show("请输入用户名");
            }
        }

        private void XtraForm1_Load(object sender, EventArgs e)
        {
            if (!(Properties.Settings.Default.isSelectSys))
            {
                labelControl4.Visible = false;
                com_sysid.Visible = false;
            }
        }

        private void textEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                textEdit2.Focus();
        }

        private void textEdit2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                simpleButton1.PerformClick();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void XtraForm1_Shown(object sender, EventArgs e)
        {
            textEdit1.Focus();
        }
    }
}