using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MediRegist
{
    class Read_card
    {
        [DllImport("chisHICIntf.dll")]
        public static extern Boolean chisHICInit(ref string OperaParams);
        [DllImport("chisHICIntf.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean chisGetHICNo(StringBuilder HICNO, StringBuilder Err);
        [DllImport("chisHICIntf.dll")]
        public static extern Boolean chisHICRelease();

        [DllImport("DC_Reader.dll")]
        public static extern int iReadIdentityCard(int iType, [Out] byte[] rbuff);
    }
}
