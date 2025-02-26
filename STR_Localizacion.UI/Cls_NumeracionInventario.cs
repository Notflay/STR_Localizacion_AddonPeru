using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SAPbouiCOM;
using SAPbobsCOM;

using STR_Localizacion.DL;
using STR_Localizacion.MetaData;
using STR_Localizacion.UTIL;

namespace STR_Localizacion.UI
{
    public class Cls_NumeracionInventario : Cls_NumeracionBase
    {
        public Cls_NumeracionInventario()
        {
            lc_NameMethod = "Cls_NumeracionInventario";
            go_InternalFormID = new List<string>() { "720", "940","1250000940" };

            ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
            InitializeEvents();
        }

        public void SetTableName(string FormID)
        {
            switch (FormID)
            {
                case "720":
                    ls_tablename = "OIGE";
                    break;
                case "940":
                    ls_tablename = "OWTR";
                    break;
                case "1250000940":
                    ls_tablename = "OWTQ";
                    break;
            }
        }

        private void InitializeEvents()
        {
            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_LOAD, string.Empty,
                s => sb_LoadForm(s)));

            itemevent.Add(new sapitemevent(BoEventTypes.et_COMBO_SELECT, "cbTipo",
                s =>
                {
                    if (!s.BeforeAction)
                        fn_actualizarSerieSUNAT();
                }));

            menuevent.Add(new sapmenuevent(s =>
            {
                if (s.BeforeAction) return;

                if (go_SBOFormActive.Mode == BoFormMode.fm_OK_MODE ||
                    go_SBOFormActive.Mode == BoFormMode.fm_FIND_MODE ||
                    go_SBOFormActive.Mode == BoFormMode.fm_ADD_MODE)
                {
                    go_SBOForm = go_SBOFormEvent;
                    go_Combo = go_SBOForm.GetControl("cbTpoTrnf");
                    if (go_Combo.Value == "TSI" || go_SBOFormActive.Mode == BoFormMode.fm_FIND_MODE)
                        go_SBOForm.SetVisibleControl(false, "stNum", "cbTipo", "cbSerie", "txNumero");
                    else if (go_Combo.Value == "TSE")
                        go_SBOForm.SetVisibleControl(true, "stNum", "cbTipo", "cbSerie", "txNumero");
                }
            }, "1281", "1282", "1288", "1289", "1290", "1291"));


            menuevent.Add(new sapmenuevent(s =>
               sb_LoadForm(s), "3080", "3079" /*Salida de mercancias*/));

        }

        /// <Creación de Controles en Formulario de Inventario>
        /// Crea variables que contendran valores iniciales de las coordenadas para los controles en el formulario.
        /// Luego procede con la creación de cada control: Tipo de salida(Externa, Interna), Tipo, Serie, Número.
        private void sb_crearControlFormInventario()
        {
            lc_NameMethod = "sb_crearControlFormInventario"; //Se asigna el nombre del método para la identificación del mismo
            try
            {
                ///Variables que usaremos para la posición de los controles
                int li_Top, li_Left, li_Height, li_LeftSt;
                bool lb_Mostrar = true;

                /// declaracion de coordenadas para el tipo salida, según el formulario.
                if (go_SBOForm.TypeEx == "720")
                {
                    li_Top = go_SBOForm.GetItem("3").Top + go_SBOForm.GetItem("3").Height + 1;
                    li_Left = go_SBOForm.GetItem("3").Left;
                    li_Height = go_SBOForm.GetItem("3").Height;
                    li_LeftSt = go_SBOForm.GetItem("20").Left;
                }
                else
                {
                    li_Top = go_SBOForm.GetItem("9").Top + go_SBOForm.GetItem("9").Height + 1;
                    li_Left = go_SBOForm.GetItem("9").Left;
                    li_Height = go_SBOForm.GetItem("31").Height;
                    li_LeftSt = go_SBOForm.GetItem("10").Left;
                }

                /// creacion del combo del tipo de salida.
                go_Item = go_SBOForm.Items.Add("cbTpoTrnf", BoFormItemTypes.it_COMBO_BOX);
                go_Item.SetDisplay(85, li_Height, li_Top, li_Left, true);
                go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 9, BoModeVisualBehavior.mvb_False);
                go_Item.Enabled = true;
                go_Item.Visible = lb_Mostrar;
                go_Combo = go_Item.Specific;
                go_Combo.ValidValues.Add("TSI", "Interna");
                go_Combo.ValidValues.Add("TSE", "Externa");
                go_Combo.DataBind.SetBound(true, ls_tablename, "U_BPP_MDTS");

                go_Item = go_SBOForm.Items.Add("stTipoS", BoFormItemTypes.it_STATIC);
                go_Item.SetDisplay(85, li_Height, li_Top, li_LeftSt);
                go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 9, BoModeVisualBehavior.mvb_False);
                go_Item.Enabled = true;
                go_Item.LinkTo = "cbTpoTrnf";
                go_Static = go_Item.Specific;
                go_Static.Caption = "Tipo de Salida";

                /// Creación del control Tipo documento.
                go_Item = go_SBOForm.Items.Add("cbTipo", BoFormItemTypes.it_COMBO_BOX);
                go_Item.SetDisplay(76, go_SBOForm.GetItem("cbTpoTrnf").Height, go_SBOForm.GetItem("cbTpoTrnf").Top + go_SBOForm.GetItem("cbTpoTrnf").Height + 1, li_Left, true);
                go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 9, BoModeVisualBehavior.mvb_False);
                go_Item.Enabled = true;
                go_Item.Visible = lb_Mostrar;
                go_Combo = go_Item.Specific;

                go_Item = go_SBOForm.Items.Add("stNum", BoFormItemTypes.it_STATIC);
                go_Item.SetDisplay(76, li_Height, go_SBOForm.GetItem("stTipoS").Top + go_SBOForm.GetItem("stTipoS").Height + 1, go_SBOForm.GetItem("stTipoS").Left);
                go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 9, BoModeVisualBehavior.mvb_False);
                go_Item.Enabled = true;
                go_Item.LinkTo = "cbTipo";
                go_Static = go_Item.Specific;
                go_Static.Caption = "N° Doc";
                go_Combo.DataBind.SetBound(true, ls_tablename, "U_BPP_MDTD");

                /// Creación del control Serie.    
                go_Item = go_SBOForm.Items.Add("cbSerie", BoFormItemTypes.it_COMBO_BOX);
                go_Item.SetDisplay(60, li_Height, go_SBOForm.GetItem("cbTipo").Top, li_Left + 77);
                go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 9, BoModeVisualBehavior.mvb_False);
                go_Item.Enabled = true;
                go_Item.Visible = lb_Mostrar;
                go_Combo = go_Item.Specific;
                go_Combo.DataBind.SetBound(true, ls_tablename, "U_BPP_MDSD");

                /// Creación del control Numero.
                go_Item = go_SBOForm.Items.Add("txNumero", BoFormItemTypes.it_EDIT);
                go_Item.SetDisplay(76, li_Height, go_SBOForm.GetItem("cbTipo").Top, go_SBOForm.GetItem("cbSerie").Left + 61);
                go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 9, BoModeVisualBehavior.mvb_False);
                go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, (int)BoAutoFormMode.afm_Add, BoModeVisualBehavior.mvb_False);
                go_Item.Visible = lb_Mostrar;
                go_Edit = go_Item.Specific;
                go_Edit.DataBind.SetBound(true, ls_tablename, "U_BPP_MDCD");
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                ExceptionPrepared.SaveInLog(false);
            }
        }

        private void sb_LoadForm(dynamic po_Event)
        {
            GC.Collect(); //Libera la memoria
            /// Se verifica que el evento este dentro de BeforeAction False
            if (po_Event.BeforeAction) return;

            if (go_SBOFormEvent.ItemExists("cbTipo"))
                return;

            try
            {
                go_SBOFormEvent.Freeze(true);
                go_SBOForm = go_SBOFormEvent;
                sb_crearControlFormInventario();
                go_SBOForm.SetVisibleControl(false, "stNum", "cbTipo", "cbSerie", "txNumero");
            }
            finally { go_SBOFormEvent.Freeze(false); }


        }
    }
}
