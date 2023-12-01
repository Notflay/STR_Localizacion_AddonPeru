using SAPbouiCOM;
using System;

namespace STR_Localizacion.UTIL
{
    public static class Cls_GEObjects
    {
        public static bool CompareFUI(this ItemEvent ie, string formname)
        {
            return ie.FormTypeEx.Equals(formname);
        }

        public static bool CompareMUI(this MenuEvent ie, string formname)
        {
            return ie.MenuUID.Equals(formname);
        }
        
        public static void Freezer(this Form oForm, string type, Action execute)
        {
            if (oForm.TypeEx.Contains(type))
            {
                oForm.Freeze(true);
                execute();
                oForm.Freeze(false);
            }
        }
    }
}
