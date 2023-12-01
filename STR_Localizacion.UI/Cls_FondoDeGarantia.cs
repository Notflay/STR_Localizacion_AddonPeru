using SAPbobsCOM;
using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;

namespace STR_Localizacion.UI
{
    public class Cls_FondoDeGarantia : Cls_PropertiesControl
    {
        public Documents FacturaProveedores { get; set; }

        public Cls_FondoDeGarantia()
        {
            lc_NameClass = "Cls_FondoDeGarantia";
            ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
            CargarEventos();
        }

        private void CargarEventos()
        {
            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_LOAD, string.Empty, e => CrearControles(e)));
            menuevent.Add(new sapmenuevent(e => CrearControles(e), "1282", "2561"));
            itemevent.Add(new sapitemevent(BoEventTypes.et_CHOOSE_FROM_LIST, "etCtaFG", e => EscogerCuentaFondoDeGarantia(e)));
        }

        private void EscogerCuentaFondoDeGarantia(ItemEvent e)
        {
            go_SBOForm = go_SBOFormEvent;
            SAPbouiCOM.ChooseFromList lo_choosefromlist;
            IChooseFromListEvent lo_cflEvent;

            if (e.BeforeAction)
            {
                //Declara condiciones
                Conditions lo_Conditions;
                Condition lo_Condition;
                Conditions lo_emptyCon = new Conditions();
                lo_cflEvent = (IChooseFromListEvent)e;
                lo_choosefromlist = go_SBOForm.ChooseFromLists.Item(lo_cflEvent.ChooseFromListUID);
                lo_choosefromlist.SetConditions(lo_emptyCon);
                //Asigna los valores a las propiedades de la condición
                lo_Conditions = lo_choosefromlist.GetConditions();
                lo_Condition = lo_Conditions.Add();
                lo_Condition.Alias = "LocManTran";
                lo_Condition.Operation = BoConditionOperation.co_EQUAL;
                lo_Condition.CondVal = "Y";
                lo_choosefromlist.SetConditions(lo_Conditions);
            }
            else
            {
                //Recupera los valores del po_ItemEvent
                DataTable lo_datatable;
                lo_cflEvent = (IChooseFromListEvent)e;
                lo_choosefromlist = go_SBOForm.ChooseFromLists.Item(lo_cflEvent.ChooseFromListUID);
                lo_datatable = lo_cflEvent.SelectedObjects; //Ingresa los valores de la lista en el DataTable
                if (lo_datatable == null) throw new InvalidOperationException();

                try { go_SBOForm.GetEditText("etCtaFG").Value = lo_datatable.GetValue("FormatCode", 0); } catch (Exception) { }
                go_SBOForm.Select();
                go_SBOForm.Refresh();
                go_SBOForm.Update();
            }
        }

        private void CrearControles(dynamic e)
        {
            if (!e.BeforeAction)
            {
                try
                {
                    go_SBOForm = go_SBOFormEvent;
                    go_SBOForm.Freeze(true);
                    if (go_SBOForm.ItemExists("lblCtaFG")) return;

                    go_Item = go_SBOForm.Items.Add("lblCtaFG", BoFormItemTypes.it_STATIC);

                    if (!go_SBOForm.ItemExists("stAsDtr2")) return; // Agregado 25012022
                    go_Item.Left = go_SBOForm.Items.Item("stAsDtr2").Left;
                    go_Item.Top = go_SBOForm.Items.Item("stAsDtr2").Top + 15;
                    go_Item.Width = go_SBOForm.Items.Item("stAsDtr2").Width - 10;
                    go_Item.Specific.Caption = "Cta. asoc. fondo de garantía";
                    go_Item.LinkTo = "etCtaFG";
                    go_Item.FromPane = 10;
                    go_Item.ToPane = 10;

                    go_Item = go_SBOForm.Items.Add("lnkCtaFG", BoFormItemTypes.it_LINKED_BUTTON);
                    go_LinkButton = go_Item.Specific;
                    go_LinkButton.LinkedObjectType = "1";
                    go_LinkButton.LinkedObject = BoLinkedObject.lf_GLAccounts;
                    go_LinkButton.Item.LinkTo = "etCtaFG";
                    go_Item.FromPane = 10;
                    go_Item.ToPane = 10;
                    go_Item.Left = go_SBOForm.Items.Item("lblCtaFG").Left + go_SBOForm.Items.Item("lblCtaFG").Width + 1;
                    go_Item.Top = go_SBOForm.Items.Item("lblCtaFG").Top;

                    go_Item = go_SBOForm.Items.Add("etCtaFG", BoFormItemTypes.it_EDIT);
                    go_Item.Left = go_SBOForm.Items.Item("txtCuenta").Left;
                    go_Item.Top = go_SBOForm.Items.Item("txtCuenta").Top + 15;
                    go_Item.Width = go_SBOForm.Items.Item("txtCuenta").Width;
                    go_Item.FromPane = 10;
                    go_Item.ToPane = 10;

                    go_Edit = go_Item.Specific;
                    ChooseFromListCreationParams cflCreationParams;
                    cflCreationParams = go_SBOApplication.CreateObject(BoCreatableObjectType.cot_ChooseFromListCreationParams);
                    cflCreationParams.MultiSelection = false;
                    cflCreationParams.UniqueID = "cflCtaG";
                    cflCreationParams.ObjectType = "1";
                    ChooseFromListCollection cflCollection;
                    cflCollection = go_SBOForm.ChooseFromLists;
                    cflCollection.Add(cflCreationParams);

                    go_Edit.DataBind.SetBound(true, "OCRD", "U_STR_CTA_FG");

                    go_Edit.ChooseFromListUID = "cflCtaG";
                    go_Edit.ChooseFromListAlias = "FormatCode";
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
        }

        internal void GenerarAsientoRetencionDeGarantia()
        {
            lc_NameMethod = "GenerarAsientoRetencionDeGarantia"; //Se asigna el nombre del método para la identificación del mismo
            go_SBOForm = go_SBOFormEvent;

            FacturaProveedores = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);
            string docEntry = go_SBOForm.DataSources.DBDataSources.Item("OPCH").GetValue("DocEntry", 0);

            if (FacturaProveedores.GetByKey(Convert.ToInt32(docEntry)))
            {
                if (FacturaProveedores.Cancelled == BoYesNoEnum.tNO)
                {
                    if (DebeCrearAsientoRetencionDeGarantia())
                    {
                        JournalEntries asientoRetencionDeGarantia = GetEstructuraAsiento();
                        if (asientoRetencionDeGarantia.Add() == 0)
                            ActualizarReferenciaAlAsiento(go_SBOCompany.GetNewObjectKey());
                        else
                        {
                            go_SBOApplication.MessageBox("Error al crear asiento de provisión IGV No Domiciliado. Error: " + go_SBOCompany.GetLastErrorDescription());
                            throw new Exception("Error al crear asiento de provisión IGV No Domiciliado. Error: " + go_SBOCompany.GetLastErrorDescription());
                        }
                    }
                }
                else
                {
                    string transID = ExtornarAsiento();
                    ActualizarReferenciaAlAsiento(transID, "EX");
                }
            }
            else
                throw new Exception("Internal Error. No se ha podido obtener la factura registrada.");
        }

        private string ExtornarAsiento()
        {
            JournalEntries asientoRetencionGarantia = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
            JournalEntries asientoExtornado = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);

            SBObob bridge = go_SBOCompany.GetBusinessObject(BoObjectTypes.BoBridge);
            string monedaLocal = bridge.GetLocalCurrency().Fields.Item(0).Value;

            if (asientoRetencionGarantia.GetByKey((int)FacturaProveedores.UserFields.Fields.Item("U_STR_ASRG").Value))
            {
                asientoExtornado.Reference2 = FacturaProveedores.NumAtCard;

                if (FacturaProveedores.DocCurrency.Equals(monedaLocal))
                {
                    for (int i = 0; i < asientoRetencionGarantia.Lines.Count; i++)
                    {
                        asientoRetencionGarantia.Lines.SetCurrentLine(i);
                        asientoExtornado.Lines.SetCurrentLine(i);

                        asientoExtornado.Lines.AccountCode = asientoRetencionGarantia.Lines.AccountCode;

                        if (asientoRetencionGarantia.Lines.Debit > 0)
                            asientoExtornado.Lines.Credit = asientoRetencionGarantia.Lines.Debit;

                        if (asientoRetencionGarantia.Lines.Credit > 0)
                        {
                            asientoExtornado.Lines.Debit = asientoRetencionGarantia.Lines.Credit;
                            asientoExtornado.Lines.ShortName = asientoRetencionGarantia.Lines.ShortName;
                        }

                        asientoExtornado.Lines.Add();
                    }
                }
                else
                {
                    for (int i = 0; i < asientoRetencionGarantia.Lines.Count; i++)
                    {
                        asientoRetencionGarantia.Lines.SetCurrentLine(i);
                        asientoExtornado.Lines.SetCurrentLine(i);

                        asientoExtornado.Lines.AccountCode = asientoRetencionGarantia.Lines.AccountCode;

                        if (asientoRetencionGarantia.Lines.FCDebit > 0)
                        {
                            asientoExtornado.Lines.Credit = asientoRetencionGarantia.Lines.Debit;
                            asientoExtornado.Lines.FCCredit = asientoRetencionGarantia.Lines.FCDebit;
                            asientoExtornado.Lines.FCCurrency = asientoRetencionGarantia.Lines.FCCurrency;
                        }

                        if (asientoRetencionGarantia.Lines.FCCredit > 0)
                        {
                            asientoExtornado.Lines.Debit = asientoRetencionGarantia.Lines.Credit;
                            asientoExtornado.Lines.FCDebit = asientoRetencionGarantia.Lines.FCCredit;
                            asientoExtornado.Lines.FCCurrency = asientoRetencionGarantia.Lines.FCCurrency;
                            asientoExtornado.Lines.ShortName = asientoRetencionGarantia.Lines.ShortName;
                        }

                        asientoExtornado.Lines.Add();
                    }
                }

                if (asientoExtornado.Add() == 0)
                    return go_SBOCompany.GetNewObjectKey();
                else
                    throw new Exception(go_SBOCompany.GetLastErrorDescription());
            }
            else
            {
                go_SBOApplication.MessageBox("No se ha generado el asiento de retencion de garantía y no se puede revertirlo");
                throw new Exception("No se ha generado el asiento de retencion de garantía y no se puede revertirlo");
            }
        }

        private void ActualizarReferenciaAlAsiento(string transID, string tipo = "RG")
        {
            string campo = string.Empty;

            switch (tipo)
            {
                case "RG": campo = "U_STR_ASRG"; break;
                case "EX":
                    campo = "U_STR_EX_ASRG";
                    FacturaProveedores.Lines.SetCurrentLine(0);
                    FacturaProveedores.GetByKey(FacturaProveedores.Lines.BaseEntry);

                    break;
            }

            FacturaProveedores.UserFields.Fields.Item(campo).Value = Convert.ToInt32(transID);
            if (FacturaProveedores.Update() != 0)
                throw new Exception(go_SBOCompany.GetLastErrorDescription());
        }

        private JournalEntries GetEstructuraAsiento()
        {
            try
            {
                BusinessPartners oSocio = go_SBOCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);
                oSocio.GetByKey(FacturaProveedores.CardCode);

                string memo = "Fondo de Garantía - " + oSocio.CardName;

                JournalEntries oAsiento = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
                oAsiento.TransactionCode = "FG";
                oAsiento.Memo = memo.Length > 50 ? memo.Substring(0, 49) : memo;
                oAsiento.Reference2 = FacturaProveedores.NumAtCard;
                oAsiento.Reference3 = FacturaProveedores.UserFields.Fields.Item("U_CL_AGREELINK").Value.ToString();

                SBObob bridge = go_SBOCompany.GetBusinessObject(BoObjectTypes.BoBridge);
                string monedaLocal = bridge.GetLocalCurrency().Fields.Item(0).Value;
                string formatoCuentaCredito = oSocio.UserFields.Fields.Item("U_STR_CTA_FG").Value;
                string cuentaCredito = Cls_QueryManager.Retorna(Cls_Query.get_CodigoFormato, "AcctCode", formatoCuentaCredito);
                string cuentaDebito = GetCuentaDebito();

                double monto = GetMontoDesdeFactura();

                if (FacturaProveedores.DocCurrency.Equals(monedaLocal))
                {
                    oAsiento.Lines.AccountCode = cuentaDebito;
                    oAsiento.Lines.Debit = monto;
                    oAsiento.Lines.AdditionalReference = FacturaProveedores.UserFields.Fields.Item("U_CL_AGREELINK").Value.ToString();
                    oAsiento.Lines.Add();

                    oAsiento.Lines.ShortName = FacturaProveedores.CardCode;
                    oAsiento.Lines.AccountCode = cuentaCredito;
                    oAsiento.Lines.AdditionalReference = FacturaProveedores.UserFields.Fields.Item("U_CL_AGREELINK").Value.ToString();
                    oAsiento.Lines.Credit = monto;
                }
                else
                {
                    oAsiento.Lines.AccountCode = cuentaDebito;
                    oAsiento.Lines.Debit = monto * FacturaProveedores.DocRate;
                    oAsiento.Lines.FCDebit = monto;
                    oAsiento.Lines.FCCurrency = FacturaProveedores.DocCurrency;
                    oAsiento.Lines.AdditionalReference = FacturaProveedores.UserFields.Fields.Item("U_CL_AGREELINK").Value.ToString();

                    oAsiento.Lines.Add();
                    oAsiento.Lines.AccountCode = cuentaCredito;
                    oAsiento.Lines.ShortName = FacturaProveedores.CardCode;
                    oAsiento.Lines.Credit = monto * FacturaProveedores.DocRate;
                    oAsiento.Lines.FCCredit = monto;
                    oAsiento.Lines.FCCurrency = FacturaProveedores.DocCurrency;
                    oAsiento.Lines.AdditionalReference = FacturaProveedores.UserFields.Fields.Item("U_CL_AGREELINK").Value.ToString();
                }

                return oAsiento;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetCuentaDebito()
        {
            for (int i = 0; i < FacturaProveedores.WithholdingTaxData.Count; i++)
            {
                FacturaProveedores.WithholdingTaxData.SetCurrentLine(i);
                if (FacturaProveedores.WithholdingTaxData.WTCode.Equals(Cls_Global.ImpuestoRetencionDeGarantia))
                    return FacturaProveedores.WithholdingTaxData.GLAccount;
            }

            return string.Empty;
        }

        private double GetMontoDesdeFactura()
        {
            double monto = 0;

            SBObob bridge = go_SBOCompany.GetBusinessObject(BoObjectTypes.BoBridge);
            string monedaLocal = bridge.GetLocalCurrency().Fields.Item(0).Value;

            for (int i = 0; i < FacturaProveedores.WithholdingTaxData.Count; i++)
            {
                FacturaProveedores.WithholdingTaxData.SetCurrentLine(i);

                if (FacturaProveedores.WithholdingTaxData.WTCode == Cls_Global.ImpuestoRetencionDeGarantia)
                {
                    monto += FacturaProveedores.DocCurrency == monedaLocal ? FacturaProveedores.WithholdingTaxData.WTAmount : FacturaProveedores.WithholdingTaxData.WTAmountFC;
                    break;
                }
            }

            return monto;
        }

        private bool DebeCrearAsientoRetencionDeGarantia()
        {
            if (Cls_Global.RetencionDeGarantiaActivo.Equals("Y"))
            {
                if (!string.IsNullOrEmpty(Cls_Global.ImpuestoRetencionDeGarantia))
                {
                    WithholdingTaxData inv5 = FacturaProveedores.WithholdingTaxData;
                    if (!ExisteCodigoRetencionEnFactura(inv5, Cls_Global.ImpuestoRetencionDeGarantia))
                        return false;
                }
                else
                    return false;
            }
            else
                return false;

            return true;
        }

        private bool ExisteCodigoRetencionEnFactura(WithholdingTaxData inv5, string codigoRetencion)
        {
            if (inv5.Count > 0)
            {
                for (int i = 0; i < inv5.Count; i++)
                {
                    inv5.SetCurrentLine(i);
                    if (inv5.WTCode.Equals(codigoRetencion))
                        return true;
                }
            }

            return false;
        }
    }
}