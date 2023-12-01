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
    public class Cls_NumeracionVenta : Cls_NumeracionBase
    {
        public Cls_NumeracionVenta()
        {
            lc_NameMethod = "Cls_NumeracionVenta";
            go_InternalFormID = new List<string>() { "133", "140", "179", "180", "65300", "65302", "65303", "65304", "65305", "65307", "60090", "60091", };
            ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
            InitializeEvents();
        }

        public void SetTableName(string FormID)
        {
            switch (FormID)
            {
                case "140":
                    ls_tablename = "ODLN";
                    break;
                case "179":
                    ls_tablename = "ORIN";
                    break;
                case "180":
                    ls_tablename = "ORDN";
                    break;
                case "65300":
                    ls_tablename = "ODPI";
                    break;
                case "133":
                case "65302":
                case "65303":
                case "65304":
                case "65305":
                case "65307":
                case "60090":
                case "60091":
                    ls_tablename = "OINV";
                    break;
            }
        }

        private void InitializeEvents()
        {

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_LOAD, string.Empty,
                s => sb_LoadForm(s)));

            itemevent.Add(BoEventTypes.et_COMBO_SELECT,
                new sapitemevent("cbTipo", s =>
                {
                    if (!s.BeforeAction)
                        fn_actualizarSerieSUNAT();
                })
                /*,
               new sapitemevent("10000329", s =>
                {
                    if ((!s.BeforeAction && s.FormTypeEx == "133" || s.FormTypeEx == "180"))
                    {
                        go_SBOForm = go_SBOFormEvent;
                        go_Combo = go_SBOForm.GetControl("cbTipo");
                        go_Combo.ValidValues.Add(string.Empty, string.Empty);
                        go_Combo.Select(string.Empty, BoSearchKey.psk_ByValue);
                        go_Combo = go_SBOForm.GetControl("cbSerie");
                        go_Combo.ValidValues.Add(string.Empty, string.Empty);
                        go_Combo.Select(string.Empty, BoSearchKey.psk_ByValue);
                        go_SBOForm.GetEditText("txNumero").Value = string.Empty;
                    }
                })*/);

            itemevent.Add(BoEventTypes.et_ITEM_PRESSED,
                new sapitemevent("1", s =>
                {
                    if (s.BeforeAction && go_SBOFormEvent.Mode == BoFormMode.fm_ADD_MODE)
                    {
                        try
                        {
                            go_SBOFormEvent.Freeze(true);

                            string ls_Comentario;
                            go_Edit = go_SBOFormEvent.GetControl("16");
                            ls_Comentario = go_Edit.Value;
                            go_Edit.Value = "*";

                            go_SBOFormEvent.Items.Item("4").Click();
                            go_Edit.Value = ls_Comentario;
                        }
                        finally { go_SBOFormEvent.Freeze(false); }
                    }
                }));


            menuevent.Add(new sapmenuevent(s =>
                sb_LoadForm(s)
            , "2051"/*Entrega*/, "2052"/*Devolucion*/, "2053"/*Factura*/, "2054" /*F. Pago*/, "2055" /*NC*/, "2056" /*F. Reserva*/, "2060" /*F. Exenta*/, "2064"/*ND*/, "2065" /*Boleta*/, "2066"/*B. Exenta*/, "2067"/*F. Exportacion*/, "2071"/*Anticipos*/));


            menuevent.Add(new sapmenuevent("1282", s => sb_asignarTipoXDefecto()));
        }

        private void sb_LoadForm(dynamic po_Event)
        {
            try
            {
                GC.Collect(); //Libera la memoria
                /// Se verifica que el evento este dentro de BeforeAction False
                if (po_Event.BeforeAction) return;
                
                if (go_SBOFormEvent.ItemExists("cbTipo"))
                    return;

                go_SBOFormEvent.Freeze(true);
                go_SBOForm = go_SBOFormEvent;

                int li_Top, li_Left, li_Height, li_LeftSt;
                bool blMostrar = true;
                ComboBox lo_ComboSerie;

                InitializeControl(out li_Top, out li_Left, out li_Height, out li_LeftSt);
                /// Creación del control Serie           '
                go_Item = go_SBOForm.Items.Add("cbSerie", BoFormItemTypes.it_COMBO_BOX);
                go_Item.SetDisplay(40, li_Height, li_Top, go_SBOForm.GetItem("cbTipo").Left + go_SBOForm.GetItem("cbTipo").Width + 1);
                go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 9, BoModeVisualBehavior.mvb_False);
                go_Item.Enabled = true;
                go_Item.Visible = blMostrar;
                lo_ComboSerie = go_Item.Specific;

                lo_ComboSerie.DataBind.SetBound(true, ls_tablename, "U_BPP_MDSD");

                /// Creación del control Numero
                go_Item = go_SBOForm.Items.Add("txNumero", BoFormItemTypes.it_EDIT);
                go_Item.SetDisplay(76, li_Height, li_Top, go_SBOForm.GetItem("cbSerie").Left + go_SBOForm.GetItem("cbSerie").Width + 1);
                go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 9, BoModeVisualBehavior.mvb_False);
                go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, (int)BoAutoFormMode.afm_Add, BoModeVisualBehavior.mvb_False);
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
