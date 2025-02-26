using SAPbobsCOM;
using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace STR_Localizacion.UI
{
    partial class Cls_PagoMasivoDetraccion : Cls_PropertiesControl
    {
        public Cls_PagoMasivoDetraccion()
        {
            gs_FormName = "frmPagoMasivoDetraccion";
            gs_FormPath = "Resources/Localizacion/PagarDetracciones.srf";
            lc_NameClass = "Cls_PagoMasivoDetraccion";
        }

        public void sb_FormLoad()
        {
            try
            {
                if (go_SBOForm == null)
                {
                    // Que el sistema detecté el tipo de Cambio según el formato
                    CultureInfo culturaPersonalizada = new CultureInfo("es-PE");
                    culturaPersonalizada.NumberFormat.NumberDecimalSeparator = ".";
                    culturaPersonalizada.NumberFormat.NumberGroupSeparator = ",";
                    System.Threading.Thread.CurrentThread.CurrentCulture = culturaPersonalizada;

                    go_SBOForm = Cls_Global.fn_CreateForm(gs_FormName, gs_FormPath);
                    ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
                    sb_DataFormLoad();
                    Sb_LoadLogo();
                    InitializeEvents();
                }
            }
            catch (Exception exc)
            {
                go_SBOApplication.SetStatusBarMessage(exc.Message, BoMessageTime.bmt_Short, true);
            }
            finally { go_SBOForm.Visible = true; }
        }

        #region SBOFormControles

        private void sb_DataFormLoad()
        {
            lc_NameMethod = "sb_CreacionFormulario"; //Se asigna el nombre del método para la identificación del mismo
            string ls_serie = string.Empty;

            try
            {
                //go_SBOForm.Freeze(true); //Congela el formulario
                go_SBOForm.Items.Item(this.lrs_MtxPayDTRDET).Visible = false; //Oculta el control Matrix
                go_SBOForm.Height = 410;
                try
                {
                    go_SBOForm.Items.Item(this.lrs_EdtDocEntry).Visible = false;
                    //Recupera los valores de los controles del formulario al comboBox
                    go_Combo = go_SBOForm.Items.Item(this.lrs_CmbSeries).Specific;
                    go_Edit = go_SBOForm.Items.Item(this.lrs_EdtDocNum).Specific;
                    go_Combo.ValidValues.LoadSeries(go_SBOForm.BusinessObject.Type, BoSeriesMode.sf_Add);
                    if (go_Combo.Selected == null && go_Combo.ValidValues.Count > 0)
                        go_SBOForm.DataSources.DBDataSources.Item(this.lrs_DtcPAYDTR).SetValue(this.lrs_UflSeries, 0, go_Combo.ValidValues.Item(0).Value);
                    else
                        ls_serie = go_Combo.Selected.Value;

                    //Asigna un nuevo valor al DocNum
                    go_SBOForm.DataSources.DBDataSources.Item(this.lrs_DtcPAYDTR).SetValue(this.lrs_UflDocNum, 0, go_SBOForm.BusinessObject.GetNextSerialNumber(ls_serie, go_SBOForm.BusinessObject.Type).ToString());

                    //Añade datatable cuando se carga el formulario
                    go_SBOForm.DataSources.DataTables.Add(this.lrs_DttAux);
                    go_SBOForm.Items.Item(this.lrs_BtnCrear).Enabled = false;
                    go_SBOForm.Items.Item(this.lrs_BtnTXT).Enabled = false;
                }
                catch (Exception)
                {
                    go_SBOApplication.MessageBox("Debe definir la serie para el periodo actual"); //Muestra una ventana con el mensaje de Excepción
                    go_SBOForm.Close(); //Cierra el formulario
                    return;
                }
                //Asigna el manejo automatico de los atributos de los control
                string[] ls_NomControl = new string[] { this.lrs_EdtPrvDd,   this.lrs_EdtPrvHt,
                                                        this.lrs_EdtFchCnDd, this.lrs_EdtFchCnHt,
                                                        this.lrs_EdtFchVnDd, this.lrs_EdtFchVnHt,
                                                        this.lrs_CmbSeries,  this.lrs_BtnBuscar};
                foreach (string Name in ls_NomControl)
                {
                    go_SBOForm.Items.Item(Name).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, (int)BoAutoFormMode.afm_Ok, BoModeVisualBehavior.mvb_False);
                    go_SBOForm.Items.Item(lrs_CmbArticulo).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, (int)BoAutoFormMode.afm_Ok, BoModeVisualBehavior.mvb_True);
                }

                go_SBOForm.Items.Item(this.lrs_EdtCtaTrs).Left = go_SBOForm.Items.Item(this.lrs_EdtPrvDd).Left;
                go_SBOForm.DataBrowser.BrowseBy = this.lrs_EdtDocEntry;
                go_SBOForm.Items.Item(this.lrs_CmbArticulo).Left = go_SBOForm.Items.Item(this.lrs_EdtCtaTrs).Left;
                go_SBOForm.Items.Item(this.lrs_lblDesCtaTrs).Left = go_SBOForm.Items.Item("28").Left - 15;

                go_Combo = go_SBOForm.Items.Item(this.lrs_CmbArticulo).Specific;
                //Ejecuta la consulta en el go_recordSet
                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_NombreDocumentoFlujoCaja);
                //Carga los datos del recordSet en el ComboBox
                
                Cls_Global.sb_comboLlenar(go_Combo, go_RecordSet);
                go_Combo.ValidValues.Add("---", "---");
                //Asigna la posición de los controles
                go_SBOForm.Items.Item(this.lrs_EdtFchDp).Left = go_SBOForm.Items.Item(this.lrs_EdtCtaTrs).Left;
                go_SBOForm.Items.Item(this.lrs_EdtNumDp).Left = go_SBOForm.Items.Item(this.lrs_EdtCtaTrs).Left;
                go_SBOForm.Items.Item("26").Left = go_SBOForm.Items.Item("17").Left;

                go_SBOForm.DataSources.DBDataSources.Item(this.lrs_DtcPAYDTR).SetValue(this.lrs_UflCrtDate, 0, DateTime.Now.ToString(this.lrs_FchFormat));
                go_SBOForm.DataSources.DBDataSources.Item(this.lrs_DtcPAYDTR).SetValue(this.lrs_UflFchCnPg, 0, DateTime.Now.ToString(this.lrs_FchFormat));
            }
            catch (Exception ex) { ExceptionPrepared.inner(ex.Message, lc_NameMethod); ExceptionPrepared.SaveInLog(false); } //Método para el manejo de las operaciones de Log
            finally { go_SBOForm.Freeze(false); } //Descongela el Formulario
        }

        /// <Prepara los controles para la carga de datos>
        /// Se inicia cuando el estado del pago esta abierto, se preparan los controles y se
        /// cargan los datos respectivos.
        /// </summary>
        /// <param name="go_SBOApplication"></param>
        /// <param name="go_SBOForm"></param>
        private void sb_prepararControlesEstadoAbierto()
        {
            try
            {
                //Recupera valores de los controles
                go_Combo = go_SBOForm.Items.Item(this.lrs_CmbEstado).Specific;
                go_Grid = go_SBOForm.Items.Item(this.lrs_GrdPayDTRDET).Specific;
                go_Grid.DataTable.Clear();
                go_Edit = go_SBOForm.Items.Item(this.lrs_EdtCtaTrs).Specific;

                go_SBOForm.GetStaticText(this.lrs_lblDesCtaTrs).Caption =
                    (string)Cls_QueryManager.Retorna(Cls_Query.get_NombreCuenta, "AcctName", go_Edit.Value);

                //Recupera el valor del Control EditText del formulario
                go_Edit = go_SBOForm.Items.Item(this.lrs_EdtDocEntry).Specific;
                go_SBOForm.Items.Item(this.lrs_BtnBuscar).Enabled = false; //Deshabilita el Control de búsqueda

                //Recupera valores del go_Grid
                go_Grid = go_SBOForm.Items.Item(this.lrs_GrdPayDTRDET).Specific;
                //Ejecuto el Procedimiento en el go_Grid
                go_Grid.DataTable.Consulta(Cls_Query.get_DetraccionesPagadas, go_Edit.Value);

               
                //Asigno valores a las propiedades del Grid global
                go_Grid.AutoResizeColumns();
                go_Grid.CollapseLevel = 1;

                //Recupero los valores de las columas del go_Grid
                go_EditColumn = (EditTextColumn)go_Grid.Columns.Item(this.lrs_CabDttCodTrn);
                go_EditColumn.LinkedObjectType = Cls_Global.metCalculoTC == "O2" ? "18" : "30";

                go_EditColumn = (EditTextColumn)go_Grid.Columns.Item(this.lrs_CabDttCodPrv);
                go_EditColumn.LinkedObjectType = "2";

                go_EditColumn = (EditTextColumn)go_Grid.Columns.Item(this.lrs_CabDttCodPg);
                go_EditColumn.LinkedObjectType = "46";

                //go_EditColumn = (SAPbouiCOM.EditTextColumn)go_Grid.Columns.Item(this.lrs_CabDttNumDoc);
                //go_EditColumn.LinkedObjectType = "18";

                go_EditColumn = (EditTextColumn)go_Grid.Columns.Item(lrs_CabDttSelect);
                go_EditColumn.Type = BoGridColumnType.gct_CheckBox;
                go_EditColumn.Editable = true;

                //Si a las detracciones ya les generaron el pago , bloqueará los controles

                bool lb_Enable = (go_Combo.Value != "C" && go_Combo.Value != "A");

                go_SBOForm.Items.Item(this.lrs_BtnGnrPagos).Enabled = lb_Enable;
                go_SBOForm.Items.Item(this.lrs_EdtFchCn).Enabled = lb_Enable;
                go_SBOForm.Items.Item(this.lrs_CmbArticulo).Enabled = lb_Enable;
                go_SBOForm.Items.Item(this.lrs_EdtCtaTrs).Enabled = lb_Enable;
                
                if (go_SBOForm.Mode == BoFormMode.fm_OK_MODE)
                {
                    go_SBOForm.Items.Item(this.lrs_BtnCrear).Enabled = true;
                    go_SBOForm.Items.Item(this.lrs_BtnTXT).Enabled = true;
                }
            }
            catch (Exception ex) { go_SBOApplication.MessageBox(ex.Message); } //Muestra una ventana con el mensaje de Excepción
        }
        private void Sb_LoadLogo()
        {
            string text = System.Windows.Forms.Application.StartupPath.ToString();
            SAPbouiCOM.Button button = (SAPbouiCOM.Button)(dynamic)go_SBOForm.Items.Item("btnTxt").Specific;
            button.Type = BoButtonTypes.bt_Image;
            button.Image = text + "\\Resources\\Imgs\\boton_archivo_1.png";
        }

        /// <Usa la propiedad Editable y Visible del las columnas>
        /// Si la columna es diferente del Número de constancia bloquea las demás, sino la propiedad
        /// Visible de las columas será False
        /// </>
        /// <param name="go_SBOApplication"></param>
        /// <param name="go_SBOForm"></param>
        /// <param name="pi_estado"></param>
        private void sb_bloquearColumnas(int pi_estado)
        {
            lc_NameMethod = "sb_bloquearColumnas"; //Se asigna el nombre del método para la identificación del mismo
            try
            {
                bool lb_block; //Variable para guardar el estado del documento
                go_Grid = go_SBOForm.GetGrid(lrs_GrdPayDTRDET);
                lb_block = !Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflEstado).Equals("A");
                //Bloquea las columnas según un estado y el número de constancia
                // Si en caso está Abierto debe permitir colocar check 
                foreach (GridColumn lo_GrdClm in go_Grid.Columns)
                {
                    if (pi_estado == 1)
                    {
                        if (Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflEstado).Equals("O") && (lo_GrdClm.UniqueID == lrs_CabDttSelect))
                            lo_GrdClm.Editable = true;
                        else if (lo_GrdClm.UniqueID != lrs_CabDttNumCns) lo_GrdClm.Editable = false;
                        else
                            lo_GrdClm.Editable = lb_block; //Si el documento está anulado, la celda no será editable
                    }
                    else
                    {
                        if (lo_GrdClm.UniqueID == lrs_CabDttNumCns) lo_GrdClm.Visible = false;
                        else if (lo_GrdClm.UniqueID != lrs_CabDttSelect) lo_GrdClm.Editable = false;
                    }
                }
                //Si el documento está anulado, los controles no será editable
                go_SBOForm.Items.Item(this.lrs_EdtFchDp).Enabled = lb_block;
                go_SBOForm.Items.Item(this.lrs_EdtNumDp).Enabled = lb_block;
            }
            catch (Exception ex) { ExceptionPrepared.inner(ex.Message, lc_NameMethod); ExceptionPrepared.SaveInLog(false); } //Método para el manejo de las operaciones de Log
        }

        /// <Recupera solo las filas selecionadas para el pago>
        /// Verifica si el Check de cada fila ha sido seleccionado. Luego removerá aquellas filas que no
        /// fueron seleccionadas, quedando solo con las que se realizarán la operación.
        /// </>
        /// <param name="go_SBOForm"></param>
        /// <param name="go_SBOApplication"></param>
        private void sb_pruebaFilasSeleccionadas()
        {
            lc_NameMethod = "sb_pruebaFilasSeleccionadas"; //Se asigna el nombre del método para la identificación del mismo
            try
            {
                bool lb_Flag;
                go_Grid = go_SBOForm.Items.Item(this.lrs_GrdPayDTRDET).Specific;
                go_SBOForm.Freeze(true); //Congela el formulario para la carga de datos
                //Recorrerá el dataTable, por Socio de Negocio
                while (true)
                {
                    lb_Flag = false;
                    //Recorre las filas del DataTable
                    for (int iDetalle = 0; iDetalle < go_Grid.DataTable.Rows.Count; iDetalle++)
                    {   //Verifica que filas han sido selecciondas para el pago
                        if (go_Grid.DataTable.Columns.Item(lrs_CabDttSelect).Cells.Item(iDetalle).Value != this.lrs_ValAnulado)
                        {   //Sino han sido seleccionadas serán removidas del DataTable
                            go_Grid.DataTable.Rows.Remove(iDetalle);
                            lb_Flag = true;
                            break;
                        }
                    }
                    if (!lb_Flag) break;
                }
                this.fn_generarPagos();
                go_SBOForm.Freeze(false); //Descongela el formulario
            }
            catch (Exception ex) { ExceptionPrepared.inner(ex.Message, lc_NameMethod); ExceptionPrepared.SaveInLog(false); } //Método para el manejo de las operaciones de Log
            finally { go_SBOForm.Freeze(false); } //Descongela el formulario
        }
        private void sb_pruebaFilasSeleccionadasSinPago()
        {
            lc_NameMethod = "sb_pruebaFilasSeleccionadas"; //Se asigna el nombre del método para la identificación del mismo
            try
            {
                bool lb_eliminado = false;
                bool lb_Flag;
                go_Grid = go_SBOForm.Items.Item(this.lrs_GrdPayDTRDET).Specific;
                go_SBOForm.Freeze(true); //Congela el formulario para la carga de datos
                //Recorrerá el dataTable, por Socio de Negocio
                while (true)
                {
                    lb_Flag = false;
                    //Recorre las filas del DataTable
                    for (int iDetalle = 0; iDetalle < go_Grid.DataTable.Rows.Count; iDetalle++)
                    {   //Verifica que filas han sido selecciondas para el pago
                        if (go_Grid.DataTable.Columns.Item(lrs_CabDttSelect).Cells.Item(iDetalle).Value != this.lrs_ValAnulado)
                        {
                            Sb_EliminaLineaUDO(Convert.ToInt32(Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflDocEntry)), iDetalle);
                            //Sino han sido seleccionadas serán removidas del DataTable
                            go_Grid.DataTable.Rows.Remove(iDetalle);
                            lb_eliminado = true;
                            lb_Flag = true;
                            break;
                        }
                    }
                    if (!lb_Flag) break;
                }
                //if (lb_eliminado) return false;
                go_SBOForm.Freeze(false); //Descongela el formulario
            }
            catch (Exception ex) { ExceptionPrepared.inner(ex.Message, lc_NameMethod); ExceptionPrepared.SaveInLog(false); } //Método para el manejo de las operaciones de Log
            finally { go_SBOForm.Freeze(false); } //Descongela el formulario
        }
        private void Sb_EliminaLineaUDO(int pi_docEntry, int pi_fila) 
        {
            SAPbobsCOM.GeneralService oGeneralService = null;
            SAPbobsCOM.GeneralData oGeneralData = null;
            SAPbobsCOM.GeneralDataCollection oChildren = null;
            SAPbobsCOM.GeneralDataParams oGeneralParams = null;

            try
            {
                // Obtener el servicio del UDO
                SAPbobsCOM.CompanyService oCompanyService = go_SBOCompany.GetCompanyService();
                oGeneralService = (SAPbobsCOM.GeneralService)oCompanyService.GetGeneralService("BPP_PAYDTR");

                // Establecer los parámetros de búsqueda (reemplaza 'DocEntry' con el valor correcto)
                oGeneralParams = (SAPbobsCOM.GeneralDataParams)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);
                oGeneralParams.SetProperty("DocEntry", pi_docEntry); // Ajusta el DocEntry correspondiente

                // Obtener el documento UDO
                oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                // Obtener las líneas (hijos del documento)
                oChildren = oGeneralData.Child("BPP_PAYDTRDET"); // Asegúrate de usar el nombre correcto de la tabla hija

                // Verificar si la fila existeq en la colección
                if (pi_fila >= 0 && pi_fila < oChildren.Count)
                {
                    oChildren.Remove(pi_fila); // Eliminar la línea indicada
                }

                // Actualizar el documento con la fila eliminada
                oGeneralService.Update(oGeneralData);

                int errorCode;
                string errorMessage;

                errorCode = go_SBOCompany.GetLastErrorCode();
                errorMessage = go_SBOCompany.GetLastErrorDescription();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                // Liberar objetos COM
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oGeneralService);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oGeneralData);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oChildren);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oGeneralParams);

                oGeneralService = null;
                oGeneralData = null;
                oChildren = null;
                oGeneralParams = null;

                GC.Collect();
            }
        }
        /// <Recupera y asigna el valor del número de serie en la tabla>
        /// </>
        /// <param name="go_SBOApplication"></param>
        /// <param name="go_SBOForm"></param>
        public void sb_SeleccionarSerie()
        {
            lc_NameMethod = "sb_SeleccionarSerie"; //Se asigna el nombre del método para la identificación del mismo
            string ls_serie = string.Empty;

            try
            {
                go_SBOForm.Freeze(true); //Congela el formulario para las carga de datos

                go_SBOFormEvent.Items.Item("cbCshFlw").Enabled = true;
                go_SBOFormEvent.Items.Item("txCtTr").Enabled = true;

                //Recupera los valores del Control Static
                go_Static = go_SBOForm.Items.Item(this.lrs_lblDesCtaTrs).Specific;
                go_Static.Caption = string.Empty;

                go_SBOForm.Items.Item(this.lrs_MtxPayDTRDET).Visible = false; //Oculta el control Matrix
                go_Grid = go_SBOForm.Items.Item(this.lrs_GrdPayDTRDET).Specific; //Recupera los valores de del control Grid
                go_Grid.DataTable.Clear(); //Limpia los registros del DataTable

                //Recupera los valores del ComboBox del formulario
                go_Combo = go_SBOForm.Items.Item(this.lrs_CmbSeries).Specific;
                //Agrega valores al ComboBox
                go_Combo.ValidValues.LoadSeries(go_SBOForm.BusinessObject.Type, BoSeriesMode.sf_Add);
                //Válida que el ComboBox tenga elementos y que no se haya seleccionado ninguno
                if (go_Combo.Selected == null && go_Combo.ValidValues.Count > 0)
                    Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflSeries, go_Combo.ValidValues.Item(0).Value, 0);
                else
                    ls_serie = go_Combo.Selected.Value; //Guarda el código de la serie

                if (go_SBOForm.Mode == BoFormMode.fm_FIND_MODE) return;

                //Recupera el DocNum
                go_SBOForm.DataSources.DBDataSources.Item(this.lrs_DtcPAYDTR).SetValue(this.lrs_UflDocNum, 0, go_SBOForm.BusinessObject.GetNextSerialNumber(ls_serie, go_SBOForm.BusinessObject.Type).ToString());

                go_SBOForm.DataSources.DBDataSources.Item(this.lrs_DtcPAYDTR).SetValue(this.lrs_UflCrtDate, 0, DateTime.Now.ToString(this.lrs_FchFormat));
                go_SBOForm.DataSources.DBDataSources.Item(this.lrs_DtcPAYDTR).SetValue(this.lrs_UflFchCnPg, 0, DateTime.Now.ToString(this.lrs_FchFormat));

                go_SBOForm.Items.Item(this.lrs_BtnCrear).Enabled = false; //Deshabilita el botón gestionar
            }
            catch (Exception ex) { ExceptionPrepared.inner(ex.Message, lc_NameMethod); ExceptionPrepared.SaveInLog(false); } //Método para el manejo de las operaciones de Log
            finally { go_SBOForm.Freeze(false); } //Descongela el formulario
        }

        #endregion SBOFormControles

        #region Metodos del Negocio

        /// <Busca las detracciones correspondientes a los filtros ingresados>
        /// Muestra las detracciones que cumplan con el filtro de búsqueda, además asigna las propiedades a las columnas
        /// del control Grid, llamando al método que las sb_bloquearColumnas y tambíen realiza el calculo del monto total a pagar
        /// llamando al método fn_calcularTotalAPagar.
        /// </>
        /// <param name="go_SBOApplication"></param>
        /// <param name="go_SBOForm"></param>
        /// <returns></returns>
        private bool fn_mostrarDetracciones()
        {
            lc_NameMethod = "fn_mostrarDetracciones"; //Se asigna el nombre del método para la identificación del mismo
            try
            {
                //Declara las variables del método
                string ls_PvDd = string.Empty;
                string ls_PvHt = string.Empty;
                string ls_FcDd = string.Empty;
                string ls_FcHt = string.Empty;
                string ls_FvDd = string.Empty;
                string ls_FvHt = string.Empty;
                string ls_CdSu = string.Empty;

                //Recupera valores de los controles de búsqueda del formulario
                go_Edit = go_SBOForm.Items.Item(this.lrs_EdtPrvDd).Specific;
                ls_PvDd = go_Edit.Value;
                go_Edit = go_SBOForm.Items.Item(this.lrs_EdtPrvHt).Specific;
                ls_PvHt = go_Edit.Value;

                if (ls_PvHt == string.Empty) { ls_PvHt = ls_PvDd; }
                else if (ls_PvDd == string.Empty) { ls_PvDd = ls_PvHt; }

                go_Edit = go_SBOForm.Items.Item(this.lrs_EdtFchCnDd).Specific;
                ls_FcDd = go_Edit.Value;
                go_Edit = go_SBOForm.Items.Item(this.lrs_EdtFchCnHt).Specific;
                ls_FcHt = go_Edit.Value;
                go_Edit = go_SBOForm.Items.Item(this.lrs_EdtFchVnDd).Specific;
                ls_FvDd = go_Edit.Value;
                go_Edit = go_SBOForm.Items.Item(this.lrs_EdtFchVnHt).Specific;
                ls_FvHt = go_Edit.Value;

                try
                {
                    go_Combo = go_SBOForm.Items.Item(this.lrs_CmbSuc).Specific;
                    ls_CdSu = go_Combo.Selected?.Value;
                }
                catch {
                    ls_CdSu = null;
                }

                //Avisa al usuario que se está realizando la búsqueda de las detracciones
                go_SBOApplication.StatusBar.SetText("Buscando detracciones por pagar...", BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Warning);

                //Ejecuta del Procedimiento en el go_Grid
                go_Grid = go_SBOForm.Items.Item(this.lrs_GrdPayDTRDET).Specific;
                go_Grid.DataTable.Clear();
                //Recupera los registros de las detracciones por pagar, según los filtros ingresados

               // Cls_Global.WriteToFile(Cls_Query.get_DetraccionesPorPagar, ls_PvDd, ls_PvHt, ls_FcDd, ls_FcHt, ls_FvDd, ls_FvHt, Cls_Global.metCalculoTC, ls_CdSu);
                go_Grid.DataTable.Consulta(Cls_Query.get_DetraccionesPorPagar, ls_PvDd, ls_PvHt, ls_FcDd, ls_FcHt, ls_FvDd, ls_FvHt, Cls_Global.metCalculoTC, ls_CdSu);
               

                //Oculta las columnas que no son necesarias, sino hasta generar el pago masivo
                go_Grid.Columns.Item(this.lrs_CabDttCodObj).Visible = false;
                go_Grid.Columns.Item(this.lrs_CabDttCodDtr).Visible = false;
                go_Grid.AutoResizeColumns();
                go_Grid.CollapseLevel = 1;

                //Recupera y asigna valores a las propiedades de las columnas del Grid
                go_EditColumn = (EditTextColumn)go_Grid.Columns.Item(lrs_CabDttSelect);
                go_EditColumn.Type = BoGridColumnType.gct_CheckBox;
                go_EditColumn.Editable = true;

                go_EditColumn = (EditTextColumn)go_Grid.Columns.Item(this.lrs_CabDttCodTrn);
                go_EditColumn.LinkedObjectType = Cls_Global.metCalculoTC == "O2" ? "18" : "30";

                go_EditColumn = (EditTextColumn)go_Grid.Columns.Item(this.lrs_CabDttCodPrv);
                go_EditColumn.LinkedObjectType = "2";

                //go_EditColumn = (SAPbouiCOM.EditTextColumn)go_Grid.Columns.Item(this.lrs_CabDttNumDoc);
                //go_EditColumn.LinkedObjectType = "18";

                go_EditColumn = (EditTextColumn)go_Grid.Columns.Item(this.lrs_CabDttImpDtr);
                go_EditColumn.Visible = false;

                this.fn_calcularTotalAPagar();
                this.sb_bloquearColumnas(2);

                Cls_Global.sb_msjStatusBarSAP("Búsqueda finalizada", BoStatusBarMessageType.smt_Warning, go_SBOApplication);

                //Si el modo del formulario es de agregar un nuevo registro, entonces habilita el botón de gestión
                if (go_SBOForm.Mode == BoFormMode.fm_ADD_MODE)
                    go_SBOForm.Items.Item(this.lrs_BtnCrear).Enabled = true;

                return true;
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod); ExceptionPrepared.SaveInLog(false);
                Cls_Global.sb_msjStatusBarSAP("Se produjo un error durante la búsqueda:" + ex.Message, BoStatusBarMessageType.smt_Error, go_SBOApplication);
                return false;
            } //Método para el manejo de las operaciones de Log
        }

        /// <Recupera los valores ingresados en el Grid y los almacena en la Matrix>
        /// Obtiene los valores de la celda del Grid y los pasa al control Matrix, seguidamente los registra
        /// en el Detalle de Pago Masivo de Detracción
        /// </>
        /// <param name="go_SBOApplication"></param>
        /// <param name="go_SBOForm"></param>
        /// <returns></returns>
        private void fn_ingresarDetalleUDO()
        {
            lc_NameMethod = "fn_ingresarDetalleUDO"; //Se asigna el nombre del método para la identificación del mismo
            go_Grid = go_SBOForm.Items.Item(this.lrs_GrdPayDTRDET).Specific;
            this.sb_pruebaFilasSeleccionadas();

            for (int iFilasGrid = 0; iFilasGrid <= go_Grid.DataTable.Rows.Count - 1; iFilasGrid++)
            {   //Entra si el CheckBox de la fila ha sido seleccionado
                if (go_Grid.DataTable.GetValue(lrs_CabDttSelect, iFilasGrid) == this.lrs_ValAnulado)
                {
                    string ls_getValue = string.Empty;
                    go_Matrix = go_SBOForm.Items.Item(this.lrs_MtxPayDTRDET).Specific; //Recupera los valores de la matrix
                    go_Matrix.AddRow();//Agrega una nueva fila

                    //Recupera los valores de las celdas del Control Matrix
                    go_Edit = go_Matrix.GetCellSpecific(this.lrs_ClmMtxCodPrv, iFilasGrid + 1);
                    //Guarda el valor ingresado en la celda del DataTable del control Grid
                    ls_getValue = go_Grid.DataTable.GetValue(this.lrs_CabDttCodPrv, iFilasGrid);
                    //Almacena el valor recuperado del Grid a la celda del control Matrix
                    go_Edit.Value = ls_getValue;
                    //Asigna el valor al detalle del Grid
                    Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTRDET, this.lrs_UflCodPrv, ls_getValue, iFilasGrid);

                    go_Edit = go_Matrix.GetCellSpecific(this.lrs_ClmMtxNomPv, iFilasGrid + 1);
                    ls_getValue = go_Grid.DataTable.GetValue(this.lrs_CabDttNomRs, iFilasGrid);
                    go_Edit.Value = ls_getValue;
                    Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTRDET, this.lrs_UflNomPv, ls_getValue, iFilasGrid);

                    go_Edit = go_Matrix.GetCellSpecific(this.lrs_ClmMtxNumDc, iFilasGrid + 1);
                    ls_getValue = go_Grid.DataTable.GetValue(this.lrs_CabDttNumDoc, iFilasGrid);
                    go_Edit.Value = ls_getValue;
                    Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTRDET, this.lrs_UflNumDc, ls_getValue, iFilasGrid);

                    go_Edit = go_Matrix.GetCellSpecific(this.lrs_ClmMtxFchDc, iFilasGrid + 1);
                    ls_getValue = Convert.ToDateTime(go_Grid.DataTable.GetValue(this.lrs_CabDttFchDoc, iFilasGrid)).ToString(this.lrs_FchFormat);
                    go_Edit.Value = ls_getValue;
                    Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTRDET, this.lrs_UflFchDc, ls_getValue, iFilasGrid);

                    go_Edit = go_Matrix.GetCellSpecific(this.lrs_ClmMtxIdDtr, iFilasGrid + 1);
                    ls_getValue = go_Grid.DataTable.GetValue(this.lrs_CabDttCodTrn, iFilasGrid).ToString();
                    go_Edit.Value = ls_getValue;
                    Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTRDET, this.lrs_UflIdDtr, ls_getValue, iFilasGrid);

                    go_Edit = go_Matrix.GetCellSpecific(this.lrs_ClmMtxDtrXPg, iFilasGrid + 1);
                    ls_getValue = go_Grid.DataTable.GetValue(this.lrs_CabDttImpDtr, iFilasGrid).ToString();
                    go_Edit.Value = ls_getValue;
                    Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTRDET, this.lrs_UflDtrXPg, ls_getValue, iFilasGrid);

                    go_Edit = go_Matrix.GetCellSpecific(this.lrs_ClmMtxCodBn, iFilasGrid + 1);
                    ls_getValue = go_Grid.DataTable.GetValue(this.lrs_CabDttCodBn, iFilasGrid);
                    go_Edit.Value = ls_getValue;
                    Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTRDET, this.lrs_UflCodBn, ls_getValue, iFilasGrid);

                    go_Edit = go_Matrix.GetCellSpecific(this.lrs_ClmMtxCodOpe, iFilasGrid + 1);
                    ls_getValue = go_Grid.DataTable.GetValue(this.lrs_CabDttCodOpe, iFilasGrid);
                    go_Edit.Value = ls_getValue;
                    Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTRDET, this.lrs_UflCodOpe, ls_getValue, iFilasGrid);

                    go_Edit = go_Matrix.GetCellSpecific(this.lrs_ClmMtxCodDtr, iFilasGrid + 1);
                    ls_getValue = go_Grid.DataTable.GetValue(this.lrs_CabDttCodDtr, iFilasGrid);
                    go_Edit.Value = ls_getValue;
                    Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTRDET, this.lrs_UflCodDtr, ls_getValue, iFilasGrid);

                    go_Edit = go_Matrix.GetCellSpecific(this.lrs_ClmMtxTipObj, iFilasGrid + 1);
                    ls_getValue = go_Grid.DataTable.GetValue(this.lrs_CabDttCodObj, iFilasGrid);
                    go_Edit.Value = ls_getValue;
                    Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTRDET, this.lrs_UflTipObj, ls_getValue, iFilasGrid);

                    go_Edit = go_Matrix.GetCellSpecific(this.lrs_ClmMtxNumCns, iFilasGrid + 1);
                    ls_getValue = go_Grid.DataTable.GetValue(lrs_CabDttNumCns, iFilasGrid).ToString();
                    go_Edit.Value = ls_getValue;
                    Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTRDET, this.lrs_UflNumCns, ls_getValue, iFilasGrid);
                }
            }
        }

        /// <Añade el dato ingresado en el número de constancia>
        /// Recupera el valor ingresado en el EditText del DataTable y lo añade en la tabla Pago masivo de detracción
        /// </>
        /// <param name="go_SBOApplication"></param>
        /// <param name="go_SBOForm"></param>
        /// <returns></returns>
        private bool fn_añadirNumeroConstancia()
        {
            try
            {
                go_Grid = go_SBOForm.Items.Item(this.lrs_GrdPayDTRDET).Specific;
                go_SBOForm.Freeze(true); //Congela el formulario para la carga de datos

                //Carga los datos en el control Matrix
                for (int i = 0; i <= go_Grid.DataTable.Rows.Count - 1; i++)
                {
                    go_Matrix = go_SBOForm.Items.Item(this.lrs_MtxPayDTRDET).Specific;
                    go_Matrix.FlushToDataSource();
                    Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTRDET, this.lrs_UflNumCns, go_Grid.DataTable.GetValue(lrs_CabDttNumCns, i), i);
                    go_Matrix.LoadFromDataSource();
                }
                go_SBOForm.Freeze(false); //Descongela el formulario
                return true;
            }
            catch (Exception ex)
            {   //Muestra mensaje de error, si se presenta alguna excepción
                go_SBOApplication.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                return false;
            }
            finally { go_SBOForm.Freeze(false); } //Descongela el formulario
        }

        /// <Genera los pagos de facturas con tasas de detracción UDF>
        /// El pago es de 1 a 1
        /// </>
        /// <param name="go_SBOApplication"></param>
        /// <param name="go_SBOForm"></param>
        /// <returns></returns>
        ///
        private void GenerarPagoAplicadoAFactura()
        {
            lc_NameMethod = "GenerarPagoAplicadoAFactura"; //Se asigna el nombre del método para la identificación del mismo

            string ls_numDocEntry = Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflDocEntry).ToString();

            go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.insert_PagoDetraccion, null, (string.IsNullOrEmpty(ls_numDocEntry) ? "0" : ls_numDocEntry).ToString());

            if (Cls_Global.go_SBOCompany.InTransaction)
                Cls_Global.go_SBOCompany.EndTransaction(BoWfTransOpt.wf_Commit);

            go_ProgressBar = go_SBOApplication.StatusBar.CreateProgressBar("Espera mientras el sistema genera los pagos", go_RecordSet.RecordCount, true);

            Cls_Global.go_SBOCompany.StartTransaction();
            string docEntryPago = string.Empty;

            try
            {
                go_ProgressBar.Maximum = go_RecordSet.RecordCount;
                int i = 0;

                while (!go_RecordSet.EoF)
                {
                    docEntryPago = CrearPagoAFactura(go_RecordSet);
                    go_RecordSet.MoveNext();
                    i++;
                    go_ProgressBar.Value = i;
                }
            }
            catch (Exception ex)
            {
                Cls_Global.go_SBOCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                throw new InvalidOperationException(ex.Message); ;
            }
            finally
            {
                if (Cls_Global.go_SBOCompany.InTransaction)
                    Cls_Global.go_SBOCompany.EndTransaction(BoWfTransOpt.wf_Commit);

                go_ProgressBar.Stop();
                go_ProgressBar = null;
            }

            try
            {
                Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflEstado, "C", 0);

                //Pregunta por el Modo del Formulario, si es OK pasará al modo de Actualización
                if (go_SBOForm.Mode == BoFormMode.fm_OK_MODE)
                    go_SBOForm.Mode = BoFormMode.fm_UPDATE_MODE;
                //Una vez que el formulario esté en el modo de Actualización, se guardarán los cambios
                if (go_SBOForm.Mode == BoFormMode.fm_UPDATE_MODE)
                    go_SBOForm.Items.Item(this.lrs_BtnCrear).Click(); //Simulación de presionar el Click en el Botón de Gestión

                //COLOCA LOS ID DE PAGO EN LA MATRIX
                Cls_QueryManager.Procesa(Cls_Query.update_PagoDetraccion,
                    Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflDocEntry));

                //MENSAJE DE TERMINO DE PROCESO
                Cls_Global.sb_msjStatusBarSAP("El proceso finalizó con exito", BoStatusBarMessageType.smt_Warning, go_SBOApplication);
                go_SBOApplication.Menus.Item("1282").Activate(); //Llama al formulario del Registro Diario
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string CrearPagoAFactura(Recordset go_RecordSet)
        {
            try
            {
                string ls_TpMoneda = (string)Cls_QueryManager.Retorna(Cls_Query.get_Moneda, (int)0, go_RecordSet.Fields.Item(this.lrs_UflCodPrv).Value);

                Payments oPagoEfectuado = go_SBOCompany.GetBusinessObject(BoObjectTypes.oVendorPayments);
                Documents oFactura = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);

                int docEntryFt = int.Parse(go_RecordSet.Fields.Item("U_BPP_DEAs").Value.ToString());
                oFactura.GetByKey(docEntryFt);

                double tasaDetraccion = oFactura.UserFields.Fields.Item("U_STR_TasaDTR").Value;

                //CABECERA
                oPagoEfectuado.CardCode = go_RecordSet.Fields.Item(this.lrs_UflCodPrv).Value;
                oPagoEfectuado.CardName = go_RecordSet.Fields.Item(this.lrs_UflNomPv).Value;

                if (Cls_Global.fuenteTC == "F")
                {
                    oPagoEfectuado.DocDate = oFactura.DocDate;
                    oPagoEfectuado.TaxDate = oFactura.TaxDate;
                }
                else
                {
                    oPagoEfectuado.DocDate = DateTime.ParseExact(Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflFchCnPg).Trim(), this.lrs_FchFormat, DateTimeFormatInfo.InvariantInfo).Date;
                    oPagoEfectuado.TaxDate = DateTime.ParseExact(Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflFchCnPg).Trim(), this.lrs_FchFormat, DateTimeFormatInfo.InvariantInfo).Date;
                }

                oPagoEfectuado.Remarks = "";
                oPagoEfectuado.JournalRemarks = "Pago Masivo Detrac. - " + oPagoEfectuado.CardCode;
                oPagoEfectuado.DocCurrency = (ls_TpMoneda == "USD" ? Cls_Global.fdi_ObtenerMonedaSistema() : Cls_Global.fdi_ObtenerMonedaLocal());
                oPagoEfectuado.UserFields.Fields.Item("U_BPP_MPPG").Value = "003";

                //DETALLE DEL PAGO (Transferencia bancaria)
                double tcdol = Cls_Global.sb_ObtenerTipodeCambioXDia(oFactura.DocCurrency,
                            DateTime.ParseExact(Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflFchCnPg).Trim(), this.lrs_FchFormat, DateTimeFormatInfo.InvariantInfo).Date);
                if (!oPagoEfectuado.DocCurrency.Equals(Cls_Global.fdi_ObtenerMonedaLocal()))
                    oPagoEfectuado.DocRate = tcdol;

                //double montoAPagar = (oFactura.DocTotal - oFactura.VatSum) * (tasaDetraccion / 100);
                double montoAPagar = (oFactura.DocTotal) * (tasaDetraccion / 100);

                oPagoEfectuado.TransferSum = Math.Round(montoAPagar, 0, MidpointRounding.AwayFromZero);
                oPagoEfectuado.TransferAccount = ObtenerCuentaTransferencia();

                oPagoEfectuado.Invoices.InvoiceType = BoRcptInvTypes.it_PurchaseInvoice;
                oPagoEfectuado.Invoices.DocEntry = oFactura.DocEntry;
                oPagoEfectuado.Invoices.DocLine = 0;
                oPagoEfectuado.Invoices.SumApplied = Math.Round(montoAPagar, 0, MidpointRounding.AwayFromZero);

                oPagoEfectuado.Invoices.AppliedFC = Math.Round(Math.Round(montoAPagar, 0, MidpointRounding.AwayFromZero) / tcdol, 2, MidpointRounding.AwayFromZero);

                oPagoEfectuado.Invoices.Add();

                string ls_CashFlowId = Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflArtPr).Trim();
                oPagoEfectuado.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtBankTransfer;
                oPagoEfectuado.PrimaryFormItems.CashFlowLineItemID = int.Parse((string)Cls_Global.IIf(string.IsNullOrEmpty(ls_CashFlowId), "0", ls_CashFlowId));
                oPagoEfectuado.BankChargeAmount = 0.0;

                if (oPagoEfectuado.Add() != 0)
                {
                    throw new Exception(go_SBOCompany.GetLastErrorDescription());
                }

                return go_SBOCompany.GetNewObjectKey();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string ObtenerCuentaTransferencia()
        {
            Recordset lo_RecSetAux = null;

            try
            {
                string ls_query = Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflCtaTrs);
                lo_RecSetAux = Cls_QueryManager.Retorna(Cls_Query.get_CodigoDetraccion, null, ls_query);

                if (lo_RecSetAux.RecordCount == 0)
                {
                    if (go_ProgressBar != null) { go_ProgressBar.Stop(); go_ProgressBar = null; }
                    throw new InvalidOperationException("Ocurrió un error al obtener la cuenta para el pago por transferencia. Verifique el valor ingresado.");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return lo_RecSetAux.Fields.Item(0).Value;
        }

        /// <Genera los pagos de todas las transacciones>
        /// Después de haber registrado las detracciones a pagar e ingresar los datos del archivo de texto devuelto por
        /// Sunat, se podrá realizar la Pago respectivo de las detracciones.
        /// </>
        /// <param name="go_SBOApplication"></param>
        /// <param name="go_SBOForm"></param>
        /// <returns></returns>
        private void fn_generarPagos()
        {
            lc_NameMethod = "fn_generarPagos"; //Se asigna el nombre del método para la identificación del mismo
            GC.Collect(); //Libera la memoria
            string numDesposito = "";
            try
            {
                //Recupera valores del comboBox del formulario
                go_Combo = go_SBOForm.Items.Item(this.lrs_CmbEstado).Specific;
                if (go_Combo.Value != this.lrs_ValEstado) throw new InvalidOperationException(); //Si el estado es Cerrado, sale del método

                //Recupera los valores del Número de depósito del Formulario
                go_Edit = go_SBOForm.Items.Item(this.lrs_EdtNumDp).Specific;
                go_Edit.Active = true;
                numDesposito = go_Edit.Value;
                go_ProgressBar = null;

                try
                {   //Pregunta si se está ejecutando la transación, si es así, entonces cuando finalice guardará los cambios
                    if (Cls_Global.go_SBOCompany.InTransaction)
                        Cls_Global.go_SBOCompany.EndTransaction(BoWfTransOpt.wf_Commit);
                }
                catch (Exception ex) { throw new InvalidOperationException(ex.Message); }

                //Recupera el número de DocEntry
                string ls_numDocEntry = Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflDocEntry).ToString();
                //Ejecuto el Procedimiento en el go_recordSet

                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.insert_PagoDetraccion, null, (string.IsNullOrEmpty(ls_numDocEntry) ? "0" : ls_numDocEntry).ToString());

                if (go_RecordSet.RecordCount == 0) return;

                //Declara las variables del método
                int li_CRS = 0;
                int li_Fila = 0;
                Payments lo_PagoEfectuado;
                string ls_SigCardCode = string.Empty;
                string ls_CashFlowId;
                string ls_TpMoneda;
                bool lb_ILinea = false;
                double ldb_TotalPagar = 0.0;
                int li_Resultado;

                lo_PagoEfectuado = Cls_Global.go_SBOCompany.GetBusinessObject(BoObjectTypes.oVendorPayments);
                Cls_Global.go_SBOCompany.StartTransaction(); //Inicia la transacción

                Documents oFactura = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);
                double tasaDetraccion = 0;

                SAPbobsCOM.Recordset recordset = null; // Nuevo
                string Sucursal = string.Empty;
                // ------ Validar SI Sociedad tiene configuracion de Sucursales
                recordset = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string query = $"SELECT \"MltpBrnchs\" FROM OADM";
                Cls_Global.WriteToFile(query);
                recordset.DoQuery(query);
                Sucursal = recordset.Fields.Item(0).Value;
                //-------------------------------------------------------

                //Asigna valor de texto al progressBar
                go_ProgressBar = go_SBOApplication.StatusBar.CreateProgressBar("Espera mientras el sistema genera los pagos", go_RecordSet.RecordCount, true);

                while (!go_RecordSet.EoF)
                {
                    li_CRS++;
                    if (!lb_ILinea)
                    {   //Asigna los valores de los controles del formulario a las propiedades de la variable lo_PagoEfectuado
                        ls_TpMoneda = (string)Cls_QueryManager.Retorna(Cls_Query.get_Moneda, (int)0, go_RecordSet.Fields.Item(this.lrs_UflCodPrv).Value);

                        lo_PagoEfectuado = null;
                        lo_PagoEfectuado = Cls_Global.go_SBOCompany.GetBusinessObject(BoObjectTypes.oVendorPayments);
                        ldb_TotalPagar = 0.0;
                        lo_PagoEfectuado.CardCode = go_RecordSet.Fields.Item(this.lrs_UflCodPrv).Value;
                        lo_PagoEfectuado.CardName = go_RecordSet.Fields.Item(this.lrs_UflNomPv).Value;

                        if (Cls_Global.fuenteTC == "F" && Cls_Global.metCalculoTC == "O2")  //toma fecha de pago igual a la factura
                        {
                            int idDocBase = int.Parse(go_RecordSet.Fields.Item(this.lrs_UflIdDtr).Value.ToString());
                            //int ObjectCode = int.Parse(asientoDetraccion.UserFields.Fields.Item("U_BPP_CtaTdoc").Value.ToString());

                            Documents DocBase = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);

                            DocBase.GetByKey(idDocBase);
                            lo_PagoEfectuado.DocDate = DocBase.DocDate;
                        }
                        else
                        {
                            lo_PagoEfectuado.DocDate = DateTime.ParseExact(Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflFchCnPg).Trim(), this.lrs_FchFormat, DateTimeFormatInfo.InvariantInfo).Date;
                        }

                        //lo_PagoEfectuado.DocDate = DateTime.ParseExact(Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflFchCnPg).Trim(), this.lrs_FchFormat, DateTimeFormatInfo.InvariantInfo).Date;

                        lo_PagoEfectuado.TaxDate = DateTime.ParseExact(Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflFchCnPg).Trim(), this.lrs_FchFormat, DateTimeFormatInfo.InvariantInfo).Date;

                        if (Cls_Global.ConfiguracionMasiva == "2")
                        {
                            lo_PagoEfectuado.Remarks = $"{numDesposito} - {numDesposito}";
                            lo_PagoEfectuado.JournalRemarks = $"{numDesposito} - {numDesposito}";
                        }
                        else
                        {
                            lo_PagoEfectuado.JournalRemarks = "Pago Masivo Detrac. - " + lo_PagoEfectuado.CardCode;
                        }
                        lo_PagoEfectuado.DocCurrency = (ls_TpMoneda == "USD" ? Cls_Global.fdi_ObtenerMonedaSistema() : Cls_Global.fdi_ObtenerMonedaLocal());
                        lo_PagoEfectuado.UserFields.Fields.Item("U_BPP_MPPG").Value = "003";
                    }

                    //Asigna valores a las propiedades de factura de la variable lo_PagoEfectuado
                    lo_PagoEfectuado.Invoices.InvoiceType = BoRcptInvTypes.it_JournalEntry;
                    lo_PagoEfectuado.Invoices.DocEntry = int.Parse(go_RecordSet.Fields.Item(this.lrs_UflIdDtr).Value);

                    //Ejecuto el Procedimiento y lo almaceno en lo_RecSetAux
                    Recordset lo_RecSetAux;
                    lo_RecSetAux = Cls_Global.go_SBOCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                    if (Cls_Global.metCalculoTC == "O2")
                    {
                        int docEntryFt = int.Parse(go_RecordSet.Fields.Item("U_BPP_DEAs").Value.ToString());
                        oFactura.GetByKey(docEntryFt);
                        tasaDetraccion = oFactura.UserFields.Fields.Item("U_STR_TasaDTR").Value;

                        double tcdol = 0;
                        if (Cls_Global.fuenteTC == "F" && Cls_Global.metCalculoTC == "O2") // Tipo de Cambio de Factura
                        {
                            if (oFactura.DocCurrency.Equals(Cls_Global.fdi_ObtenerMonedaSistema()))
                                tcdol = Cls_Global.sb_ObtenerTipodeCambioXDia(oFactura.DocCurrency, oFactura.DocDate);
                        }
                        else // Tipo de cambio de Pago
                        {
                            tcdol = Cls_Global.sb_ObtenerTipodeCambioXDia(oFactura.DocCurrency,
                            DateTime.ParseExact(Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflFchCnPg).Trim(), this.lrs_FchFormat, DateTimeFormatInfo.InvariantInfo).Date);
                        }

                        if (!lo_PagoEfectuado.DocCurrency.Equals(Cls_Global.fdi_ObtenerMonedaLocal()))
                            lo_PagoEfectuado.DocRate = tcdol;

                        //double montoAPagar = (oFactura.DocTotal - oFactura.VatSum) * (tasaDetraccion / 100);
                        //double montoAPagar = (oFactura.DocTotal) * (tasaDetraccion / 100);
                        
                        //go_Edit= Cls_QueryManager.Retorna(Cls_Query.get_ImporteDocumento, null, oFactura.DocEntry);
                        lo_RecSetAux = Cls_QueryManager.Retorna(Cls_Query.get_ImporteDocumento, null, oFactura.DocEntry);
                        double montoAPagarDocumento = lo_RecSetAux.Fields.Item(0).Value;
                        //if (go_RecordSet.RecordCount != 0)                       
                        //    montoAPagarDocumento = decimal.Parse(go_RecordSet.Fields.Item("DocTotal").Value);

                        double montoAPagar = (montoAPagarDocumento) * (tasaDetraccion / 100);

                        lo_PagoEfectuado.TransferSum = Math.Round(montoAPagar, 0, MidpointRounding.AwayFromZero);
                        lo_PagoEfectuado.TransferAccount = ObtenerCuentaTransferencia();

                        lo_PagoEfectuado.Invoices.InvoiceType = BoRcptInvTypes.it_PurchaseInvoice;
                        lo_PagoEfectuado.Invoices.DocEntry = oFactura.DocEntry;
                        lo_PagoEfectuado.Invoices.DocLine = li_Fila;
                        lo_PagoEfectuado.Invoices.SumApplied = Math.Round(montoAPagar, 0, MidpointRounding.AwayFromZero);

                        lo_PagoEfectuado.Invoices.AppliedFC = Math.Round(Math.Round(montoAPagar, 0, MidpointRounding.AwayFromZero) / tcdol, 2, MidpointRounding.AwayFromZero);

                        if (Sucursal.Equals("Y"))// Agregado 21/01/2022
                            lo_PagoEfectuado.BPLID = oFactura.BPL_IDAssignedToInvoice;

                        ldb_TotalPagar += lo_PagoEfectuado.Invoices.SumApplied;
                    }
                    else
                    {
                        //Ejecuto el Procedimiento y lo almaceno en el go_recordSet
                        lo_RecSetAux = Cls_QueryManager.
                            Retorna(Cls_Query.get_IDLinea, null, lo_PagoEfectuado.CardCode, lo_PagoEfectuado.Invoices.DocEntry);
                        //Válida que se haya recuperar la detracción del asiento contable de detracción
                        if (lo_RecSetAux.RecordCount == 0)
                        {
                            if (go_ProgressBar != null) { go_ProgressBar.Stop(); go_ProgressBar = null; }
                            throw new InvalidOperationException("Ocurrió un error al obtener la detracción del asiento contable de detracción.");
                        }

                        //Asigna valores a las propiedades de factura de la variable lo_PagoEfectuado
                        lo_PagoEfectuado.Invoices.DocLine = lo_RecSetAux.Fields.Item(0).Value;
                        lo_PagoEfectuado.Invoices.SumApplied = go_RecordSet.Fields.Item(this.lrs_UflDtrXPg).Value;

                        if (Sucursal.Equals("Y"))// Agregado 21/01/2022
                            lo_PagoEfectuado.BPLID = lo_RecSetAux.Fields.Item(1).Value;

                        //Suma los montos para obtener el pago total
                        ldb_TotalPagar += lo_PagoEfectuado.Invoices.SumApplied;
                    }

                    if (li_CRS < go_RecordSet.RecordCount)
                    {
                        try
                        {
                            go_RecordSet.MoveNext(); //Busca en el siguiente registro del recordSet
                          /*  ls_SigCardCode = go_RecordSet.Fields.Item(this.lrs_UflCodPrv).Value;
                            string ls_SigSerie = go_RecordSet.Fields.Item("Serie").Value;*/
                            go_RecordSet.MovePrevious(); //Busca en el anterior requisito del recordSet
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    else
                    {
                        ls_SigCardCode = string.Empty;
                        lb_ILinea = false;
                    }

                    if (lo_PagoEfectuado.CardCode == ls_SigCardCode)
                        lb_ILinea = true;
                    else
                    {
                        ls_SigCardCode = string.Empty;
                        lb_ILinea = false;
                    }

                    if (lb_ILinea)
                    {   //Agrega la factura
                        lo_PagoEfectuado.Invoices.Add();
                        goto siguiente;
                    }
                    else //Agrega la factura
                        lo_PagoEfectuado.Invoices.Add();

                    //Ejecuto el Procedimiento y lo almaceno en lo_RecSetAux
                    string ls_query = Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflCtaTrs);
                    lo_RecSetAux = Cls_QueryManager.Retorna(Cls_Query.get_CodigoDetraccion, null, ls_query);

                    if (lo_RecSetAux.RecordCount == 0)
                    {
                        if (go_ProgressBar != null) { go_ProgressBar.Stop(); go_ProgressBar = null; }
                        throw new InvalidOperationException("Ocurrió un error al obtener la cuenta para el pago por transferencia. Verifique el valor ingresado.");
                    }

                    if (Cls_Global.metCalculoTC == "O2")
                    {
                        lo_PagoEfectuado.TransferSum = Math.Round(ldb_TotalPagar, 0, MidpointRounding.AwayFromZero);
                        lo_PagoEfectuado.TransferAccount = ObtenerCuentaTransferencia();
                        if (lb_registrarCshFlw)
                        {
                            ls_CashFlowId = Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflArtPr).Trim();
                            lo_PagoEfectuado.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtBankTransfer;
                            lo_PagoEfectuado.PrimaryFormItems.CashFlowLineItemID = int.Parse((string)Cls_Global.IIf(string.IsNullOrEmpty(ls_CashFlowId), "0", ls_CashFlowId));
                        }
                        lo_PagoEfectuado.BankChargeAmount = 0.0;
                    }
                    else
                    {
                        lo_PagoEfectuado.TransferAccount = lo_RecSetAux.Fields.Item(0).Value;
                        lo_PagoEfectuado.TransferSum = Math.Round(ldb_TotalPagar, 0, MidpointRounding.AwayFromZero);//2407 
                        if (lb_registrarCshFlw)
                        {
                            ls_CashFlowId = Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflArtPr).Trim();
                            lo_PagoEfectuado.PrimaryFormItems.CashFlowLineItemID = int.Parse((string)Cls_Global.IIf(string.IsNullOrEmpty(ls_CashFlowId), "0", ls_CashFlowId));
                            lo_PagoEfectuado.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtBankTransfer;
                            lo_PagoEfectuado.PrimaryFormItems.AmountLC = Math.Round(ldb_TotalPagar);
                        }

                        lo_PagoEfectuado.BankChargeAmount = 0.0;
                    }

                    li_Resultado = lo_PagoEfectuado.Add();

                    //Válida el resultado, si se agrego la factura debería devolver 0
                    if (li_Resultado != 0)
                    {   //Detiene la carga del ProgressBar y limpia sus valores
                        go_ProgressBar.Stop();
                        go_ProgressBar = null;
                        Cls_Global.sb_msjStatusBarSAP(Cls_Global.go_SBOCompany.GetLastErrorDescription(), BoStatusBarMessageType.smt_Error, go_SBOApplication);
                        try
                        {   //Termina la operación del transacción con un RollBack
                            if (Cls_Global.go_SBOCompany.InTransaction) { Cls_Global.go_SBOCompany.EndTransaction(BoWfTransOpt.wf_RollBack); }
                        }
                        catch (Exception ex) { throw new InvalidOperationException(ex.Message); }
                        return;
                    }

                    //goto
                    siguiente:
                    go_ProgressBar.Value++; //Aumenta la carga del ProgressBar en 1
                    li_Fila++;
                    go_RecordSet.MoveNext();  //Pasa al siguiente registro del RecordSet
                }

                try
                {   //Al finalizar la operación del transacción, se aplica el Commit para asegurar que los cambios queden grabados en el disco
                    if (Cls_Global.go_SBOCompany.InTransaction)
                        Cls_Global.go_SBOCompany.EndTransaction(BoWfTransOpt.wf_Commit);
                    //Detiene la carga del ProgressBar y limpia sus valores
                    if (go_ProgressBar != null) { go_ProgressBar.Stop(); go_ProgressBar = null; }

                    //Asigna valor de Cerrado al campo "Status" de la tabla Pago masivo de Detracción
                    Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflEstado, "C", 0);

                    //Pregunta por el Modo del Formulario, si es OK pasará al modo de Actualización
                    if (go_SBOForm.Mode == BoFormMode.fm_OK_MODE)
                        go_SBOForm.Mode = BoFormMode.fm_UPDATE_MODE;
                    //Una vez que el formulario esté en el modo de Actualización, se guardarán los cambios
                    if (go_SBOForm.Mode == BoFormMode.fm_UPDATE_MODE)
                        go_SBOForm.Items.Item(this.lrs_BtnCrear).Click(); //Simulación de presionar el Click en el Botón de Gestión

                    //Ejecuto el Procedimiento en el go_recordSet
                    Cls_QueryManager.Procesa(Cls_Query.update_PagoDetraccion,
                        Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflDocEntry));

                    //mensaje de advertencia
                    Cls_Global.sb_msjStatusBarSAP("El proceso finalizó con exito", BoStatusBarMessageType.smt_Warning, go_SBOApplication);
                    go_SBOApplication.Menus.Item("1282").Activate(); //Llama al formulario del Registro Diario
                }
                catch (Exception ex)
                {
                    //Detiene la carga del ProgressBar y limpia sus valores
                    if (go_ProgressBar != null) { go_ProgressBar.Stop(); go_ProgressBar = null; }
                    throw new InvalidOperationException(ex.Message);
                }
            }
            catch (Exception ex)
            {
                if (!(ex is InvalidOperationException || ex is ArgumentException))
                {
                    ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                    ExceptionPrepared.SaveInLog(false);
                }
                if (go_ProgressBar != null) { go_ProgressBar.Stop(); go_ProgressBar = null; }

                throw;
            }
        }

        private string Fn_GenerarTxt()
        {
            string ls_createDate = null;
            string ls_docNum = null;
            string ls_dataTxt = null;
            string ls_separador = null;
            try
            {
               
                go_SBOForm.Freeze(true);

                ls_separador = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_BPP_SEPRDR") ;
                ls_createDate = Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflCrtDate).ToString();
                ls_docNum = Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflDocNum).ToString();

                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_DtrccnsCabTXT,null, ls_docNum, ls_createDate);
                int i = 0;
                while (!go_RecordSet.EoF)
                {
                    // Recorre todas las columnas de la fila actual
                    for (int c = 0; c < go_RecordSet.Fields.Count; c++)
                    {
                        string _empty = go_RecordSet.Fields.Item(c).Value.ToString();
                        ls_dataTxt += _empty.Replace(ls_separador,string.Empty); // Concatenar el valor de la columna con un separador
                    }
                    go_RecordSet.MoveNext();
                }
                ls_dataTxt += "\n";

                i = 0;
                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_DtrccnsDetTXT, null, ls_docNum, ls_createDate);
                while (!go_RecordSet.EoF)
                {
                    // Recorre todas las columnas de la fila actual
                    for (int c = 0; c < go_RecordSet.Fields.Count; c++)
                    {
                        string _empty = go_RecordSet.Fields.Item(c).Value; 
                        ls_dataTxt += _empty.Replace(ls_separador, string.Empty); // Concatenar el valor de la columna con un separador
                    }

                    ls_dataTxt += "\n"; // Añadir un salto de línea después de procesar la fila
                    go_RecordSet.MoveNext();
                } 

                return ls_dataTxt;
            }
            catch (Exception ex) { go_SBOApplication.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Short); throw; }
            finally { go_SBOForm.Freeze(false); }
        }
        private string Fn_NombreArchivo()
        {
            try
            {
                go_SBOForm.Freeze(true);
                string ls_numDocEntry = Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflDocEntry).ToString();

                string ls_nombreArchivo = Cls_QueryManager.Retorna(Cls_Query.get_NombreArchivoTXT, "Data", ls_numDocEntry);
                return ls_nombreArchivo;
            }
            catch (Exception ex) { go_SBOApplication.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Short); throw; }
            finally { go_SBOForm.Freeze(false); }
        }
        private void Sb_DescargarArchivo(string ps_DataTXT, string ps_NombreArchivo)
        {
            try
            {
                go_SBOForm.Freeze(true);

                // Ruta donde se guardará el archivo
                string ls_numDocEntry = Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflDocEntry).ToString();
                if (string.IsNullOrWhiteSpace(Cls_Global.RutaArchivoTXTDTR)) 
                { 
                    throw new Exception("No se seleccionó una ruta de guardado.");
                }

                // Comprobar si se tiene acceso de escritura en la ruta seleccionada
                if (!TienePermisoEscritura(Cls_Global.RutaArchivoTXTDTR))
                {
                    throw new Exception($"No tiene permisos de escritura en la ruta {Cls_Global.RutaArchivoTXTDTR}.");
                }

                // Ruta completa del archivo
                string ls_rutaCompleta = System.IO.Path.Combine(Cls_Global.RutaArchivoTXTDTR, ps_NombreArchivo + ".txt");
                // Escribir el archivo
                System.IO.File.WriteAllText(ls_rutaCompleta, ps_DataTXT);

                Cls_QueryManager.Procesa(Cls_Query.update_ArchivoTXT, ls_rutaCompleta, ls_numDocEntry);
                go_SBOForm.Mode = BoFormMode.fm_ADD_MODE;
                // Mostrar mensaje de éxito
                go_SBOApplication.SetStatusBarMessage("Archivo generado y guardado correctamente en: " + ls_rutaCompleta, BoMessageTime.bmt_Short, false);
            }
            catch (Exception ex)
            {
                // Mostrar el error en la barra de estado de SAP
                go_SBOApplication.SetStatusBarMessage("Error al generar el archivo: " + ex.Message, BoMessageTime.bmt_Short, true);
                throw;
            }
            finally
            {
                go_SBOForm.Freeze(false); // Descongelar el formulario en caso de éxito o error
            }
        }
       private bool TienePermisoEscritura(string ruta)
        {
            try
            {
                // Comprobar si se puede crear un archivo temporal en el directorio
                string archivoPrueba = System.IO.Path.Combine(ruta, System.IO.Path.GetRandomFileName());
                using (System.IO.FileStream fs = System.IO.File.Create(archivoPrueba, 1, System.IO.FileOptions.DeleteOnClose))
                {
                    // Si llega aquí, significa que se puede escribir en la carpeta
                    return true;
                }
            }
            catch
            {
                // Si ocurre un error, no se tiene permiso de escritura
                return false;
            }
        }

        /// <Selecciona todos los registros dentro de la grilla>
        /// Al dar doble clic en la cabecera de la columna "Seleccion" la función se encarga de recorrer todas la filas
        /// y asignarles el valor de True o False
        /// </summary>
        /// <param name="go_SBOApplication"></param>
        /// <param name="go_SBOForm"></param>
        private void sb_MultipleCheckingGrid()
        {
            try
            {
                go_SBOForm.Freeze(true);
                li_CountRowSelect += 1;
                go_Grid = go_SBOForm.Items.Item(this.lrs_GrdPayDTRDET).Specific;
                if (li_CountRowSelect % 2 == 0)
                {
                    for (int i = 0; i <= go_Grid.DataTable.Rows.Count - 1; i++)
                    {
                        if (go_Grid.DataTable.GetValue(lrs_CabDttSelect, i) == "N")
                            go_Grid.DataTable.Columns.Item(lrs_CabDttSelect).Cells.Item(i).Value = "Y";
                    }
                    li_CountRowSelect = 0;
                }
                else
                {
                    for (int i = 0; i <= go_Grid.DataTable.Rows.Count - 1; i++)
                    {
                        if (go_Grid.DataTable.Columns.Item(lrs_CabDttSelect).Cells.Item(i).Value == "Y")
                            go_Grid.DataTable.Columns.Item(lrs_CabDttSelect).Cells.Item(i).Value = "N";
                    }
                    li_CountRowSelect = 1;
                }
                this.fn_calcularTotalAPagar();
            }
            catch (Exception ex) { go_SBOApplication.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Short); }
            finally { go_SBOForm.Freeze(false); }
        }

        /// <Opera una suma de los montos de las detracciones seleccionadas>
        /// </>
        /// <param name="go_SBOApplication"></param>
        /// <param name="go_SBOForm"></param>
        /// <returns></returns>
        public void fn_calcularTotalAPagar()
        {
            lc_NameMethod = "fn_calcularTotalAPagar";//Se asigna el nombre del método para la identificación del mismo
            bool lb_isCreate = false;
            try
            {   //Declara las variables del método
                int li_FilasGrid = 0;
                double ldb_TotalAPagar = 0.0;
                int li_ContadorErrorDataGrid = 0;
                int li_TransIdAnterior = 0;
                //Recupera los valores del control Grid
                go_Grid = go_SBOForm.Items.Item(this.lrs_GrdPayDTRDET).Specific;
                li_FilasGrid = go_Grid.DataTable.Rows.Count; //Obtiene el número de filas del DataTable

                lb_isCreate = go_SBOForm.Mode == BoFormMode.fm_ADD_MODE; 
                for (int i = 0; i <= li_FilasGrid - 1; i++)
                {
                    if (go_Grid.DataTable.GetValue(lrs_CabDttSelect, i) == this.lrs_ValAnulado)
                    {
                        if (li_TransIdAnterior == Convert.ToInt64(go_Grid.DataTable.GetValue(this.lrs_CabDttCodTrn, i)))
                        {
                            li_ContadorErrorDataGrid++;
                        }

                        ldb_TotalAPagar = ldb_TotalAPagar + (lb_isCreate ? go_Grid.DataTable.GetValue(this.lrs_CabDttImpDtr2, i) : go_Grid.DataTable.GetValue(this.lrs_CabDttImpDtr, i));
                    }
                }

                go_Edit = go_SBOForm.Items.Item(this.lrs_EdtTtlPg).Specific;
                go_Edit.Value = ldb_TotalAPagar.ToString();
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                ExceptionPrepared.SaveInLog(false);
                throw;
            } //Método para el manejo de las operaciones de Log
        }

        private void sb_ManejarNumerodeSerie()
        {
            string serie;
            //Valida valores vacíos
            go_Edit = go_SBOForm.Items.Item(this.lrs_EdtFchCn).Specific;
            if (go_Edit.Value == string.Empty) return;

            go_Combo = go_SBOForm.Items.Item(this.lrs_CmbSeries).Specific;
            if (go_Combo.Value == string.Empty) return;
            //Válida que el año de la cuenta Contable coincida con la serie
            if (go_Edit.Value.ToString().Substring(0, 4) != go_Combo.Selected.Description)
            {
                //Ejecuto el Procedimiento en el go_recordSet
                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_NumeroSerie, null, go_SBOForm.BusinessObject.Type, go_Edit.Value.ToString().Substring(0, 4));
                if (go_RecordSet.RecordCount == 0)
                {
                    go_SBOApplication.StatusBar.SetText("No existen series creadas", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                    return;
                }
                //Guarda los elementos del ComboBox
                go_Validvalues = go_Combo.ValidValues;
                //Remueve los elementos del ValidValues
                while (go_Validvalues.Count > 0)
                    go_Validvalues.Remove(0, BoSearchKey.psk_Index);
                //Agrega elementos
                while (!go_RecordSet.EoF)
                {
                    go_Validvalues.Add(go_RecordSet.Fields.Item(this.lrs_UflSeries).Value, go_RecordSet.Fields.Item("SeriesName").Value);
                    go_RecordSet.MoveNext(); //Pasa al siguiente registro del RecordSet
                }
                //Selecciona por defecto el elemento de índice 0
                go_Combo.Select(0, BoSearchKey.psk_Index);
                //Recupera el número de serie de la tabla Pago Masivo de Detracción
                serie = Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflSeries);

                //Ejecuto el Procedimiento en el go_recordSet
                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_NumeroSiguiente, null, go_SBOForm.BusinessObject.Type, serie.ToString());
                Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflDocNum, go_RecordSet.Fields.Item("NextNumber").Value.ToString(), 0);
            }
        }

        #endregion Metodos del Negocio
    }
}