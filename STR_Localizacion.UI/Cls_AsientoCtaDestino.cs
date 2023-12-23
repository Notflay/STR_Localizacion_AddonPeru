using SAPbobsCOM;
using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;
using System.Globalization;

namespace STR_Localizacion.UI
{
    public class Cls_AsientoCtaDestino : Cls_PropertiesControl
    {
        private static string fechaDesde;
        private static string fechaFin;

        public Cls_AsientoCtaDestino()
        {
            gs_FormName = "frmAsientoDestino";
            gs_FormPath = "Resources/Localizacion/AsientoDestino.srf";
            lc_NameClass = "Cls_AsientoCtaDestino";
        }

        public void FormLoad()
        {
            ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);

            try
            {
                if (go_SBOForm == null)
                {
                    go_SBOForm = Cls_Global.fn_CreateForm(gs_FormName, gs_FormPath);
                    go_SBOForm.Visible = true;
                    InitializeEvents();
                }
            }
            catch (Exception exc)
            {
                go_SBOApplication.SetStatusBarMessage(exc.Message, BoMessageTime.bmt_Short, true);
            }
        }

        private void InitializeEvents()
        {
            itemevent.Add(BoEventTypes.et_GOT_FOCUS,

                new sapitemevent(s =>
                {
                    if (!s.BeforeAction)
                    {
                        go_SBOForm = go_SBOFormEvent;
                        go_SBOForm.DefButton = "btBuscar";
                    }
                }, "txDesde", "txHasta")

            );

            itemevent.Add(BoEventTypes.et_ITEM_PRESSED,

                new sapitemevent("btBuscar", s =>
                {
                    if (!s.BeforeAction)
                        MostrarAsiento();
                }),

                new sapitemevent("btGenerar", s =>
                {
                    if (!s.BeforeAction)
                    {
                        go_SBOForm = go_SBOFormEvent;

                        string ls_Resultado = CrearAsientoDestino();

                        if (ls_Resultado != string.Empty)
                            go_SBOApplication.StatusBar.SetText(ls_Resultado); //Muestra una ventana con el mensaje de Excepción
                        else
                        {
                            string transID = go_SBOCompany.GetNewObjectKey();
                            go_SBOForm.GetItem("txtTrnId").Specific.Value = transID;

                            go_SBOApplication.MessageBox("El asiento se creó de manera correcta");
                            ActualizarFacturasBase(fechaDesde, fechaFin);
                            ActualizarFormPostCreacion();
                        }
                    }
                }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, s => Dispose()));
        }

        private void ActualizarFormPostCreacion()
        {
            try
            {
                go_SBOForm.GetEditText("txDesde").Active = true;
                go_SBOForm.GetButton("btGenerar").Item.Enabled = false;
                CambiarEstadoCamposDeAsiento(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Metodos del Negocio

        /// <Recupera los datos del registro diario y opera los resultados>
        /// Busca los registros del Asiento y los muestra en el grid, además suma los montos de débito y haber
        /// </summary>
        private void MostrarAsiento()
        {
            lc_NameMethod = "MostrarAsiento";
            string query = string.Empty;
            try
            {
                GC.Collect(); //Libera la memoria

                string fDesde = go_SBOForm.GetEditText("txDesde").Value;
                string fFin = go_SBOForm.GetEditText("txHasta").Value;

                fechaDesde = fDesde;
                fechaFin = fFin;

                switch (Cls_Global.metAsientoDestino)
                {
                    case "E": query = Cls_Query.info_AsientoDestino; break;
                    case "D": query = Cls_Query.info_AsientoDestinoAlt; break;
                }

                if (HayRegistros(query, fDesde, fFin))
                {
                    LimpiarGrid();
                    LlenarGrid(query, fDesde, fFin);
                }
                else
                {
                    LimpiarGrid();
                    go_SBOApplication.StatusBar.SetText("No existen registros para el rango de fechas seleccionado.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                ExceptionPrepared.SaveInLog();
            }
        }

        private void LlenarGrid(string query, string fDesde, string fFin)
        {
            try
            {
                go_Grid.DataTable.Consulta(query, fDesde, fFin);

                go_Grid.Columns.Item("Codigo").Visible = false;
                go_Grid.Columns.Item("Debito").RightJustified = true;
                go_Grid.Columns.Item("Credito").RightJustified = true;

                if (Cls_Global.metAsientoDestino.Equals("D"))
                    go_Grid.Columns.Item("Grupo").Visible = false;

                go_Grid.AutoResizeColumns();
                go_Grid.CollapseLevel = 0;

                go_SBOForm.Items.Item("btGenerar").Enabled = !go_Grid.DataTable.IsEmpty;
                CambiarEstadoCamposDeAsiento(true);
                go_SBOForm.DefButton = "btGenerar";

                SetearTotales();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetearTotales()
        {
            double ldb_TotalesD = 0;
            double ldb_TotalesH = 0;

            try
            {
                for (int i = 0; i < go_Grid.DataTable.Rows.Count; i++)
                {
                    ldb_TotalesD = ldb_TotalesD + go_Grid.DataTable.GetValue("Debito", i);
                    ldb_TotalesH = ldb_TotalesH + go_Grid.DataTable.GetValue("Credito", i);
                }

                go_SBOForm.Items.Item("txDebT").Specific.Value = ldb_TotalesD.ToString();
                go_SBOForm.Items.Item("txCredT").Specific.Value = ldb_TotalesH.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool HayRegistros(string query, string fDesde, string fFin)
        {
            int limit = 0;

            try
            {
                switch (Cls_Global.metAsientoDestino)
                {
                    case "E": query = Cls_Query.info_AsientoDestino; limit = 1; break;
                    case "D": query = Cls_Query.info_AsientoDestinoAlt; limit = 0; break;
                }

                go_RecordSet = Cls_QueryManager.Retorna(query, null, fDesde, fFin);

                if (go_RecordSet.RecordCount == limit)
                    return false;
            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }

        private void LimpiarGrid()
        {
            try
            {
                go_Grid = go_SBOForm.Items.Item("gdAsiento").Specific;
                go_Grid.DataTable.Clear();

                go_SBOForm.Items.Item("txDesde").Specific.Active = true;

                go_SBOForm.Items.Item("txtTrnId").Specific.Value = string.Empty;
                go_SBOForm.Items.Item("txFchConta").Specific.Value = string.Empty;
                go_Combo = go_SBOForm.GetComboBox("cbCodOper");
                go_Combo.Select(0, BoSearchKey.psk_Index);
                go_SBOForm.Items.Item("txRef1").Specific.Value = string.Empty;
                go_SBOForm.Items.Item("txRef2").Specific.Value = string.Empty;
                go_SBOForm.Items.Item("txRef3").Specific.Value = string.Empty;
                go_SBOForm.Items.Item("txComent").Specific.Value = string.Empty;
                go_SBOForm.Items.Item("txDebT").Specific.Value = 0.0;
                go_SBOForm.Items.Item("txCredT").Specific.Value = 0.0;

                go_SBOForm.Items.Item("gdAsiento").Enabled = false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CambiarEstadoCamposDeAsiento(bool estado)
        {
            go_SBOForm.Items.Item("btGenerar").Enabled = estado;
            go_SBOForm.Items.Item("txFchConta").Enabled = estado;
            go_SBOForm.Items.Item("cbCodOper").Enabled = estado;
            go_SBOForm.Items.Item("txRef1").Enabled = estado;
            go_SBOForm.Items.Item("txRef2").Enabled = estado;
            go_SBOForm.Items.Item("txRef3").Enabled = estado;
            go_SBOForm.Items.Item("txComent").Enabled = estado;
        }

        /// <Realiza los pasos para la creación del asiento>
        /// Recuperar los valores del registro diario, junto con los valores ingresados en los campos del mismo para
        /// generar el Asiento correspondiente,
        /// </>
        /// <returns></returns>
        public string CrearAsientoDestino()
        {
            lc_NameMethod = "CrearAsientoDestino";
            string mensaje = string.Empty;
            go_Grid = go_SBOForm.GetGrid("gdAsiento"); // DATOS DEL ASIENTO (DETALLE)

            try
            {
                if (DatosValidos(out mensaje))
                {
                    string fContabilizacion = go_SBOForm.GetItem("txFchConta").Specific.Value;
                    DateTime fContDate = DateTime.ParseExact(fContabilizacion, "yyyyMMdd", CultureInfo.InvariantCulture);

                    JournalEntries oJournalEntry = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
                    oJournalEntry.ReferenceDate = fContDate;
                    oJournalEntry.DueDate = fContDate;
                    oJournalEntry.TaxDate = fContDate;
                    oJournalEntry.Reference = go_SBOForm.GetItem("txRef1").Specific.Value;
                    oJournalEntry.Reference2 = go_SBOForm.GetItem("txRef2").Specific.Value;
                    oJournalEntry.Reference3 = go_SBOForm.GetItem("txRef3").Specific.Value;
                    oJournalEntry.Memo = go_SBOForm.GetItem("txComent").Specific.Value;
                    oJournalEntry.TransactionCode = go_SBOForm.GetItem("cbCodOper").Specific.Value;

                    for (int i = 0; i < go_Grid.Rows.Count; i++)
                    {
                        string codigo = go_Grid.DataTable.GetValue("Codigo", i);
                        oJournalEntry.Lines.AccountCode = go_Grid.DataTable.GetValue("Codigo", i);
                        oJournalEntry.Lines.Debit = go_Grid.DataTable.GetValue("Debito", i);
                        oJournalEntry.Lines.Credit = go_Grid.DataTable.GetValue("Credito", i);
                        oJournalEntry.Lines.Add();
                    }

                    if (oJournalEntry.Add() != 0)
                        return go_SBOCompany.GetLastErrorDescription();
                }
                else
                {
                    return mensaje;
                }
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                ExceptionPrepared.SaveInLog();
                return ex.Message;
            }

            return string.Empty;
        }

        private bool DatosValidos(out string mensaje)
        {
            mensaje = string.Empty;
            bool cuentaDestinoConfigurada = true, fechaContabilizacionIngresada;

            if (Cls_Global.metAsientoDestino.Equals("E"))
            {
                cuentaDestinoConfigurada = ValidarCuentaConfigurada(out mensaje);
                if (mensaje != string.Empty)
                    return false;
            }

            fechaContabilizacionIngresada = ValidarFechaContabilizacion(out mensaje);

            return cuentaDestinoConfigurada && fechaContabilizacionIngresada;
        }

        private bool ValidarFechaContabilizacion(out string mensaje)
        {
            string fechaContabilizacion = string.Empty;
            fechaContabilizacion = go_SBOForm.GetItem("txFchConta").Specific.Value;

            mensaje = fechaContabilizacion.Equals(string.Empty) ? "Ingrese la fecha de contabilización" : string.Empty;
            if (mensaje == string.Empty)
            {
                string fechaLimite = go_SBOForm.GetItem("txHasta").Specific.Value;
                DateTime fechaConta = DateTime.ParseExact(fechaContabilizacion, "yyyyMMdd", null);
                DateTime fechaHasta = DateTime.ParseExact(fechaLimite, "yyyyMMdd", null);

                mensaje = fechaConta < fechaHasta ? "La fecha de contabilización no puede ser mayor a la de fecha final" : string.Empty;

            }
            return mensaje == string.Empty;
        }

        private bool ValidarCuentaConfigurada(out string mensaje)
        {
            ChartOfAccounts oAccounts;
            UserTable oUserTable;
            mensaje = string.Empty;
            string cuenta = string.Empty;

            try
            {
                oUserTable = go_SBOCompany.UserTables.Item("BPP_CONFIG");

                if (oUserTable.GetByKey("01"))
                {
                    cuenta = oUserTable.UserFields.Fields.Item("U_BPP_CdgCuenta").Value;
                    if (!string.IsNullOrEmpty(cuenta))
                    {
                        //VERIFICAMOS VALIDEZ DE CUENTA
                        oAccounts = go_SBOCompany.GetBusinessObject(BoObjectTypes.oChartOfAccounts);

                        if (!oAccounts.GetByKey(cuenta))
                        {
                            mensaje = "Se ingresado un valor incorrecto en el formulario de Configuración BPP (columna: Codigo Cuenta Destino)";
                            return false;
                        }
                    }
                    else
                    {
                        mensaje = "No se ha configurado ninguna cuenta en el formulario de Configuración BPP";
                        return false;
                    }
                }
                else
                {
                    mensaje = "No se han configurado datos en el formulario de Configuración BPP";
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }

        private void ActualizarFacturasBase(string fDesde, string fFin)
        {
            try
            {
                // Se cambio a que se actulize el asiento OJDT - y se agrego campo U_STR_ADP
                //Documents oFactura = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);
                Documents oAsiento = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);

                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_FacturasPendientesAsientoDestino, null, fDesde, fFin, Cls_Global.metAsientoDestino);

                while (!go_RecordSet.EoF)
                {
                    //int docEntry = go_RecordSet.Fields.Item("CreatedBy").Value;
                    int transId = go_RecordSet.Fields.Item("TransId").Value;

                    oAsiento.GetByKey(transId);
                    oAsiento.UserFields.Fields.Item("U_STR_ADP").Value = "Y";
                    oAsiento.Update();
                    go_RecordSet.MoveNext();
                }
            }
            catch (Exception ex)
            {
                go_SBOApplication.StatusBar.SetText("Error al actualizar Factura base. Mensaje: " + ex.Message, BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Error);
            }
        }

        /// <Agrega valores de UserDataSource>
        /// Recupera los valores de los controles EditText del formulario. Luego agrega datos al UserDataSource
        /// para asignarlos a la propiedad SetBound de dichos EditText
        /// </>
        /// <param name="lo_Form"></param>
        public void sb_PrepareDataSource(Form lo_Form)
        {
            lc_NameMethod = "sb_PrepareDataSource";//Se asigna el nombre del metodo para la identificación del mismo
            try
            {
                //Agrega UserDataSources para el SetBound de DataBind
                go_Edit = lo_Form.Items.Item("txDesde").Specific;
                lo_Form.DataSources.UserDataSources.Add("UDS_Desde", BoDataType.dt_DATE);
                go_Edit.DataBind.SetBound(true, string.Empty, "UDS_Desde");

                go_Edit = lo_Form.Items.Item("txHasta").Specific;
                lo_Form.DataSources.UserDataSources.Add("UDS_Hasta", BoDataType.dt_DATE);
                go_Edit.DataBind.SetBound(true, string.Empty, "UDS_Hasta");

                go_Edit = lo_Form.Items.Item("txFchConta").Specific;
                lo_Form.DataSources.UserDataSources.Add("UDS_FCont", BoDataType.dt_DATE);
                go_Edit.DataBind.SetBound(true, string.Empty, "UDS_FCont");

                //Recupera los valores del comboBox del formulario
                go_Combo = lo_Form.Items.Item("cbCodOper").Specific;

                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_CodigoRegistroDiario);

                go_Combo.ValidValues.Add("", "");

                while (!go_RecordSet.EoF)
                {
                    go_Combo.ValidValues.Add(go_RecordSet.Fields.Item("TrnsCode").Value, go_RecordSet.Fields.Item("TrnsCodDsc").Value);
                    go_RecordSet.MoveNext();
                }
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                ExceptionPrepared.SaveInLog();
            }
        }

        #endregion Metodos del Negocio
    }
}