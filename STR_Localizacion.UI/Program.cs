using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace STR_Localizacion.UI
{
    public static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Task.Factory.StartNew(() => { Cls_Main lo_Cls_Main = new Cls_Main(); });
            Application.Run();
        }
    }
}
