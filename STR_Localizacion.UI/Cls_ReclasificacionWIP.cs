using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;

namespace STR_Localizacion.UI
{
    public class Cls_ReclasificacionWIP : Cls_PropertiesControl
    {
        public Cls_ReclasificacionWIP()
        {
            lc_NameClass = "Cls_ReclasificacionWIP";
            ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
        }

        public bool fn_HandleItemEvent(SAPbouiCOM.Application po_SBOApplication, SAPbouiCOM.Form po_Form, SAPbouiCOM.ItemEvent po_ItmEvnt)
        {
            return true;
        }

        public bool fn_HandleFormDataEvent(SAPbouiCOM.Application po_SBOApplication, SAPbobsCOM.Company po_SBOCompany, SAPbouiCOM.BusinessObjectInfo po_BsnssObjInf, SAPbouiCOM.Form po_Form)
        {
            bool lb_Rslt = true;
            string ls_XmlOrdPrd = String.Empty;
            switch (po_BsnssObjInf.EventType)
            {
                case SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE:
                    if (!po_BsnssObjInf.BeforeAction && po_BsnssObjInf.ActionSuccess && po_Form.DataSources.DBDataSources.Item("OWOR").GetValue("Status", 0) == "L")
                    {
                        try
                        {
                            ls_XmlOrdPrd = po_BsnssObjInf.ObjectKey;
                            ls_XmlOrdPrd = ls_XmlOrdPrd.Substring(ls_XmlOrdPrd.IndexOf("<AbsoluteEntry>") + 15, ls_XmlOrdPrd.IndexOf("</AbsoluteEntry>") - ls_XmlOrdPrd.IndexOf("<AbsoluteEntry>") - 15);
                            lb_Rslt = fn_GenerarAsientoReclasificacion(po_SBOCompany, po_SBOApplication, ls_XmlOrdPrd);
                        }
                        catch (Exception ex)
                        {
                            po_SBOApplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                        }
                    }
                    break;
            }
            return lb_Rslt;
        }

        private bool fn_GenerarAsientoReclasificacion(SAPbobsCOM.Company po_SBOCompany, SAPbouiCOM.Application po_SBOApplication, string ps_DocEnt)
        {
            bool lb_result = true;
            SAPbobsCOM.JournalEntries lo_AsOrdPrd = null;
            SAPbobsCOM.JournalEntries lo_AsReclas = null;
            SAPbobsCOM.ProductionOrders lo_PrdOrd = null;
            System.Data.DataTable lo_DataTbl = null;
            System.Data.DataRow lo_DR = null;

            string ls_CdgCta = String.Empty;
            string ls_Qry = String.Empty;
            string ls_CdgCtaWIP = String.Empty;
            string ls_CdgCtaPte = String.Empty;
            string ls_CdgCtaWIPPP1 = String.Empty;
            string ls_CdgCtaWIPPP2 = String.Empty;
            int li_IndexRow = -1;

            try
            {
                lo_DataTbl = new System.Data.DataTable();
                lo_DataTbl.Columns.Add(new System.Data.DataColumn("CTA1", typeof(string)));
                lo_DataTbl.Columns.Add(new System.Data.DataColumn("CTA2", typeof(string)));
                lo_PrdOrd = po_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oProductionOrders);
                lo_AsOrdPrd = po_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
                lo_AsReclas = po_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);

                this.sb_ListarCtaSWip(ps_DocEnt);

                if (go_RecordSet.RecordCount > 0)
                {
                    while (!go_RecordSet.EoF)
                    {
                        lo_DR = lo_DataTbl.NewRow();
                        lo_DR["CTA1"] = go_RecordSet.Fields.Item(0).Value.ToString().Trim();
                        lo_DR["CTA2"] = go_RecordSet.Fields.Item(1).Value.ToString().Trim();
                        if (go_RecordSet.Fields.Item(3).Value.ToString() == "PP")
                        {
                            ls_CdgCtaPte = go_RecordSet.Fields.Item(1).Value.ToString();
                            ls_CdgCtaWIPPP1 = go_RecordSet.Fields.Item(0).Value.ToString();
                            ls_CdgCtaWIPPP2 = go_RecordSet.Fields.Item(2).Value.ToString();
                        }
                        lo_DataTbl.Rows.Add(lo_DR);
                        go_RecordSet.MoveNext();
                    }

                    if (lo_PrdOrd.GetByKey(int.Parse(ps_DocEnt)))
                    {
                        if (lo_AsOrdPrd.GetByKey(lo_PrdOrd.TransactionNumber))
                        {
                            lo_AsReclas.ReferenceDate = lo_AsOrdPrd.ReferenceDate;
                            lo_AsReclas.DueDate = lo_AsOrdPrd.DueDate;
                            lo_AsReclas.TaxDate = lo_AsOrdPrd.TaxDate;
                            lo_AsReclas.Reference = lo_AsOrdPrd.Reference;
                            lo_AsReclas.Reference2 = lo_AsOrdPrd.Reference2;
                            lo_AsReclas.Reference3 = lo_AsOrdPrd.Reference3;
                            lo_AsReclas.Memo = lo_AsOrdPrd.Memo;
                            lo_AsReclas.ProjectCode = lo_AsOrdPrd.ProjectCode;
                            //------------------------------------------
                            //lo_AsReclas.UserFields.Fields.Item("U_OUT_NMRO").Value = ps_DocEnt;
                            string trans = Convert.ToInt64(SAPbobsCOM.BoObjectTypes.oProductionOrders).ToString();
                            lo_AsReclas.UserFields.Fields.Item("U_BPP_SubTDoc").Value = trans;

                            // lo_AsReclas.UserFields.Fields.Item("U_BPP_SubTDoc").Value = Integer.Parse(SAPbobsCOM.BoObjectTypes.oProductionOrders).ToString()

                            //  ''''--------------------------------------

                            //'* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
                            //'Asiento detalle * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *

                            for (int i = 0; i < lo_AsOrdPrd.Lines.Count; i++)
                            {
                                lo_AsOrdPrd.Lines.SetCurrentLine(i);
                                li_IndexRow = fn_FindAccountCodeInDataTable(po_SBOApplication, lo_DataTbl, lo_AsOrdPrd.Lines.AccountCode);

                                if (li_IndexRow > -1)
                                {
                                    if (lo_AsOrdPrd.Lines.AccountCode == ls_CdgCtaWIPPP1)
                                    {
                                        ls_CdgCtaWIP = ls_CdgCtaWIPPP2;
                                    }
                                    else
                                    {
                                        ls_CdgCtaWIP = lo_DataTbl.Rows[li_IndexRow][1].ToString().Trim();
                                    }
                                    ls_CdgCta = ls_CdgCtaPte;
                                    lo_DataTbl.Rows.RemoveAt(li_IndexRow);
                                }
                                else
                                {
                                    ls_CdgCta = ls_CdgCtaWIP;
                                }
                                lo_AsReclas.Lines.ProjectCode = lo_AsOrdPrd.Lines.ProjectCode;
                                lo_AsReclas.Lines.CostingCode = lo_AsOrdPrd.Lines.CostingCode;
                                lo_AsReclas.Lines.CostingCode2 = lo_AsOrdPrd.Lines.CostingCode2;
                                lo_AsReclas.Lines.CostingCode3 = lo_AsOrdPrd.Lines.CostingCode3;
                                lo_AsReclas.Lines.CostingCode4 = lo_AsOrdPrd.Lines.CostingCode4;
                                lo_AsReclas.Lines.CostingCode5 = lo_AsOrdPrd.Lines.CostingCode5;
                                lo_AsReclas.Lines.AccountCode = ls_CdgCta;

                                if (lo_AsOrdPrd.Lines.Debit > 0 | lo_AsOrdPrd.Lines.FCDebit > 0)
                                {
                                    if (lo_AsOrdPrd.Lines.FCCredit + lo_AsOrdPrd.Lines.FCDebit > 0)
                                    {
                                        lo_AsReclas.Lines.Debit = lo_AsOrdPrd.Lines.Debit;
                                        lo_AsReclas.Lines.FCDebit = lo_AsOrdPrd.Lines.FCDebit;
                                        lo_AsReclas.Lines.FCCurrency = lo_AsOrdPrd.Lines.FCCurrency;
                                    }
                                    else
                                    {
                                        lo_AsReclas.Lines.Debit = lo_AsOrdPrd.Lines.Debit;
                                    }
                                }
                                else
                                {
                                    if (lo_AsOrdPrd.Lines.FCCredit + lo_AsOrdPrd.Lines.FCDebit > 0)
                                    {
                                        lo_AsReclas.Lines.Credit = lo_AsOrdPrd.Lines.Credit;
                                        lo_AsReclas.Lines.FCCredit = lo_AsOrdPrd.Lines.FCCredit;
                                        lo_AsReclas.Lines.FCCurrency = lo_AsOrdPrd.Lines.FCCurrency;
                                    }
                                    else
                                    {
                                        lo_AsReclas.Lines.Credit = lo_AsOrdPrd.Lines.Credit;
                                    }
                                }

                                lo_AsReclas.Lines.Add();
                            }
                            if (lo_AsReclas.Add() != 0)
                            {
                                po_SBOApplication.StatusBar.SetText(po_SBOCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                lb_result = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                po_SBOApplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                lb_result = false;
            }
            return lb_result;
        }

        private int fn_FindAccountCodeInDataTable(SAPbouiCOM.Application po_SBOApplication, System.Data.DataTable po_DataTable, string ps_CdgCta)
        {
            int li_Fila = -1;

            try
            {
                for (int i = 0; i < po_DataTable.Rows.Count; i++)
                {
                    if (po_DataTable.Rows[i][0].ToString().Trim() == ps_CdgCta)
                    {
                        li_Fila = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                po_SBOApplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            return li_Fila;
        }

        //private  string fn_ListarCtaSWip(string ps_DocEnt)
        //{
        //    lc_NameMethod = "fn_ListarCtaSWip"; //Se asigna el nombre del método para la identificación del mismo

        //    try
        //    {
        //        //Recupero el query del Resources y lo ejecuto en el recordSet
        //        string ls_query = string.Empty;
        //        ls_query = Cls_Global.gso_resxSet.GetString("LISTARCTASWIP") + ps_DocEnt + Cls_Global.gss_finLine;
        //        return ls_query;

        //    }
        //    catch (Exception ex)
        //    {
        //        this.sb_GuardarLog(ex.Message, ls_nomMet); return null;
        //    }

        //}

        private void sb_ListarCtaSWip(string ps_DocEnt)
        {
            lc_NameMethod = "fn_ListarCtaSWip"; //Se asigna el nombre del método para la identificación del mismo

            try
            {
                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_CuentaSWIP, null, ps_DocEnt);
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                ExceptionPrepared.SaveInLog(false);
            }
        }
    }
}