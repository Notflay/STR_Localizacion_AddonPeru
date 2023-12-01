using SAPbobsCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;
using System.Xml;

namespace STR_Localizacion.UI
{
    internal class Cls_GenerarTxt : Cls_PropertiesControl
    {
        public Cls_GenerarTxt()
        {
            lc_NameClass = "Cls_GenerarTxt";
            InitializeEvents();
        }

        private static string ls_path = string.Empty;
        private static string ls_Ruta = string.Empty;
        private static string ls_HeadTitle = string.Empty;
        private static string ls_DetailTitle = string.Empty;

        private static string ls_DataCab = string.Empty;
        private static string ls_DataDet = string.Empty;

        private void InitializeEvents()
        {
        }

        public void sb_EventFormData(string prm_Objectkey)
        {
            try
            {
                int li_count = 0;

                Recordset go_RecordSetDet = null;
                go_SBOForm = go_SBOFormEvent;
                XmlDocument lo_XmlDoc = new XmlDocument();
                XmlNodeList lo_XmlNdeLst;
                int li_DocEntDoc;
                int li_DocEntDocLine = 0;

                lo_XmlDoc.LoadXml(prm_Objectkey);
                lo_XmlNdeLst = lo_XmlDoc.GetElementsByTagName("DocEntry");
                li_DocEntDoc = Convert.ToInt32(lo_XmlNdeLst[0].InnerXml);

                Documents lo_OrdenCompra;

                lo_OrdenCompra = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseRequest);

                //go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_RutaArchivoTxt, null,"ORDR");
                //// Validar ruta que exista en base de datos
                //if (go_RecordSet.RecordCount > 0)
                //{
                //    ls_Ruta = go_RecordSet.Fields.Item(0).Value;
                //    ls_HeadTitle = go_RecordSet.Fields.Item(1).Value;
                //    ls_DetailTitle = go_RecordSet.Fields.Item(2).Value;

                //    ls_DataCab = string.Empty;
                //    ls_DataDet = string.Empty;

                //go_RecordSetDet = Cls_QueryManager.Retorna(Cls_Query.get_DetailData, null, li_DocEntDoc);
                //if (go_RecordSetDet.RecordCount > 0)
                //{
                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_HeaderData, null, li_DocEntDoc);

                while (!go_RecordSet.EoF)
                {
                    lo_OrdenCompra.DocType = (BoDocumentTypes)Convert.ToInt32(go_RecordSet.GetValue("DocType"));
                    lo_OrdenCompra.DocDate = Convert.ToDateTime(go_RecordSet.GetValue("DocDate"));
                    lo_OrdenCompra.DocDueDate = Convert.ToDateTime(go_RecordSet.GetValue("DocDueDate"));
                    lo_OrdenCompra.TaxDate = Convert.ToDateTime(go_RecordSet.GetValue("TaxDate"));
                    lo_OrdenCompra.RequriedDate = Convert.ToDateTime(go_RecordSet.GetValue("RequriedDate"));
                    lo_OrdenCompra.ReqType = go_RecordSet.GetValue("ReqType");
                    lo_OrdenCompra.Requester = Convert.ToString(go_RecordSet.GetValue("Requester"));
                    lo_OrdenCompra.RequesterName = go_RecordSet.GetValue("RequesterName");

                    lo_OrdenCompra.UserFields.Fields.Item("U_STR_FILE").Value = go_RecordSet.GetValue("U_STR_FILE");
                    lo_OrdenCompra.UserFields.Fields.Item("U_CE_MNDA").Value = go_RecordSet.GetValue("U_CE_MNDA");
                    lo_OrdenCompra.SendNotification = (BoYesNoEnum)Convert.ToInt32(go_RecordSet.GetValue("SendNotification"));

                    li_DocEntDocLine = go_RecordSet.Fields.Item("DocNum").Value;
                    go_RecordSetDet = Cls_QueryManager.Retorna(Cls_Query.get_DetailData, null, li_DocEntDoc, li_DocEntDocLine);
                    //Carga los datos en el control Matrix
                    while (!go_RecordSetDet.EoF)
                    {
                        lo_OrdenCompra.Lines.ItemCode = go_RecordSetDet.Fields.Item("ItemCode").Value.ToString();
                        lo_OrdenCompra.Lines.ItemDescription = go_RecordSetDet.Fields.Item("Dscription").Value.ToString();
                        lo_OrdenCompra.Lines.Quantity = Convert.ToDouble(go_RecordSetDet.Fields.Item("Quantity").Value);
                        lo_OrdenCompra.Lines.Price = Convert.ToDouble(go_RecordSetDet.Fields.Item("Price").Value);
                        lo_OrdenCompra.Lines.UserFields.Fields.Item("U_CE_IMSL").Value = go_RecordSetDet.Fields.Item("U_CE_IMSL").Value.ToString();
                        lo_OrdenCompra.Lines.ProjectCode = go_RecordSetDet.Fields.Item("U_STR_FILE").Value.ToString();

                        lo_OrdenCompra.Lines.Add();
                        go_RecordSetDet.MoveNext(); //Pasa al siguiente registro del RecordSet
                    }

                    if (lo_OrdenCompra.Add() != 0)
                        throw new Exception(string.Format("{0}-{1}", go_SBOCompany.GetLastErrorCode(), go_SBOCompany.GetLastErrorDescription()));
                    go_RecordSet.MoveNext();
                }

                //ls_path = ls_Ruta + @"\" + ls_HeadTitle;
                //System.IO.StreamWriter lo_StreamW = null;
                //sb_validarExiste(ls_path);
                //lo_StreamW = System.IO.File.AppendText(ls_path);
                //int li_column;
                //int li_record = 0;
                //// Datos de Cabecera
                //while (!go_RecordSet.EoF)
                //{
                //    li_column = go_RecordSet.Fields.Count;
                //    for (int i = 0; i < go_RecordSet.Fields.Count; i++)
                //    {
                //        li_record++;
                //        if (li_column == li_record)
                //        {
                //            ls_DataCab += go_RecordSet.Fields.Item(i).Value.ToString();
                //        }
                //        else
                //        { ls_DataCab += go_RecordSet.Fields.Item(i).Value.ToString() + "\t"; }
                //    }
                //    lo_StreamW.WriteLine(ls_DataCab);
                //    lo_StreamW.Flush();
                //    ls_DataCab = string.Empty;
                //    li_record = 0;
                //    go_RecordSet.MoveNext();
                //}
                //lo_StreamW.Close();

                //ls_path = null;
                //ls_path = ls_Ruta + @"\" + ls_DetailTitle;
                //System.IO.StreamWriter lo_StreamDetail = null;
                //sb_validarExiste(ls_path);
                //lo_StreamDetail = System.IO.File.AppendText(ls_path);

                //// Datos de Detalle
                //while (!go_RecordSetDet.EoF)
                //{
                //    li_column = go_RecordSetDet.Fields.Count;
                //    for (int i = 0; i < go_RecordSetDet.Fields.Count; i++)
                //    {
                //        li_record++;
                //        if (li_column == li_record)
                //        {
                //            ls_DataDet += go_RecordSetDet.Fields.Item(i).Value.ToString();
                //        }
                //        else
                //        { ls_DataDet += go_RecordSetDet.Fields.Item(i).Value.ToString() + "\t"; }

                //    }
                //    lo_StreamDetail.WriteLine(ls_DataDet);
                //    lo_StreamDetail.Flush();
                //    ls_DataDet = string.Empty;
                //    li_record = 0;
                //    go_RecordSetDet.MoveNext();
                //}

                //lo_StreamDetail.Close();
                //}
                //}
            }
            catch (Exception ex) { go_SBOApplication.MessageBox(ex.Message); } //Muestra una ventana con el mensaje de Excepción
        }

        //private static void sb_validarExiste(string ls_path1)
        //{
        //    if (File.Exists(ls_path1))
        //    {
        //        File.Delete(ls_path1);
        //    }
        //}
    }
}