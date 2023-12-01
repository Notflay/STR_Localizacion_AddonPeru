using SAPbobsCOM;
using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;

namespace STR_Localizacion.UI
{
    partial class Cls_AnulCorrelativo : Cls_PropertiesControl
    {
        private Folder pnlGeneral;
        private Folder pnlDetalle;
        private ComboBox cboDetalle;
        private string serie = string.Empty;

        public Cls_AnulCorrelativo()
        {
            gs_FormName = "frAnulCorr";
            gs_FormPath = "Resources/Localizacion/AnularCorrelativos.srf";
            lc_NameClass = "Cls_AnulCorrelativo";
        }

        public void sb_FormLoad()
        {
            try
            {
                if (go_SBOForm == null)
                {
                    go_SBOForm = Cls_Global.fn_CreateForm(gs_FormName, gs_FormPath);
                    ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
                    if (!sb_DataFormLoad())
                        return;

                    InitializeEvents();
                }
                go_SBOForm.Visible = true;
            }
            catch (Exception exc)
            {
                go_SBOApplication.SetStatusBarMessage(exc.Message, BoMessageTime.bmt_Short, true);
            }
        }

        #region SBOFormControles

        /// <Carga formulario desde un archivo SRF>
        /// Carga los valores del formulario mediante un archivo .srf
        /// </>
        /// <param name="go_SBOApplication"></param>
        private bool sb_DataFormLoad()
        {
            lc_NameMethod = "sb_DataFormLoad";
            go_Combo = go_SBOForm.Items.Item("cboSeries").Specific;
            try
            {
                try
                {
                    //Inicia el formulario en modo Registrar, importante para evitar que los controles se bloqueen luego de asignarles estados por defecto.
                    go_SBOForm.Mode = BoFormMode.fm_ADD_MODE;
                    //Carga datos en ComboBox
                    go_Combo.ValidValues.LoadSeries(go_SBOForm.BusinessObject.Type, BoSeriesMode.sf_Add);
                    if (go_Combo.Selected == null && go_Combo.ValidValues.Count > 0)
                        go_SBOForm.DataSources.DBDataSources.Item("@BPP_ANULCORR").SetValue("Series", 0, go_Combo.ValidValues.Item(0).Value);
                    else
                        serie = go_Combo.Selected.Value; //Recupara el número de serie del go_Combo
                }
                catch (Exception)
                {
                    go_SBOApplication.MessageBox("Debe definir la serie para el periodo actual"); //Muestra ventana con mensaje de excepción
                    go_SBOForm.Close(); //Cierra el formulario
                    Dispose();
                    return false;
                }

                try
                {
                    //Recupera controles del formulario
                    go_SBOForm.DataSources.DBDataSources.Item("@BPP_ANULCORR").SetValue("DocNum", 0, go_SBOForm.BusinessObject.GetNextSerialNumber(serie, go_SBOForm.BusinessObject.Type).ToString());
                    go_Edit = go_SBOForm.Items.Item("edtTpDoc").Specific;
                    go_Edit.Value = "Venta";
                    go_Matrix = go_SBOForm.Items.Item("mtxDetalle").Specific;
                    go_Matrix.AddRow();

                    cboDetalle = go_Matrix.Columns.Item("clmEstado").Cells.Item(1).Specific;
                    cboDetalle.ValidValues.Add("0", "Anulado");
                    cboDetalle.Select(0, BoSearchKey.psk_Index);
                    sb_CargarTipoSunat(); //Llena los datos del comboBox
                    go_Matrix.DeleteRow(1); //Eliminar una fila de la Matrix
                    //Limpia el valor de número de correlativo
                    go_Edit = go_SBOForm.Items.Item("txNumCorrD").Specific;
                    go_Edit.Value = string.Empty;
                    //Asigna valores a las propiedades del formulario
                    go_SBOForm.DataBrowser.BrowseBy = "txDocEntry";
                    go_SBOForm.AutoManaged = true;
                    go_SBOForm.PaneLevel = 1;
                    //Recupera los valores de los paneles
                    pnlGeneral = go_SBOForm.Items.Item("pnlGeneral").Specific;
                    pnlDetalle = go_SBOForm.Items.Item("pnlDetalle").Specific;
                    pnlGeneral.GroupWith("pnlDetalle");
                    pnlGeneral.Select(); //Selecciona el panel general por defecto
                    //go_SBOForm.Freeze(true); // Descongela el formulario
                    go_SBOForm.SupportedModes = Convert.ToInt32(BoAutoFormMode.afm_All);
                    //Recorre los tipos de controle que hay en el formulario
                    foreach (Item item in go_SBOForm.Items)
                    {
                        if (item.Type == BoFormItemTypes.it_EDIT || item.Type == BoFormItemTypes.it_COMBO_BOX)
                            if (item.UniqueID != "edtDocNum") go_SBOForm.Items.Item(item.UniqueID).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 9, BoModeVisualBehavior.mvb_False);
                    }
                    go_Item = go_SBOForm.Items.Item("edtDocNum");
                    go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_True);
                }
                catch (Exception ex) { go_SBOApplication.MessageBox(ex.Message); } //Muestra una ventana con el mensaje de Excepción
                finally { go_SBOForm.Freeze(false); } //Descongela el formulario
            }
            catch (Exception ex) { ExceptionPrepared.inner(ex.Message, lc_NameMethod); ExceptionPrepared.SaveInLog(false); } //Método para el manejo de las operaciones de Log

            return true;
        }

        #endregion SBOFormControles

        #region Metodos del Negocio

        /// <Carga los comprobantes de ventas que pueden ser anulados>
        /// </>
        /// <param name="go_SBOForm"></param>
        private void sb_CargarTipoSunat()
        {
            lc_NameMethod = "sb_CargarTipoSunat"; //Se asigna el nombre del método para la identificación del mismo
            try
            {
                //Carga datos en el ComboBox
                go_Combo = go_SBOForm.Items.Item("cbTpSUNAT").Specific;
                Cls_Global.sb_comboLlenar(go_Combo, Cls_QueryManager.Retorna(Cls_Query.get_TipoXFormularioAnulado));
            }
            catch (Exception ex) { ExceptionPrepared.inner(ex.Message, lc_NameMethod); ExceptionPrepared.SaveInLog(false); }
        }

        /// <Carga la Series Sunat según el tipo de comprobante>
        /// </>
        /// <param name="go_SBOForm"></param>
        /// <param name="go_SBOApplication"></param>
        /// <returns></returns>
        private bool fn_ActualizarSerieSunat()
        {
            lc_NameMethod = "fn_ActualizarSerieSunat"; //Se asigna el nombre del método para la identificación del mismo
            try
            {
                string ls_ComboTipo;
                GC.Collect(); //Libera la memoria
                //Recupera valores de ComboBox
                go_Combo = go_SBOForm.Items.Item("cbTpSUNAT").Specific;
                ls_ComboTipo = go_Combo.Value.Trim();
                go_Combo = go_SBOForm.Items.Item("cbSerie").Specific;

                //Recupera datos en el recordSet
                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_SerieSunatAnulado, null, ls_ComboTipo);
                //Carga valores en el ComboBox
                Cls_Global.sb_comboLlenar(go_Combo, go_RecordSet);
                if (go_RecordSet.RecordCount > 0)
                    go_Combo.Select(0, BoSearchKey.psk_Index);
                return true;
            }
            catch (Exception ex) { ExceptionPrepared.inner(ex.Message, lc_NameMethod); ExceptionPrepared.SaveInLog(false); return false; } //Método para el manejo de las operaciones de Log
        }

        /// <Muestra los datos en el detalle>
        /// </>
        /// <param name="go_SBOForm"></param>
        /// <param name="go_SBOApplication"></param>
        /// <returns></returns>
        private void sb_DetalleAnulacion()
        {
            //variables del método
            string ls_CorrAnul, ls_NumCorrD, ls_NumCorrH;
            int li_NumCorrD, li_NumCorrH;
            int li_fila = 0;
            go_Matrix = go_SBOForm.Items.Item("mtxDetalle").Specific;
            //Valida que el correlativo se pueda anular sino, limpia la Matrix
            try
            {
                sb_ValidarAnulacion();
            }
            catch (Exception)
            {
                go_Matrix.Clear();
                throw;
            }
            //Guarda el primer y último número de correlativo ingresado
            ls_NumCorrD = ((EditText)go_SBOForm.Items.Item("txNumCorrD").Specific).Value.Trim();
            li_NumCorrD = int.Parse(ls_NumCorrD);
            ls_NumCorrH = ((EditText)go_SBOForm.Items.Item("txNumCorrH").Specific).Value.Trim();
            li_NumCorrH = int.Parse(ls_NumCorrH);
            go_Matrix.Clear();
            for (int i = li_NumCorrD; i <= li_NumCorrH; i++)
            {
                go_Matrix.AddRow(1); //Agrega una fila a la matrix
                ls_CorrAnul = i.ToString();
                for (int j = (i.ToString().Length); j <= ls_NumCorrD.Length - 1; j++)
                {
                    ls_CorrAnul = "0" + ls_CorrAnul;
                }
                go_Matrix.FlushToDataSource();
                Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, "@BPP_ANULCORRDET", "U_BPP_NmCr", ls_CorrAnul, li_fila);
                Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, "@BPP_ANULCORRDET", "U_BPP_Estd", "0", li_fila);
                go_Matrix.LoadFromDataSource();
                li_fila++;
            }
        }

        /// <Verifica que el rango de correlativos se pueda anular>
        /// </>
        /// <param name="go_SBOForm"></param>
        /// <param name="go_SBOApplication"></param>
        /// <returns></returns>
        private void sb_ValidarAnulacion()
        {
            //Variables del método
            //bool lb_Result = false;
            string ls_UltCorr = string.Empty;
            DateTime ld_UltFecha;
            DateTime ld_fchAnul;

            Recordset lo_recordSet;
            lo_recordSet = Cls_Global.go_SBOCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

            string ls_TpSUNAT, ls_Serie,
                   ls_NumCorrD, ls_NumCorrH;
            int li_NumCorrD, li_NumCorrH, ln_Value;

            //Recupera valor de los controles
            go_Combo = go_SBOForm.Items.Item("cbTpSUNAT").Specific;
            ls_TpSUNAT = go_Combo.Value.Trim();

            go_Combo = go_SBOForm.Items.Item("cbSerie").Specific;
            ls_Serie = go_Combo.Value.Trim();

            go_Edit = go_SBOForm.Items.Item("txNumCorrD").Specific;
            ls_NumCorrD = go_Edit.Value.Trim();

            go_Edit = go_SBOForm.Items.Item("txNumCorrH").Specific;
            ls_NumCorrH = go_Edit.Value.Trim();

            //Valida que el rango de correlativos ingresados se puedan anular
            if (ls_TpSUNAT == string.Empty)
                throw new InvalidOperationException("Debe seleccionar el tipo de documento SUNAT.");
            else if (ls_Serie == string.Empty)
                throw new InvalidOperationException("Debe seleccionar la serie de documento SUNAT.");
            else if (ls_NumCorrD == string.Empty || ls_NumCorrH == string.Empty)
                throw new InvalidOperationException("Debe ingresar el número de correlativo SUNAT.");
            else if (!int.TryParse(ls_NumCorrD, out ln_Value) || !int.TryParse(ls_NumCorrH, out ln_Value))
                throw new InvalidOperationException("El valor ingresado en el Número de Correlativo, no es válido.");
            else
            {
                li_NumCorrD = int.Parse(ls_NumCorrD);
                li_NumCorrH = int.Parse(ls_NumCorrH);
            }

            go_Edit = go_SBOForm.Items.Item("txFchAnul").Specific;
            if (go_Edit.Value != string.Empty)
                ld_fchAnul = Cls_Global.fn_Format_StringToDate(Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, "@BPP_ANULCORR", "U_BPP_FchAnl")).Date;
            else
                throw new InvalidOperationException("Debe ingresar la fecha de anulación.");
            //Recupera la fecha del último correlativo anulado
            ld_UltFecha = (DateTime)Cls_QueryManager.Retorna(Cls_Query.get_FechaAnulacion, "U_BPP_FchAnl", ls_TpSUNAT, ls_Serie);
            if (ld_fchAnul < ld_UltFecha)
                throw new InvalidOperationException("La fecha de anulación no puede ser menor a la última fecha de anulación registrada para este tipo y serie de documento");
            //se carga el recordset
            ls_UltCorr = (string)Cls_QueryManager.Retorna(Cls_Query.validate_AnulacionSunat, "U_BPP_NDCD", ls_TpSUNAT, ls_Serie);
            if (ls_NumCorrD.Length < ls_UltCorr.Length || ls_NumCorrH.Length < ls_UltCorr.Length)
                throw new InvalidOperationException("La longitud de los números correlativos no puede ser menor a la longitud de los números correlativos usados por esta serie y por este tipo de documento");

            //Validación del registro
            string ls_CorrAnul;

            for (int i = li_NumCorrD; i <= li_NumCorrH; i++)
            {
                ls_CorrAnul = i.ToString();
                for (int j = i.ToString().Length; j <= ls_NumCorrD.Length - 1; j++)
                {
                    ls_CorrAnul = "0" + ls_CorrAnul;
                }
                //Verifica que el número de correlativo no haya pasado al sistema
                string ls_Msj = Cls_QueryManager.Retorna(Cls_Query.validate_RegistroSAP, (int)0, ls_TpSUNAT, ls_Serie, ls_CorrAnul);

                if (!string.IsNullOrEmpty(ls_Msj))
                    throw new InvalidOperationException(ls_Msj);
            }
        }

        #endregion Metodos del Negocio
    }
}