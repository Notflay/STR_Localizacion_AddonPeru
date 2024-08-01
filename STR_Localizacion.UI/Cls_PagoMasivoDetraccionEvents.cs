using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SAPbouiCOM;
using System.Globalization;
using STR_Localizacion.UTIL;
using STR_Localizacion.DL;

namespace STR_Localizacion.UI
{
    partial class Cls_PagoMasivoDetraccion
    {
        private void InitializeEvents()
        {

            menuevent.Add(new sapmenuevent("1284", s =>
            {
                try
                {
                    if (s.BeforeAction)
                    {
                        go_SBOForm = go_SBOFormEvent;

                        SAPbobsCOM.Payments lo_Payments;
                        bool lb_estado;

                        lb_estado = (Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflEstado) == this.lrs_ValEstado);
                        ls_msjCancelacion = (string)Cls_Global.IIf(lb_estado, "Se procederá a realizar la cancelación del registro", "Se procederá a realizar la cancelación de los pagos efectuados");
                        go_SBOApplication.SetStatusBarMessage(ls_msjCancelacion, BoMessageTime.bmt_Short, false);
                        li_rsptCancelacion = go_SBOApplication.MessageBox(ls_msjCancelacion + ". ¿Está seguro que desea continuar?", 1, "Sí", "No");
                        if (li_rsptCancelacion == 1)
                        {
                            ls_msjCancelacion = "El registro fue cancelado exitosamente.";
                            go_SBOApplication.SetStatusBarMessage(ls_msjCancelacion, BoMessageTime.bmt_Short, false);
                            ls_docEntry = Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflDocEntry);
                            //Recupera el DocEntry de los pagos

                            if (Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflCanceled).Equals(this.lrs_ValAnulado))
                            {
                                throw new InvalidOperationException("El documento ya fue anulado.");
                            }
                            if (!lb_estado)
                            {
                                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_PagosEfectuados, null, ls_docEntry);

                                if (go_RecordSet.EoF)
                                    throw new InvalidOperationException("No existe ningun pago efectuado.");

                                lo_Payments = Cls_Global.go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oVendorPayments);
                                while (!go_RecordSet.EoF)
                                {
                                    if (lo_Payments.GetByKey(go_RecordSet.Fields.Item(0).Value))
                                    {
                                        if (lo_Payments.Cancel() != 0)
                                        {
                                            go_SBOApplication.SetStatusBarMessage(Cls_Global.go_SBOCompany.GetLastErrorDescription(), BoMessageTime.bmt_Short, true);
                                            throw new InvalidOperationException();
                                        }
                                    }
                                    go_RecordSet.MoveNext();
                                }
                                ls_msjCancelacion = "Los pagos fueron cancelados exitosamente.";
                                go_SBOApplication.SetStatusBarMessage(ls_msjCancelacion, BoMessageTime.bmt_Short, false);
                            }
                        }
                    }
                    else
                    {//Actualiza estado a Anulado
                        if (li_rsptCancelacion == 1)
                        {
                            if (!Cls_QueryManager.Procesa(Cls_Query.update_EstadoAnulado, ls_docEntry))
                                throw new InvalidOperationException("Se generó algún error al momento de actualizar el estado");
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!(ex is InvalidOperationException || ex is ArgumentException))
                    {
                        ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                        ExceptionPrepared.SaveInLog(false);
                    }
                    throw;
                }
            }));

            itemevent.Add(BoEventTypes.et_ITEM_PRESSED,
                new sapitemevent(lrs_BtnBuscar, s =>
                {
                    if (!s.BeforeAction)
                    {
                        go_SBOForm = go_SBOFormEvent;
                        fn_mostrarDetracciones();
                    }

                }),
                new sapitemevent(lrs_BtnCrear, s =>
                {
                    if (s.BeforeAction)
                    {   //Válida si el formulario está en modo de registrar
                        go_SBOForm = go_SBOFormEvent;
                        if (go_SBOForm.Mode == BoFormMode.fm_ADD_MODE)
                        {
                              string ls_msgStatus = string.Empty;
                                //Recupera valores de la Matrix del formulario
                                go_Grid = go_SBOForm.Items.Item(this.lrs_GrdPayDTRDET).Specific;
                                //Válida si el DataTable está vacío
                                if (go_Grid.DataTable.IsEmpty)
                                    throw new InvalidOperationException("No hay detracciones a pagar.");
                                //Válida que se haya ingresado el valor de Fecha contable
                                if (Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflFchCnPg) == string.Empty)
                                    throw new InvalidOperationException("Debe ingresar Fecha Contable Pagos.");
                                //Inicio de validación del flujo de efectivo
                                lb_registrarCshFlw = false;
                                SAPbobsCOM.ChartOfAccounts chrtAcct = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oChartOfAccounts);
                                string cashFlowID = go_SBOForm.GetComboBox("cbCshFlw").Value;
                                string ctaBank = ls_CtaTransferSYS;
                            // Para 9.2 JAMPAR no hacer está validación porque se cae 
                            //if (chrtAcct.GetByKey(ctaBank) && chrtAcct.CashFlowRelevant == SAPbobsCOM.BoYesNoEnum.tYES)
                            //{
                            //    if (string.IsNullOrEmpty(cashFlowID))
                            //        throw new ArgumentNullException("Art. Form. Primario", "Seleccione el flujo de caja");
                            //    lb_registrarCshFlw = true;
                            //}

                            this.fn_ingresarDetalleUDO();
                        }
                        else if (go_SBOForm.Mode == BoFormMode.fm_UPDATE_MODE) //Válida si el Formulario está en modo de Actualización
                            this.fn_añadirNumeroConstancia(); //Recupera el valor del campo Número de constancia
                    }
                    else
                    {
                        if (go_SBOForm.Mode == BoFormMode.fm_OK_MODE) //Válida si el formulario está en modo de registrar
                            this.sb_prepararControlesEstadoAbierto(); //Prepara los controles para los registrar que no han sido generado su pago

                        go_SBOApplication.Menus.Item("1291").Activate();
                    }
                }),
                new sapitemevent(lrs_BtnGnrPagos, s =>
                {
                    go_SBOForm = go_SBOFormEvent;
                    string ls_Message = string.Empty;
                    //Válida que los campos necesarios hayan sido completados
                    if (Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflCtaTrs) == string.Empty)
                        throw new InvalidOperationException("Debe ingresar la Cuenta contable.");
                    if (Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflFchDp) == string.Empty)
                        throw new InvalidOperationException("Debe ingresar la Fecha de depósito.");
                    if (Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflNumDp) == string.Empty)
                        throw new InvalidOperationException("Debe ingresar el Número de depósito.");
                    //Recupera los controles del Grid
                    go_Grid = go_SBOForm.Items.Item(this.lrs_GrdPayDTRDET).Specific;
                    //Verifica que se haya ingresado el número de constancia en los registros

                    for (int iDetalle = 0; iDetalle <= go_Grid.DataTable.Rows.Count - 1; iDetalle++)
                    {
                        if (go_Grid.DataTable.Columns.Item(lrs_CabDttNumCns).Cells.Item(iDetalle).Value == string.Empty)
                            throw new InvalidOperationException("Debe ingresar el Número de constancia.");
                    }



                    if (!s.BeforeAction)

                        //if (Cls_Global.metCalculoTC == "O2")
                        //    //Invoca al método para proceder a generar el pago aplicado a la misma factura, no al asiento de detracción
                        //    try
                        //    {
                        //        GenerarPagoAplicadoAFactura();
                        //    }
                        //    catch (Exception)
                        //    {
                        //        throw;
                        //    }

                        //else
                        this.fn_generarPagos(); //Invoca al método para proceder a generar el pago
                }),
                new sapitemevent(lrs_EdtFchCn, s =>
                {
                    if (!s.BeforeAction) sb_ManejarNumerodeSerie();
                }));

            itemevent.Add(BoEventTypes.et_CHOOSE_FROM_LIST,
                new sapitemevent(s =>
                {
                    go_SBOForm = go_SBOFormEvent;

                    if (s.BeforeAction)
                    {
                        //Declara las variables de condiciones
                        Conditions lo_Conditions;
                        Condition lo_Condition;
                        Conditions lo_emptyCon = new Conditions();
                        ChooseFromList lo_choosefromlist;
                        IChooseFromListEvent lo_cflEvent;

                        //Recupera la lista de registros para aplicar las condiciones
                        lo_cflEvent = (IChooseFromListEvent)s;
                        lo_choosefromlist = go_SBOForm.ChooseFromLists.Item(lo_cflEvent.ChooseFromListUID);
                        lo_choosefromlist.SetConditions(lo_emptyCon);
                        lo_Conditions = lo_choosefromlist.GetConditions();
                        lo_Condition = lo_Conditions.Add();

                        //Ingresa los filtros en la variable de condición
                        lo_Condition.Alias = (string)Cls_Global.IIf(s.ItemUID == this.lrs_EdtCtaTrs, "Postable", "CardType");
                        lo_Condition.CondVal = (string)Cls_Global.IIf(s.ItemUID == this.lrs_EdtCtaTrs, this.lrs_ValAnulado, "S");
                        lo_Condition.Operation = BoConditionOperation.co_EQUAL;
                        lo_choosefromlist.SetConditions(lo_Conditions);
                    }
                    else
                    {   //Se declara las variables 
                        DataTable lo_dataTable;
                        ChooseFromList lo_choosefromlist;
                        IChooseFromListEvent lo_cflEvent;
                        try
                        {
                            //Recupera una lista de registros
                            lo_cflEvent = (IChooseFromListEvent)s;
                            lo_choosefromlist = go_SBOForm.ChooseFromLists.Item(lo_cflEvent.ChooseFromListUID);
                            //Agrega los valores de la lista en el DataTable
                            lo_dataTable = lo_cflEvent.SelectedObjects;
                            if (lo_dataTable == null) //Valida que hayan datos
                                throw new InvalidOperationException();

                            DataBind lo_DataBind;
                            //Pregunta por los controles EditText para la búsqueda del Proveedor
                            if (s.ItemUID == this.lrs_EdtPrvDd || s.ItemUID == this.lrs_EdtPrvHt)
                            {
                                string ls_nomControl = s.ItemUID;
                                go_Edit = go_SBOForm.Items.Item(ls_nomControl).Specific; //Recupera el valor del EdiText
                                lo_DataBind = go_Edit.DataBind; //Guarda los valores de DataBind en la variable local
                                //Guarda el valor del campo CardCode en la  tabla de Pago Masivo de Detracciones
                                Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, lo_DataBind.TableName, lo_DataBind.Alias, lo_dataTable.GetValue("CardCode", 0).ToString(), 0);
                            }
                            else
                            {
                                go_Edit = go_SBOForm.Items.Item(this.lrs_EdtCtaTrs).Specific; //Recupera el valor del EdiText
                                lo_DataBind = go_Edit.DataBind; //Guarda los valores de DataBind en la variable local
                                //Guarda el valor del formato de cuenta en la Tabla Pago Masivo de Detracciones
                                Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, lo_DataBind.TableName, lo_DataBind.Alias, lo_dataTable.GetValue("FormatCode", 0).ToString(), 0);
                                go_SBOForm.GetStaticText(this.lrs_lblDesCtaTrs).Caption =
                                    (string)Cls_QueryManager.Retorna(Cls_Query.get_NombreCuenta, "AcctName", go_Edit.Value);
                                ls_CtaTransferSYS = lo_dataTable.GetValue(0, 0);
                            }
                        }
                        catch (Exception ex)
                        {
                            Cls_Global.WriteToFile(ex.Message);
                            throw;
                            //if (!(ex is InvalidOperationException))//Muestra una ventana con el mensaje de Excepción
                            //    go_SBOApplication.MessageBox(ex.Message);
                            //throw;
                        }
                    }
                }, "txPvDd", "txPvHt", "txCtTr"));

            itemevent.Add(new sapitemevent(BoEventTypes.et_CLICK, lrs_GrdPayDTRDET, s =>
                  {
                      go_SBOForm = go_SBOFormEvent;
                      switch (s.ColUID)
                      {
                          case lrs_CabDttSelect:
                              if (!s.BeforeAction) //Calculará el monto total a pagar de los registros seleccionados
                                  this.fn_calcularTotalAPagar();
                              break;
                          case lrs_CabDttNumCns:
                              if (s.BeforeAction)
                                  ls_NroConst = string.Empty;
                              break;
                      }
                  }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_DOUBLE_CLICK, lrs_GrdPayDTRDET, s =>
            {
                if (!s.BeforeAction)
                {
                    go_SBOForm = go_SBOFormEvent;
                    if (s.ColUID.Equals(lrs_CabDttSelect) &&
                        s.Row == -1)
                        this.sb_MultipleCheckingGrid();
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_LOST_FOCUS, lrs_EdtFchCn, s =>
            {
                if (!s.BeforeAction)
                {
                    go_SBOForm = go_SBOFormEvent;
                    sb_ManejarNumerodeSerie();
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_KEY_DOWN, lrs_GrdPayDTRDET, s =>
            {
                if (!s.BeforeAction)
                {
                    go_SBOForm = go_SBOFormEvent;
                    if (Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, this.lrs_DtcPAYDTR, this.lrs_UflEstado) != this.lrs_ValEstado)
                    {
                        go_SBOForm.Items.Item(this.lrs_CmbArticulo).Enabled = false;
                        go_SBOForm.Items.Item(this.lrs_EdtCtaTrs).Enabled = false;
                        go_SBOForm.Items.Item(this.lrs_EdtFchCn).Enabled = false;
                    }

                    if (s.ColUID == lrs_CabDttNumCns)
                    {
                        if (s.CharPressed > 47 && s.CharPressed < 58)
                            ls_NroConst += Convert.ToChar(s.CharPressed).ToString();
                        //ls_NroConst = Convert.ToChar(s.CharPressed).ToString();

                        if (s.CharPressed == 9)
                        {
                            try
                            {
                                go_SBOForm.Freeze(true);
                                go_Grid = go_SBOForm.Items.Item(this.lrs_GrdPayDTRDET).Specific;

                                for (int i = go_Grid.GetDataTableRowIndex(s.Row); i < go_Grid.DataTable.Rows.Count; i++)
                                {
                                    if (i < go_Grid.DataTable.Rows.Count - 1)
                                    {
                                        if (go_Grid.DataTable.GetValue(this.lrs_CabDttCodPrv, i) == go_Grid.DataTable.GetValue(lrs_CabDttCodPrv, i + 1))
                                            go_Grid.DataTable.SetValue(lrs_CabDttNumCns, i, ls_NroConst);
                                        else
                                        {
                                            go_Grid.DataTable.SetValue(lrs_CabDttNumCns, i, ls_NroConst);
                                            break;
                                        }
                                    }
                                    else go_Grid.DataTable.SetValue(lrs_CabDttNumCns, i, ls_NroConst);
                                }
                                ls_NroConst = string.Empty;
                            }
                            finally
                            {
                                go_SBOForm.Freeze(false);
                            }
                        }
                    }
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, s => Dispose()));
        }

        private bool IsCashFlow(string account)
        {
            string queryID = "isCashFlowAccount";
            var val = Cls_QueryManager.Retorna(queryID, "CfwRlvnt", account);

            return val == "Y";
        }

        private bool EsUnicoDocumentoSeleccionado(Grid go_Grid)
        {
            int lineas = go_Grid.Rows.Count;
            int checks = 0;
            string checkBoxValue = string.Empty;
            try
            {
                //for (int i = 0; i <= lineas - 1; i++)
                //{
                //    checkBoxValue = go_Grid.DataTable.GetValue("Seleccion", i);
                //    if (go_Grid.DataTable.GetValue(lrs_CabDttSelect, i) == "N")
                //        go_Grid.DataTable.Columns.Item(lrs_CabDttSelect).Cells.Item(i).Value = "Y";

                //    if (checkBoxValue.Equals("Y"))
                //        checks++;
                //}

                for (int i = 0; i <= go_Grid.DataTable.Rows.Count - 1; i++)
                {
                    //if (go_Grid.DataTable.GetValue(lrs_CabDttSelect, i) == "N")
                    //    go_Grid.DataTable.Columns.Item(lrs_CabDttSelect).Cells.Item(i).Value = "Y";

                    checkBoxValue = go_Grid.DataTable.GetValue("Seleccion", i);
                    if (checkBoxValue.Equals("Y"))
                        checks++;
                }
            }
            catch (Exception)
            {
                throw;
            }


            if (checks == 1)
                return true;

            return false;
        }

        #region SBOIEventsFormData

        /// <Maneja el evento de datos del formulario>
        /// </>
        /// <param name="go_SBOForm"></param>
        /// <param name="po_BusinessObjectInfo"></param>
        /// <param name="po_BubbleEvent"></param>
        /// <param name="go_SBOApplication"></param>
        public void sb_EventFormData(BoEventTypes po_EventTypes)
        {
            try
            {   //Verifica el tipo de evento
                go_SBOForm = go_SBOFormEvent;

                if (po_EventTypes == BoEventTypes.et_FORM_DATA_LOAD)
                {
                    this.sb_prepararControlesEstadoAbierto();//Llama al método para cargar las detracciones por pagar
                    this.sb_bloquearColumnas(1);//Llama al método para el bloqueo de columnas
                }
                else if (po_EventTypes == BoEventTypes.et_FORM_DATA_ADD)
                {
                    go_SBOForm.GetGrid(lrs_GrdPayDTRDET).DataTable.Clear();
                    go_SBOForm.GetStaticText(lrs_lblDesCtaTrs).Caption = string.Empty;
                }
            }
            catch (Exception ex) { go_SBOApplication.MessageBox(ex.Message); } //Muestra una ventana con el mensaje de Excepción
        }
        #endregion
    }
}
