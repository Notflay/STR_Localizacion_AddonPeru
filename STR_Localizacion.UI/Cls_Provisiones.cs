using SAPbobsCOM;
using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;

namespace STR_Localizacion.UI
{
    internal class Cls_Provisiones : Cls_PropertiesControl
    {
        private string ls_cuenta = string.Empty;

        public Cls_Provisiones()
        {
            lc_NameClass = "Cls_Provisiones";
            ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
            //InitializeEvents();
        }

        private void InitializeEvents()
        {
        }

        public void sb_generarAsientoProvision()
        {
            lc_NameMethod = "sb_generarAsientoProvision"; //Se asigna el nombre del método para la identificación del mismo
            try
            {
                //Variables del metodo
                Documents lo_Doc;

                go_SBOForm = go_SBOFormEvent;

                lo_Doc = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);
                lo_Doc.GetByKey(int.Parse(go_SBOForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0)));

                if (lo_Doc.Cancelled == BoYesNoEnum.tNO)
                    CrearAsientoProvision(lo_Doc);
                else
                    ExtornarAsientoProvision(lo_Doc);
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                ExceptionPrepared.SaveInLog(false);
            } //Método para el manejo de las operaciones de Log
        }

        private void ExtornarAsientoProvision(Documents lo_Doc)
        {
            try
            {
                JournalEntries oJournalEntry = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
                JournalEntries oReversedJournalEntry = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);

                int asientoProvision = int.TryParse(lo_Doc.UserFields.Fields.Item("U_ST_EP_ASPR").Value.ToString(), out int key) ? key : 0;
                if (asientoProvision != 0)
                {
                    oJournalEntry.GetByKey(asientoProvision);

                    //HEADER
                    oReversedJournalEntry.ReferenceDate = lo_Doc.DocDate;
                    oReversedJournalEntry.TaxDate = lo_Doc.TaxDate;
                    oReversedJournalEntry.Reference = oJournalEntry.Reference; //lo_JdtDoc.Reference;
                    oReversedJournalEntry.Reference2 = oJournalEntry.Reference2;
                    oReversedJournalEntry.Reference3 = oJournalEntry.Reference3;
                    oReversedJournalEntry.Memo = oJournalEntry.Memo;

                    oReversedJournalEntry.TransactionCode = "PRV";
                    oReversedJournalEntry.UserFields.Fields.Item("U_BPP_DocKeyDest").Value = lo_Doc.DocEntry.ToString();
                    oReversedJournalEntry.UserFields.Fields.Item("U_BPP_CtaTdoc").Value = lo_Doc.DocObjectCodeEx.ToString();
                    oReversedJournalEntry.UserFields.Fields.Item("U_BPP_SubTDoc").Value = lo_Doc.DocumentSubType.ToString();

                    //DETAILS
                    for (int i = 0; i < oJournalEntry.Lines.Count; i++)
                    {
                        oJournalEntry.Lines.SetCurrentLine(i);
                        oReversedJournalEntry.Lines.SetCurrentLine(i);

                        oReversedJournalEntry.Lines.AccountCode = oJournalEntry.Lines.AccountCode;

                        if (oJournalEntry.Lines.Credit != 0)
                            oReversedJournalEntry.Lines.Debit = oJournalEntry.Lines.Credit;
                        else
                            if (oJournalEntry.Lines.Debit == 0)
                            oReversedJournalEntry.Lines.FCDebit = oJournalEntry.Lines.FCCredit;

                        if (oJournalEntry.Lines.Debit != 0)
                            oReversedJournalEntry.Lines.Credit = oJournalEntry.Lines.Debit;
                        else
                             if (oJournalEntry.Lines.Credit == 0)
                            oReversedJournalEntry.Lines.FCCredit = oJournalEntry.Lines.FCDebit;

                        oReversedJournalEntry.Lines.UserFields.Fields.Item("U_INFOPE01").Value = oJournalEntry.Lines.UserFields.Fields.Item("U_INFOPE01").Value;
                        oReversedJournalEntry.Lines.CostingCode = oJournalEntry.Lines.CostingCode;
                        oReversedJournalEntry.Lines.CostingCode2 = oJournalEntry.Lines.CostingCode2;
                        oReversedJournalEntry.Lines.CostingCode3 = oJournalEntry.Lines.CostingCode3;
                        oReversedJournalEntry.Lines.CostingCode4 = oJournalEntry.Lines.CostingCode4;
                        oReversedJournalEntry.Lines.CostingCode5 = oJournalEntry.Lines.CostingCode5;
                        oReversedJournalEntry.Lines.ProjectCode = oJournalEntry.Lines.ProjectCode;

                        oReversedJournalEntry.Lines.Add();
                    }

                    if (oReversedJournalEntry.Add() == 0)
                    {
                        Cls_Global.sb_msjStatusBarSAP("Asiento de provisión extornado creado satisfactoriamente. ID: " + go_SBOCompany.GetNewObjectKey(), BoStatusBarMessageType.smt_Success, go_SBOApplication);
                        ActualizarCampoAsientoProvision(lo_Doc.DocEntry, Convert.ToInt32(go_SBOCompany.GetNewObjectKey()), "U_ST_EXTASPR");
                        //lo_Doc.UserFields.Fields.Item("U_ST_EXTASPR").Value = Convert.ToInt32(go_SBOCompany.GetNewObjectKey());
                        //lo_Doc.Update();
                    }
                    else
                    {
                        go_SBOCompany.GetLastError(out int errNum, out string errMsg);
                        go_SBOApplication.MessageBox(errNum.ToString() + " - " + errMsg);

                        return;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CrearAsientoProvision(Documents lo_Doc)
        {
            try
            {
                JournalEntries lo_Jdt;

                string ls_Result;
                string errMsg = string.Empty;
                int errNum = 0;

                if (HasServiceItems(lo_Doc))
                {
                    lo_Jdt = ObtenerAsientoProvisiones(lo_Doc);
                    string xml = lo_Jdt.GetAsXML();
                    ls_Result = lo_Jdt.Add().ToString();

                    if (ls_Result != "0") //Si el resultado indica que la operacion no fue exitosa, muestra un mensaje
                    {
                        go_SBOCompany.GetLastError(out errNum, out errMsg);
                        go_SBOApplication.MessageBox(errNum.ToString() + " - " + errMsg);
                    }
                    else
                    {
                        Cls_Global.sb_msjStatusBarSAP("Asiento de provisión creado satisfactoriamente. ID: " + go_SBOCompany.GetNewObjectKey(), BoStatusBarMessageType.smt_Success, go_SBOApplication);
                        //lo_Doc.UserFields.Fields.Item("U_ST_EP_ASPR").Value = Convert.ToInt32(go_SBOCompany.GetNewObjectKey());
                        //lo_Doc.Update();
                        ActualizarCampoAsientoProvision(lo_Doc.DocEntry, Convert.ToInt32(go_SBOCompany.GetNewObjectKey()), "U_ST_EP_ASPR");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ActualizarCampoAsientoProvision(int docEntry, int transId, string campo)
        {
            Recordset recordset = go_SBOCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string query = $"UPDATE OPDN SET \"{campo}\" = {transId} WHERE \"DocEntry\" = {docEntry}";
            recordset.DoQuery(query);
        }

        private bool HasServiceItems(Documents lo_Doc)
        {
            try
            {
                SAPbobsCOM.Items oItem = go_SBOCompany.GetBusinessObject(BoObjectTypes.oItems);

                for (int i = 0; i < lo_Doc.Lines.Count; i++)
                {
                    lo_Doc.Lines.SetCurrentLine(i);
                    oItem.GetByKey(lo_Doc.Lines.ItemCode);
                    if (IsServiceItem(oItem))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void sb_generarAsientoAjustePorTipoDeCambio()
        {
            try
            {
                lc_NameMethod = "sb_generarAsientoAjustePorTipoDeCambio"; //Se asigna el nombre del método para la identificación del mismo
                go_SBOForm = go_SBOFormEvent;

                Documents oFactura = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);
                Documents oEntradaMercancias = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);

                oFactura.GetByKey(Convert.ToInt32(go_SBOForm.DataSources.DBDataSources.Item("OPCH").GetValue("DocEntry", 0)));
                SAPbobsCOM.Items oItem = go_SBOCompany.GetBusinessObject(BoObjectTypes.oItems);
                bool createJournalEntry = false;

                if (oFactura.DocCurrency.Equals("USD"))
                {
                    for (int i = 0; i < oFactura.Lines.Count; i++)
                    {
                        oFactura.Lines.SetCurrentLine(i);
                        oItem.GetByKey(oFactura.Lines.ItemCode);
                        oEntradaMercancias.GetByKey(oFactura.Lines.BaseEntry);

                        if (IsServiceItem(oItem) && oFactura.DocRate != oEntradaMercancias.DocRate)
                        {
                            createJournalEntry = true;
                            break;
                            //int journalEntry = CreateRateAdjustmentJournalEntry(oFactura, i, oEntradaMercancias);
                            //oFactura.Lines.UserFields.Fields.Item("U_ST_EP_AATC").Value = journalEntry.ToString();
                        }
                    }
                }

                if (createJournalEntry)
                {
                    JournalEntries lo_Jdt = ObtenerAsientoAjustePorTipoDeCambio(oFactura);

                    string ls_Result = lo_Jdt.Add().ToString();

                    string errMsg = string.Empty;
                    int errNum = 0;

                    if (ls_Result != "0") //Si el resultado indica que la operacion no fue exitosa, muestra un mensaje
                    {
                        go_SBOCompany.GetLastError(out errNum, out errMsg);
                        go_SBOApplication.MessageBox(errNum.ToString() + " - " + errMsg);

                        return;
                    }
                    else
                    {
                        Cls_Global.sb_msjStatusBarSAP("Asiento de provisión creado satisfactoriamente. ID: " + go_SBOCompany.GetNewObjectKey(), BoStatusBarMessageType.smt_Success, go_SBOApplication);
                        oFactura.UserFields.Fields.Item("U_ST_EP_AATC").Value = Convert.ToInt32(go_SBOCompany.GetNewObjectKey());
                        oFactura.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                ExceptionPrepared.SaveInLog(false);
            }
        }

        private JournalEntries ObtenerAsientoAjustePorTipoDeCambio(Documents factura)
        {
            JournalEntries oEntry = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
            Documents entradaMercancias = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);

            SAPbobsCOM.Items oItem = go_SBOCompany.GetBusinessObject(BoObjectTypes.oItems);

            //HEADER
            SetearValoresCabecera(ref oEntry, ref factura);

            //DETAIL
            for (int i = 0; i < factura.Lines.Count; i++)
            {
                factura.Lines.SetCurrentLine(i);
                oItem.GetByKey(factura.Lines.ItemCode);
                ItemWarehouseInfo itmWhsInfo = oItem.WhsInfo;
                string ctaGasto = string.Empty;

                entradaMercancias.GetByKey(factura.Lines.BaseEntry);
                entradaMercancias.Lines.SetCurrentLine(factura.Lines.BaseLine);

                if (IsServiceItem(oItem) && factura.DocRate != entradaMercancias.DocRate)
                {
                    double amount;
                    ctaGasto = itmWhsInfo.ExpensesAccount;

                    if (factura.DocRate > entradaMercancias.DocRate)
                    {
                        amount = (factura.Lines.LineTotal) - (entradaMercancias.Lines.LineTotal);

                        if (ExistsAccountInEntry(ctaGasto, ref oEntry, out int line))
                            UpdateLine(ref oEntry, line, amount, "C");
                        else
                            AddLine(ref oEntry, amount, ctaGasto, "C", entradaMercancias.CardCode, entradaMercancias.Lines);

                        if (ExistsAccountInEntry(Cls_Global.APDifGain, ref oEntry, out int line2))
                            UpdateLine(ref oEntry, line2, amount, "D");
                        else
                            AddLine(ref oEntry, amount, Cls_Global.APDifGain, "D", entradaMercancias.CardCode, entradaMercancias.Lines);
                    }
                    else
                    {
                        amount = (entradaMercancias.Lines.LineTotal) - (factura.Lines.LineTotal);

                        if (ExistsAccountInEntry(ctaGasto, ref oEntry, out int line))
                            UpdateLine(ref oEntry, line, amount, "D");
                        else
                            AddLine(ref oEntry, amount, ctaGasto, "D", entradaMercancias.CardCode, entradaMercancias.Lines);

                        if (ExistsAccountInEntry(Cls_Global.APDifLoss, ref oEntry, out int line2))
                            UpdateLine(ref oEntry, line2, amount, "C");
                        else
                            AddLine(ref oEntry, amount, Cls_Global.APDifLoss, "C", entradaMercancias.CardCode, entradaMercancias.Lines);
                    }
                }
            }

            return oEntry;
        }

        private int CreateRateAdjustmentJournalEntry(Documents factura, int linea, Documents entradaMercancias)
        {
            try
            {
                JournalEntries oJe = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
                SetearValoresCabecera(ref oJe, ref factura);

                SAPbobsCOM.Items item = go_SBOCompany.GetBusinessObject(BoObjectTypes.oItems);
                factura.Lines.SetCurrentLine(linea);

                item.GetByKey(factura.Lines.ItemCode);
                ItemWarehouseInfo itmWhsInfo = item.WhsInfo;

                entradaMercancias.Lines.SetCurrentLine(factura.Lines.BaseLine);

                if (factura.DocRate > entradaMercancias.DocRate)
                {
                    double amount = (factura.Lines.LineTotal) - (entradaMercancias.Lines.LineTotal);

                    oJe.Lines.AccountCode = itmWhsInfo.TransferAccount;
                    oJe.Lines.Credit = amount;
                    oJe.Lines.Add();

                    oJe.Lines.AccountCode = Cls_Global.APDifGain;
                    oJe.Lines.Debit = amount;
                    oJe.Lines.Add();
                }
                else
                {
                    double amount = (entradaMercancias.Lines.LineTotal) - (factura.Lines.LineTotal);
                    oJe.Lines.AccountCode = itmWhsInfo.TransferAccount;
                    oJe.Lines.Debit = amount;
                    oJe.Lines.Add();

                    oJe.Lines.AccountCode = Cls_Global.APDifLoss;
                    oJe.Lines.Credit = amount;
                    oJe.Lines.Add();
                }

                if (oJe.Add() == 0)
                {
                    return int.Parse(go_SBOCompany.GetNewObjectKey());
                }
                else
                    throw new Exception(go_SBOCompany.GetLastErrorDescription());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JournalEntries ObtenerAsientoProvisiones(Documents EntradaMercancia)
        {
            try
            {
                JournalEntries oAsientoProvision = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);

                SetearValoresCabecera(ref oAsientoProvision, ref EntradaMercancia);
                SetearValoresDetalle(ref oAsientoProvision, ref EntradaMercancia);

                return oAsientoProvision;
            }
            catch (Exception e)
            {
                string msj = e.Message;
                throw;
            }
        }

        private void SetearValoresCabecera(ref JournalEntries oAsientoProvision, ref Documents document)
        {
            try
            {
                oAsientoProvision.ReferenceDate = document.DocDate;
                oAsientoProvision.TaxDate = document.TaxDate;
                oAsientoProvision.Reference = document.DocNum.ToString(); //lo_JdtDoc.Reference;
                oAsientoProvision.Reference2 = document.UserFields.Fields.Item("U_BPP_MDTD").Value + "-" + document.UserFields.Fields.Item("U_BPP_MDSD").Value + "-" + document.UserFields.Fields.Item("U_BPP_MDCD").Value;

                oAsientoProvision.Reference3 = document.UserFields.Fields.Item("U_BPP_OC").Value;
                //entradaMercancia.WithholdingTaxData.WTCode;
                string memo = "Provisiones - " + document.CardName;

                if (memo.Length >= 50)
                {
                    memo = memo.Substring(0, 49);
                }

                oAsientoProvision.Memo = memo;
                //+ entradaMercancia.UserFields.Fields.Item("U_BPP_MDTD").Value + "-" + entradaMercancia.UserFields.Fields.Item("U_BPP_MDSD").Value + "-" + entradaMercancia.UserFields.Fields.Item("U_BPP_MDCD").Value;

                oAsientoProvision.TransactionCode = "PRV";
                oAsientoProvision.UserFields.Fields.Item("U_BPP_DocKeyDest").Value = document.DocEntry.ToString();
                oAsientoProvision.UserFields.Fields.Item("U_BPP_CtaTdoc").Value = document.DocObjectCodeEx.ToString();
                oAsientoProvision.UserFields.Fields.Item("U_BPP_SubTDoc").Value = document.DocumentSubType.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetearValoresDetalle(ref JournalEntries oAsientoProvision, ref Documents entradaMercancia)
        {
            for (int i = 0; i < entradaMercancia.Lines.Count; i++)
            {
                entradaMercancia.Lines.SetCurrentLine(i);
                SAPbobsCOM.Items oItem = go_SBOCompany.GetBusinessObject(BoObjectTypes.oItems);
                oItem.GetByKey(entradaMercancia.Lines.ItemCode);

                if (IsServiceItem(oItem))
                {
                    double amount = entradaMercancia.Lines.LineTotal;
                    ItemWarehouseInfo whsInfo = oItem.WhsInfo;
                    string expenseAcct = whsInfo.TransferAccount;
                    string ctaCU = whsInfo.ExpensesAccount;

                    if (ExistsAccountInEntry(expenseAcct, ref oAsientoProvision, out int line))
                        UpdateLine(ref oAsientoProvision, line, amount, "C");
                    else
                        AddLine(ref oAsientoProvision, amount, expenseAcct, "C", entradaMercancia.CardCode, entradaMercancia.Lines);

                    if (ExistsAccountInEntry(ctaCU, ref oAsientoProvision, out int line2))
                        UpdateLine(ref oAsientoProvision, line2, amount, "D");
                    else
                        AddLine(ref oAsientoProvision, amount, ctaCU, "D", entradaMercancia.CardCode, entradaMercancia.Lines);
                }
            }
        }

        private void UpdateLine(ref JournalEntries oAsientoProvision, int line, double amount, string lineType)
        {
            oAsientoProvision.Lines.SetCurrentLine(line);
            switch (lineType)
            {
                case "D": oAsientoProvision.Lines.Debit += amount; break;
                case "C": oAsientoProvision.Lines.Credit += amount; break;
            }
        }

        private void AddLine(ref JournalEntries oAsientoProvision, double amount, string acct, string lineType, string infoPE01, Document_Lines entradaLinea)
        {
            oAsientoProvision.Lines.AccountCode = acct;
            CompanyService oCmpSrv = go_SBOCompany.GetCompanyService();
            AdminInfo oAdmInfo = oCmpSrv.GetAdminInfo();

            switch (lineType)
            {
                case "D":
                    oAsientoProvision.Lines.Debit = amount;
                    //if (entradaLinea.Currency == oAdmInfo.LocalCurrency)

                    //else
                    //{
                    //    oAsientoProvision.Lines.FCCurrency = entradaLinea.Currency;
                    //    oAsientoProvision.Lines.FCDebit = amount;
                    //}

                    break;

                case "C":
                    oAsientoProvision.Lines.Credit = amount;

                    //if (entradaLinea.Currency == oAdmInfo.LocalCurrency)

                    //else
                    //{
                    //    oAsientoProvision.Lines.FCCurrency = entradaLinea.Currency;
                    //    oAsientoProvision.Lines.FCCredit = amount;
                    //}

                    break;
            }

            oAsientoProvision.Lines.UserFields.Fields.Item("U_INFOPE01").Value = infoPE01;
            oAsientoProvision.Lines.CostingCode = entradaLinea.CostingCode;
            oAsientoProvision.Lines.CostingCode2 = entradaLinea.CostingCode2;
            oAsientoProvision.Lines.CostingCode3 = entradaLinea.CostingCode3;
            oAsientoProvision.Lines.CostingCode4 = entradaLinea.CostingCode4;
            oAsientoProvision.Lines.CostingCode5 = entradaLinea.CostingCode5;
            oAsientoProvision.Lines.ProjectCode = entradaLinea.ProjectCode;
            oAsientoProvision.Lines.AdditionalReference = oAsientoProvision.Reference3;
            oAsientoProvision.Lines.Add();
        }

        private bool ExistsAccountInEntry(string acct, ref JournalEntries oAsientoProvision, out int line)
        {
            try
            {
                for (line = 0; line < oAsientoProvision.Lines.Count; line++)
                {
                    oAsientoProvision.Lines.SetCurrentLine(line);
                    if (oAsientoProvision.Lines.AccountCode == acct)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }

        private bool IsServiceItem(SAPbobsCOM.Items Item)
        {
            try
            {
                return Item.InventoryItem == BoYesNoEnum.tNO && Item.UserFields.Fields.Item("U_GEN_PROV").Value == "Y";
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}