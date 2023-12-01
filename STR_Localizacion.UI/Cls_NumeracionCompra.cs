using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SAPbouiCOM;
using SAPbobsCOM;

using STR_Localizacion.MetaData;
using STR_Localizacion.UTIL;

namespace STR_Localizacion.UI
{
    public class Cls_NumeracionCompra : Cls_NumeracionBase
    {
        public Cls_NumeracionCompra()
        {
            lc_NameMethod = "Cls_NumeracionCompra";
            go_InternalFormID = new List<string>() { "141", "181", "60092", "65301", "65306" };

            ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
            InitializeEvents();
        }

        public void SetTableName(string FormID)
        {
            switch (FormID)
            {
                case "65301":
                    ls_tablename = "ODPO";
                    break;
                case "141":
                case "65306":
                case "60092":
                    ls_tablename = "OPCH";
                    break;
                case "181":
                    ls_tablename = "ORPC";
                    break;
            }
        }

        private void InitializeEvents()
        {
            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_LOAD, string.Empty,
                s => sb_LoadForm(s)));

            menuevent.Add(new sapmenuevent(s =>
               sb_LoadForm(s), "2308", "2309", "2314", "2315", "2317"));

            menuevent.Add(new sapmenuevent("1282", s => sb_asignarTipoXDefecto()));
        }

        private void sb_LoadForm(dynamic po_Event)
        {
            if (po_Event.BeforeAction) return;

            if (go_SBOFormEvent.ItemExists("cbTipo"))
                return;

            try
            {
                go_SBOFormEvent.Freeze(true);
                go_SBOForm = go_SBOFormEvent;

                int li_Top, li_Left, li_Height, li_LeftSt;
                bool blMostrar = true;

                InitializeControl(out li_Top, out li_Left, out li_Height, out li_LeftSt);
                /// Creación del control Serie
                go_Item = go_SBOForm.Items.Add("txSerie", BoFormItemTypes.it_EDIT);
                go_Item.SetDisplay(40, li_Height, li_Top, go_SBOForm.GetItem("cbTipo").Left + go_SBOForm.GetItem("cbTipo").Width + 1);
                go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 9, BoModeVisualBehavior.mvb_False);
                go_Item.Enabled = true;
                go_Item.Visible = blMostrar;
                go_Edit = go_Item.Specific;

                go_Edit.DataBind.SetBound(true, ls_tablename, "U_BPP_MDSD");

                /// Creación del control Numero
                go_Item = go_SBOForm.Items.Add("txNumero", BoFormItemTypes.it_EDIT);
                go_Item.SetDisplay(76, li_Height, li_Top, go_SBOForm.GetItem("txSerie").Left + go_SBOForm.GetItem("txSerie").Width + 1);
                go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 9, BoModeVisualBehavior.mvb_False);
                go_Item.Enabled = true;
                go_Item.Visible = blMostrar;
                go_Edit = go_Item.Specific;

                go_Edit.DataBind.SetBound(true, ls_tablename, "U_BPP_MDCD");

                /// Asignar tipo de documento SUNAT por Defecto
                sb_asignarTipoSunat();
                sb_asignarTipoXDefecto();
            }
            finally { go_SBOFormEvent.Freeze(false); }
        }

    }
}
