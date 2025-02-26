using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SAPbouiCOM;
using System.Globalization;
using STR_Localizacion.UTIL;
using STR_Localizacion.DL;

namespace STR_Localizacion.UI
{
    partial class Cls_GenerarAsiento
    {
        bool OptionEvnFormData = false;

        private void InitializeEvents()
        {
            itemevent.Add(BoEventTypes.et_ITEM_PRESSED,
                new sapitemevent(lrs_BtnBuscar, s =>
                {
                    if (!s.BeforeAction)
                    {
                        string ls_TipoAsiento = string.Empty;
                        if (go_SBOForm.GetOptionButton(lrs_RbtDtr).Selected) ls_TipoAsiento = "D";
                        else if (go_SBOForm.GetOptionButton(lrs_RbtPrc).Selected) ls_TipoAsiento = "P";
                        else if (go_SBOForm.GetOptionButton(lrs_RbtFNg).Selected) ls_TipoAsiento = "F";

                        if (string.IsNullOrEmpty(ls_TipoAsiento))
                        {
                            go_SBOForm.Items.Item(lrs_BtnCrear).Enabled = false;
                            throw new InvalidOperationException("Debe seleccionar un tipo de asiento");
                        }
                        
                        sb_CargaGrilla(new string[]{
                                        go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflPrvDd, 0),
                                        go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflPrvHt, 0),
                                        go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflFchCnDd, 0),
                                        go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflFchCnHt, 0),
                                        go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflFchVnDd, 0),
                                        go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflFchVnHt, 0),
                                        go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflBnkMnd, 0),
                                        ls_TipoAsiento });
                    }

                }),
                new sapitemevent(lrs_BtnCrear, s =>
                {
                    if (s.BeforeAction)
                    {
                        if (go_SBOForm.Mode == BoFormMode.fm_ADD_MODE || go_SBOForm.Mode == BoFormMode.fm_UPDATE_MODE)
                            if (go_SBOForm.GetOptionButton(lrs_RbtFNg).Selected)
                                sb_GenerarAsientoFNG();
                            else
                                sb_GenerarAsiento();
                    }
                    else { sb_DataFormLoad(); }
                }),

                new sapitemevent(lrs_MtxBatDTR, s =>
                {
                    if (!s.BeforeAction) return;

                    if (s.ColUID.Equals(lrs_ClmMtxSelec) && s.Row > 0)
                        sb_ActualizarImportes();
                }),
                
                new sapitemevent(s =>
                {
                    if (s.BeforeAction) return;
                    
                    try
                    {
                        bool RbtPressed = go_SBOForm.GetOptionButton(lrs_RbtFNg).Selected;
                        go_SBOForm.Freeze(true);
                        
                        go_SBOForm.GetItem(lrs_EdtPrvDd).Click();
                        go_SBOForm.GetItem(lrs_LblBnkPais).Visible = RbtPressed;
                        go_SBOForm.GetItem(lrs_LblBnkCdgo).Visible = RbtPressed;
                        go_SBOForm.GetItem(lrs_LblBnkCta).Visible = RbtPressed;
                        go_SBOForm.GetItem(lrs_LblBnkMnd).Visible = RbtPressed;
                        go_SBOForm.GetItem(lrs_LblAsnRspn).Visible = RbtPressed;

                        //go_SBOForm.GetItem(lrs_LblMntTotal).Visible = RbtPressed;
                        go_SBOForm.GetItem(lrs_LblMntPorte).Visible = RbtPressed;
                        go_SBOForm.GetItem(lrs_LblMntCmns).Visible = RbtPressed;
                        go_SBOForm.GetItem(lrs_LblMntImporte).Visible = RbtPressed;

                        go_SBOForm.GetItem(lrs_CbxBnkPais).Visible = RbtPressed;
                        go_SBOForm.GetItem(lrs_CbxBnkCdgo).Visible = RbtPressed;
                        go_SBOForm.GetItem(lrs_CbxBnkCta).Visible = RbtPressed;
                        go_SBOForm.GetItem(lrs_EdtBnkMnd).Visible = RbtPressed;

                        //go_SBOForm.GetItem(lrs_EdtMntTotal).Visible = RbtPressed;
                        go_SBOForm.GetItem(lrs_EdtMntPorte).Visible = RbtPressed;
                        go_SBOForm.GetItem(lrs_EdtMntCmns).Visible = RbtPressed;
                        go_SBOForm.GetItem(lrs_EdtMntImporte).Visible = RbtPressed;

                        go_SBOForm.GetItem(lrs_EdtAsnRsp).Visible = RbtPressed;


                        if (!OptionEvnFormData)
                        {
                            go_SBOForm.GetMatrix(lrs_MtxBatDTR).Clear();
                            go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).Clear();

                            if (!RbtPressed)
                                go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).SetValue(lrs_UflFchEje, 0, DateTime.Now.ToString(lrs_FchFormat));

                            go_SBOForm.GetItem(lrs_EdtFchEje).Enabled = RbtPressed;
                            string ls_TpoAS = go_SBOForm.GetOptionButton(lrs_RbtDtr).Selected ? "DT" : (go_SBOForm.GetOptionButton(lrs_RbtPrc).Selected ? "PR" : "FN");
                            go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).SetValue(lrs_UflTipoAsn, 0, ls_TpoAS);
                        }
                    }
                    finally { go_SBOForm.Freeze(false); }

                }, lrs_RbtDtr, lrs_RbtPrc, lrs_RbtFNg));

            itemevent.Add(new sapitemevent(BoEventTypes.et_DOUBLE_CLICK, lrs_MtxBatDTR, s =>
            {
                if (!s.BeforeAction)
                {
                    go_SBOForm = go_SBOFormEvent;
                    if (s.ColUID.Equals(lrs_ClmMtxSelec) && s.Row == 0)
                        sb_MultipleCheckingGrid();
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_CHOOSE_FROM_LIST, s =>
            {
                IChooseFromListEvent lo_CFLEvent = (IChooseFromListEvent)s;
                ChooseFromList lo_CFL = go_SBOForm.ChooseFromLists.Item(lo_CFLEvent.ChooseFromListUID);
                string lrs_UflSocio = lo_CFLEvent.ChooseFromListUID.Equals(lrs_CflSocioDsd) ? lrs_UflPrvDd : lrs_UflPrvHt;

                if (!s.BeforeAction && go_SBOForm.Mode != BoFormMode.fm_FIND_MODE)
                {
                    DataTable lo_DataTable = lo_CFLEvent.SelectedObjects;
                    if (lo_DataTable != null)
                        go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).SetValue(lrs_UflSocio, 0, lo_DataTable.GetValue(lrs_UflCardCode, 0));
                }
            }, lrs_EdtPrvDd, lrs_EdtPrvHt));

            itemevent.Add(new sapitemevent(BoEventTypes.et_COMBO_SELECT, s =>
            {
                if (!s.BeforeAction)
                {
                    if (s.ItemUID.Equals(lrs_CbxSeries))
                    {
                        string ls_serie = go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflSeries, 0).ToString();
                        string ls_NextNumber = Cls_QueryManager.Retorna(Cls_Query.get_NumeroSiguiente, 0, go_SBOForm.BusinessObject.Type, ls_serie).ToString();
                        go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).SetValue(lrs_UflDocNum, 0, ls_NextNumber);
                    }
                    else if (s.ItemUID.Equals(lrs_CbxBnkPais))  //Banco Codigo
                        Cls_Global.sb_comboLlenar(go_SBOForm.GetComboBox(lrs_CbxBnkCdgo), Cls_QueryManager.Retorna(Cls_Query.get_BancoCdgo, null, go_SBOForm.GetComboBox(lrs_CbxBnkPais).Value));
                    else if (s.ItemUID.Equals(lrs_CbxBnkCdgo))  //Banco Cuenta
                        Cls_Global.sb_comboLlenar(go_SBOForm.GetComboBox(lrs_CbxBnkCta), Cls_QueryManager.Retorna(Cls_Query.get_BancoCta, null, go_SBOForm.GetComboBox(lrs_CbxBnkCdgo).Value));
                    else if (s.ItemUID.Equals(lrs_CbxBnkCta))   //Banco Moneda
                    {
                        go_SBOForm.GetEditText(lrs_EdtBnkMnd).Value = Cls_QueryManager.Retorna(Cls_Query.get_BancoMnd, 0, go_SBOForm.GetComboBox(lrs_CbxBnkCdgo).Value, go_SBOForm.GetComboBox(lrs_CbxBnkCta).Selected.Description);
                        go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).SetValue(lrs_UflBnkMnd, 0, go_SBOForm.GetEditText(lrs_EdtBnkMnd).Value);
                    }

                }
            }, lrs_CbxSeries, lrs_CbxBnkPais, lrs_CbxBnkCdgo, lrs_CbxBnkCta));

            itemevent.Add(new sapitemevent(BoEventTypes.et_MATRIX_LINK_PRESSED, lrs_MtxBatDTR, s =>
            {
                if (s.BeforeAction)
                {
                    string ls_Tipo;
                    LinkedButton lo_Linked;

                    go_Matrix = go_SBOForm.GetControl(lrs_MtxBatDTR);
                    ls_Tipo = ((EditText)go_Matrix.GetCellSpecific(lrs_ClmMtxTpDoc, s.Row)).Value;
                    lo_Linked = go_Matrix.Columns.Item(lrs_ClmMtxDocEnt).ExtendedObject;

                    if (s.Row != -1)
                    {
                        if (ls_Tipo.Equals("Factura de Clientes") || ls_Tipo.Equals("Factura de Clientes"))
                            lo_Linked.LinkedObject = BoLinkedObject.lf_Invoice;
                        else if (ls_Tipo.Equals("Factura de Proveedor") || ls_Tipo.Equals("Factura de Proveedor"))
                            lo_Linked.LinkedObject = BoLinkedObject.lf_PurchaseInvoice;
                        else if (ls_Tipo.Equals("Fac. Anticipo de Proveedores") || ls_Tipo.Equals("Fac. Anticipo de Pro"))
                            lo_Linked.LinkedObject = (BoLinkedObject)204;
                        else if (ls_Tipo.Equals("Nota de Credito Proveedores") || ls_Tipo.Equals("Nota de Credito Prov"))
                            lo_Linked.LinkedObject = BoLinkedObject.lf_PurchaseInvoiceCreditMemo;
                    }
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_VALIDATE, s =>
            {
                double ldb_ImpTotal = 0.0;
                if (!s.BeforeAction) return;

                ldb_ImpTotal = double.Parse(go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflTotPag, 0));
                ldb_ImpTotal -= double.Parse(go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflMntPorte, 0));
                ldb_ImpTotal -= double.Parse(go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflMntCmsn, 0));
                go_SBOForm.DataSources.UserDataSources.Item(lrs_UdsImpTotal).Value = ldb_ImpTotal.ToString();

            }, lrs_EdtMntPorte, lrs_EdtMntCmns));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, s => Dispose()));
        }

        #region SBOIEventsFormData

        public void sb_EventFormData()
        {
            try
            {
                go_SBOForm = go_SBOFormEvent;
              
                string lcs_TipAsn = string.Empty;
                double ldb_ImpTotal = 0.0;
                go_SBOForm.GetItem(lrs_RbtDtr).Enabled = true;
                go_SBOForm.GetItem(lrs_RbtPrc).Enabled = true;
                go_SBOForm.GetItem(lrs_RbtFNg).Enabled = true;

                OptionEvnFormData = true;
                lcs_TipAsn = go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflTipoAsn, 0).Trim();

                if (lcs_TipAsn.Equals("DT"))
                    go_SBOForm.GetOptionButton(lrs_RbtDtr).Selected = true;
                else if (lcs_TipAsn.Equals("PR"))
                    go_SBOForm.GetOptionButton(lrs_RbtPrc).Selected = true;
                else if (lcs_TipAsn.Equals("FN"))
                    go_SBOForm.GetOptionButton(lrs_RbtFNg).Selected = true;

                OptionEvnFormData = false;
                go_SBOForm.GetItem(lrs_RbtDtr).Enabled = false;
                go_SBOForm.GetItem(lrs_RbtPrc).Enabled = false;
                go_SBOForm.GetItem(lrs_RbtFNg).Enabled = false;

                ldb_ImpTotal += double.Parse(go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflTotPag, 0));
                ldb_ImpTotal += double.Parse(go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflMntPorte, 0));
                ldb_ImpTotal += double.Parse(go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflMntCmsn, 0));
                go_SBOForm.DataSources.UserDataSources.Item(lrs_UdsImpTotal).Value = ldb_ImpTotal.ToString();
                go_SBOForm.Mode = BoFormMode.fm_OK_MODE;


                go_Matrix = go_SBOForm.GetControl(lrs_MtxBatDTR);
                if (go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflEstado, 0) == "C")
                {
                    go_SBOForm.Items.Item(lrs_MtxBatDTR).Enabled = false;
                    go_Matrix.Columns.Item(lrs_ClmMtxSelec).Visible = true;
                }
                else
                {
                    go_Matrix.Columns.Item(lrs_ClmMtxSelec).Visible = false;
                    go_SBOForm.Mode = BoFormMode.fm_UPDATE_MODE;
                }
            }
            catch (Exception ex) { go_SBOApplication.MessageBox(ex.Message); } //Muestra una ventana con el mensaje de Excepción
        }

        #endregion

        

    }
}
