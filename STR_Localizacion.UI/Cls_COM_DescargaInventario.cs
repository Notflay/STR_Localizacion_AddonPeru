using SAPbobsCOM;
using SAPbouiCOM;
using STR_Localizacion.UTIL;
using System;
using System.Linq;
using System.Xml.Linq;

namespace STR_Localizacion.UI
{
    internal class Cls_COM_DescargaInventario : Cls_PropertiesControl
    {
        public void solicitudTrasladoLoad(string formUID)
        {
            Form form = go_SBOApplication.Forms.Item(formUID);
            go_Item = form.Items.Add("sttSlcCmp", BoFormItemTypes.it_STATIC);
            go_Item.Width = form.GetItem("1470000099").Width;
            go_Item.Height = form.GetItem("1470000099").Height;
            go_Item.Top = form.GetItem("1470000099").Top + 14;
            go_Item.Left = form.GetItem("1470000099").Left;
            go_Static = go_Item.Specific;
            go_Static.Caption = "Slc. Compra";

            go_Item = form.Items.Add("cmbSlcCmp", BoFormItemTypes.it_COMBO_BOX);
            go_Item.Width = form.GetItem("1470000101").Width;
            go_Item.Height = form.GetItem("1470000101").Height;
            go_Item.Top = form.GetItem("1470000101").Top + 14;
            go_Item.Left = form.GetItem("1470000101").Left;
            go_Item.DisplayDesc = true;
            go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, (int)BoAutoFormMode.afm_All, BoModeVisualBehavior.mvb_False);
            go_Combo = go_Item.Specific;
            go_Combo.DataBind.SetBound(true, "OWTQ", "U_STR_DESlcCmp");

            go_Item = form.Items.Add("lnkSlcCmp", BoFormItemTypes.it_LINKED_BUTTON);
            go_Item.Width = form.GetItem("1470000102").Width;
            go_Item.Height = form.GetItem("1470000102").Height;
            go_Item.Top = form.GetItem("1470000102").Top + 14;
            go_Item.Left = form.GetItem("1470000102").Left;
            go_Item.LinkTo = "cmbSlcCmp";
            go_LinkButton = go_Item.Specific;
            go_LinkButton.LinkedObjectType = "1470000113";
        }

        public void generarSolicitudTraslado(BusinessObjectInfo boInfo)
        {
            CompanyService cmpSrv = null;
            SeriesService srsSrv = null;
            SeriesParams srsPrms = null;
            Documents slcComp = null;
            StockTransfer slcTrsl = null;
            try
            {
                slcComp = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseRequest);
                slcTrsl = go_SBOCompany.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);
                var doc = XDocument.Parse(boInfo.ObjectKey);
                var key = Convert.ToInt32(doc.Descendants("DocumentParams").Elements("DocEntry").FirstOrDefault().Value);
                if (slcComp.GetByKey(key))
                {
                    cmpSrv = go_SBOCompany.GetCompanyService();
                    srsSrv = cmpSrv.GetBusinessService(ServiceTypes.SeriesService);
                    srsPrms = srsSrv.GetDataInterface(SeriesServiceDataInterfaces.ssdiSeriesParams);
                    srsPrms.Series = slcComp.Series;
                    var seriesName = srsSrv.GetSeries(srsPrms).Name;
                    if (seriesName.EndsWith(Cls_MenuConfiguracion.DscInvSufijo))
                    {
                        slcTrsl.DocDate = slcComp.DocDate;
                        slcTrsl.DueDate = slcComp.DocDueDate;
                        slcTrsl.TaxDate = slcComp.TaxDate;
                        slcTrsl.UserFields.Fields.Item("U_STR_DESlcCmp").Value = slcComp.DocEntry;
                        for (int i = 0; i < slcComp.Lines.Count; i++)
                        {
                            slcComp.Lines.SetCurrentLine(i);
                            slcTrsl.Lines.SetCurrentLine(i);
                            slcTrsl.Lines.ItemCode = slcComp.Lines.ItemCode;
                            slcTrsl.Lines.FromWarehouseCode = slcComp.Lines.WarehouseCode;
                            slcTrsl.Lines.WarehouseCode = slcComp.Lines.UserFields.Fields.Item("U_STR_DSTWSCD").Value;
                            slcTrsl.Lines.Quantity = slcComp.Lines.Quantity;
                            slcTrsl.Lines.DistributionRule2 = slcComp.Lines.CostingCode2;
                            slcTrsl.Lines.DistributionRule3 = slcComp.Lines.CostingCode3;
                            slcTrsl.Lines.DistributionRule4 = slcComp.Lines.CostingCode4;
                            slcTrsl.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = slcComp.Lines.UserFields.Fields.Item("U_tipoOpT12").Value;
                            slcTrsl.Lines.Add();
                        }
                        if (slcTrsl.Add() != 0)
                            throw new InvalidOperationException(go_SBOCompany.GetLastErrorDescription());
                        key = Convert.ToInt32(go_SBOCompany.GetNewObjectKey());
                        if (slcTrsl.GetByKey(key))
                            go_SBOApplication.StatusBar.SetText($"Solicitud de trasalado N°: {slcTrsl.DocNum} , creada correctamente"
                                , BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                    }
                }
            }
            catch { throw; }
        }

        public void mostrarLinkSolcCompra(string formUID)
        {
            try
            {
                Form form = go_SBOApplication.Forms.Item(formUID);
                Documents slcComp = null;
                int.TryParse(form.DataSources.DBDataSources.Item("OWTQ")
                    .GetValue("U_STR_DESlcCmp", 0).Trim(), out var key);
                slcComp = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseRequest);
                var comboBox = form.GetComboBox("cmbSlcCmp");
                if (slcComp.GetByKey(key))
                {
                    while (comboBox.ValidValues.Count > 0)
                        comboBox.ValidValues.Remove(0, BoSearchKey.psk_Index);
                    comboBox.ValidValues.Add(slcComp.DocEntry.ToString(), slcComp.DocNum.ToString());
                }
            }
            catch { throw; }
        }
    }
}