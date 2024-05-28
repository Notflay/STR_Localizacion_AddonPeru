using SAPbobsCOM;
using SAPbouiCOM;
using STR_Localizacion.UTIL;
using System;
using System.Linq;
using System.Xml.Linq;

namespace STR_Localizacion.UI
{
    public partial class Cls_MedioPago : Cls_PropertiesControl
    {
        public Cls_MedioPago()
        {
            lc_NameClass = "Cls_MedioPago";
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            itemevent.Add(new sapitemevent(
                BoEventTypes.et_FORM_LOAD, string.Empty, s =>
                {
                    if (!s.BeforeAction) return;
                    go_SBOForm = go_SBOFormEvent;
                    string ls_NameTable = string.Empty;
                    switch (s.FormTypeEx)
                    {
                        case "146": ls_NameTable = "ORCT"; break;
                        case "196": ls_NameTable = "OVPM"; break;
                    }

                    try
                    {
                        if (s.FormTypeEx != "170") 
                        {
                            string ls_flcj = Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, ls_NameTable, "U_BPP_FlCj");

                            if (ls_flcj == "N" || ls_flcj == string.Empty)
                            {
                                go_Item = go_SBOForm.Items.Add("lbMPPG", BoFormItemTypes.it_STATIC);

                                go_Item.Left = go_SBOForm.Items.Item("116").Left;
                                go_Item.Top = go_SBOForm.Items.Item("48").Top + go_SBOForm.Items.Item("48").Height + 3;
                                go_Item.Width = 70;
                                go_Item.Description = "Medio de Pago";
                                go_Item.AffectsFormMode = false;
                                go_Item.LinkTo = "cbMPPG";
                                go_Static = go_Item.Specific;
                                go_Static.Caption = "Medio de Pago";

                                /*********************************************************************************************************/
                                go_Item = go_SBOForm.Items.Add("cbMPPG", BoFormItemTypes.it_COMBO_BOX);

                                go_Item.Left = go_SBOForm.Items.Item("116").Left + go_SBOForm.Items.Item("116").Width + 5;
                                go_Item.Top = go_SBOForm.Items.Item("48").Top + go_SBOForm.Items.Item("48").Height + 3;
                                go_Item.Width = 200;
                                go_Item.AffectsFormMode = false;
                                go_Item.DisplayDesc = true;
                                go_Item.Description = "Medio de Pago";

                                go_Combo = go_Item.Specific;
                                go_Combo.DataBind.SetBound(true, ls_NameTable, "U_BPP_MPPG");
                            }
                        }
                        
                    }
                    catch (Exception MsjExc)
                    {
                        go_SBOApplication.MessageBox(MsjExc.Message);
                    }
                }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, s => Dispose()));
        }

        #region Generar Asiento de Pago Cuenta
        public void sb_GenerarAsientoPago(BusinessObjectInfo boInfo)
        {
            //CompanyService cmpSrv = null;
            //SeriesService srsSrv = null;
            //SeriesParams srsPrms = null;
            //Documents slcComp = null;
            //StockTransfer slcTrsl = null;
            Recordset recordset = null; // Nuevo
            int ErrCode = 0;
            string ErrMsg = string.Empty;
            String _mensaje = String.Empty;
            try
            {
                if(Cls_Global.ReconciliacionActivo.Equals("Y") && Cls_Global.ReconciliacionCuenta != "")
                {
                    go_SBOForm = go_SBOFormEvent;

                    string Sucursal = string.Empty;
                    // ------ Validar SI Sociedad tiene configuracion de Sucursales
                    recordset = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    string query = $"SELECT \"MltpBrnchs\" FROM OADM";
                    Cls_Global.WriteToFile(query);
                    recordset.DoQuery(query);
                    Sucursal = recordset.Fields.Item(0).Value;

                    Payments oPagoRecibido = go_SBOCompany.GetBusinessObject(BoObjectTypes.oIncomingPayments);
                    string docEntry = go_SBOForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0);
                    
                    if (oPagoRecibido.GetByKey(int.Parse(docEntry)))
                    {
                        //oPagoRecibido.SaveToFile(@"C:\AFile\XML\pago.xml");

                        string queryPagoCuenta = $"SELECT \"PayNoDoc\",CASE WHEN \"DocCurr\" ='{oPagoRecibido.DocCurrency}' THEN \"NoDocSum\" ELSE \"NoDocSumFC\" END, \"TransId\" FROM ORCT WHERE  \"DocEntry\" = '{docEntry}'";
                        Cls_Global.WriteToFile(queryPagoCuenta);
                        recordset.DoQuery(queryPagoCuenta);
                        string PagoCuenta = recordset.Fields.Item(0).Value;
                        double PagoCuentaImporte = Convert.ToDouble(recordset.Fields.Item(1).Value);
                        int TransIdPago = Convert.ToInt32(recordset.Fields.Item(2).Value);
                        string reconAuto = oPagoRecibido.UserFields.Fields.Item("U_STR_RECO").Value.ToString().Trim();

                        if (PagoCuenta.Equals("Y") && reconAuto.Equals("Y"))
                        {
                            JournalEntries oJournal = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
                            oJournal.ReferenceDate = oPagoRecibido.DocDate;
                            oJournal.DueDate = oPagoRecibido.DueDate;
                            oJournal.TaxDate = oPagoRecibido.TaxDate;
                            oJournal.Reference = oPagoRecibido.CardName;
                            oJournal.Memo = oPagoRecibido.JournalRemarks;

                            if (oPagoRecibido.DocCurrency == Cls_Global.sb_ObtenerMonedaLocal())
                            {
                                oJournal.Lines.AccountCode = oPagoRecibido.ControlAccount;
                                oJournal.Lines.ShortName = oPagoRecibido.CardCode;
                                oJournal.Lines.Debit = PagoCuentaImporte;
                                oJournal.Lines.Credit = 0;
                                if (Sucursal.Equals("Y")) // Agregado 21/01/2022
                                    oJournal.Lines.BPLID = oPagoRecibido.BPLID;
                                oJournal.Lines.Add();

                                oJournal.Lines.AccountCode = Cls_Global.ReconciliacionCuenta;// "_SYS00000005998";
                                oJournal.Lines.Credit = PagoCuentaImporte;
                                oJournal.Lines.Debit = 0;
                                if (Sucursal.Equals("Y")) // Agregado 21/01/2022
                                    oJournal.Lines.BPLID = oPagoRecibido.BPLID;
                                oJournal.Lines.Add();
                            }                          

                            if (oJournal.Add() != 0)
                            {
                                go_SBOCompany.GetLastError(out ErrCode, out ErrMsg);
                                if (Cls_Global.go_SBOCompany.InTransaction) { Cls_Global.go_SBOCompany.EndTransaction(BoWfTransOpt.wf_RollBack); }
                                throw new Exception(ErrCode.ToString() + "-" + ErrMsg);
                            }
                            else
                            {
                                var TransIdAsiento = go_SBOCompany.GetNewObjectKey();
                                Cls_Global.sb_msjStatusBarSAP("El proceso de generación de asiento finalizado con éxito: ID Transacción  " + TransIdAsiento, BoStatusBarMessageType.smt_Success, go_SBOApplication);

                                InternalReconciliationsService service = go_SBOCompany.GetCompanyService().GetBusinessService(ServiceTypes.InternalReconciliationsService);
                                InternalReconciliationOpenTrans openTrans = service.GetDataInterface(InternalReconciliationsServiceDataInterfaces.irsInternalReconciliationOpenTrans);
                                InternalReconciliationParams reconParams = service.GetDataInterface(InternalReconciliationsServiceDataInterfaces.irsInternalReconciliationParams);

                                try
                                {
                                    openTrans.ReconDate = DateTime.Now;
                                    openTrans.CardOrAccount = CardOrAccountEnum.coaCard;
                                    openTrans.InternalReconciliationOpenTransRows.Add();
                                    openTrans.InternalReconciliationOpenTransRows.Item(0).Selected = BoYesNoEnum.tYES;
                                    openTrans.InternalReconciliationOpenTransRows.Item(0).TransId = TransIdPago;
                                    openTrans.InternalReconciliationOpenTransRows.Item(0).TransRowId = 1;
                                    openTrans.InternalReconciliationOpenTransRows.Item(0).ReconcileAmount = PagoCuentaImporte;

                                    openTrans.InternalReconciliationOpenTransRows.Add();
                                    openTrans.InternalReconciliationOpenTransRows.Item(1).Selected = BoYesNoEnum.tYES;
                                    openTrans.InternalReconciliationOpenTransRows.Item(1).TransId = int.Parse(TransIdAsiento);
                                    openTrans.InternalReconciliationOpenTransRows.Item(1).TransRowId = 0;
                                    openTrans.InternalReconciliationOpenTransRows.Item(1).ReconcileAmount = PagoCuentaImporte;

                                    reconParams = service.Add(openTrans);
                                    Cls_Global.sb_msjStatusBarSAP("El proceso de reconciliación finalizado con éxito: Con Asiento ID Transacción  " + TransIdAsiento, BoStatusBarMessageType.smt_Success, go_SBOApplication);
                                }
                                catch (Exception ex)
                                {
                                    go_SBOCompany.GetLastError(out ErrCode, out ErrMsg);
                                    if (Cls_Global.go_SBOCompany.InTransaction) { Cls_Global.go_SBOCompany.EndTransaction(BoWfTransOpt.wf_RollBack); }
                                    throw new InvalidOperationException(ErrCode.ToString() + "-" + ErrMsg == "" ? ex.Message : ErrMsg);
                                }
                            }
                        }

                    }
                }
                
            }
            catch (Exception ex)
            {
                //go_SBOApplication.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Short); 
                //if (!(ex is InvalidOperationException || ex is ArgumentException))
                //{
                //    ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                //    ExceptionPrepared.SaveInLog(false);
                //}
                //if (go_ProgressBar != null) { go_ProgressBar.Stop(); go_ProgressBar = null; }

                throw;
            } //Muestra una ventana con el mensaje de Excepción
            //return _mensaje;
        }
        #endregion
    }
}