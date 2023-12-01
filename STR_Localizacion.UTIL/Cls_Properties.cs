using System;

namespace STR_Localizacion.UTIL
{
    public class Cls_Properties
    {
        protected string lc_NameMethod;
        protected string lc_NameClass;
        protected string lc_NameLayout = "STR_Localizacion.UI";
        protected internalexception ExceptionPrepared;
        protected SAPbouiCOM.Form go_SBOForm;
        public static SAPbouiCOM.Form go_SBOFormEvent;

        protected static SAPbouiCOM.Form go_SBOFormActive
        {
            get
            {
                try { return go_SBOApplication.Forms.ActiveForm; }
                catch (Exception ex) { return null; }
            }
        }

        protected static SAPbouiCOM.Application go_SBOApplication
        {
            get { return Cls_Global.go_SBOApplication; }
        }

        protected static SAPbobsCOM.Company go_SBOCompany
        {
            get { return Cls_Global.go_SBOCompany; }
        }
    }
}