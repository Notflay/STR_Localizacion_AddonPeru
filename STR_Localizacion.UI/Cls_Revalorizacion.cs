using SAPbobsCOM;
using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;
using System.Xml;

namespace STR_Localizacion.UI
{
    public class Cls_Revalorizacion : Cls_PropertiesControl
    {
        #region SBOIEventsFormData

        public void sb_EventFormData(string prm_Objectkey)
        {
            MaterialRevaluation lo_Revalorizacion;
            XmlDocument lo_XmlDoc = new XmlDocument();
            XmlNodeList lo_XmlNdeLst;

            try
            {
                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_StratConfig, null, "REVALSTOCK"); // Configuracion para que se ejecute automaticamente Revalorizacion
                if (go_RecordSet.Fields.Item("U_STR_Valor").Value == "Y")
                {
                    go_SBOForm = go_SBOFormEvent;

                    StockTransfer lo_StkTrnsf;
                    int li_DocEntTrnsfStock;

                    lo_StkTrnsf = go_SBOCompany.GetBusinessObject(BoObjectTypes.oStockTransfer);
                    lo_XmlDoc.LoadXml(prm_Objectkey);
                    lo_XmlNdeLst = lo_XmlDoc.GetElementsByTagName("DocEntry");
                    li_DocEntTrnsfStock = Convert.ToInt32(lo_XmlNdeLst[0].InnerXml);

                    lo_StkTrnsf.GetByKey(li_DocEntTrnsfStock);
                    go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_Transferencias, null, li_DocEntTrnsfStock);

                    lo_Revalorizacion = go_SBOCompany.GetBusinessObject(BoObjectTypes.oMaterialRevaluation);
                    lo_Revalorizacion.DocDate = Convert.ToDateTime(go_RecordSet.GetValue("DocDate"));
                    lo_Revalorizacion.TaxDate = Convert.ToDateTime(go_RecordSet.GetValue("TaxDate"));
                    lo_Revalorizacion.Comments = go_RecordSet.GetValue("Comentario");
                    lo_Revalorizacion.RevalType = "M";

                    //Carga los datos en el control Matrix
                    while (!go_RecordSet.EoF)
                    {
                        lo_Revalorizacion.Lines.ItemCode = go_RecordSet.Fields.Item("ItemCode").Value.ToString();
                        lo_Revalorizacion.Lines.WarehouseCode = go_RecordSet.Fields.Item("WhsCode").Value.ToString();
                        lo_Revalorizacion.Lines.Quantity = Convert.ToDouble(go_RecordSet.Fields.Item("Quantity").Value);
                        lo_Revalorizacion.Lines.DebitCredit = go_RecordSet.Fields.Item("Debito/Credito").Value;
                        lo_Revalorizacion.Lines.RevaluationIncrementAccount = go_RecordSet.Fields.Item("Cta Aumenta").Value.ToString();
                        lo_Revalorizacion.Lines.RevaluationDecrementAccount = go_RecordSet.Fields.Item("Cta Disminuye").Value.ToString();

                        lo_Revalorizacion.Lines.DistributionRule = go_RecordSet.Fields.Item("LineaNegocio").Value.ToString();
                        lo_Revalorizacion.Lines.DistributionRule2 = go_RecordSet.Fields.Item("UnidadEconomica").Value.ToString();
                        lo_Revalorizacion.Lines.DistributionRule3 = go_RecordSet.Fields.Item("CtaDestino").Value.ToString();
                        lo_Revalorizacion.Lines.DistributionRule4 = go_RecordSet.Fields.Item("CentroCosto").Value.ToString();
                        lo_Revalorizacion.Lines.DistributionRule5 = go_RecordSet.Fields.Item("CS").Value.ToString();

                        lo_Revalorizacion.Lines.Add();
                        go_RecordSet.MoveNext(); //Pasa al siguiente registro del RecordSet
                    }

                    if (lo_Revalorizacion.Add() != 0)
                        throw new Exception(string.Format("{0}-{1}", go_SBOCompany.GetLastErrorCode(), go_SBOCompany.GetLastErrorDescription()));
                    else
                        go_SBOApplication.OpenForm(BoFormObjectEnum.fo_StockRevaluation, string.Empty, go_SBOCompany.GetNewObjectKey());
                }
            }
            catch (Exception) { throw; } //Muestra una ventana con el mensaje de Excepción go_SBOApplication.MessageBox(ex.Message);
            finally { lo_Revalorizacion = null; lo_XmlDoc = null; lo_XmlNdeLst = null; }
        }

        #endregion SBOIEventsFormData
    }
}