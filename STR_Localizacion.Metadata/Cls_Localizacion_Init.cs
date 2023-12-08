using STR_Localizacion.UTIL;
using System;
using System.Windows.Forms;

namespace STR_Localizacion.MetaData
{
    public class Cls_Localizacion_Init : Cls_Properties
    {
        private int li_IndInstal = 1;
        private string ls_Path = string.Empty;

        public Cls_Localizacion_Init()
        {
            ls_Path = Application.StartupPath;
        }
    }
}
