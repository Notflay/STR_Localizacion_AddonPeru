using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STR_Localizacion.UI
{
    partial class Cls_GenerarAsiento : Cls_PropertiesControl
    {
        public Cls_GenerarAsiento()
        {
            gs_FormName = "frmGeneradorAsiento";
            gs_FormPath = "Resources/Localizacion/GenerarAsiento.srf";
            lc_NameClass = "Cls_GenerarAsiento";
        }

        public void sb_FormLoad()
        {
            try
            {
                if (go_SBOForm == null)
                {
                    go_SBOForm = Cls_Global.fn_CreateForm(gs_FormName, gs_FormPath);
                    ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
                    sb_DataFormLoad();
                    InitializeEvents();
                }
            }
            catch (Exception exc)
            {
                go_SBOApplication.SetStatusBarMessage(exc.Message, BoMessageTime.bmt_Short, true);
                go_SBOForm = null;
            }
        }

        #region SBOForm

        public void sb_DataFormLoad()
        {
            lc_NameMethod = "sb_CreacionFormulario"; //Se asigna el nombre del método para la identificación del mismo
            string ls_serie = string.Empty;

            try
            {
                go_SBOForm.Freeze(true); //Congela el formulario

                int li_NumDoc = Cls_QueryManager.Retorna(Cls_Query.get_NumeroDocumento, 0, go_SBOForm.BusinessObject.Type);

                if (li_NumDoc == 0)
                {
                    go_SBOForm.Close();
                    throw new InvalidOperationException("Para generar este documento, primero defina la serie de numeración en el módulo Gestión.");
                }

                //Recupera los valores de los controles del formulario al comboBox
                go_Combo = go_SBOForm.Items.Item(lrs_CbxSeries).Specific;
                go_Combo.ValidValues.LoadSeries(go_SBOForm.BusinessObject.Type, BoSeriesMode.sf_Add);
                go_Combo.Select(0, BoSearchKey.psk_Index);

                ls_serie = go_Combo.Selected.Value;

                //Asigna un nuevo valor al DocNum
                go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).SetValue(lrs_UflSeries, 0, ls_serie);
                go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).SetValue(lrs_UflFchEje, 0, DateTime.Now.ToString(lrs_FchFormat));
                go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).SetValue(lrs_UflDocNum, 0, go_SBOForm.BusinessObject.GetNextSerialNumber(ls_serie, go_SBOForm.BusinessObject.Type).ToString());

                //Banco Paises
                Cls_Global.sb_comboLlenar(go_SBOForm.GetComboBox(lrs_CbxBnkPais), Cls_QueryManager.Retorna(Cls_Query.get_BancoPais));

                go_SBOForm.Items.Item(lrs_BtnCrear).Enabled = false;

                //Asigna el manejo automatico de los atributos de los control
                string[] ls_Controls = new string[] { lrs_EdtPrvDd,    lrs_EdtPrvHt,    lrs_EdtFchCnDd,
                                                      lrs_EdtFchCnHt,  lrs_EdtFchVnDd,  lrs_EdtFchVnHt,
                                                      lrs_EdtDocNum,   lrs_EdtFchEje,    lrs_EdtBnkMnd,
                                                      lrs_EdtMntTotal, lrs_EdtMntPorte, lrs_EdtMntCmns,
                                                      lrs_EdtAsnRsp,   lrs_EdtDocEnt,   lrs_BtnBuscar,
                                                      lrs_RbtDtr,      lrs_RbtPrc,      lrs_RbtFNg,
                                                      lrs_CbxBnkPais,  lrs_CbxBnkCdgo,  lrs_CbxBnkCta,
                                                      lrs_CbxEstado,   lrs_CbxSeries
                                                    };

                foreach (string item in ls_Controls)
                {
                    BoModeVisualBehavior lo_BehaviorOk = 0;
                    BoModeVisualBehavior lo_BehaviorFind = 0;

                    if (item.Equals(lrs_EdtDocNum) || item.Equals(lrs_EdtFchEje) || item.Equals(lrs_CbxSeries) || item.Equals(lrs_CbxEstado))
                        lo_BehaviorFind = BoModeVisualBehavior.mvb_True;

                    go_SBOForm.Items.Item(item).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, (int)BoAutoFormMode.afm_Ok, lo_BehaviorOk);
                    go_SBOForm.Items.Item(item).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, (int)BoAutoFormMode.afm_Find, lo_BehaviorFind);
                }

                string ls_TpoAS = go_SBOForm.GetOptionButton(lrs_RbtDtr).Selected ? "DT" : (go_SBOForm.GetOptionButton(lrs_RbtPrc).Selected ? "PR" : "FN");
                go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).SetValue(lrs_UflTipoAsn, 0, ls_TpoAS);

                go_SBOForm.GetOptionButton(lrs_RbtPrc).GroupWith(lrs_RbtDtr);
                go_SBOForm.GetOptionButton(lrs_RbtFNg).GroupWith(lrs_RbtDtr);
                go_SBOForm.DataSources.UserDataSources.Item(lrs_UdsImpTotal).Value = "0.0";

                go_SBOForm.Freeze(false);
            }
            finally { }
        }

        private void sb_CargaGrilla(string[] prs_parameters)
        {
            DataTable lo_DataSource = null;
            try
            {
                go_Matrix = go_SBOForm.Items.Item(lrs_MtxBatDTR).Specific;
                go_Matrix.Clear();

                if (lo_DataSource == null)
                {
                    if (go_SBOForm.DataSources.DataTables.Count == 0)
                        go_SBOForm.DataSources.DataTables.Add(lrs_DtcBATDTRDET);

                    lo_DataSource = go_SBOForm.DataSources.DataTables.Item(lrs_DtcBATDTRDET);
                }
                lo_DataSource.Consulta(Cls_Query.get_FacturasSinAsiento, prs_parameters);

                if (lo_DataSource.IsEmpty)
                    go_SBOApplication.StatusBar.SetText("No se encontraron documentos sin asiento.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                else
                {
                    go_SBOApplication.StatusBar.SetText("Cargando documentos...", BoMessageTime.bmt_Long + 200, BoStatusBarMessageType.smt_Warning);

                    go_Matrix.Columns.Item(lrs_ClmMtxSelec).Editable = true;
                    go_Matrix.Columns.Item(lrs_ClmMtxSelec).DataBind.Bind(lrs_DtcBATDTRDET, lrs_SqlSelec);
                    go_Matrix.Columns.Item(lrs_ClmMtxCodPrv).DataBind.Bind(lrs_DtcBATDTRDET, lrs_SqlCodProv);
                    go_Matrix.Columns.Item(lrs_ClmMtxRzSocial).DataBind.Bind(lrs_DtcBATDTRDET, lrs_SqlRznScl);
                    go_Matrix.Columns.Item(lrs_ClmMtxCodTrans).DataBind.Bind(lrs_DtcBATDTRDET, lrs_SqlCodTrans);
                    go_Matrix.Columns.Item(lrs_ClmMtxDocEnt).DataBind.Bind(lrs_DtcBATDTRDET, lrs_SqlDocEntry);
                    go_Matrix.Columns.Item(lrs_ClmMtxTpDoc).DataBind.Bind(lrs_DtcBATDTRDET, lrs_SqlTpDoc);
                    go_Matrix.Columns.Item(lrs_ClmMtxNrDoc).DataBind.Bind(lrs_DtcBATDTRDET, lrs_SqlNroDoc);
                    go_Matrix.Columns.Item(lrs_ClmMtxFchDoc).DataBind.Bind(lrs_DtcBATDTRDET, lrs_SqlFchDoc);
                    go_Matrix.Columns.Item(lrs_ClmMtxCptoRet).DataBind.Bind(lrs_DtcBATDTRDET, lrs_SqlCptoRet);
                    go_Matrix.Columns.Item(lrs_ClmMtxImpRet).DataBind.Bind(lrs_DtcBATDTRDET, lrs_SqlImpDet);
                    go_Matrix.Columns.Item(lrs_ClmMtxCodPry).DataBind.Bind(lrs_DtcBATDTRDET, lrs_SqlCodPry);
                    go_Matrix.Columns.Item(lrs_ClmMtxDesPry).DataBind.Bind(lrs_DtcBATDTRDET, lrs_SqlDesPry);
                    go_Matrix.Columns.Item(lrs_ClmMtxEstDoc).DataBind.Bind(lrs_DtcBATDTRDET, lrs_SqlEstDoc);
                    go_Matrix.Columns.Item(lrs_ClmMtxAsGnr).DataBind.Bind(lrs_DtcBATDTRDET, lrs_SqlAsDtr);
                    go_Matrix.Columns.Item(lrs_ClmMtxAsGnr).Visible = false;
                    go_Matrix.LoadFromDataSource();

                    go_SBOForm.Items.Item(lrs_BtnCrear).Enabled = true;
                    go_SBOApplication.StatusBar.SetText("Se cargaron " + go_Matrix.RowCount + " documento(s).", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                }
            }
            catch (Exception ex) { go_SBOApplication.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Short); }
            finally { lo_DataSource = null; }
        }

        public Action sb_SetBound()
        {
            Action ACT = () =>
            {
                try
                {
                    go_Matrix = go_SBOForm.Items.Item(lrs_MtxBatDTR).Specific;
                    go_Matrix.Clear();

                    bool DataBound = go_Matrix.Columns.Item(lrs_ClmMtxSelec).DataBind.DataBound;
                    bool DataBindAlias = true;

                    if (DataBound) DataBindAlias = !go_Matrix.Columns.Item(lrs_ClmMtxSelec).DataBind.Alias.Equals(lrs_UflSelec);

                    if (DataBindAlias)
                    {
                        go_Matrix.Columns.Item(lrs_ClmMtxSelec).Editable = false;
                        go_Matrix.Columns.Item(lrs_ClmMtxSelec).DataBind.SetBound(true, lrs_DtcBATDTRDET, lrs_UflSelec);
                        go_Matrix.Columns.Item(lrs_ClmMtxCodPrv).DataBind.SetBound(true, lrs_DtcBATDTRDET, lrs_UflCodProv);
                        go_Matrix.Columns.Item(lrs_ClmMtxRzSocial).DataBind.SetBound(true, lrs_DtcBATDTRDET, lrs_UflRznScl);
                        go_Matrix.Columns.Item(lrs_ClmMtxCodTrans).DataBind.SetBound(true, lrs_DtcBATDTRDET, lrs_UflCodTrans);
                        go_Matrix.Columns.Item(lrs_ClmMtxDocEnt).DataBind.SetBound(true, lrs_DtcBATDTRDET, lrs_UflDocEntry);
                        go_Matrix.Columns.Item(lrs_ClmMtxTpDoc).DataBind.SetBound(true, lrs_DtcBATDTRDET, lrs_UflTpDoc);
                        go_Matrix.Columns.Item(lrs_ClmMtxNrDoc).DataBind.SetBound(true, lrs_DtcBATDTRDET, lrs_UflNroDoc);
                        go_Matrix.Columns.Item(lrs_ClmMtxFchDoc).DataBind.SetBound(true, lrs_DtcBATDTRDET, lrs_UflFchDoc);
                        go_Matrix.Columns.Item(lrs_ClmMtxCptoRet).DataBind.SetBound(true, lrs_DtcBATDTRDET, lrs_UflCptoRet);
                        go_Matrix.Columns.Item(lrs_ClmMtxImpRet).DataBind.SetBound(true, lrs_DtcBATDTRDET, lrs_UflImpDet);
                        go_Matrix.Columns.Item(lrs_ClmMtxCodPry).DataBind.SetBound(true, lrs_DtcBATDTRDET, lrs_UflCodPry);
                        go_Matrix.Columns.Item(lrs_ClmMtxDesPry).DataBind.SetBound(true, lrs_DtcBATDTRDET, lrs_UflDesPry);
                        go_Matrix.Columns.Item(lrs_ClmMtxEstDoc).DataBind.SetBound(true, lrs_DtcBATDTRDET, lrs_UflEstDoc);
                        go_Matrix.Columns.Item(lrs_ClmMtxAsGnr).DataBind.SetBound(true, lrs_DtcBATDTRDET, lrs_UflAsDtr);
                        go_Matrix.Columns.Item(lrs_ClmMtxAsGnr).Visible = true;
                    }
                }
                catch (Exception ex) { go_SBOApplication.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Short); }
            };

            return ACT;
        }

        private void sb_CargarUDO()
        {
            try
            {
                int lci_Filas = 0;
                if (go_SBOForm.DataSources.DataTables.Count == 0) return;

                DataTable pbo_DataTable = go_SBOForm.DataSources.DataTables.Item(lrs_DtcBATDTRDET);
                lci_Filas = pbo_DataTable.Rows.Count;
                go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).Clear();

                for (int i = 0; i < lci_Filas; i++)
                {
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).InsertRecord(i);
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).Offset = i;
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflSelec, i, "Y");
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflCodProv, i, pbo_DataTable.GetValue(lrs_SqlCodProv, i));
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflRznScl, i, pbo_DataTable.GetValue(lrs_SqlRznScl, i));
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflCodTrans, i, pbo_DataTable.GetValue(lrs_SqlCodTrans, i));
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflDocEntry, i, pbo_DataTable.GetValue(lrs_SqlDocEntry, i));
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflTpDoc, i, pbo_DataTable.GetValue(lrs_SqlTpDoc, i));
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflNroDoc, i, pbo_DataTable.GetValue(lrs_SqlNroDoc, i));
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflFchDoc, i, pbo_DataTable.GetValue(lrs_SqlFchDoc, i));
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflCptoRet, i, pbo_DataTable.GetValue(lrs_SqlCptoRet, i));
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflImpDet, i, pbo_DataTable.GetValue(lrs_SqlImpDet, i));
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflCodPry, i, pbo_DataTable.GetValue(lrs_SqlCodPry, i));
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflDesPry, i, pbo_DataTable.GetValue(lrs_SqlDesPry, i));
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflEstDoc, i, pbo_DataTable.GetValue(lrs_SqlEstDoc, i));
                }
            }
            catch (Exception ex)
            {
                go_SBOApplication.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Short);
            }
        }

        /// <summary>
        /// <Selecciona todos los registros dentro de la grilla>
        /// Al dar doble clic en la cabecera de la columna "Seleccion" la función se encarga de recorrer todas la filas
        /// y asignarles el valor de True o False
        /// </summary>
        private void sb_MultipleCheckingGrid()
        {
            try
            {
                go_SBOForm.Freeze(true);
                go_Matrix = go_SBOForm.Items.Item(lrs_MtxBatDTR).Specific;
                go_SBOApplication.StatusBar.SetText("Cantidad de registros a (des)seleccionar: " + go_Matrix.RowCount + ".", BoMessageTime.bmt_Long + 200, BoStatusBarMessageType.smt_Warning);

                bool lb_Check = ((CheckBox)go_Matrix.GetCellSpecific(lrs_ClmMtxSelec, 1)).Checked;

                for (int i = 1; i <= go_Matrix.RowCount; i++)
                    ((CheckBox)go_Matrix.GetCellSpecific(lrs_ClmMtxSelec, i)).Checked = !lb_Check;

                sb_ActualizarImportes();

                go_Matrix.FlushToDataSource();
                go_SBOApplication.StatusBar.SetText("Se (des)seleccionaron " + go_Matrix.RowCount + " registros.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            }
            catch (Exception ex) { go_SBOApplication.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Short); }
            finally { go_SBOForm.Freeze(false); }
        }

        private void sb_ActualizarImportes()
        {
            try
            {
                go_SBOForm.Freeze(true);

                go_Matrix = go_SBOForm.GetMatrix(lrs_MtxBatDTR);
                go_Matrix.FlushToDataSource();
                go_Matrix.LoadFromDataSourceEx(false);

                double ldb_ImpTotal = 0.0;
                double ldb_Sum = 0.0;
                int lci_Filas = 0;
                if (go_SBOForm.DataSources.DataTables.Count == 0) return;

                DataTable pbo_DataTable = go_SBOForm.DataSources.DataTables.Item(lrs_DtcBATDTRDET);
                lci_Filas = pbo_DataTable.Rows.Count;

                for (int i = 1; i <= lci_Filas; i++)
                {
                    go_CheckBox = go_Matrix.GetCellSpecific(lrs_ClmMtxSelec, i);
                    if (go_CheckBox.Checked)
                        ldb_Sum += pbo_DataTable.GetValue(lrs_SqlImpDet, i - 1);
                }

                ldb_ImpTotal += double.Parse(go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflMntPorte, 0));
                ldb_ImpTotal += double.Parse(go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflMntCmsn, 0));
                ldb_ImpTotal += ldb_Sum;

                go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).SetValue(lrs_UflTotPag, 0, ldb_Sum.ToString());
                go_SBOForm.DataSources.UserDataSources.Item(lrs_UdsImpTotal).Value = ldb_ImpTotal.ToString();
            }
            catch (Exception ex) { go_SBOApplication.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Short); }
            finally { go_SBOForm.Freeze(false); }
        }

        private bool sb_GenerarAsientoFNG()
        {
            SAPbobsCOM.Documents lo_Doc = null;
            AsientoProperties lo_AsPrp = null;
            AsientoProperties.AsientoCabecera lo_AsCab = null;
            AsientoProperties.AsientoDetalle lo_AsDet = null;
            AsientoProperties.LineData lo_LnData = null;
            List<AsientoProperties.AsientoDetalle> lo_LstAsDet = null;
            List<AsientoProperties.AsientoDetalle> lo_LstAsDet2 = null;
            List<string> lo_LstCrdCds = null;
            SAPbobsCOM.UserTable lo_UsrTbl = default(SAPbobsCOM.UserTable);
            string ls_CtaFacNeg = string.Empty;
            string ls_CtaRsp = string.Empty;
            string ls_CtaDoc = string.Empty;
            string ls_CtaBnk = string.Empty;
            string ls_CtaPrts = string.Empty;
            string ls_CtaCmsn = string.Empty;
            DBDataSource lo_DBDtaSrc = default(DBDataSource);
            List<AsientoProperties.LineData> lo_LstLnData = default(List<AsientoProperties.LineData>);
            double ld_MntTot = 0.0;
            double ld_Portes = 0.0;
            double ld_Comsns = 0.0;
            bool lb_FlagAdd = false;
            int li_TrnsId = 0;
            string ls_DscBanco = string.Empty;
            int li_CntChk = 0;
            DateTime FchAsn;

            try
            {
                go_SBOForm.Freeze(true);
                go_Matrix = go_SBOForm.Items.Item(lrs_MtxBatDTR).Specific;

                for (int i = 1; i <= go_Matrix.RowCount; i++)
                {
                    go_CheckBox = go_Matrix.GetCellSpecific(lrs_ClmMtxSelec, i);
                    if (go_CheckBox.Checked)
                        li_CntChk++;
                }
                if (go_Matrix.RowCount == 0)
                    throw new InvalidOperationException("No hay registros para seleccionar.");
                else if (li_CntChk == 0)
                    throw new InvalidOperationException("No ha seleccionado ningún documento.");

                var count = go_Matrix.RowCount;
                for (int i = 1; i <= count; i++)
                {
                    go_CheckBox = go_Matrix.GetCellSpecific(lrs_ClmMtxSelec, i);
                    if (!go_CheckBox.Checked)
                    {
                        go_Matrix.DeleteRow(i);
                        count--;
                        i--;
                    }
                }

                go_SBOForm.Freeze(false);
                go_Matrix.FlushToDataSource();
                sb_CargarUDO();

                lo_UsrTbl = go_SBOCompany.UserTables.Item("BPP_CONFIG");
                lo_DBDtaSrc = go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET);
                lo_LstLnData = new List<AsientoProperties.LineData>();
                lo_LstAsDet = new List<AsientoProperties.AsientoDetalle>();
                lo_LstAsDet2 = new List<AsientoProperties.AsientoDetalle>();
                lo_LstCrdCds = new List<string>();

                go_SBOCompany.StartTransaction();

                if (lo_UsrTbl.GetByKey("1"))
                {
                    // Obtencion de los valores registrados en la tabla BPP_CONFIG
                    ls_CtaFacNeg = lo_UsrTbl.UserFields.Fields.Item("U_STR_CTFNG").Value.ToString().Trim();
                    ls_CtaPrts = lo_UsrTbl.UserFields.Fields.Item("U_STR_CTPRT").Value.ToString().Trim();
                    ls_CtaCmsn = lo_UsrTbl.UserFields.Fields.Item("U_STR_CTCMS").Value.ToString().Trim();
                    ls_CtaRsp = lo_UsrTbl.UserFields.Fields.Item("U_STR_CTRSP").Value.ToString().Trim();
                    ls_CtaBnk = go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflBnkCta, 0).Trim();
                }
                else
                    throw new Exception("Tabla @BPP_CONFIG vacia...");

                // Validacion de Registro de Cuentas
                if (string.IsNullOrEmpty(ls_CtaFacNeg))
                    throw new Exception("Cuenta asociada facturas negociables no registrada.");
                else if (string.IsNullOrEmpty(ls_CtaRsp))
                    throw new Exception("Cuenta resposabilidad bancaria no registrada.");
                else if (string.IsNullOrEmpty(ls_CtaBnk))
                    throw new Exception("Cuenta de bancos no seleccionada.");

                for (int i = 0; i <= (lo_DBDtaSrc.Size - 1); i++)
                    lo_LstCrdCds.Add(lo_DBDtaSrc.GetValue(lrs_UflCodProv, i).Trim());

                lo_LstCrdCds = lo_LstCrdCds.Distinct().ToList();
                for (int i = 0; i <= (lo_LstCrdCds.Count - 1); i++)
                {
                    try
                    {
                        lo_AsPrp = new AsientoProperties();
                        lo_AsCab = new AsientoProperties.AsientoCabecera();

                        FchAsn = DateTime.ParseExact(go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflFchEje, 0).Trim(), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);

                        lo_AsCab.FechaContabilizacion = FchAsn;
                        lo_AsCab.FechaDocumento = FchAsn;
                        lo_AsCab.FechaVencimiento = FchAsn;
                        lo_AsCab.CodigoTransaccion = "FNG";
                        lo_AsCab.Comentarios = "Reclasificacion - Factura Negociable";
                        lo_AsCab.Referencia = string.Empty;
                        lo_AsCab.Referencia2 = string.Empty;
                        lo_AsCab.Referencia3 = string.Empty;
                        lo_AsCab.FCMoneda = go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflBnkMnd, 0).Trim();

                        for (int j = 0; j <= (lo_DBDtaSrc.Size - 1); j++)
                        {
                            if ((lo_DBDtaSrc.GetValue(lrs_UflCodProv, j).Trim() == lo_LstCrdCds[i].Trim() && lo_DBDtaSrc.GetValue(lrs_UflSelec, i).Trim() == "Y"))
                            {
                                if ((lo_DBDtaSrc.GetValue(lrs_UflTpDoc, j).Trim() == "AN"))
                                    lo_Doc = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDownPayments);
                                else
                                    lo_Doc = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);

                                lo_Doc.GetByKey(int.Parse(lo_DBDtaSrc.GetValue(lrs_UflDocEntry, j).Trim()));
                                // Cuenta Asociada Factura Negociable
                                lo_AsDet = new AsientoProperties.AsientoDetalle();
                                lo_AsDet.CodigoCuenta = ls_CtaFacNeg;
                                lo_AsDet.NombreCuenta = lo_Doc.CardCode;
                                lo_AsDet.Debito = double.Parse(lo_DBDtaSrc.GetValue(lrs_UflImpDet, j).Trim());
                                lo_AsDet.Credito = 0;
                                lo_AsDet.Referencia2 = lo_Doc.NumAtCard;
                                lo_LstAsDet.Add(lo_AsDet);
                                // Cuenta por cobrar
                                lo_AsDet = new AsientoProperties.AsientoDetalle();
                                lo_AsDet.CodigoCuenta = lo_Doc.ControlAccount;
                                lo_AsDet.NombreCuenta = lo_Doc.CardCode;
                                lo_AsDet.Debito = 0;
                                lo_AsDet.Credito = double.Parse(lo_DBDtaSrc.GetValue(lrs_UflImpDet, j).Trim());
                                lo_AsDet.Referencia2 = lo_Doc.NumAtCard;
                                lo_AsDet.BaseType = int.Parse(lo_Doc.DocObjectCodeEx);
                                lo_AsDet.BaseEntry = lo_Doc.DocEntry;
                                lo_LstAsDet.Add(lo_AsDet);
                                // Cuenta de Responsabilidad
                                lo_AsDet = new AsientoProperties.AsientoDetalle();
                                lo_AsDet.CodigoCuenta = ls_CtaRsp;
                                lo_AsDet.NombreCuenta = ls_CtaRsp;
                                lo_AsDet.Debito = 0;
                                lo_AsDet.Credito = double.Parse(lo_DBDtaSrc.GetValue(lrs_UflImpDet, j).Trim());
                                lo_AsDet.Referencia2 = lo_Doc.NumAtCard;
                                lo_AsDet.Referencia3 = lo_Doc.CardCode;
                                lo_LstAsDet2.Add(lo_AsDet);

                                lo_LnData = new AsientoProperties.LineData();
                                lo_LnData.CardCode = lo_DBDtaSrc.GetValue(lrs_UflCodProv, j).Trim();
                                lo_LnData.LineID = j;
                                lo_LstLnData.Add(lo_LnData);
                                ld_MntTot = ld_MntTot + double.Parse(lo_DBDtaSrc.GetValue(lrs_UflImpDet, j).Trim());
                            }
                        }

                        lo_AsPrp.AsCabecera = lo_AsCab;
                        lo_AsPrp.AsCabecera.AsientoDetalle = lo_LstAsDet;
                        li_TrnsId = fn_GenerarAsientoFNG(lo_AsPrp);

                        foreach (AsientoProperties.LineData ln in lo_LstLnData)
                        {
                            if (li_TrnsId != 0)
                            {
                                go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflEstDoc, ln.LineID, "Creado");
                                go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflAsDtr, ln.LineID, li_TrnsId.ToString());
                            }
                            else
                                go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflEstDoc, ln.LineID, "Error");
                        }
                    }
                    finally
                    {
                        lo_LstAsDet.Clear();
                        lo_LstLnData.Clear();
                    }
                }

                lo_AsPrp = new AsientoProperties();
                lo_AsCab = new AsientoProperties.AsientoCabecera();
                ls_DscBanco = go_SBOForm.GetComboBox(lrs_CbxBnkCdgo).Selected.Description;

                ld_Portes = double.Parse(go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflMntPorte, 0));
                ld_Comsns = double.Parse(go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflMntCmsn, 0));

                FchAsn = DateTime.ParseExact(go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflFchEje, 0).Trim(), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                lo_AsCab.FechaContabilizacion = FchAsn;
                lo_AsCab.FechaDocumento = FchAsn;
                lo_AsCab.FechaVencimiento = FchAsn;

                lo_AsCab.CodigoTransaccion = "FNG";
                lo_AsCab.Comentarios = "Asiento de Responsabilidad";
                lo_AsCab.Referencia = string.Empty;
                lo_AsCab.Referencia2 = string.Empty;
                lo_AsCab.Referencia3 = string.Empty;
                lo_AsCab.FCMoneda = go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).GetValue(lrs_UflBnkMnd, 0).Trim();
                // Cuenta de banco
                lo_AsDet = new AsientoProperties.AsientoDetalle();
                lo_AsDet.CodigoCuenta = ls_CtaBnk;
                lo_AsDet.NombreCuenta = ls_CtaBnk;
                lo_AsDet.Debito = ld_MntTot - (ld_Portes + ld_Comsns);
                lo_AsDet.Credito = 0;
                lo_LstAsDet.Add(lo_AsDet);

                // Cuenta de Responsabilidad
                lo_LstAsDet.AddRange(lo_LstAsDet2.ToArray());

                // Cuenta de Portes
                lo_AsDet = new AsientoProperties.AsientoDetalle();
                lo_AsDet.CodigoCuenta = ls_CtaPrts;
                lo_AsDet.NombreCuenta = ls_CtaPrts;
                lo_AsDet.Debito = ld_Portes;
                lo_AsDet.Credito = 0;
                if (ld_Portes > 0) lo_LstAsDet.Add(lo_AsDet);

                // Cuenta de Comisiones
                lo_AsDet = new AsientoProperties.AsientoDetalle();
                lo_AsDet.CodigoCuenta = ls_CtaCmsn;
                lo_AsDet.NombreCuenta = ls_CtaCmsn;
                lo_AsDet.Debito = ld_Comsns;
                lo_AsDet.Credito = 0;
                if (ld_Comsns > 0) lo_LstAsDet.Add(lo_AsDet);

                lo_AsPrp.AsCabecera = lo_AsCab;
                lo_AsPrp.AsCabecera.AsientoDetalle = lo_LstAsDet;
                li_TrnsId = fn_GenerarAsientoFNG(lo_AsPrp);
                lb_FlagAdd = li_TrnsId != 0;
                if (lb_FlagAdd)
                {
                    go_SBOCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).SetValue(lrs_UflAsnRsp, 0, li_TrnsId.ToString());
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).SetValue(lrs_UflEstado, 0, "C");
                }

                return lb_FlagAdd;
            }
            catch (Exception)
            {
                if (go_SBOCompany.InTransaction) go_SBOCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                throw;
            }
            finally { go_SBOForm.Freeze(false); }
        }

        private int fn_GenerarAsientoFNG(AsientoProperties po_ap)
        {
            int li_Rslt = 0;
            SAPbobsCOM.JournalEntries lo_AsFacNeg = null;
            SAPbobsCOM.SBObob lo_SBOBob = null;
            double lo_TCDia = 0.0;

            try
            {
                lo_AsFacNeg = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
                lo_SBOBob = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);
                lo_AsFacNeg.ReferenceDate = po_ap.AsCabecera.FechaContabilizacion;
                lo_AsFacNeg.TaxDate = po_ap.AsCabecera.FechaDocumento;
                lo_AsFacNeg.DueDate = po_ap.AsCabecera.FechaVencimiento;
                lo_AsFacNeg.Reference = po_ap.AsCabecera.Referencia;
                lo_AsFacNeg.Reference2 = po_ap.AsCabecera.Referencia2;
                lo_AsFacNeg.Reference3 = po_ap.AsCabecera.Referencia3;
                lo_AsFacNeg.Memo = po_ap.AsCabecera.Comentarios;
                lo_AsFacNeg.TransactionCode = po_ap.AsCabecera.CodigoTransaccion;
                foreach (AsientoProperties.AsientoDetalle ad in po_ap.AsCabecera.AsientoDetalle)
                {
                    lo_AsFacNeg.Lines.AccountCode = ad.CodigoCuenta;
                    lo_AsFacNeg.Lines.ShortName = ad.NombreCuenta;
                    lo_AsFacNeg.Lines.Debit = ad.Debito;
                    lo_AsFacNeg.Lines.Credit = ad.Credito;
                    if (po_ap.AsCabecera.FCMoneda != lo_SBOBob.GetLocalCurrency().Fields.Item(0).Value.ToString().Trim())
                    {
                        lo_TCDia = lo_SBOBob.GetCurrencyRate(po_ap.AsCabecera.FCMoneda, po_ap.AsCabecera.FechaContabilizacion).Fields.Item(0).Value;
                        lo_AsFacNeg.Lines.Debit = ad.Debito * lo_TCDia;
                        lo_AsFacNeg.Lines.Credit = ad.Credito * lo_TCDia;
                        lo_AsFacNeg.Lines.FCDebit = ad.Debito;
                        lo_AsFacNeg.Lines.FCCredit = ad.Credito;
                        lo_AsFacNeg.Lines.FCCurrency = po_ap.AsCabecera.FCMoneda;
                    }
                    lo_AsFacNeg.Lines.Reference2 = ad.Referencia2;
                    lo_AsFacNeg.Lines.AdditionalReference = ad.Referencia3;
                    lo_AsFacNeg.Lines.UserFields.Fields.Item("U_STR_BsTyp").Value = ad.BaseType;
                    lo_AsFacNeg.Lines.UserFields.Fields.Item("U_STR_BsEnt").Value = ad.BaseEntry;
                    lo_AsFacNeg.Lines.Add();
                }
                if (lo_AsFacNeg.Add() != 0)
                {
                    li_Rslt = 0;
                    throw new Exception(string.Format("{0}. {1}", go_SBOCompany.GetLastErrorCode(), go_SBOCompany.GetLastErrorDescription()));
                }
                else
                    li_Rslt = int.Parse(go_SBOCompany.GetNewObjectKey());
            }
            catch (Exception) { throw; }
            return li_Rslt;
        }

        private void sb_GenerarAsiento()
        {
            string ls_TipoDoc = string.Empty, ls_NroAsiento = string.Empty, ls_NroDoc = string.Empty;
            string ls_ErrMsg = string.Empty;
            int li_DocEnt = 0, li_cntascrd = 0, li_CntChk = 0;
            SAPbobsCOM.Documents lo_Documents = null;

            try
            {
                go_SBOForm.Freeze(true);
                go_Matrix = go_SBOForm.Items.Item(lrs_MtxBatDTR).Specific;

                for (int i = 1; i <= go_Matrix.RowCount; i++)
                {
                    go_CheckBox = go_Matrix.GetCellSpecific(lrs_ClmMtxSelec, i);
                    if (go_CheckBox.Checked)
                        li_CntChk++;
                }
                if (go_Matrix.RowCount == 0)
                    throw new InvalidOperationException("No hay registros para seleccionar.");
                else if (li_CntChk == 0)
                    throw new InvalidOperationException("No ha seleccionado ningún documento.");

                var count = go_Matrix.RowCount;
                for (int i = 1; i <= count; i++)
                {
                    go_CheckBox = go_Matrix.GetCellSpecific(lrs_ClmMtxSelec, i);
                    if (!go_CheckBox.Checked)
                    {
                        go_Matrix.DeleteRow(i);
                        count--;
                        i--;
                    }
                }

                go_SBOForm.Freeze(false);
                go_Matrix.FlushToDataSource();
                sb_CargarUDO();

                go_ProgressBar = go_SBOApplication.StatusBar.CreateProgressBar("Generando asientos...", go_Matrix.RowCount, false);
                for (int i = 1; i <= go_Matrix.RowCount; i++)
                {
                    go_CheckBox = go_Matrix.GetCellSpecific(lrs_ClmMtxSelec, i);
                    ls_NroAsiento = go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).GetValue(lrs_UflAsDtr, i - 1).Trim();
                    if (string.IsNullOrEmpty(ls_NroAsiento))
                    {
                        li_DocEnt = int.Parse(((EditText)go_Matrix.GetCellSpecific(lrs_ClmMtxDocEnt, i)).Value);
                        ls_TipoDoc = ((EditText)go_Matrix.GetCellSpecific(lrs_ClmMtxTpDoc, i)).Value;
                        ls_NroDoc = ((EditText)go_Matrix.GetCellSpecific(lrs_ClmMtxNrDoc, i)).Value;

                        SAPbobsCOM.BoObjectTypes le_ObjType =
                            ls_TipoDoc.Equals("Factura de Proveedores") ? SAPbobsCOM.BoObjectTypes.oPurchaseInvoices :
                            ((ls_TipoDoc.Equals("Nota de Credito Proveedores") || ls_TipoDoc.Equals("Nota de Credito Prov")) ? SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes : SAPbobsCOM.BoObjectTypes.oPurchaseDownPayments);

                        if (ls_TipoDoc.Equals("Factura de Clientes"))
                        {
                            go_ProgressBar.Text = "Creando asiento de percepción del documento: " + ls_NroDoc;
                            ls_ErrMsg = fn_AsientoPercepcion(li_DocEnt);
                        }
                        else
                        {
                            go_ProgressBar.Text = "Creando asiento de detracción del documento: " + ls_NroDoc;
                            ls_ErrMsg = fn_AsientoDetraccion(li_DocEnt, le_ObjType);
                        }

                        if (string.IsNullOrEmpty(ls_ErrMsg))
                        {
                            string ls_BPPAsiento = ls_TipoDoc.Equals("Factura de Clientes") ? lrs_UflAstPercepcion : lrs_UflAstDetraccion;

                            lo_Documents = go_SBOCompany.GetBusinessObject(le_ObjType);
                            lo_Documents.GetByKey(li_DocEnt);
                            go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflEstDoc, i - 1, "Creado");
                            go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflAsDtr, i - 1, lo_Documents.UserFields.Fields.Item(ls_BPPAsiento).Value.ToString());

                            go_SBOApplication.StatusBar.SetText("Asiento creado con éxito.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                        }
                        else
                        {
                            go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).SetValue(lrs_UflEstDoc, i - 1, "No creado: " + ls_ErrMsg);
                            go_SBOApplication.StatusBar.SetText(ls_ErrMsg, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                        }
                        go_ProgressBar.Value++;
                    }
                }

                go_Matrix.LoadFromDataSource();
                for (int i = 0; i < go_Matrix.RowCount; i++)
                {
                    ls_NroAsiento = go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTRDET).GetValue(lrs_UflAsDtr, 0).Trim();
                    if (!string.IsNullOrEmpty(ls_NroAsiento))
                        li_cntascrd++;
                }
                if (li_cntascrd == go_Matrix.RowCount)
                {
                    go_SBOForm.Items.Item(lrs_MtxBatDTR).Enabled = false;
                    go_SBOForm.DataSources.DBDataSources.Item(lrs_DtcBATDTR).SetValue(lrs_UflEstado, 0, "C");
                }
                else
                    go_SBOApplication.StatusBar.SetText("Este documento permanecerá abierto, hasta que se generen todos los asientos seleccionados", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
                go_SBOForm.Freeze(false);
                if (go_ProgressBar != null) go_ProgressBar.Stop();
                go_ProgressBar = null;
                GC.Collect();
            }
        }

        private string fn_AsientoDetraccion(int pri_DocEntry, SAPbobsCOM.BoObjectTypes pre_ObjType)
        {
            SAPbobsCOM.Documents lo_Documents = null;
            SAPbobsCOM.JournalEntries lo_Journal = null;
            SAPbobsCOM.JournalEntries lo_JournalDoc = null;
            SAPbobsCOM.BusinessPartners lo_BusinessPartners = null;
            SAPbobsCOM.Recordset recordset = null; // Nuevo

            int li_NumLine = 0, li_DiasDtr = 0;
            double ldb_SumDetrD = 0, ldb_SumDetrH = 0, ldb_SumDetrDME = 0, ldb_SumDetrCME = 0;
            string ls_AccountCode = string.Empty;
            int ErrCode = 0;
            string ErrMsg = string.Empty;
            string Sucursal = string.Empty;
            try
            {
                lo_Documents = go_SBOCompany.GetBusinessObject(pre_ObjType);
                lo_Journal = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
                lo_JournalDoc = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
                lo_BusinessPartners = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
                lo_Documents.GetByKey(pri_DocEntry);

                // ------ Validar SI Sociedad tiene configuracion de Sucursales
                 recordset = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string query = $"SELECT \"MltpBrnchs\" FROM OADM" ;
                Cls_Global.WriteToFile(query);
                recordset.DoQuery(query);
                Sucursal = recordset.Fields.Item(0).Value;
                //-------------------------------------------------------

                if (lo_Documents.WithholdingTaxData.Count > 0)
                {
                    lo_Documents.WithholdingTaxData.SetCurrentLine(0);
                    lo_JournalDoc.GetByKey(lo_Documents.TransNum);
                    lo_BusinessPartners.GetByKey(lo_Documents.CardCode);

                    if (!lo_Documents.WithholdingTaxData.WTCode.StartsWith("D"))
                        return "La retencion de impuestos no corresponde a la detraccion";

                    li_DiasDtr = Cls_QueryManager.Retorna(Cls_Query.get_AddonConfig, "U_BPP_NDiasDtrac");

                    lo_Journal.ReferenceDate = lo_Documents.DocDate;
                    DateTime dFchVencAs;
                    if (lo_Documents.DocDueDate.Month == 12)
                        dFchVencAs = lo_Documents.DocDueDate.AddYears(1).AddMonths(-lo_Documents.DocDueDate.Month + 1).AddDays(-lo_Documents.DocDueDate.Day + li_DiasDtr);
                    else
                        dFchVencAs = lo_Documents.DocDueDate.AddMonths(1).AddDays(-lo_Documents.DocDueDate.Day + li_DiasDtr);

                    lo_Journal.DueDate = dFchVencAs;
                    lo_Journal.TaxDate = lo_Documents.TaxDate;
                    lo_Journal.Reference = lo_JournalDoc.Reference;
                    lo_Journal.Reference2 = lo_Documents.NumAtCard;
                    lo_Journal.Reference3 = lo_Documents.WithholdingTaxData.WTCode;
                    lo_Journal.Memo = "Detraccion Factura - " + lo_JournalDoc.Reference2;
                    lo_Journal.TransactionCode = "DTR";
                    lo_Journal.UserFields.Fields.Item("U_BPP_DocKeyDest").Value = lo_Documents.DocEntry.ToString();
                    lo_Journal.UserFields.Fields.Item("U_BPP_CtaTdoc").Value = lo_Documents.DocObjectCodeEx.ToString();
                    lo_Journal.UserFields.Fields.Item("U_BPP_SubTDoc").Value = ((int)lo_Documents.DocumentSubType).ToString();
                    while (li_NumLine < lo_JournalDoc.Lines.Count)
                    {
                        lo_JournalDoc.Lines.SetCurrentLine(li_NumLine);
                        if (lo_JournalDoc.Lines.AccountCode == lo_Documents.WithholdingTaxData.GLAccount)
                        {
                            ldb_SumDetrD += lo_JournalDoc.Lines.Debit;
                            ldb_SumDetrH += lo_JournalDoc.Lines.Credit;
                            if (Cls_Global.metCalculoTC == "E") // Opcion Standar
                            {
                                if ((lo_JournalDoc.Lines.FCDebit + lo_JournalDoc.Lines.FCCredit) > 0)
                                {
                                    ldb_SumDetrDME += lo_JournalDoc.Lines.FCDebit;
                                    ldb_SumDetrCME += lo_JournalDoc.Lines.FCCredit;
                                }
                            }
                        }
                        li_NumLine++;
                    }
                    if (ldb_SumDetrD + ldb_SumDetrH == 0)
                        return "El monto es 0";

                    if (Sucursal.Equals("Y"))// Agregado 21/01/2022
                        lo_Journal.Lines.BPLID = lo_Documents.BPL_IDAssignedToInvoice;
                    lo_Journal.Lines.Reference2 = lo_Documents.NumAtCard; // Agregado 25/01/2022

                    lo_Journal.Lines.AccountCode = lo_Documents.WithholdingTaxData.GLAccount;
                    lo_Journal.Lines.Debit = ldb_SumDetrH;
                    lo_Journal.Lines.Credit = ldb_SumDetrD;
                    if ((ldb_SumDetrDME + ldb_SumDetrCME) > 0)
                    {
                        if (Cls_Global.metCalculoTC == "E") // Opcion Standar
                        {
                            lo_Journal.Lines.FCDebit = ldb_SumDetrCME;
                            lo_Journal.Lines.FCCredit = ldb_SumDetrDME;
                            lo_Journal.Lines.FCCurrency = lo_Documents.DocCurrency;
                        }
                    }
                    lo_Journal.Lines.Add(); //Linea 1
                    if (lo_BusinessPartners.UserFields.Fields.Item("U_BPP_CtaDetrac").Value.ToString() == string.Empty)
                        return "No se ha definido cuenta de detracción";

                    ls_AccountCode = Cls_QueryManager.Retorna(Cls_Query.get_CuentaMayor, "AcctCode", lo_BusinessPartners.UserFields.Fields.Item("U_BPP_CtaDetrac").Value);
                    lo_Journal.Lines.AccountCode = ls_AccountCode;
                    lo_Journal.Lines.ShortName = lo_BusinessPartners.CardCode;
                    lo_Journal.Lines.Debit = ldb_SumDetrD;
                    lo_Journal.Lines.Credit = ldb_SumDetrH;
                    if ((ldb_SumDetrDME + ldb_SumDetrCME) > 0)
                    {
                        if (Cls_Global.metCalculoTC == "E") // Opcion Standar
                        {
                            lo_Journal.Lines.FCDebit = ldb_SumDetrDME;
                            lo_Journal.Lines.FCCredit = ldb_SumDetrCME;
                            lo_Journal.Lines.FCCurrency = lo_Documents.DocCurrency;
                        }
                    }

                    if (Sucursal.Equals("Y")) // Agregado 21/01/2022
                        lo_Journal.Lines.BPLID = lo_Documents.BPL_IDAssignedToInvoice;
                    lo_Journal.Lines.Reference2 = lo_Documents.NumAtCard; // Agregado 25/01/2022

                    lo_Journal.Lines.Add(); // Linea 2
                    if (lo_Journal.Add() != 0)
                    {
                        go_SBOCompany.GetLastError(out ErrCode, out ErrMsg);
                        return ErrCode.ToString() + " " + ErrMsg;
                    }
                    lo_Documents.UserFields.Fields.Item("U_BPP_AstDetrac").Value = go_SBOCompany.GetNewObjectKey();
                    if (lo_Documents.Update() != 0)
                    {
                        go_SBOCompany.GetLastError(out ErrCode, out ErrMsg);
                        return ErrCode.ToString() + " " + ErrMsg;
                    }
                }
                return string.Empty;
            }
            finally
            {
                lo_Documents = null;
                lo_Journal = null;
                lo_JournalDoc = null;
                lo_BusinessPartners = null;
                //GC.Collect();
            }
        }

        private string fn_AsientoPercepcion(int pri_DocEntry)
        {
            try
            {
                SAPbobsCOM.Documents lo_Documents;
                SAPbobsCOM.JournalEntries lo_Journal;
                SAPbobsCOM.JournalEntries lo_JournalDoc;
                SAPbobsCOM.BusinessPartners lo_BusinessPartners;
                SAPbobsCOM.Recordset recordset = null; // Nuevo
                string ls_AccountCode = string.Empty;
                int ErrCode;
                string ErrMsg = string.Empty;
                string Sucursal = string.Empty;

                lo_Journal = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
                lo_JournalDoc = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);

                lo_Documents = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                lo_BusinessPartners = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);

                // ------ Validar SI Sociedad tiene configuracion de Sucursales
                recordset = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string query = $"SELECT \"MltpBrnchs\" FROM OADM";
                Cls_Global.WriteToFile(query);
                recordset.DoQuery(query);
                Sucursal = recordset.Fields.Item(0).Value;
                //-------------------------------------------------------

                lo_Documents.GetByKey(pri_DocEntry);
                lo_JournalDoc.GetByKey(lo_Documents.TransNum);
                lo_BusinessPartners.GetByKey(lo_Documents.CardCode);
                lo_Journal.ReferenceDate = lo_Documents.DocDate;
                lo_Journal.TaxDate = lo_Documents.TaxDate;
                lo_Journal.ReferenceDate = lo_Documents.DocDate;
                lo_Journal.DueDate = lo_Documents.DocDueDate;
                lo_Journal.Reference = lo_JournalDoc.Reference;
                lo_Journal.Reference2 = lo_Documents.NumAtCard;
                lo_Journal.Reference3 = lo_Documents.WithholdingTaxData.WTCode;
                lo_Journal.Memo = "Monto de la Percepcion";
                lo_Journal.TransactionCode = "PRC";
                lo_Journal.UserFields.Fields.Item("U_BPP_DocKeyDest").Value = lo_Documents.DocEntry.ToString();
                lo_Journal.UserFields.Fields.Item("U_BPP_CtaTdoc").Value = lo_Documents.DocObjectCode.ToString();
                lo_Journal.UserFields.Fields.Item("U_BPP_SubTDoc").Value = lo_Documents.DocumentSubType.ToString();

                ls_AccountCode = Cls_QueryManager.Retorna(Cls_Query.get_CuentaMayor, "AcctCode", lo_BusinessPartners.UserFields.Fields.Item("U_BPP_CtPc").Value);
                if (string.IsNullOrEmpty(ls_AccountCode))
                    return "No se generó el asiento de percepción debido a que no se configuró la cuenta respectiva en el socio de negocio.";

                if (lo_Documents.UserFields.Fields.Item("U_BPP_TtPc").Value = 0)
                    return "El monto de la Percepcion es cero";

                if (lo_Documents.DocCurrency == Cls_Global.sb_ObtenerMonedaLocal())
                {
                    if (lo_JournalDoc.Lines.Debit > 0)
                    {
                        lo_Journal.Lines.AccountCode = lo_JournalDoc.Lines.AccountCode;
                        lo_Journal.Lines.ShortName = lo_BusinessPartners.CardCode;
                        lo_Journal.Lines.Debit = lo_Documents.UserFields.Fields.Item("U_BPP_TtPc").Value;
                        lo_Journal.Lines.Credit = 0;
                        if (Sucursal.Equals("Y")) // Agregado 21/01/2022
                            lo_Journal.Lines.BPLID = lo_Documents.BPL_IDAssignedToInvoice;
                        lo_Journal.Lines.Add();

                        lo_Journal.Lines.AccountCode = go_RecordSet.Fields.Item("AcctCode").Value;
                        lo_Journal.Lines.Credit = lo_Documents.UserFields.Fields.Item("U_BPP_TtPc").Value;
                        lo_Journal.Lines.Debit = 0;
                        if (Sucursal.Equals("Y")) // Agregado 21/01/2022
                            lo_Journal.Lines.BPLID = lo_Documents.BPL_IDAssignedToInvoice;
                        lo_Journal.Lines.Add();
                    }
                    if (lo_JournalDoc.Lines.Credit > 0)
                    {
                        lo_Journal.Lines.AccountCode = lo_JournalDoc.Lines.AccountCode;
                        lo_Journal.Lines.ShortName = lo_BusinessPartners.CardCode;
                        lo_Journal.Lines.Credit = lo_Documents.UserFields.Fields.Item("U_BPP_TtPc").Value;
                        lo_Journal.Lines.Debit = 0;
                        if (Sucursal.Equals("Y")) // Agregado 21/01/2022
                            lo_Journal.Lines.BPLID = lo_Documents.BPL_IDAssignedToInvoice;
                        lo_Journal.Lines.Add();

                        lo_Journal.Lines.AccountCode = go_RecordSet.Fields.Item("AcctCode").Value;
                        lo_Journal.Lines.Debit = lo_Documents.UserFields.Fields.Item("U_BPP_TtPc").Value;
                        lo_Journal.Lines.Credit = 0;
                        if (Sucursal.Equals("Y")) // Agregado 21/01/2022
                            lo_Journal.Lines.BPLID = lo_Documents.BPL_IDAssignedToInvoice;
                        lo_Journal.Lines.Add();
                    }
                    else
                    {
                        if (lo_JournalDoc.Lines.FCDebit > 0)
                        {
                            lo_Journal.Lines.AccountCode = lo_JournalDoc.Lines.AccountCode;
                            lo_Journal.Lines.ShortName = lo_BusinessPartners.CardCode;
                            lo_Journal.Lines.FCDebit = lo_Documents.UserFields.Fields.Item("U_BPP_TtPc").Value;
                            lo_Journal.Lines.FCCredit = 0;
                            lo_Journal.Lines.FCCurrency = lo_Documents.DocCurrency;
                            if (Sucursal.Equals("Y")) // Agregado 21/01/2022
                                lo_Journal.Lines.BPLID = lo_Documents.BPL_IDAssignedToInvoice;
                            lo_Journal.Lines.Add();

                            lo_Journal.Lines.AccountCode = go_RecordSet.Fields.Item("AcctCode").Value;
                            lo_Journal.Lines.FCCredit = lo_Documents.UserFields.Fields.Item("U_BPP_TtPc").Value;
                            lo_Journal.Lines.FCDebit = 0;
                            lo_Journal.Lines.FCCurrency = lo_Documents.DocCurrency;
                            if (Sucursal.Equals("Y")) // Agregado 21/01/2022
                                lo_Journal.Lines.BPLID = lo_Documents.BPL_IDAssignedToInvoice;
                            lo_Journal.Lines.Add();
                        }
                        if (lo_JournalDoc.Lines.FCCredit > 0)
                        {
                            lo_Journal.Lines.AccountCode = lo_JournalDoc.Lines.AccountCode;
                            lo_Journal.Lines.ShortName = lo_BusinessPartners.CardCode;
                            lo_Journal.Lines.FCCredit = lo_Documents.UserFields.Fields.Item("U_BPP_TtPc").Value;
                            lo_Journal.Lines.FCDebit = 0;
                            lo_Journal.Lines.FCCurrency = lo_Documents.DocCurrency;
                            if (Sucursal.Equals("Y")) // Agregado 21/01/2022
                                lo_Journal.Lines.BPLID = lo_Documents.BPL_IDAssignedToInvoice;
                            lo_Journal.Lines.Add();

                            lo_Journal.Lines.AccountCode = go_RecordSet.Fields.Item("AcctCode").Value;
                            lo_Journal.Lines.FCDebit = lo_Documents.UserFields.Fields.Item("U_BPP_TtPc").Value;
                            lo_Journal.Lines.FCCredit = 0;
                            lo_Journal.Lines.FCCurrency = lo_Documents.DocCurrency;
                            if (Sucursal.Equals("Y")) // Agregado 21/01/2022
                                lo_Journal.Lines.BPLID = lo_Documents.BPL_IDAssignedToInvoice;
                            lo_Journal.Lines.Add();
                        }
                    }
                }

                if (lo_Journal.Add() != 0)
                {
                    go_SBOCompany.GetLastError(out ErrCode, out ErrMsg);
                    return ErrCode.ToString() + " - " + ErrMsg;
                }
                lo_Documents.UserFields.Fields.Item("U_BPP_AsPc").Value = go_SBOCompany.GetNewObjectKey();
                if (lo_Documents.Update() != 0)
                {
                    go_SBOCompany.GetLastError(out ErrCode, out ErrMsg);
                    return ErrCode.ToString() + " - " + ErrMsg;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion SBOForm
    }
}