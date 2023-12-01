using STR_Localizacion.UTIL;

namespace STR_Localizacion.DL
{
    public static class Cls_Log
    {
        public static void SaveInLog(this internalexception ie, bool showbox = true, bool showstatus = true)
        {
            if (showstatus)
                Cls_Global.go_SBOApplication.StatusBar.SetText(
                    ie.Exception,
                    SAPbouiCOM.BoMessageTime.bmt_Short,
                    ie.MessageType);
            else if (showbox)
                Cls_Global.go_SBOApplication.MessageBox(ie.Exception);
            /*
            var current = DateTime.Now;

            Cls_QueryManager.Procesa(
                Cls_Query.insert_TablaLog,
                (int)ie.MessageType,
                ie.NameLayout,
                ie.NameClass,
                ie.NameMethod,
                ie.Message,
                current.Date,
                int.Parse(current.Hour + string.Empty + current.Minute),
                -1,
                -1,
                Cls_Global.gs_addon,
                Cls_Global.go_SBOCompany.CompanyDB);*/
        }
    }
}