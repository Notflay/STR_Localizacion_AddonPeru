using System;

namespace STR_Localizacion.UTIL
{
    public class Cls_PropertiesControl : Cls_Properties
    {
        public sapitemeventList itemevent = new sapitemeventList();
        public sapmenueventList menuevent = new sapmenueventList();
        public string gs_FormName;
        public string gs_FormPath;

        //Declaracion de Variables para los controles de los formularios
        protected SAPbouiCOM.Item go_Item = null;

        protected SAPbouiCOM.StaticText go_Static = null;
        protected SAPbouiCOM.EditText go_Edit = null;
        protected SAPbouiCOM.ComboBox go_Combo = null;
        protected SAPbouiCOM.Button go_Button = null;
        protected SAPbouiCOM.Grid go_Grid = null;
        protected SAPbouiCOM.Matrix go_Matrix = null;
        protected SAPbouiCOM.OptionBtn go_OptionButton = null;
        protected SAPbouiCOM.CheckBox go_CheckBox = null;
        protected SAPbobsCOM.Recordset go_RecordSet = null;
        protected SAPbouiCOM.Folder go_Folder = null;
        protected SAPbouiCOM.LinkedButton go_LinkButton = null;
        protected SAPbouiCOM.GridColumn go_GridColumn = null;
        protected SAPbouiCOM.EditTextColumn go_EditColumn = null;
        protected SAPbouiCOM.ProgressBar go_ProgressBar = null;
        protected SAPbouiCOM.ValidValues go_Validvalues = null;

        protected void Dispose()
        {
            go_SBOForm = null;

            go_Item = null;
            go_Static = null;
            go_Edit = null;
            go_Combo = null;
            go_Button = null;
            go_Grid = null;
            go_Matrix = null;
            go_OptionButton = null;
            go_CheckBox = null;
            go_RecordSet = null;
            go_LinkButton = null;

            go_ProgressBar = null;
            go_Validvalues = null;

            GC.Collect();
        }
    }
}