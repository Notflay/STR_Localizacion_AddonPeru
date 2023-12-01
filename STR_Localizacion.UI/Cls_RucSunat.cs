using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace STR_Localizacion.UI
{
    public class Cls_RucSunat : Cls_PropertiesControl
    {
        public List<string> go_InternalFormID = new List<string>() { "134" };

        public Cls_RucSunat()
        {
            lc_NameClass = "Cls_RucSunat";
            ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            menuevent.Add(new sapmenuevent(
                s => sb_cargarcontroles(s), "2561"));

            menuevent.Add(new sapmenuevent(s => sb_cargarcontroles(s), "1282"));

            itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, s =>
             {
                 if (s.BeforeAction) return;
                 if (go_SBOForm is null) go_SBOForm = go_SBOFormActive;

                 try
                 {
                     if (go_SBOForm.Mode == BoFormMode.fm_ADD_MODE || go_SBOForm.Mode == BoFormMode.fm_EDIT_MODE
                       || go_SBOForm.Mode == BoFormMode.fm_OK_MODE || go_SBOForm.Mode == BoFormMode.fm_UPDATE_MODE)
                     {
                         go_SBOForm.Freeze(true);

                         string ruc = go_SBOForm.GetEditText("41").Value;
                         if (!Cls_Sunat.EsRucValido(ruc))
                         {
                             go_SBOApplication.SetStatusBarMessage("El RUC no es válido", BoMessageTime.bmt_Short);
                             return;
                         }
                         go_SBOApplication.SetStatusBarMessage("Recuperando información de SUNAT...", BoMessageTime.bmt_Medium, false);

                         string tipopersona = string.Empty;
                         if (!string.IsNullOrEmpty(ruc))
                             tipopersona = ruc.Substring(0, 2).Equals("20") ? "TPJ" : (ruc.Substring(0, 2).Equals("10") ? "TPN" : string.Empty);

                         /*NUEVA FUNCIONALIDAD OBTENER DATOS RUC */

                         Cls_ObtRuc obtruc = new Cls_ObtRuc();
                         obtruc.ConsultarRucMedianteNumeroRandom(ruc, go_SBOForm);


                         /*NUEVA FUNCIONALIDAD OBTENER DATOS RUC */


                         //if (Cls_Sunat.ObtenerDatosDesdeSUNAT(ruc) == 0)
                         //{
                         //  LlenarValoresEnCampos();
                         //}
                         //else
                         //    go_SBOApplication.StatusBar.SetText(Cls_Sunat.MensajeError, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);

                         // go_SBOApplication.StatusBar.SetText("Información recuperada correctamente.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                     }
                 }
                 catch (Exception)
                 {
                     throw;
                 }
                 finally { go_SBOForm.Freeze(false); }
             }, "btnSunat"));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_LOAD, string.Empty,
                s => sb_cargarcontroles(s)));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, s => Dispose()));
        }

        private void LlenarValoresEnCampos()
        {
            try
            {
                string ruc = go_SBOForm.GetEditText("41").Value;
                //Form oForm = go_SBOApplication.Forms.GetFormByTypeAndCount(-134, go_SBOApplication.Forms.ActiveForm.TypeCount);
                //razon social
                go_SBOForm.GetEditText("7").Value = Cls_Sunat.RazonSocial.Split('-')[1].Trim();
                //tipo persona
                string tipoPersona = ruc.Substring(0, 2).Equals("20") ? "TPJ" : (ruc.Substring(0, 2).Equals("10") ? "TPN" : string.Empty);

                if (tipoPersona != string.Empty)
                    go_SBOForm.GetComboBox("cbxTipPrs").SelectExclusive(tipoPersona, BoSearchKey.psk_ByValue);

                go_SBOForm.GetComboBox("cbxHabido").SelectExclusive(Cls_Sunat.Habido.Equals("HABIDO") ? "N" : "Y", BoSearchKey.psk_ByValue);
                go_SBOForm.GetComboBox("cbxAgtRet").SelectExclusive(Cls_Sunat.AgRetencion, BoSearchKey.psk_ByValue);
                go_SBOForm.GetComboBox("cbxAgtPer").SelectExclusive(Cls_Sunat.AgPercepcion, BoSearchKey.psk_ByValue);
                go_SBOForm.GetComboBox("cbxBnCntr").SelectExclusive(Cls_Sunat.BuenContrib.Equals("ACTIVO") ? "Y" : "N", BoSearchKey.psk_ByValue);

                go_SBOForm.PaneLevel = 7;
                //DIRECCION
                Matrix matrix = go_SBOForm.GetMatrix("178");
                matrix.Columns.Item("1").Cells.Item(1).Specific.Value = "SUNAT";
                matrix.Columns.Item("3").Cells.Item(1).Specific.Value = Cls_Sunat.Distrito;
                matrix.Columns.Item("4").Cells.Item(1).Specific.Value = Cls_Sunat.Provincia;
                matrix.Columns.Item("6").Cells.Item(1).Specific.Value = Cls_Sunat.Direccion;

                ComboBox comboBox = matrix.GetCellSpecific("7", 1);

                var depa = comboBox.ValidValues.OfType<IValidValue>().Where(item => item.Description.Contains(Cls_Sunat.Departamento)).FirstOrDefault();
                if (depa != null)
                    comboBox.SelectExclusive(depa.Value, BoSearchKey.psk_ByValue);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void sb_cargarcontroles(dynamic po_Event)
        {
            if (!po_Event.BeforeAction)
            {
                try
                {
                    go_SBOForm = go_SBOFormEvent;
                    go_SBOForm.Freeze(true);
                    string ls_FormTypeEx = go_SBOForm.TypeEx;

                    if (go_SBOForm.Mode == BoFormMode.fm_ADD_MODE || go_SBOForm.Mode == BoFormMode.fm_UPDATE_MODE || go_SBOForm.Mode == BoFormMode.fm_OK_MODE || go_SBOForm.Mode == BoFormMode.fm_EDIT_MODE)
                    {
                        if (ls_FormTypeEx.Equals("134"))
                        {
                            if (go_SBOForm.ItemExists("btnSunat"))                           
                                return;
                           
                            //go_SBOForm.PaneLevel = 7;

                            go_Item = go_SBOForm.GetItem("41");
                            go_Item.Width = go_SBOForm.GetItem("41").Width - 40;

                            //Boton Sunat
                            go_Item = go_SBOForm.Items.Add("btnSunat", BoFormItemTypes.it_BUTTON);
                            go_Item.Width = 37;
                            go_Item.Height = go_SBOForm.GetItem("41").Height;
                            go_Item.SetPosition(
                                go_SBOForm.GetItem("41").Top,
                                go_SBOForm.GetItem("41").Left + go_SBOForm.GetItem("41").Width + 3);
                            go_Button = go_Item.Specific;
                            go_Button.Caption = "Sunat";

                            //Edit Tipo Persona
                            go_Item = go_SBOForm.Items.Add("cbxTipPrs", BoFormItemTypes.it_COMBO_BOX);
                            go_Item.FromPane = 7;
                            go_Item.ToPane = 7;
                            go_Item.Width = go_SBOForm.GetItem("41").Width;
                            go_Item.Height = go_SBOForm.GetItem("41").Height;
                            go_Item.Visible = false;
                            go_Item.SetPosition(
                                    go_SBOForm.GetItem("70").Top + go_SBOForm.GetItem("70").Height + 6,
                                    go_SBOForm.GetItem("70").Left);
                            go_Combo = go_Item.Specific;
                            go_Combo.DataBind.SetBound(true, "OCRD", "U_BPP_BPTP");

                            //Agente Retencion
                            go_Item = go_SBOForm.Items.Add("cbxAgtRet", BoFormItemTypes.it_COMBO_BOX);
                            go_Item.FromPane = 7;
                            go_Item.ToPane = 7;
                            go_Item.Width = go_SBOForm.GetItem("41").Width;
                            go_Item.Height = go_SBOForm.GetItem("41").Height;
                            go_Item.Visible = false;
                            go_Item.SetPosition(
                                go_SBOForm.GetItem("70").Top + go_SBOForm.GetItem("70").Height * 4,
                                go_SBOForm.GetItem("70").Left);
                            go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                            go_Combo = go_Item.Specific;
                            go_Combo.DataBind.SetBound(true, "OCRD", "U_STR_AR");

                            //Agente Percepcion
                            go_Item = go_SBOForm.Items.Add("cbxAgtPer", BoFormItemTypes.it_COMBO_BOX);
                            go_Item.FromPane = 7;
                            go_Item.ToPane = 7;
                            go_Item.Width = go_SBOForm.GetItem("41").Width;
                            go_Item.Height = go_SBOForm.GetItem("41").Height;
                            go_Item.Visible = false;
                            go_Item.SetPosition(
                                go_SBOForm.GetItem("70").Top + go_SBOForm.GetItem("70").Height * 5,
                                go_SBOForm.GetItem("70").Left);
                            go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                            go_Combo = go_Item.Specific;
                            go_Combo.DataBind.SetBound(true, "OCRD", "U_STR_AP");

                            //Buen Contribuyente
                            go_Item = go_SBOForm.Items.Add("cbxBnCntr", BoFormItemTypes.it_COMBO_BOX);
                            go_Item.FromPane = 7;
                            go_Item.ToPane = 7;
                            go_Item.Width = go_SBOForm.GetItem("41").Width;
                            go_Item.Height = go_SBOForm.GetItem("41").Height;
                            go_Item.Visible = false;
                            go_Item.SetPosition(
                                go_SBOForm.GetItem("70").Top + go_SBOForm.GetItem("70").Height * 3,
                                go_SBOForm.GetItem("70").Left);
                            go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                            go_Combo = go_Item.Specific;
                            go_Combo.DataBind.SetBound(true, "OCRD", "U_STR_BC");

                            //Edit Habido
                            go_Item = go_SBOForm.Items.Add("cbxHabido", BoFormItemTypes.it_COMBO_BOX);
                            go_Item.FromPane = 7;
                            go_Item.ToPane = 7;
                            go_Item.Width = go_SBOForm.GetItem("41").Width;
                            go_Item.Height = go_SBOForm.GetItem("41").Height;
                            go_Item.Visible = false;
                            go_Item.SetPosition(
                                go_SBOForm.GetItem("70").Top + go_SBOForm.GetItem("70").Height * 2,
                                go_SBOForm.GetItem("70").Left);
                            go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                            go_Combo = go_Item.Specific;
                            go_Combo.DataBind.SetBound(true, "OCRD", "U_STR_NH");

                            //go_SBOForm.PaneLevel = 1;
                        }
                    }
                }
                finally { go_SBOForm.Freeze(false); }
            }
        }
    }
}