using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;
using System.Collections.Generic;

namespace STR_Localizacion.UI
{
    internal class Cls_MenuConfiguracion : Cls_PropertiesControl
    {
        public static bool DscInvActivo { get; set; }
        public static string DscInvSufijo { get; set; }

        public static string ProvisionIGVNoDomiciliadoActivo { get; set; }
        public static string ProvisionIGVNDImpuestosRetencion { get; set; }
        public static string ProvisionIGVNDCuentaDebito { get; set; }
        public static string ProvisionIGVNDCuentaCredito { get; set; }
        public static string ProvisionIGVNDFormatoDebito { get; set; }
        public static string ProvisionIGVNDFormatoCredito { get; set; }
        public static string RetencionDeGarantiaActivo { get; set; }
        public static string RetencionDeGarantiaImpuesto { get; set; }
        public static string ReconciliacionActivo { get; set; }
        public static string ReconciliacionCuenta { get; set; }
        static Cls_MenuConfiguracion()
        {
            DscInvActivo = string.Equals(Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_DIACT"), "Y");
            DscInvSufijo = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_DISFJ");
        }

        public Cls_MenuConfiguracion()
        {
            gs_FormName = "ConfLocalizacion";
            gs_FormPath = "Resources/Localizacion/FrmConfiguracion.srf";
            lc_NameClass = "Cls_MenuConfiguracion";
        }

        public void Sb_FormLoad()
        {
            try
            {
                if (go_SBOForm == null)
                {
                    CrearFormulario();
                    CargarDatos();
                    ConfigurarEventos();
                }

                go_SBOForm.Visible = true;
            }
            catch (Exception exc)
            {
                go_SBOApplication.SetStatusBarMessage(exc.Message, BoMessageTime.bmt_Short, true);
            }
        }

        private void CrearFormulario()
        {
            try
            {
                go_SBOForm = Cls_Global.fn_CreateForm(gs_FormName, gs_FormPath);
                go_SBOForm.DataSources.UserDataSources.Add("OpBtnDS", BoDataType.dt_SHORT_TEXT, 1);

                go_OptionButton = go_SBOForm.GetOptionButton("rbEstndr");
                go_OptionButton.DataBind.SetBound(true, "", "OpBtnDS");

                go_OptionButton = go_SBOForm.GetOptionButton("rbOptn1");
                go_OptionButton.DataBind.SetBound(true, "", "OpBtnDS");
                go_OptionButton.GroupWith("rbEstndr");

                go_OptionButton = go_SBOForm.GetOptionButton("rbOptn2");
                go_OptionButton.DataBind.SetBound(true, "", "OpBtnDS");
                go_OptionButton.GroupWith("rbEstndr");

                go_SBOForm.DataSources.UserDataSources.Add("OpAsntDS", BoDataType.dt_SHORT_TEXT, 1);
                //MET ASIENTO DESTINO
                go_OptionButton = go_SBOForm.GetOptionButton("rbAstD1");
                go_OptionButton.DataBind.SetBound(true, "", "OpAsntDS");

                go_OptionButton = go_SBOForm.GetOptionButton("rbAstD2");
                go_OptionButton.DataBind.SetBound(true, "", "OpAsntDS");
                go_OptionButton.GroupWith("rbAstD1");

                go_Folder = go_SBOForm.GetItem("tcDetr").Specific;
                go_Folder.Select();

                go_SBOForm.GetItem("edtAux").Width = 1;
                go_SBOForm.GetItem("Item_4").TextStyle = 4;
                go_SBOForm.GetItem("Item_3").TextStyle = 4;
                go_SBOForm.GetItem("Item_18").TextStyle = 4;
                go_SBOForm.GetItem("Item_6").TextStyle = 4;
                go_SBOForm.GetItem("lblMetodo").TextStyle = 4;
                go_SBOForm.GetItem("Item_19").TextStyle = 4;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CargarDatos()
        {
            try
            {
                lc_NameMethod = "CargarDatos";

                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData);
                go_SBOForm.Freeze(true);

                if (go_RecordSet.RecordCount > 0)
                {
                    string metodoTC = go_RecordSet.Fields.Item("U_STR_MCTC").Value.ToString();
                    string fuenteTC = go_RecordSet.Fields.Item("U_STR_SRCTC").Value.ToString();
                    string metAsientoDestino = go_RecordSet.Fields.Item("U_STR_MTAD").Value.ToString();

                    go_OptionButton = null;

                    CargarDatosProvisionIGVNoDomiciliados();
                    CargarDatosDetracciones(metodoTC, fuenteTC);
                    CargarDatosAsientosDestino(metAsientoDestino);
                    CargarDatosRetencionDeGarantia();
                    ActivarProvisionIGVNoDomiciliados();
                    ActivarRetencionDeGarantia();

                    CargarDatosReconciliacionAuto();
                    ActivarReconciliacionAuto();

                    go_SBOForm.DataSources.UserDataSources.Item("UD_DIACT").Value = go_RecordSet.Fields.Item("U_STR_DIACT").Value.ToString();
                    go_SBOForm.DataSources.UserDataSources.Item("UD_DISFJ").Value = go_RecordSet.Fields.Item("U_STR_DISFJ").Value.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                go_SBOForm.Freeze(false);
            }
        }

        private void ActivarRetencionDeGarantia()
        {
            go_SBOForm.GetItem("edtAux").Specific.Active = true;

            go_CheckBox = go_SBOForm.GetCheckBox("Item_21");
            go_SBOForm.GetItem("Item_22").Enabled = go_CheckBox.Checked;
        }

        private void CargarDatosRetencionDeGarantia()
        {
            go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData);

            RetencionDeGarantiaActivo = go_RecordSet.Fields.Item("U_STR_RG_ACT").Value.ToString();
            RetencionDeGarantiaImpuesto = go_RecordSet.Fields.Item("U_STR_IMPRG").Value.ToString();

            go_SBOForm.DataSources.UserDataSources.Item("UD_RG").Value = RetencionDeGarantiaActivo;
            go_SBOForm.DataSources.UserDataSources.Item("UD_IMPRG").Value = RetencionDeGarantiaImpuesto;
        }
        // -----
        private void ActivarReconciliacionAuto()
        {
            go_SBOForm.GetItem("edtAux").Specific.Active = true;
            go_CheckBox = go_SBOForm.GetCheckBox("Item_10");
            go_SBOForm.GetItem("Item_25").Enabled = go_CheckBox.Checked;
        }

        private void CargarDatosReconciliacionAuto()
        {
            go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData);

            ReconciliacionActivo = go_RecordSet.Fields.Item("U_STR_RECO_ACT").Value.ToString();
            ReconciliacionCuenta = go_RecordSet.Fields.Item("U_STR_RECO_CTA").Value.ToString();

            go_SBOForm.DataSources.UserDataSources.Item("UD_RACT").Value = ReconciliacionActivo;
            go_SBOForm.DataSources.UserDataSources.Item("UD_RCTA").Value = ReconciliacionCuenta;
        }
        // ---
        private void CargarDatosProvisionIGVNoDomiciliados()
        {
            go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData);

            ProvisionIGVNoDomiciliadoActivo = go_RecordSet.Fields.Item("U_STR_PROV_IGV_ND").Value.ToString();
            ProvisionIGVNDImpuestosRetencion = go_RecordSet.Fields.Item("U_STR_IMPRET").Value.ToString();
            ProvisionIGVNDCuentaDebito = go_RecordSet.Fields.Item("U_STR_IDCTA_DEB").Value.ToString();
            ProvisionIGVNDCuentaCredito = go_RecordSet.Fields.Item("U_STR_IDCTA_CRE").Value.ToString();
            ProvisionIGVNDFormatoDebito = go_RecordSet.Fields.Item("U_STR_FMCTA_DEB").Value.ToString();
            ProvisionIGVNDFormatoCredito = go_RecordSet.Fields.Item("U_STR_FMCTA_CRE").Value.ToString();

            go_SBOForm.DataSources.UserDataSources.Item("UD_5").Value = ProvisionIGVNoDomiciliadoActivo;
            go_SBOForm.DataSources.UserDataSources.Item("UD_IMPRET").Value = ProvisionIGVNDImpuestosRetencion;
            go_SBOForm.DataSources.UserDataSources.Item("UD_CD").Value = ProvisionIGVNDCuentaDebito;
            go_SBOForm.DataSources.UserDataSources.Item("UD_CC").Value = ProvisionIGVNDCuentaCredito;
            go_SBOForm.DataSources.UserDataSources.Item("UD_FCD").Value = ProvisionIGVNDFormatoDebito;
            go_SBOForm.DataSources.UserDataSources.Item("UD_FCC").Value = ProvisionIGVNDFormatoCredito;
        }

        private void CargarDatosAsientosDestino(string metodo)
        {
            switch (metodo)
            {
                case "E": go_OptionButton = go_SBOForm.GetOptionButton("rbAstD1"); break;
                case "D": go_OptionButton = go_SBOForm.GetOptionButton("rbAstD2"); break;
            }

            go_OptionButton.Selected = true;
        }

        private void CargarDatosDetracciones(string metodo, string fuenteTC)
        {
            switch (metodo)
            {
                case "E": go_OptionButton = go_SBOForm.GetOptionButton("rbEstndr"); break;
                case "O1": go_OptionButton = go_SBOForm.GetOptionButton("rbOptn1"); break;
                case "O2":
                    go_OptionButton = go_SBOForm.GetOptionButton("rbOptn2");
                    go_Combo = go_SBOForm.GetComboBox("cbSrc");
                    go_Combo.Item.Visible = true;
                    go_Static = go_SBOForm.GetStaticText("lblSrc");
                    go_Static.Item.Visible = true;

                    switch (fuenteTC)
                    {
                        case "F": go_Combo.Select("F", BoSearchKey.psk_ByValue); break;
                        case "P": go_Combo.Select("P", BoSearchKey.psk_ByValue); break;
                    }
                    break;
            }

            go_OptionButton.Selected = true;
        }

        private void ConfigurarEventos()
        {
            try
            {
                itemevent.Add(new sapitemevent(BoEventTypes.et_CHOOSE_FROM_LIST, "Item_11", e => EscogerImpuestosRetencion(e)));
                itemevent.Add(new sapitemevent(BoEventTypes.et_CHOOSE_FROM_LIST, "Item_22", e => EscogerImpuestosRetencionDeGarantia(e)));
                itemevent.Add(new sapitemevent(BoEventTypes.et_CHOOSE_FROM_LIST, "Item_12", e => EscogerCuentaDebito(e)));
                itemevent.Add(new sapitemevent(BoEventTypes.et_CHOOSE_FROM_LIST, "Item_13", e => EscogerCuentaCredito(e)));
                itemevent.Add(new sapitemevent(BoEventTypes.et_CHOOSE_FROM_LIST, "Item_25", e => EscogerCuentaReconciliacionAuto(e)));
                itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "Item_5", e => { if (!e.BeforeAction) ActivarProvisionIGVNoDomiciliados(); }));
                itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "Item_21", e => { if (!e.BeforeAction) ActivarRetencionDeGarantia(); }));
                itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "Item_10", e => { if (!e.BeforeAction) ActivarReconciliacionAuto(); }));
                itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "btnSave", s =>
                {
                    if (s.BeforeAction)
                    {
                        int rpta = go_SBOApplication.MessageBox("Se grabaran los datos consignados. ¿Desea continuar?", 1, "Sí", "No");

                        if (rpta == 1)
                        {
                            GrabarCambios();
                            Cls_Global.metCalculoTC = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_MCTC");
                            Cls_Global.fuenteTC = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_SRCTC");
                            Cls_Global.metAsientoDestino = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_MTAD");
                            Cls_Global.ProvisionNDActivo = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_PROV_IGV_ND");
                            Cls_Global.ProvisionNDCodigosRetencion = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_IMPRET");
                            Cls_Global.ProvisionNDCuentaDebito = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_IDCTA_DEB");
                            Cls_Global.ProvisionNDCuentaCredito = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_IDCTA_CRE");
                            Cls_Global.ProvisionNDFormatoCuentaDebito = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_FMCTA_DEB");
                            Cls_Global.ProvisionNDFormatoCuentaCredito = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_FMCTA_CRE");
                            Cls_Global.RetencionDeGarantiaActivo = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_RG_ACT");
                            Cls_Global.ImpuestoRetencionDeGarantia = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_IMPRG");

                            Cls_Global.ReconciliacionActivo = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_RECO_ACT");
                            Cls_Global.ReconciliacionCuenta = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_RECO_CTA");
                        }
                        else
                            return;
                    }
                }));

                itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "rbOptn2", s =>
                {
                    if (!s.BeforeAction)
                    {
                        go_SBOForm = go_SBOApplication.Forms.ActiveForm;
                        go_OptionButton = go_SBOForm.GetOptionButton("rbOptn2");

                        if (go_OptionButton.Selected)
                        {
                            try
                            {
                                go_Static = go_SBOForm.GetStaticText("lblSrc");
                                go_Static.Item.Visible = true;

                                go_Combo = go_SBOForm.GetComboBox("cbSrc");
                                go_Combo.Item.Visible = true;
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                ));

                itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "rbEstndr", s =>
                {
                    go_SBOForm = go_SBOApplication.Forms.ActiveForm;

                    if (!s.BeforeAction)
                    {
                        go_OptionButton = go_SBOForm.GetOptionButton(s.ItemUID);

                        if (go_OptionButton.Selected)
                        {
                            try
                            {
                                go_Static = go_SBOForm.GetStaticText("lblSrc");
                                go_Static.Item.Visible = false;

                                go_OptionButton = go_SBOForm.GetOptionButton("rbEstndr");

                                go_Combo = go_SBOForm.GetComboBox("cbSrc");
                                go_Edit = go_SBOForm.GetEditText("itm");
                                go_Edit.Item.Click(BoCellClickType.ct_Regular);

                                go_Combo.Item.Visible = false;
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                ));

                itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "rbOptn1", s =>
                {
                    go_SBOForm = go_SBOApplication.Forms.ActiveForm;

                    if (!s.BeforeAction)
                    {
                        go_OptionButton = go_SBOForm.GetOptionButton(s.ItemUID);

                        if (go_OptionButton.Selected)
                        {
                            try
                            {
                                go_Static = go_SBOForm.GetStaticText("lblSrc");
                                go_Static.Item.Visible = false;

                                go_OptionButton = go_SBOForm.GetOptionButton("rbEstndr");

                                go_Combo = go_SBOForm.GetComboBox("cbSrc");
                                go_Edit = go_SBOForm.GetEditText("itm");
                                go_Edit.Item.Click(BoCellClickType.ct_Regular);

                                go_Combo.Item.Visible = false;
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
        ));

                itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "chkDIActv", e =>
                {
                    if (!e.BeforeAction)
                    {
                        go_CheckBox = go_SBOForm.GetCheckBox(e.ItemUID);
                        go_SBOForm.GetItem("edtAux").Click();
                        go_SBOForm.GetItem("sttSfjSre").Visible = go_CheckBox.Checked;
                        go_SBOForm.GetItem("edtSfjSre").Visible = go_CheckBox.Checked;
                    }
                }));

                itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "tcDsInv", e =>
                {
                    if (!e.BeforeAction)
                    {
                        try
                        {
                            go_SBOForm.Freeze(true);
                            go_CheckBox = go_SBOForm.GetCheckBox("chkDIActv");
                            go_SBOForm.GetItem("edtAux").Click();
                            go_SBOForm.GetItem("sttSfjSre").Visible = go_CheckBox.Checked;
                            go_SBOForm.GetItem("edtSfjSre").Visible = go_CheckBox.Checked;
                        }
                        finally
                        {
                            go_SBOForm.Freeze(false);
                        }
                    }
                }));

                itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, s => { Dispose(); }));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void EscogerImpuestosRetencionDeGarantia(ItemEvent e)
        {
            if (!e.BeforeAction)
            {
                var cflEvnt = (ChooseFromListEvent)e;
                if (cflEvnt.SelectedObjects is DataTable dtbl)
                {
                    go_SBOForm.DataSources.UserDataSources.Item("UD_IMPRG").Value = (string)dtbl.GetValue("WTCode", 0);
                }
            }
        }

        private void ActivarProvisionIGVNoDomiciliados()
        {
            go_CheckBox = go_SBOForm.GetCheckBox("Item_5");

            go_SBOForm.GetItem("edtAux").Specific.Active = true;

            go_SBOForm.GetItem("Item_11").Enabled = go_CheckBox.Checked;
            go_SBOForm.GetItem("Item_12").Enabled = go_CheckBox.Checked;
            go_SBOForm.GetItem("Item_14").Enabled = go_CheckBox.Checked;
            go_SBOForm.GetItem("Item_13").Enabled = go_CheckBox.Checked;
            go_SBOForm.GetItem("Item_15").Enabled = go_CheckBox.Checked;
        }

        private void EscogerCuentaCredito(ItemEvent e)
        {
            if (!e.BeforeAction)
            {
                var cflEvnt = (ChooseFromListEvent)e;
                if (cflEvnt.SelectedObjects is DataTable dtbl)
                {
                    go_SBOForm.DataSources.UserDataSources.Item("UD_CC").Value = (string)dtbl.GetValue("AcctCode", 0);
                    go_SBOForm.DataSources.UserDataSources.Item("UD_FCC").Value = (string)dtbl.GetValue("FormatCode", 0);
                }
            }
        }

        private void EscogerCuentaDebito(ItemEvent e)
        {
            if (!e.BeforeAction)
            {
                var cflEvnt = (ChooseFromListEvent)e;
                if (cflEvnt.SelectedObjects is DataTable dtbl)
                {
                    go_SBOForm.DataSources.UserDataSources.Item("UD_CD").Value = (string)dtbl.GetValue("AcctCode", 0);
                    go_SBOForm.DataSources.UserDataSources.Item("UD_FCD").Value = (string)dtbl.GetValue("FormatCode", 0);
                }
            }
        }

        private void EscogerCuentaReconciliacionAuto(ItemEvent e)
        {
            if (!e.BeforeAction)
            {
                var cflEvnt = (ChooseFromListEvent)e;                
                if (cflEvnt.SelectedObjects is DataTable dtbl)
                {
                    go_SBOForm.DataSources.UserDataSources.Item("UD_RCTA").Value = (string)dtbl.GetValue("AcctCode", 0);
                    //go_SBOForm.DataSources.UserDataSources.Item("UD_FCD").Value = (string)dtbl.GetValue("FormatCode", 0);
                }
            }
        }
        private void EscogerImpuestosRetencion(ItemEvent e)
        {
            if (!e.BeforeAction)
            {
                var cflEvnt = (ChooseFromListEvent)e;
                if (cflEvnt.SelectedObjects is DataTable dtbl)
                {
                    List<string> impuestosSeleccionados = new List<string>();
                    for (int i = 0; i < dtbl.Rows.Count; i++)
                        impuestosSeleccionados.Add((string)dtbl.GetValue("WTCode", i));

                    go_SBOForm.DataSources.UserDataSources.Item("UD_IMPRET").Value = string.Join(";", impuestosSeleccionados.ToArray());
                }
            }
        }

        private void GrabarCambios()
        {
            string metTC = string.Empty;
            string fuenteTC = "F";
            string metAsientoDestino = string.Empty;

            go_SBOForm = go_SBOApplication.Forms.Item(gs_FormName);

            try
            {
                metTC = ObtenerMetodoDeObtencionTC(go_SBOForm);
                metAsientoDestino = ObtenerMetodoAsientoDestino(go_SBOForm);

                if (metTC == "O2")
                    fuenteTC = ObtenerFuenteTC(go_SBOForm);

                DscInvActivo = string.Equals(go_SBOForm.DataSources.UserDataSources.Item("UD_DIACT").Value, "Y");
                DscInvSufijo = go_SBOForm.DataSources.UserDataSources.Item("UD_DISFJ").Value;
                ProvisionIGVNoDomiciliadoActivo = go_SBOForm.DataSources.UserDataSources.Item("UD_5").Value;
                ProvisionIGVNDImpuestosRetencion = go_SBOForm.DataSources.UserDataSources.Item("UD_IMPRET").Value;
                ProvisionIGVNDCuentaDebito = go_SBOForm.DataSources.UserDataSources.Item("UD_CD").Value;
                ProvisionIGVNDCuentaCredito = go_SBOForm.DataSources.UserDataSources.Item("UD_CC").Value;
                ProvisionIGVNDFormatoDebito = go_SBOForm.DataSources.UserDataSources.Item("UD_FCD").Value;
                ProvisionIGVNDFormatoCredito = go_SBOForm.DataSources.UserDataSources.Item("UD_FCC").Value;
                RetencionDeGarantiaActivo = go_SBOForm.DataSources.UserDataSources.Item("UD_RG").Value;
                RetencionDeGarantiaImpuesto = go_SBOForm.DataSources.UserDataSources.Item("UD_IMPRG").Value;

                ReconciliacionActivo = go_SBOForm.DataSources.UserDataSources.Item("UD_RACT").Value;
                ReconciliacionCuenta = go_SBOForm.DataSources.UserDataSources.Item("UD_RCTA").Value;

                Grabar(metTC, fuenteTC, metAsientoDestino, DscInvActivo, DscInvSufijo);
                go_SBOApplication.StatusBar.SetText("Configuración actualizada correctamente.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string ObtenerMetodoAsientoDestino(Form go_SBOForm)
        {
            try
            {
                go_OptionButton = go_SBOForm.GetOptionButton("rbAstD1");

                if (go_OptionButton.Selected)
                    return "E";
                else return "D";
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Grabar(string metTC, string fuenteTC, string metAsientoDestino, bool diActivo, string diSufijo)
        {
            int registros = int.Parse(Cls_QueryManager.Retorna(Cls_Query.obt_RegConfig, "Registros").ToString());
            var strDIActivo = diActivo ? "Y" : "N";//Descarga de inventario activo?
            Cls_QueryManager.Procesa(registros == 0 ? Cls_Query.create_LOCConfig : Cls_Query.update_LOCConfig, metTC, fuenteTC, metAsientoDestino, strDIActivo, diSufijo, ProvisionIGVNoDomiciliadoActivo, ProvisionIGVNDImpuestosRetencion, ProvisionIGVNDCuentaDebito, ProvisionIGVNDCuentaCredito, ProvisionIGVNDFormatoDebito, ProvisionIGVNDFormatoCredito, RetencionDeGarantiaActivo, RetencionDeGarantiaImpuesto, ReconciliacionCuenta, ReconciliacionActivo);
        }

        private string ObtenerFuenteTC(Form go_SBOForm)
        {
            go_Combo = go_SBOForm.GetComboBox("cbSrc");
            return go_Combo.Value;
        }

        private string ObtenerMetodoDeObtencionTC(Form go_SBOForm)
        {
            go_OptionButton = go_SBOForm.GetOptionButton("rbEstndr");

            if (go_OptionButton.Selected)
            {
                return "E";
            }
            else
            {
                go_OptionButton = go_SBOForm.GetOptionButton("rbOptn1");

                if (go_OptionButton.Selected)
                    return "O1";
                else
                    return "O2";
            }
        }
    }
}