using SAPbobsCOM;
using STR_Localizacion.UTIL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STR_Localizacion.UI
{
    public class Cls_ProvisionesNoDomiciliado : Cls_PropertiesControl
    {
        public Documents FacturaProveedores { get; set; }

        public Cls_ProvisionesNoDomiciliado()
        {
            lc_NameClass = "Cls_ProvisionesNoDomiciliado";
            ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
        }

        public void GenerarAsientoProvisionIGVNoDomiciliado()
        {
            try
            {
                lc_NameMethod = "GenerarAsientoProvisionIGVNoDomiciliado"; //Se asigna el nombre del método para la identificación del mismo
                go_SBOForm = go_SBOFormEvent;

                FacturaProveedores = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);
                string docEntry = go_SBOForm.DataSources.DBDataSources.Item("OPCH").GetValue("DocEntry", 0);

                if (FacturaProveedores.GetByKey(Convert.ToInt32(docEntry)))
                {
                    if (FacturaProveedores.Cancelled == BoYesNoEnum.tNO)
                    {
                        if (DebeCrearAsientoProvisionIGVNoDomiciliado())
                        {
                            JournalEntries asientoProvisionNoDomiciliado = GetEstructuraAsiento();
                            if (asientoProvisionNoDomiciliado.Add() == 0)
                                ActualizarReferenciaAlAsiento(go_SBOCompany.GetNewObjectKey());
                            else
                                throw new Exception("Error al crear asiento de provisión IGV No Domiciliado. Error: " + go_SBOCompany.GetLastErrorDescription());
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
            catch (Exception)
            {
                throw;
            }
        }

        private void ActualizarReferenciaAlAsiento(string transID, string tipo = "PR")
        {
            string campo = string.Empty;

            switch (tipo)
            {
                case "PR": campo = "U_STR_ASPR_ND"; break;
                case "EX":
                    campo = "U_STR_EXT_ASPR_ND";
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
                JournalEntries oAsiento = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
                oAsiento.Reference2 = FacturaProveedores.NumAtCard;
                oAsiento.TransactionCode = "IND";

                string memo = "IGV No Domiciliado - " + FacturaProveedores.CardName;
                oAsiento.Memo = memo.Length > 50 ? memo.Substring(0, 49) : memo;

                SBObob bridge = go_SBOCompany.GetBusinessObject(BoObjectTypes.BoBridge);
                string monedaLocal = bridge.GetLocalCurrency().Fields.Item(0).Value;

                double monto = GetMontoDesdeFactura();

                if (FacturaProveedores.DocCurrency.Equals(monedaLocal))
                {
                    oAsiento.Lines.AccountCode = Cls_Global.ProvisionNDCuentaDebito;
                    oAsiento.Lines.Debit = monto;
                    oAsiento.Lines.LineMemo = memo;
                    oAsiento.Lines.Add();

                    oAsiento.Lines.AccountCode = Cls_Global.ProvisionNDCuentaCredito;
                    oAsiento.Lines.Credit = monto;
                    oAsiento.Lines.LineMemo = memo;
                }
                else
                {
                    oAsiento.Lines.AccountCode = Cls_Global.ProvisionNDCuentaDebito;
                    oAsiento.Lines.Debit = monto * FacturaProveedores.DocRate;
                    oAsiento.Lines.FCDebit = monto;
                    oAsiento.Lines.FCCurrency = FacturaProveedores.DocCurrency;
                    oAsiento.Lines.LineMemo = memo;

                    oAsiento.Lines.Add();
                    oAsiento.Lines.AccountCode = Cls_Global.ProvisionNDCuentaCredito;
                    oAsiento.Lines.Credit = monto * FacturaProveedores.DocRate;
                    oAsiento.Lines.FCCredit = monto;
                    oAsiento.Lines.FCCurrency = FacturaProveedores.DocCurrency;
                    oAsiento.Lines.LineMemo = memo;
                }

                return oAsiento;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private double GetMontoDesdeFactura()
        {
            double monto = 0;

            SBObob bridge = go_SBOCompany.GetBusinessObject(BoObjectTypes.BoBridge);
            string monedaLocal = bridge.GetLocalCurrency().Fields.Item(0).Value;
            double tasaIGV = GetTasaIGV();

            for (int i = 0; i < FacturaProveedores.Lines.Count; i++)
            {
                FacturaProveedores.Lines.SetCurrentLine(i);
                if (FacturaProveedores.Lines.WTLiable == BoYesNoEnum.tYES)
                {
                    monto += FacturaProveedores.DocCurrency == monedaLocal ? FacturaProveedores.Lines.LineTotal - FacturaProveedores.Lines.NetTaxAmount : FacturaProveedores.Lines.RowTotalFC - FacturaProveedores.Lines.NetTaxAmountFC;
                }
            }

            return monto * tasaIGV;
        }

        private double GetTasaIGV()
        {
            SalesTaxCodes impuestos = go_SBOCompany.GetBusinessObject(BoObjectTypes.oSalesTaxCodes);
            if (impuestos.GetByKey("IGV"))
                return impuestos.Rate / 100;
            else
                throw new Exception("Error al generar asiento de provisión no domiciliado. No se ha definido el impuesto con código IGV");
        }

        private string ExtornarAsiento()
        {
            JournalEntries asientoProvisionIGVND = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
            JournalEntries asientoExtornado = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);

            SBObob bridge = go_SBOCompany.GetBusinessObject(BoObjectTypes.BoBridge);
            string monedaLocal = bridge.GetLocalCurrency().Fields.Item(0).Value;

            if (asientoProvisionIGVND.GetByKey((int)FacturaProveedores.UserFields.Fields.Item("U_STR_ASPR_ND").Value))
            {
                asientoExtornado.Reference2 = FacturaProveedores.NumAtCard;

                if (FacturaProveedores.DocCurrency.Equals(monedaLocal))
                {
                    for (int i = 0; i < asientoProvisionIGVND.Lines.Count; i++)
                    {
                        asientoProvisionIGVND.Lines.SetCurrentLine(i);
                        asientoExtornado.Lines.SetCurrentLine(i);

                        asientoExtornado.Lines.AccountCode = asientoProvisionIGVND.Lines.AccountCode;

                        if (asientoProvisionIGVND.Lines.Debit > 0)
                            asientoExtornado.Lines.Credit = asientoProvisionIGVND.Lines.Debit;

                        if (asientoProvisionIGVND.Lines.Credit > 0)
                            asientoExtornado.Lines.Debit = asientoProvisionIGVND.Lines.Credit;

                        asientoExtornado.Lines.Add();
                    }
                }
                else
                {
                    for (int i = 0; i < asientoProvisionIGVND.Lines.Count; i++)
                    {
                        asientoProvisionIGVND.Lines.SetCurrentLine(i);
                        asientoExtornado.Lines.SetCurrentLine(i);

                        asientoExtornado.Lines.AccountCode = asientoProvisionIGVND.Lines.AccountCode;

                        if (asientoProvisionIGVND.Lines.FCDebit > 0)
                        {
                            asientoExtornado.Lines.Credit = asientoProvisionIGVND.Lines.Debit;
                            asientoExtornado.Lines.FCCredit = asientoProvisionIGVND.Lines.FCDebit;
                            asientoExtornado.Lines.FCCurrency = asientoProvisionIGVND.Lines.FCCurrency;
                        }

                        if (asientoProvisionIGVND.Lines.FCCredit > 0)
                        {
                            asientoExtornado.Lines.Debit = asientoProvisionIGVND.Lines.Credit;
                            asientoExtornado.Lines.FCDebit = asientoProvisionIGVND.Lines.FCCredit;
                            asientoExtornado.Lines.FCCurrency = asientoProvisionIGVND.Lines.FCCurrency;
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
                go_SBOApplication.MessageBox("El asiento de provisión no domiciliado no se ha generado, no se puede revertirlo");
                throw new Exception("El asiento de provisión no domiciliado no se ha generado, no se puede revertirlo");
            }
        }

        private bool DebeCrearAsientoProvisionIGVNoDomiciliado()
        {
            if (Cls_Global.ProvisionNDActivo.Equals("Y"))
            {
                List<string> codigosRetencion = Cls_Global.ProvisionNDCodigosRetencion.Split(';').ToList();

                if (codigosRetencion.Count > 0)
                {
                    WithholdingTaxData inv5 = FacturaProveedores.WithholdingTaxData;

                    for (int i = 0; i < inv5.Count; i++)
                    {
                        if (codigosRetencion.Contains(inv5.WTCode))
                            return true;
                    }
                }
                else
                    return false;
            }
            else
                return false;

            return false;
        }

        //private bool ExisteCodigoRetencionEnFactura(WithholdingTaxData inv5, string codigoRetencion)
        //{
        //    if(inv5.Count > 0)
        //    {
        //        for (int i = 0; i < inv5.Count; i++)
        //        {
        //            inv5.SetCurrentLine(i);
        //            if (inv5.WTCode.Equals(codigoRetencion))
        //                return true;
        //        }
        //    }

        //    return false;
        //}
    }
}