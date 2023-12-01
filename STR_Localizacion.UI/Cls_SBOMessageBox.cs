using STR_Localizacion.UTIL;

namespace STR_Localizacion.UI
{
    internal class Cls_SBOMessageBox : Cls_PropertiesControl
    {
        public bool ApplyAction { get; set; }
        private SAPbouiCOM.Form form;
        private bool answer;

        public SAPbouiCOM.Form SetForm { set => this.form = value; }
        public bool Answer { set => this.answer = value; }

        public void answerQuestion()
        {
            if (!ApplyAction) return;
            if (answer)
                go_Item = form.Items.Item("1");
            else
                go_Item = form.Items.Item("2");
            go_Item.Click(SAPbouiCOM.BoCellClickType.ct_Regular);
        }
    }
}