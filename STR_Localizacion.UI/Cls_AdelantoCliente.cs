using SAPbobsCOM;
using SAPbouiCOM;
using STR_Localizacion.UTIL;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static STR_Localizacion.UTIL.Cls_Global;

namespace STR_Localizacion.UI
{
    internal class Cls_AdelantoCliente : Cls_PropertiesControl
    {
        //* * * * * * * * * * * * * * DataSources* * * * * * * * * * * * * * *
        private const string gs_DtcADLPRY = "@STR_ADLPRY";

        private const string gs_DtdADLPRY1 = "@STR_ADLPRY1";

        //* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
        //* * * * * * * * * * * * * * * Menus * * * * * * * * * * * * * * * * * * * * * * * *
        public const string gs_MnuBorrarFila = "MNU_DltLineParam";

        //* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
        //* * * * * * * * * * * * * * Variables de Clase * * * * * * * * * * * * * * * * * * * *
        private int gi_RightClickRow = -1;

        private string gs_TpoFrm = string.Empty;
        //* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *

        //Folders
        private const string gs_FldCheque = "fldCheque";

        private const string gs_FldTransf = "fldTransf";
        private const string gs_FldSinPgo = "fldSinPago";
        private const string gs_FldEfectv = "fldEfec";

        public Cls_AdelantoCliente()
        {
            gs_FormName = "frmAdelantoCliente";
            gs_FormPath = "Resources/Localizacion/AdelantoCliente.srf";
            lc_NameClass = "Cls_AdelantoCliente";
        }

        public void sb_FormLoad()
        {
            try
            {
                if (go_SBOForm == null)
                {
                    go_SBOForm = fn_CreateForm(gs_FormName, gs_FormPath);
                    ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
                    sb_DataFormLoad();
                    sb_DataFormLoadAdd();
                    initializeEvents();
                }
            }
            catch (Exception exc)
            {
                Cls_Global.WriteToFile(exc.Message);
                go_SBOApplication.SetStatusBarMessage(exc.Message, BoMessageTime.bmt_Short, true);
            }
            finally { go_SBOForm.Visible = true; }
        }

        private void sb_DataFormLoad()
        {
            DimensionsService lo_DmnsSrv = null;
            CompanyService lo_CmpSrv = null;
            Dimension lo_Dim = null;
            Conditions lo_Cnds = null;
            Condition lo_Cnd = null;
            try
            {
                go_SBOForm.EnableMenu("1285", false);
                go_Matrix = go_SBOForm.Items.Item("mtxADPY").Specific;
                go_SBOForm.DataBrowser.BrowseBy = "txtDocEnt";
                go_SBOForm.PaneLevel = 2;
                lo_Cnds = go_SBOApplication.CreateObject(BoCreatableObjectType.cot_Conditions);
                lo_Cnd = lo_Cnds.Add();
                lo_Cnd.Alias = "Postable";
                lo_Cnd.Operation = BoConditionOperation.co_EQUAL;
                lo_Cnd.CondVal = "Y";
                go_SBOForm.ChooseFromLists.Item("CFLTRNCTA").SetConditions(lo_Cnds);
                go_SBOForm.ChooseFromLists.Item("CFLEFCCTA").SetConditions(lo_Cnds);
                go_SBOForm.AutoManaged = true;
                go_SBOForm.Items.Cast<Item>().ToList().ForEach(i =>
                {
                    if (i.Type != BoFormItemTypes.it_BUTTON && i.Type != BoFormItemTypes.it_LINKED_BUTTON)
                    {
                        i.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, (int)BoAutoFormMode.afm_Ok, BoModeVisualBehavior.mvb_False);
                        i.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, (int)BoAutoFormMode.afm_View, BoModeVisualBehavior.mvb_False);
                        if (i.UniqueID == "txtNroOpr" || i.UniqueID == "txtTotApr" || i.UniqueID == "txtDocNum" || i.UniqueID == "txtTotG")
                            i.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, (int)BoAutoFormMode.afm_Add, BoModeVisualBehavior.mvb_False);
                        else
                            i.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, (int)BoAutoFormMode.afm_Add, BoModeVisualBehavior.mvb_True);
                    }
                });
                go_SBOForm.Items.Item("fld1").Visible = false;

                go_SBOForm.Items.Item("txtTotG").Enabled = false;
            }
            catch (Exception ex)
            {Cls_Global.WriteToFile(ex.Message);
                go_SBOApplication.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
            finally
            {
            }
        }

        public void sb_DataFormLoadAdd()
        {
            string ls_Serie = string.Empty;
            string qry = string.Empty;
            Recordset recordSet = null;
            SBObob sboBob = null;

            recordSet = go_SBOCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            sboBob = go_SBOCompany.GetBusinessObject(BoObjectTypes.BoBridge);

            go_SBOForm.DataSources.DBDataSources.Item("@STR_ADLPRY").SetValue("U_AP_FCHC", 0, DateTime.Now.ToString("yyyyMMdd"));
            go_SBOForm.DataSources.DBDataSources.Item("@STR_ADLPRY").SetValue("U_AP_FCHV", 0, DateTime.Now.ToString("yyyyMMdd"));
            go_SBOForm.DataSources.DBDataSources.Item("@STR_ADLPRY").SetValue("U_AP_FCHD", 0, DateTime.Now.ToString("yyyyMMdd"));
            //go_SBOForm.DataSources.DBDataSources.Item("@STR_ADLPRY").SetValue(gs_UflChqFchVnc, 0, DateTime.Now.ToString("yyyyMMdd"));
            //((SAPbouiCOM.Folder)go_SBOForm.Items.Item("fldCheque").Specific).Select();
            qry = "select \"CurrCode\",\"CurrCode\" from OCRN";
            WriteToFile(qry);
            recordSet.DoQuery(qry);
            go_Combo = go_SBOForm.GetComboBox("cmbMnda");
            sb_comboLlenar(go_Combo, recordSet);

            qry = "SELECT \"CFWId\",\"CFWName\" FROM OCFW";
            WriteToFile(qry);
            recordSet.DoQuery(qry);
            sb_comboLlenar(go_SBOForm.GetComboBox("cmbCshFlw"), recordSet);

            go_Matrix = go_SBOForm.GetMatrix("mtxADPY");
            qry = "select \"Code\",\"Name\" from \"@STR_ADLPRYCNF\"";
            WriteToFile(qry);
            recordSet.DoQuery(qry);
            go_Matrix.AddRow();
            go_Matrix.FlushToDataSource();
            go_Combo = go_Matrix.Columns.Item(1).Cells.Item(1).Specific;
            sb_comboLlenar(go_Combo, recordSet);
            setRowMatrixNumbers();
            //go_Matrix.DeleteRow(1);
            go_Matrix.LoadFromDataSourceEx();

            go_Combo = go_SBOForm.Items.Item("cmbSerie").Specific;
            go_Combo.ValidValues.LoadSeries(go_SBOForm.BusinessObject.Type, BoSeriesMode.sf_Add);
            if (go_Combo.Selected == null && go_Combo.ValidValues.Count > 0) ls_Serie = go_Combo.ValidValues.Item(0).Value;
            else ls_Serie = go_Combo.Selected.Value;
            go_SBOForm.DataSources.DBDataSources.Item("@STR_ADLPRY").SetValue("Series", 0, ls_Serie);
            this.sb_GetNextDocumentNumber();
        }

        private void sb_GetNextDocumentNumber()
        {
            string ls_Serie = string.Empty;
            ls_Serie = go_SBOForm.DataSources.DBDataSources.Item("@STR_ADLPRY").GetValue("Series", 0);
            go_SBOForm.DataSources.DBDataSources.Item("@STR_ADLPRY").SetValue("DocNum", 0, go_SBOForm.BusinessObject.GetNextSerialNumber(ls_Serie, go_SBOForm.BusinessObject.Type).ToString());
        }

        private void initializeEvents()
        {
            itemevent.Add(new sapitemevent(BoEventTypes.et_CHOOSE_FROM_LIST, e =>
            {
                DataTable lo_DataTable = null;
                ChooseFromListEvent lo_CFLEvnt = null;
                SAPbouiCOM.ChooseFromList lo_CFL = null;
                lo_CFLEvnt = (ChooseFromListEvent)e;
                go_Matrix = go_SBOForm.GetMatrix("mtxADPY");
                Conditions conditions;
                Condition condition;

                if (e.ItemUID == "edtCdgClnt")
                {
                    if (lo_CFLEvnt.BeforeAction)
                    {
                        lo_CFL = go_SBOForm.ChooseFromLists.Item(lo_CFLEvnt.ChooseFromListUID);
                        lo_CFL.SetConditions(null);
                        conditions = lo_CFL.GetConditions();
                        condition = conditions.Add();
                        condition.Alias = "CardType";
                        condition.Operation = BoConditionOperation.co_EQUAL;
                        condition.CondVal = "C";
                        lo_CFL.SetConditions(conditions);
                    }
                    else
                    {
                        lo_DataTable = lo_CFLEvnt.SelectedObjects;
                        if (lo_DataTable != null)
                        {
                            go_SBOForm.DataSources.DBDataSources.Item("@STR_ADLPRY").SetValue("U_AP_CDGC", 0, lo_DataTable.GetValue(0, 0));
                            go_SBOForm.DataSources.DBDataSources.Item("@STR_ADLPRY").SetValue("U_AP_NMBC", 0, lo_DataTable.GetValue(1, 0));
                        }
                    }
                }
                else if (e.ItemUID == "mtxADPY")
                {
                    go_Matrix.FlushToDataSource();
                    lo_DataTable = lo_CFLEvnt.SelectedObjects;
                    if (lo_DataTable != null)
                    {
                        go_SBOForm.DataSources.DBDataSources.Item("@STR_ADLPRY1").SetValue("U_AP_PRYC", e.Row - 1, lo_DataTable.GetValue(0, 0));
                        go_Matrix.LoadFromDataSourceEx();
                    }
                }
                else if (e.ItemUID == "txtTrnCta")
                {
                    lo_DataTable = lo_CFLEvnt.SelectedObjects;
                    if (lo_DataTable != null)
                        go_SBOForm.DataSources.DBDataSources.Item("@STR_ADLPRY").SetValue("U_AP_CTBN", 0, lo_DataTable.GetValue(0, 0));
                }
            }, "edtCdgClnt", "mtxADPY", "txtTrnCta"));

            itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "1", e =>
            {
                try
                {
                    if (e.BeforeAction && go_SBOForm.Mode == BoFormMode.fm_ADD_MODE && validacionesGenerales())
                    {
                        if (fn_GenerarAsiento())
                        {
                            if (string.IsNullOrEmpty(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_CNCP", go_Matrix.RowCount - 1)))
                            {
                                go_Matrix = go_SBOForm.GetMatrix("mtxADPY");
                                go_Matrix.DeleteRow(go_Matrix.RowCount);
                                go_Matrix.FlushToDataSource();
                            }
                        }
                    }
                }
                catch { throw; }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_VALIDATE, "mtxADPY", e =>
            {
                var totImp = 0.0;
                var totGastos = 0.0;

                if (!e.BeforeAction && e.ColUID == "clmImporte")
                {
                    go_Matrix = go_SBOForm.GetMatrix("mtxADPY");
                    go_Matrix.FlushToDataSource();

                    for (int i = 0; i < go_SBOForm.DataSources.DBDataSources.Item(gs_DtdADLPRY1).Size; i++)
                    {
                        totImp += Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPT", i));
                        totGastos += Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPTG", i));
                    }
                    go_SBOForm.SetHeaderDBValue(gs_DtcADLPRY, "U_AP_MNTT", (totImp).ToString());
                    go_SBOForm.SetHeaderDBValue(gs_DtcADLPRY, "U_AP_MNTTG", totGastos.ToString());
                }

                if (!e.BeforeAction && e.ColUID == "clmGst")
                {
                    go_Matrix = go_SBOForm.GetMatrix("mtxADPY");
                    go_Matrix.FlushToDataSource();
                    for (int i = 0; i < go_SBOForm.DataSources.DBDataSources.Item(gs_DtdADLPRY1).Size; i++)
                    {
                        totImp += Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPT", i));
                        totGastos += Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPTG", i));
                    }
                    go_SBOForm.SetHeaderDBValue(gs_DtcADLPRY, "U_AP_MNTT", (totImp).ToString());
                    go_SBOForm.SetHeaderDBValue(gs_DtcADLPRY, "U_AP_MNTTG", totGastos.ToString());
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_LOST_FOCUS, e =>
            {
                try
                {
                    go_SBOForm.Freeze(true);
                    string fecha = go_SBOForm.GetEditText(e.ItemUID).Value;
                    go_SBOForm.GetEditText("txtFchVnc").Value = fecha;
                    go_SBOForm.GetEditText("txtFchDcm").Value = fecha;
                    var codMnda = go_SBOForm.GetComboBox("cmbMnda").Value;
                    DateTime dtFecha = go_SBObob.Format_StringToDate(fecha).Fields.Item(0).Value;
                    if (!string.IsNullOrEmpty(codMnda))
                        go_SBOForm.GetEditText("edtTpoCmb").Value = go_SBObob.GetCurrencyRate(codMnda, dtFecha).Fields.Item(0).Value.ToString();
                }
                catch (Exception ex)
                {
                    Cls_Global.WriteToFile(ex.Message);
                    throw;
                }
                finally
                {
                    go_SBOForm.Freeze(false);
                }
            }, "txtFchCnt"));

            itemevent.Add(new sapitemevent(BoEventTypes.et_COMBO_SELECT, "mtxADPY", e =>
            {
                if (!e.BeforeAction && e.ColUID == "clmCncpt")
                {
                    string qry = string.Empty;
                    Recordset recordSet = null;
                    try
                    {
                        go_SBOForm.Freeze(true);
                        go_Matrix = go_SBOForm.GetMatrix("mtxADPY");
                        go_Matrix.FlushToDataSource();
                        recordSet = go_SBOCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                        go_Combo = go_Matrix.Columns.Item(1).Cells.Item(e.Row).Specific;
                        qry = $"select * from \"@STR_ADLPRYCNF\" where \"Code\" = '{go_Combo.Value}'";
                        WriteToFile(qry);
                        recordSet.DoQuery(qry);
                        if (go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_MNDA").Equals("USD"))
                        {
                            string nmbCta = Convert.ToString(recordSet.Fields.Item(4).Value);
                            go_SBOForm.SetBodyDBValue(gs_DtdADLPRY1, "U_AP_NMCT", e.Row - 1, nmbCta);
                            string cdgCta = Convert.ToString(recordSet.Fields.Item(5).Value);
                            go_SBOForm.SetBodyDBValue(gs_DtdADLPRY1, "U_AP_CGCT", e.Row - 1, cdgCta);
                            string cdgCtaGasto = Convert.ToString(recordSet.Fields.Item(6).Value);
                            go_SBOForm.SetBodyDBValue(gs_DtdADLPRY1, "U_AP_CGST", e.Row - 1, cdgCtaGasto);
                        }
                        else
                        {
                            string nmbCta = Convert.ToString(recordSet.Fields.Item(2).Value);
                            go_SBOForm.SetBodyDBValue(gs_DtdADLPRY1, "U_AP_NMCT", e.Row - 1, nmbCta);
                            string cdgCta = Convert.ToString(recordSet.Fields.Item(3).Value);
                            go_SBOForm.SetBodyDBValue(gs_DtdADLPRY1, "U_AP_CGCT", e.Row - 1, cdgCta);
                            string cdgCtaGasto = Convert.ToString(recordSet.Fields.Item(6).Value);
                            go_SBOForm.SetBodyDBValue(gs_DtdADLPRY1, "U_AP_CGST", e.Row - 1, cdgCtaGasto);
                        }
                        if (e.Row == go_Matrix.RowCount)
                        {
                            go_Matrix.LoadFromDataSourceEx();
                            go_Matrix.AddRow();
                            go_Matrix.ClearRowData(go_Matrix.RowCount);
                            go_Matrix.FlushToDataSource();
                        }
                        setRowMatrixNumbers();
                        go_Matrix.LoadFromDataSourceEx();
                        go_Matrix.AutoResizeColumns();
                    }
                    catch { throw; }
                    finally { go_SBOForm.Freeze(false); }
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_COMBO_SELECT, "cmbMnda", e =>
            {
                if (!e.BeforeAction)
                {
                    SBObob sboBob = null;
                    double tpoCmb = 1.0;

                    sboBob = go_SBOCompany.GetBusinessObject(BoObjectTypes.BoBridge);
                    string mndAdlPry = go_SBOForm.GetComboBox("cmbMnda").Value;
                    string mndLocal = sboBob.GetLocalCurrency().Fields.Item(0).Value;
                    if (mndAdlPry != mndLocal)
                    {
                        DateTime fchCntb = sboBob.Format_StringToDate(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_FCHC")).Fields.Item(0).Value;
                        tpoCmb = sboBob.GetCurrencyRate(mndAdlPry, fchCntb).Fields.Item(0).Value;
                    }
                    go_SBOForm.SetHeaderDBValue(gs_DtcADLPRY, "U_AP_TPCMB", tpoCmb.ToString());
                    go_SBOForm.SetVisibleControl(!mndAdlPry.Equals(mndLocal), "edtTpoCmb");
                }
            }));
            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, e => Dispose()));
        }

        public void setLastRecord()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                go_SBOApplication.Menus.Item("1289").Activate();
            });
        }

        internal bool fn_HandleRightClickEvent(ContextMenuInfo po_RghClkEvent)
        {
            bool lb_Result = true;
            try
            {
                //* * * * * * Fila del Evento * * * * * * * * * * * * * *
                gi_RightClickRow = po_RghClkEvent.Row;
                //* * * * * * * * * * * * * * * * * * * * * * * * * * * *
                sb_AddDeleteRowMenu(po_RghClkEvent);
                sb_DeleteRemoveMenu(po_RghClkEvent);
            }
            catch (Exception ex)
            {
                Cls_Global.WriteToFile(ex.Message);
                go_SBOApplication.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Short, true);
            }
            return lb_Result;
        }

        private void sb_DeleteRemoveMenu(ContextMenuInfo po_RghClkEvent)
        {
            if (po_RghClkEvent.BeforeAction)
            {
                SAPbouiCOM.MenuItem lo_MenuItem = null;
                lo_MenuItem = go_SBOApplication.Menus.Item("1283");
                lo_MenuItem.Enabled = false;
                lo_MenuItem = null;
            }
        }

        private void sb_AddDeleteRowMenu(ContextMenuInfo po_RghClkEvent)
        {
            if (po_RghClkEvent.ItemUID != string.Empty)
            {
                if (go_SBOForm.Items.Item(po_RghClkEvent.ItemUID).Type == BoFormItemTypes.it_MATRIX)
                {
                    if (po_RghClkEvent.BeforeAction)
                    {
                        Menus lo_Menus = null;
                        IMenuItem lo_MnuItm = null;
                        MenuCreationParams lo_MnuCrtPrms = null;
                        lo_MnuItm = go_SBOApplication.Menus.Item("1280");
                        lo_MnuCrtPrms = go_SBOApplication.CreateObject(BoCreatableObjectType.cot_MenuCreationParams);
                        lo_Menus = lo_MnuItm.SubMenus;
                        lo_MnuCrtPrms = null;
                        if (po_RghClkEvent.Row > 1)
                        {
                            lo_MnuCrtPrms = go_SBOApplication.CreateObject(BoCreatableObjectType.cot_MenuCreationParams);
                            lo_MnuCrtPrms.Type = BoMenuType.mt_STRING;
                            lo_MnuCrtPrms.UniqueID = gs_MnuBorrarFila;
                            lo_MnuCrtPrms.String = "Borrar linea";
                            lo_MnuCrtPrms.Enabled = true;
                            lo_Menus.AddEx(lo_MnuCrtPrms);
                            lo_MnuCrtPrms = null;
                        }
                    }
                    else
                    {
                        if (go_SBOApplication.Menus.Exists(gs_MnuBorrarFila))
                            go_SBOApplication.Menus.RemoveEx(gs_MnuBorrarFila);
                    }
                }
            }
        }

        public void sb_DeleteRowMatrix()
        {
            go_Matrix = go_SBOForm.Items.Item("mtxADPY").Specific;
            DialogResult lo_Resultado;
            if (gi_RightClickRow > 1)
            {
                lo_Resultado = (DialogResult)go_SBOApplication.MessageBox("¿Desea eliminar esta fila", 1, "Si", "No");
                if (lo_Resultado == DialogResult.OK)
                {
                    go_Matrix.DeleteRow(gi_RightClickRow);
                }
            }
            go_SBOForm.Update();
        }

        public bool fn_GenerarAsiento()
        {
            JournalEntries asiento = null;
            ChartOfAccounts chartOfAcounts = null;
            string ls_CtaPte = string.Empty;

            asiento = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
            chartOfAcounts = go_SBOCompany.GetBusinessObject(BoObjectTypes.oChartOfAccounts);

            asiento.ReferenceDate = DateTime.ParseExact(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_FCHC"), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            asiento.DueDate = DateTime.ParseExact(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_FCHV"), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            asiento.TaxDate = DateTime.ParseExact(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_FCHD"), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            asiento.Reference = go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_TBRF");
            asiento.Memo = "Adelanto de cliente";

            for (int i = 0; i < go_SBOForm.DataSources.DBDataSources.Item(gs_DtdADLPRY1).Size - 1; i++)
            {
                asiento.Lines.SetCurrentLine(asiento.Lines.Count - 1);
                asiento.Lines.ShortName = go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_CDGC");
                asiento.Lines.AccountCode = go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_CGCT", i);

                if (go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_MNDA").Equals("USD"))
                {
                    if (Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPT", i)) > 0.0)
                    {
                        //asiento.Lines.Credit = Convert.ToDouble(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_TPCMB"))
                        //* (Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPT", i))
                        //+ Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPTG", i)));
                        asiento.Lines.FCCredit = Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPT", i))
                            + Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPTG", i));
                    }
                    else
                    {
                        asiento.Lines.Debit = Math.Abs(Convert.ToDouble(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_TPCMB"))
                        * (Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPT", i))
                        + Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPTG", i))));
                        asiento.Lines.FCDebit = Math.Abs(Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPT", i))
                            + Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPTG", i)));
                    }
                    asiento.Lines.FCCurrency = "USD";
                }
                else
                {
                    if (Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPT", i)) > 0.0)
                        asiento.Lines.Credit = Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPT", i))
                            + Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPTG", i));
                    else
                        asiento.Lines.Debit = Math.Abs(Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPT", i))
                            + Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPTG", i)));
                }
                if (chartOfAcounts.GetByKey(asiento.Lines.AccountCode) && chartOfAcounts.CashFlowRelevant == BoYesNoEnum.tYES)
                {
                    if (string.IsNullOrEmpty(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_CSHF")))
                        throw new Exception("No se ha seleccionado ninguna opción del flujo de efectivo....");
                    asiento.Lines.PrimaryFormItems.CashFlowLineItemID = Convert.ToInt32(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_CSHF"));
                    asiento.Lines.PrimaryFormItems.JDTLineId = asiento.Lines.Line_ID;
                    asiento.Lines.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtNotAssigned;
                    asiento.Lines.PrimaryFormItems.Add();
                }
                asiento.Lines.ProjectCode = go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_PRYC", i);
                asiento.Lines.LineMemo = go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_CMNT", i);
                asiento.Lines.Add();

                double gastoLinea = Convert.ToDouble(go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_IMPTG", i));

                if (gastoLinea != 0)
                {
                    asiento.Lines.SetCurrentLine(asiento.Lines.Count - 1);
                    asiento.Lines.AccountCode = go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_CGST", i).ToString(); //CUENTA GASTO

                    if (go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_MNDA").Equals("USD"))
                    {
                        if (gastoLinea > 0)
                        {
                            //asiento.Lines.Debit = Convert.ToDouble(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_TPCMB")) * gastoLinea;
                            asiento.Lines.FCDebit = gastoLinea;
                        }
                        else
                        {
                            asiento.Lines.Credit = Convert.ToDouble(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_TPCMB")) * gastoLinea * -1;
                            asiento.Lines.FCCredit = gastoLinea * -1;
                        }
                        asiento.Lines.FCCurrency = "USD";
                    }
                    else
                    {
                        if (gastoLinea > 0)
                            asiento.Lines.Debit = gastoLinea;
                        else
                            asiento.Lines.Credit = gastoLinea * -1;
                    }

                    asiento.Lines.ProjectCode = go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_PRYC", i);
                    asiento.Lines.LineMemo = go_SBOForm.GetBodyDBValue(gs_DtdADLPRY1, "U_AP_CMNT", i);
                    asiento.Lines.Add();
                }
            }

            asiento.Lines.SetCurrentLine(asiento.Lines.Count - 1);
            asiento.Lines.AccountCode = go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_CTBN");

            double montoTotal = Convert.ToDouble(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_MNTT"));
            double totalGasto = Convert.ToDouble(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_MNTTG"));

            double montoBanco = montoTotal; // - totalGasto;

            if (go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_MNDA").Equals("USD"))
            {
                if (montoBanco > 0)
                {
                    //asiento.Lines.Debit = Convert.ToDouble(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_TPCMB")) * montoBanco;
                    asiento.Lines.FCDebit = montoBanco;
                }
                else
                {
                    //asiento.Lines.Credit = Convert.ToDouble(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_TPCMB")) * montoBanco * -1;
                    asiento.Lines.FCCredit = montoBanco * -1;
                }
                asiento.Lines.FCCurrency = "USD";
            }
            else
            {
                if (montoBanco > 0)
                    asiento.Lines.Debit = montoBanco;
                else
                    asiento.Lines.Credit = montoBanco * -1;
            }

            if (chartOfAcounts.GetByKey(asiento.Lines.AccountCode) && chartOfAcounts.CashFlowRelevant == BoYesNoEnum.tYES)
            {
                if (string.IsNullOrEmpty(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_CSHF")))
                    throw new Exception("No se ha seleccionado ninguna opción del flujo de efectivo....");
                asiento.Lines.PrimaryFormItems.CashFlowLineItemID = Convert.ToInt32(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_CSHF"));
                asiento.Lines.PrimaryFormItems.JDTLineId = asiento.Lines.Line_ID; ;
                asiento.Lines.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtNotAssigned;
                asiento.Lines.PrimaryFormItems.Add();
            }

            string asientoAsXML = asiento.GetAsXML();

            if (asiento.Add() != 0)
                throw new InvalidOperationException(string.Concat(go_SBOCompany.GetLastErrorCode(), " - ", go_SBOCompany.GetLastErrorDescription()));
            go_SBOForm.SetHeaderDBValue(gs_DtcADLPRY, "U_AP_NROP", go_SBOCompany.GetNewObjectKey());
            return true;
        }

        private void setRowMatrixNumbers()
        {
            for (int i = 0; i < go_SBOForm.DataSources.DBDataSources.Item("VPM1").Size; i++)
                go_SBOForm.SetBodyDBValue("VPM1", "LineID", i, (i + 1).ToString());
        }

        public bool cancelarDocumento()
        {
            JournalEntries asAdenlanto = null;

            DialogResult lo_Rslt = ((DialogResult)Cls_Global.go_SBOApplication.MessageBox("La cancelación de un documento es irreversible. El estado del documento será cambiado a \"Cancelado\"\n¿Desea continuar con esta acción?", 1, "SI", "NO"));
            if (lo_Rslt == DialogResult.OK)
            {
                int transId = Convert.ToInt32(go_SBOForm.GetHeaderDBValue(gs_DtcADLPRY, "U_AP_NROP"));
                asAdenlanto = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
                if (asAdenlanto.GetByKey(transId))
                {
                    if (asAdenlanto.Cancel() != 0)
                    {
                        go_SBOCompany.GetLastError(out int errCode, out string errMsg);
                        throw new InvalidOperationException($"{errCode}-{errMsg}");
                    }
                    return true;
                }
            }
            return false;
        }

        private bool validacionesGenerales()
        {
            if (string.IsNullOrEmpty(go_SBOForm.GetComboBox("cmbMnda").Value))
                throw new InvalidOperationException("Seleccione la moneda de adelanto");
            return true;
        }
    }
}