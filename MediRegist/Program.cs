using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;

namespace MediRegist
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BonusSkins.Register();
            //new XtraForm1().Show();
            //Application.Run();

            XtraForm1 frmLogin = new XtraForm1();
            if (frmLogin.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new F_main());
            }
        }
    }
}
