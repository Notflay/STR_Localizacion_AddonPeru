using SAPbouiCOM;
using STR_Localizacion.DL;
using System.Linq;
using STR_Localizacion.UI;
using STR_Localizacion.UTIL;
using System;
using STR_Localizacion.BL;

namespace STR_Localizacion.UI
{
    partial class Cls_Main
    {
        #region Eventos

        /// <ItemEvent>
        /// Creación del método que utiliza las propiedades del Evento "ItemEvent"
        /// </summary>
        /// <param name="ps_FormUID"></param>
        /// <param name="pe_itemEvent"></param>
        /// <param name="pb_BubbleEvent"></param>
        private void lo_SBOApplication_ItemEvent(string ps_FormUID, ref ItemEvent pe_itemEvent, out bool pb_BubbleEvent)
        {
            pb_BubbleEvent = true;
            if (go_SBOFormActive == null) return;

            try
            {
                if (ps_FormUID.Equals("-141"))
                {
                }
                go_SBOFormEvent = go_SBOApplication.Forms.Item(ps_FormUID);
            }
            catch (Exception)
            {
                go_SBOFormEvent = null;
            }

            lc_NameMethod = "lo_SBOApplication_ItemEvent";

            try
            {
                string ls_FormTypeEx = pe_itemEvent.FormTypeEx;
                if (pe_itemEvent.FormTypeEx == "ConfLocalizacion")
                    pb_BubbleEvent = lo_MenuConfiguracion.itemevent.Perform(pe_itemEvent);


                if (go_SBOApplication.Forms.ActiveForm.TypeEx == "42")
                {
                    if (lo_StratSettings.fn_StratSettings("FEFO"))
                    {
                        if (lo_FEFO.fn_validaSN())
                            pb_BubbleEvent = lo_FEFO.itemevent.Perform(pe_itemEvent);
                    }
                }


                else if (lo_NumeracionVenta.go_InternalFormID.Any(s => s.Equals(ls_FormTypeEx)))
                {
                    lo_NumeracionVenta.SetTableName(ls_FormTypeEx);
                    pb_BubbleEvent = lo_NumeracionVenta.itemevent.Perform(pe_itemEvent);
                    //if (ls_FormTypeEx == "133")
                    //{
                    //    pb_BubbleEvent = snSolidario.itemevent.Perform(pe_itemEvent);
                    //    snSolidario.SetSBOMessageBox = sboMessageBox;
                    //}
                }
                else if (lo_NumeracionCompra.go_InternalFormID.Any(s => s.Equals(ls_FormTypeEx)))
                {
                    lo_NumeracionCompra.SetTableName(ls_FormTypeEx);
                    pb_BubbleEvent = lo_NumeracionCompra.itemevent.Perform(pe_itemEvent);
                    pb_BubbleEvent = lo_Detraccion.itemevent.Perform(pe_itemEvent);
                }
                else if (lo_NumeracionInventario.go_InternalFormID.Any(s => s.Equals(ls_FormTypeEx)))
                {
                    lo_NumeracionInventario.SetTableName(ls_FormTypeEx);
                    pb_BubbleEvent = lo_NumeracionInventario.itemevent.Perform(pe_itemEvent);
                }
                else if (lo_Folio.go_InternalFormID.Any(s => s.Equals(ls_FormTypeEx)))
                    lo_Folio.itemevent.Perform(pe_itemEvent);

                else if (pe_itemEvent.FormTypeEx == "134")
                {
                    pb_BubbleEvent = lo_Detraccion.itemevent.Perform(pe_itemEvent);
                    pb_BubbleEvent = lo_RucSunat.itemevent.Perform(pe_itemEvent);
                    pb_BubbleEvent = lo_FondoDeGarantia.itemevent.Perform(pe_itemEvent);
                }

                /// Anulacion
                else if (lo_AnulCorrelativo.gs_FormName == pe_itemEvent.FormTypeEx)
                    pb_BubbleEvent = lo_AnulCorrelativo.itemevent.Perform(pe_itemEvent);
                //    /// Pago de detracciones
                else if (lo_PagaDetracciones.gs_FormName == pe_itemEvent.FormTypeEx)
                    pb_BubbleEvent = lo_PagaDetracciones.itemevent.Perform(pe_itemEvent);

                //    /// Asiento Cuenta Destino
                else if (pe_itemEvent.FormTypeEx == lo_AsientoCtaDestino.gs_FormName)
                    pb_BubbleEvent = lo_AsientoCtaDestino.itemevent.Perform(pe_itemEvent);

                else if (pe_itemEvent.FormTypeEx == "196" || pe_itemEvent.FormTypeEx == "146" || pe_itemEvent.FormTypeEx == "170")
                    pb_BubbleEvent = lo_MedioPago.itemevent.Perform(pe_itemEvent);

                else if (pe_itemEvent.FormTypeEx == lo_GenerarAsiento.gs_FormName)
                    pb_BubbleEvent = lo_GenerarAsiento.itemevent.Perform(pe_itemEvent);

                /// Asiento Provision
                else if (pe_itemEvent.FormTypeEx == lo_GenerarAsientoProvision.gs_FormName)
                    pb_BubbleEvent = lo_GenerarAsientoProvision.itemevent.Perform(pe_itemEvent);
                //else if (pe_itemEvent.FormTypeEx == "0")
                //{
                //    if (pe_itemEvent.EventType == BoEventTypes.et_FORM_LOAD && !pe_itemEvent.BeforeAction)
                //    {
                //        sboMessageBox.SetForm = go_SBOApplication.Forms.Item(ps_FormUID);
                //        sboMessageBox.answerQuestion();
                //    }
                //}
                else if (pe_itemEvent.FormTypeEx == lo_AdelantoCliente.gs_FormName)
                    pb_BubbleEvent = lo_AdelantoCliente.itemevent.Perform(pe_itemEvent);
                else if (pe_itemEvent.FormTypeEx == lo_SeleccionSeriesArticulo.gs_FormName || pe_itemEvent.FormTypeEx == "25")
                {
                    if (pe_itemEvent.EventType == BoEventTypes.et_FORM_LOAD && pe_itemEvent.BeforeAction && pe_itemEvent.FormTypeEx == "25")
                        lo_SeleccionSeriesArticulo.sb_FormSAPLoad(go_SBOApplication.Forms.Item(pe_itemEvent.FormUID));
                    pb_BubbleEvent = lo_SeleccionSeriesArticulo.itemevent.Perform(pe_itemEvent);
                }
                else if (pe_itemEvent.FormTypeEx == "1250000940" && Cls_MenuConfiguracion.DscInvActivo)
                {
                    if (pe_itemEvent.EventType == BoEventTypes.et_FORM_LOAD && pe_itemEvent.BeforeAction)
                        lo_DescargaInventario.solicitudTrasladoLoad(pe_itemEvent.FormUID);
                    pb_BubbleEvent = lo_DescargaInventario.itemevent.Perform(pe_itemEvent);
                }

                else if (pe_itemEvent.FormTypeEx == "65211")
                {
                    pb_BubbleEvent = lo_ListaMateriales.itemevent.Perform(pe_itemEvent);
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //else if (pe_itemEvent.FormTypeEx == "65211")
                //{
                //    if (pe_itemEvent.BeforeAction == true)
                //        pb_BubbleEvent = lo_ReclasificacionWIP.fn_HandleItemEvent(go_SBOApplication, go_SBOApplication.Forms.Item(ps_FormUID), pe_itemEvent);
                //}
                //else if (pe_itemEvent.FormTypeEx == lo_StratSettings.gs_FormName)
                //    pb_BubbleEvent = lo_StratSettings.itemevent.Perform(pe_itemEvent);
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                ExceptionPrepared.SaveInLog();
            }
        }

        /// <MenuEvent>
        /// Manejo de los eventos de menu de la aplicacion: MenuEvent.
        /// </summary>
        /// <param name="pe_MenuEvent"></param>
        /// <param name="pb_BubbleEvent"></param>
        private void lo_SBOApplication_MenuEvent(ref MenuEvent pe_MenuEvent, out bool pb_BubbleEvent)
        {
            pb_BubbleEvent = true;
            lc_NameMethod = "sb_SBOApplication_MenuEvent";

            try
            {
                if (pe_MenuEvent.BeforeAction)
                {
                    switch (pe_MenuEvent.MenuUID)
                    {
                        case "ST_LOC_GenAsProv":
                            if (Validacion.fn_getComparacion(1) == true)
                                lo_GenerarAsientoProvision.sb_FormLoad();
                            else return;
                            break;
                        case "STR_LOC_AnulCorrelativo":
                            if (Validacion.fn_getComparacion(1) == true)
                                lo_AnulCorrelativo.sb_FormLoad();
                            else return;
                            break;
                        case "STR_LOC_PagoMasivoDtr":
                            if (Validacion.fn_getComparacion(1) == true)
                                lo_PagaDetracciones.sb_FormLoad();
                            else return;
                            break;
                        case "STR_LOC_GeneradorAsiento":
                            if (Validacion.fn_getComparacion(1) == true)
                                lo_GenerarAsiento.sb_FormLoad();
                            else return;
                            break;
                        case "STR_LOC_AsientoDestino":
                            if (Validacion.fn_getComparacion(1) == true)
                            {
                                lo_AsientoCtaDestino.FormLoad();
                                lo_AsientoCtaDestino.sb_PrepareDataSource(go_SBOApplication.Forms.ActiveForm);
                            }
                            else return;
                            break;
                        case "MNU_AdlntClnt":
                            if (Validacion.fn_getComparacion(1) == true)
                                lo_AdelantoCliente.sb_FormLoad();
                            else return;
                            break;
                        case Cls_AdelantoCliente.gs_MnuBorrarFila:
                            if (pe_MenuEvent.BeforeAction)
                            {
                                lo_AdelantoCliente.sb_DeleteRowMatrix();
                            }
                            break;
                        case "1281":
                        case "1288":
                        case "1289":
                        case "1290":
                        case "1291":
                        case "1304":
                            if (go_SBOFormActive.TypeEx.Contains(lo_GenerarAsiento.gs_FormName))
                                go_SBOFormActive.Freezer(lo_GenerarAsiento.gs_FormName, lo_GenerarAsiento.sb_SetBound());
                            // case "STR_mnu_Settings":
                            //lo_StratSettings.sb_FormLoad();

                            break;
                        //case "2053":
                        //    snSolidario.formLoad();
                        //    break;
                        case "STR_mnu_Settings":
                            if (Validacion.fn_getComparacion(1) == true)
                                lo_MenuConfiguracion.Sb_FormLoad();
                            else return;
                            break;
                        case "1284":
                            if (pe_MenuEvent.BeforeAction)
                            {
                                var lo_FrmAux = go_SBOApplication.Forms.ActiveForm;
                                var ls_NmbFrm = lo_FrmAux.TypeEx;
                                if (ls_NmbFrm.StartsWith("-")) ls_NmbFrm = ls_NmbFrm.Remove(0, 1);
                                if (lo_AdelantoCliente.gs_FormName.Equals(ls_NmbFrm))
                                    pb_BubbleEvent = lo_AdelantoCliente.cancelarDocumento();
                            }
                            break;

                    }
                }
                else
                {
                    switch (pe_MenuEvent.MenuUID)
                    {
                        case "1282":
                        case "1281":
                            if (go_SBOFormActive.TypeEx.Contains(lo_PagaDetracciones.gs_FormName))
                                lo_PagaDetracciones.sb_SeleccionarSerie();
                            else if (go_SBOFormActive.TypeEx.Contains(lo_GenerarAsiento.gs_FormName))
                            {
                                if (pe_MenuEvent.MenuUID.Equals("1282"))
                                {
                                    if (Validacion.fn_getComparacion(1) == true)
                                        lo_GenerarAsiento.sb_DataFormLoad();
                                    else return;
                                }
                            }
                            else if (go_SBOFormActive.TypeEx.Contains(lo_AdelantoCliente.gs_FormName))
                                if (pe_MenuEvent.MenuUID.Equals("1282"))
                                    lo_AdelantoCliente.sb_DataFormLoadAdd();
                            break;
                    }
                }

                if (go_SBOFormActive == null) return;

                string ls_TypeEx = go_SBOFormActive.TypeEx;

                if (lo_NumeracionVenta.go_InternalFormID.Any(s => s.Equals(ls_TypeEx)))
                    pb_BubbleEvent = lo_NumeracionVenta.menuevent.Perform(pe_MenuEvent);
                else if (lo_NumeracionCompra.go_InternalFormID.Any(s => s.Equals(ls_TypeEx)))
                {
                    pb_BubbleEvent = lo_NumeracionCompra.menuevent.Perform(pe_MenuEvent);
                    pb_BubbleEvent = lo_Detraccion.menuevent.Perform(pe_MenuEvent);
                }
                else if (lo_NumeracionInventario.go_InternalFormID.Any(s => s.Equals(ls_TypeEx)))
                    pb_BubbleEvent = lo_NumeracionInventario.menuevent.Perform(pe_MenuEvent);
                else if (lo_AnulCorrelativo.gs_FormName == ls_TypeEx)
                    pb_BubbleEvent = lo_AnulCorrelativo.menuevent.Perform(pe_MenuEvent);
                else if (ls_TypeEx == lo_PagaDetracciones.gs_FormName)
                    pb_BubbleEvent = lo_PagaDetracciones.menuevent.Perform(pe_MenuEvent);
                else if (ls_TypeEx == "134")
                {
                    pb_BubbleEvent = lo_Detraccion.menuevent.Perform(pe_MenuEvent);
                    pb_BubbleEvent = lo_RucSunat.menuevent.Perform(pe_MenuEvent);
                    pb_BubbleEvent = lo_FondoDeGarantia.menuevent.Perform(pe_MenuEvent);
                }
                else if (ls_TypeEx == "170")
                    pb_BubbleEvent = lo_MedioPago.menuevent.Perform(pe_MenuEvent);

                else if (ls_TypeEx == "65211")
                    pb_BubbleEvent = lo_ListaMateriales.menuevent.Perform(pe_MenuEvent);
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                ExceptionPrepared.SaveInLog(false);
            }
        }

        /// <FormDataEvent>
        /// Manejo de los eventos de datos de formulario de la aplicacion: FormDataEvent
        /// </summary>
        /// <param name="pe_BusinessObjectInfo"></param>
        /// <param name="pb_BubbleEvent"></param>
        private void lo_SBOApplication_FormDataEvent(ref BusinessObjectInfo pe_BusinessObjectInfo, out bool pb_BubbleEvent)
        {
            lc_NameMethod = "sb_SBOApplication_FormDataEvent";
            pb_BubbleEvent = true;
            try
            {
                go_SBOFormEvent = go_SBOApplication.Forms.Item(pe_BusinessObjectInfo.FormUID);

                switch (pe_BusinessObjectInfo.FormTypeEx)
                {
                    case "141":
                    case "60092":
                    case "65306":
                    case "65301":
                    case "181":
                        if (pe_BusinessObjectInfo.ActionSuccess && pe_BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD) //ACTUALIZAR AL FINAL -> MODO ADD
                        {
                            lo_Detraccion.sb_generarAsientoDetraccion();

                            if (pe_BusinessObjectInfo.FormTypeEx.Equals("141"))
                            {
                                lo_Provisiones.sb_generarAsientoAjustePorTipoDeCambio();
                                lo_FondoDeGarantia.GenerarAsientoRetencionDeGarantia();
                                lo_ProvisionesNoDomiciliado.GenerarAsientoProvisionIGVNoDomiciliado();
                            }
                        }

                        break;
                    case "frmPagoMasivoDetraccion":
                        if (pe_BusinessObjectInfo.ActionSuccess)
                            lo_PagaDetracciones.sb_EventFormData(pe_BusinessObjectInfo.EventType);
                        break;
                    case "frmGeneradorAsiento":
                        if (!pe_BusinessObjectInfo.BeforeAction && pe_BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD)
                            lo_GenerarAsiento.sb_EventFormData();
                        break;
                    case "940":
                        if (pe_BusinessObjectInfo.ActionSuccess && pe_BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                            lo_Revalorizacion.sb_EventFormData(pe_BusinessObjectInfo.ObjectKey);
                        break;
                    //case "139": // Orden de Venta - TXT
                    //    if (pe_BusinessObjectInfo.ActionSuccess && pe_BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                    //        //lo_GenerarTxt.sb_EventFormData(pe_BusinessObjectInfo.ObjectKey);
                    //    break;
                    case "frmAdelantoCliente":
                        if (pe_BusinessObjectInfo.ActionSuccess && pe_BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD && !pe_BusinessObjectInfo.BeforeAction)
                            lo_AdelantoCliente.setLastRecord();
                        break;
                    case "1470000200":
                        if (!pe_BusinessObjectInfo.BeforeAction && pe_BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD &&
                            pe_BusinessObjectInfo.ActionSuccess && Cls_MenuConfiguracion.DscInvActivo)
                            lo_DescargaInventario.generarSolicitudTraslado(pe_BusinessObjectInfo);
                        break;
                    case "1250000940":
                        if (!pe_BusinessObjectInfo.BeforeAction && pe_BusinessObjectInfo.ActionSuccess
                            && pe_BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD)
                            lo_DescargaInventario.mostrarLinkSolcCompra(pe_BusinessObjectInfo.FormUID);
                        break;
                    case "143":
                        if (pe_BusinessObjectInfo.ActionSuccess && go_SBOApplication.Forms.Item(pe_BusinessObjectInfo.FormUID).Mode == BoFormMode.fm_ADD_MODE)
                            lo_Provisiones.sb_generarAsientoProvision();
                        break;
                    case "FrmAsientosProvision":
                        if (!pe_BusinessObjectInfo.BeforeAction && pe_BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD)
                            lo_GenerarAsientoProvision.LoadAction();
                        break;
                    case "170": // Pago Recibido
                        if /*(!pe_BusinessObjectInfo.BeforeAction)*/(pe_BusinessObjectInfo.ActionSuccess && go_SBOApplication.Forms.Item(pe_BusinessObjectInfo.FormUID).Mode == BoFormMode.fm_ADD_MODE)
                            lo_MedioPago.sb_GenerarAsientoPago(pe_BusinessObjectInfo);
                        break;

                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //case "65211":
                        //    if (!pe_BusinessObjectInfo.BeforeAction)
                        //        lo_ReclasificacionWIP.fn_HandleFormDataEvent(go_SBOApplication, Cls_Global.go_SBOCompany, pe_BusinessObjectInfo, go_SBOApplication.Forms.Item(pe_BusinessObjectInfo.FormUID));
                        //    ///                                    (go_SBOApplication.Forms.Item(pe_BusinessObjectInfo.FormUID),pe_BusinessObjectInfo,pb_BubbleEvent,go_SBOApplication);

                        //    break;
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                ExceptionPrepared.SaveInLog(false);
            }
        }

        /// <AppEvent>
        /// Procedimientos disparados mediante eventos de la aplicacion
        /// </summary>
        /// <param name="EventType"></param>
        private void lo_SBOApplication_AppEvent(BoAppEventTypes EventType)
        {
            lc_NameMethod = "sb_SBOApplication_AppEvent";
            /// Desinstaciar el objeto go_recordSet para evitar errores.
            if (EventType != BoAppEventTypes.aet_LanguageChanged)
                Cls_Global.go_RecordSet = null;
            try
            {
                switch (EventType)
                {
                    case BoAppEventTypes.aet_ShutDown: /// Finalización de la sesión de SBO
                    case BoAppEventTypes.aet_CompanyChanged: /// Cambio de compañía
                    case BoAppEventTypes.aet_ServerTerminition: /// Pérdida de la comunicación con la UI                                                   
                        go_SBOCompany.Disconnect();
                        System.Windows.Forms.Application.Exit();
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                ExceptionPrepared.SaveInLog();
            }
        }

        private void lo_SBOApplication_RightClickEvent(ref ContextMenuInfo po_RghClkEvent, out bool pb_BubbleEvent)
        {
            pb_BubbleEvent = true;
            Form lo_FrmAux = null;
            string ls_NmbFrm = string.Empty;

            try
            {
                lo_FrmAux = go_SBOApplication.Forms.Item(po_RghClkEvent.FormUID);
                lo_FrmAux = go_SBOApplication.Forms.ActiveForm;
                ls_NmbFrm = lo_FrmAux.TypeEx;
                if (ls_NmbFrm.StartsWith("-")) ls_NmbFrm = ls_NmbFrm.Remove(0, 1);
                if (ls_NmbFrm == lo_AdelantoCliente.gs_FormName)
                    pb_BubbleEvent = lo_AdelantoCliente.fn_HandleRightClickEvent(po_RghClkEvent);

            }
            catch (Exception ex)
            {
                go_SBOApplication.StatusBar.SetText("RghClcEvnt: " + ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }
        #endregion
    }
}
