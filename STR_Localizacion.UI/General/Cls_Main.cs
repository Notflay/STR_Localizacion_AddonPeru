using STR_Localizacion.UTIL;
using System;
using System.Windows.Forms;
using System.Xml;
using THR = System.Threading;
using STR_Localizacion.DL;
using System.Runtime.CompilerServices;
using SAPbobsCOM;

namespace STR_Localizacion.UI
{
    partial class Cls_Main : Cls_Properties
    {

        #region "Inicializacion del add-on"

        public Cls_Main()
        {
            lc_NameClass = "Cls_Main";
            ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);

            sb_SetApplication();
            //sb_setApplicationMenu()
            if (go_SBOApplication != null && go_SBOCompany != null)
            {
                go_SBOApplication.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(lo_SBOApplication_MenuEvent);
                go_SBOApplication.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(lo_SBOApplication_ItemEvent);
                go_SBOApplication.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(lo_SBOApplication_FormDataEvent);
                go_SBOApplication.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(lo_SBOApplication_AppEvent);
                go_SBOApplication.RightClickEvent += new SAPbouiCOM._IApplicationEvents_RightClickEventEventHandler(lo_SBOApplication_RightClickEvent);

                RuntimeHelpers.RunClassConstructor(typeof(Cls_MenuConfiguracion).TypeHandle);
                sb_InitObjects();
                sb_SetFilters();
                if (true)
                {
                    //   go_Localizacion_Init.sb_VerificarInstalacion();

                    try
                    {
                        sb_CargarDatosDeConfiguracion();
                    }
                    catch (Exception e)
                    {
                        go_SBOApplication.StatusBar.SetText(e.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        Environment.Exit(0);
                    }

                    sb_AddMenu();
                }
            }
        }


        private void sb_CargarDatosDeConfiguracion()
        {
            try
            {
                SAPbobsCOM.Recordset go_RecordSet;
                Cls_Global.APDifGain = Cls_QueryManager.Retorna(Cls_Query.get_CtasDifCambio, "APConDiffG", go_SBOCompany.GetDBServerDate().Year);
                Cls_Global.APDifLoss = Cls_QueryManager.Retorna(Cls_Query.get_CtasDifCambio, "APConDiffL", go_SBOCompany.GetDBServerDate().Year);

                if (string.IsNullOrEmpty(Cls_Global.APDifGain) || string.IsNullOrEmpty(Cls_Global.APDifGain))
                    throw new Exception("Las cuentas de ganancia/perdida por diferencia de cambio para compras no han sido configuradas. Gestion/Finanzas/Determinación de cuentas - Compras");

                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, null);

                if (go_RecordSet.RecordCount == 0)
                    Cls_QueryManager.Procesa(Cls_Query.create_LOCConfig, "E", "F", "E");

                Cls_Global.metCalculoTC = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_MCTC");
                Cls_Global.fuenteTC = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_SRCTC");
                Cls_Global.metAsientoDestino = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_MTAD");
                Cls_Global.ProvisionNDActivo = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_PROV_IGV_ND");
                Cls_Global.ProvisionNDCodigosRetencion = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_IMPRET");
                Cls_Global.ProvisionNDCuentaDebito = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_IDCTA_DEB");
                Cls_Global.ProvisionNDCuentaCredito = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_IDCTA_CRE");
                Cls_Global.ProvisionNDFormatoCuentaDebito = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_FMCTA_DEB");
                Cls_Global.ProvisionNDFormatoCuentaCredito = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_FMCTA_CRE");
                Cls_Global.RetencionDeGarantiaActivo = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_RG_ACT");
                Cls_Global.ImpuestoRetencionDeGarantia = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_IMPRG");
                Cls_Global.ReconciliacionActivo = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_RECO_ACT");
                Cls_Global.ReconciliacionCuenta = Cls_QueryManager.Retorna(Cls_Query.get_ConfigData, "U_STR_RECO_CTA");

                Recordset oRecordSet = (Recordset)go_SBOCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string query = string.Format("SELECT TOP 1 * FROM \"@BPP_PARAMS\" ");
                oRecordSet.DoQuery(query);

                Cls_Global.cuentaPuente = oRecordSet.Fields.Item("U_BPP_CNTPUENTE").Value.ToString();
                Cls_Global.numeroLote = oRecordSet.Fields.Item("U_BPP_NROLOTE").Value.ToString();
                Cls_Global.rutaDetraciones = oRecordSet.Fields.Item("U_BPP_DETRUTA").Value.ToString();
                Cls_Global.rutaPagos = oRecordSet.Fields.Item("U_BPP_PGMRUTA").Value.ToString();

            }
            catch (Exception)
            {
                Cls_Global.metCalculoTC = string.Empty;
                Cls_Global.fuenteTC = string.Empty;
                throw;
            }

        }

        public void sb_setApplicationMenu(SAPbobsCOM.Company po_company, SAPbouiCOM.Application po_application)
        {
            Cls_Global.go_SBOApplication = po_application;
            Cls_Global.go_SBOCompany = po_company;
        }
        private void sb_InitObjects()
        {
            lo_AnulCorrelativo = new Cls_AnulCorrelativo();
            lo_AsientoCtaDestino = new Cls_AsientoCtaDestino();
            lo_Detraccion = new Cls_Detraccion();
            lo_RucSunat = new Cls_RucSunat();

            lo_FEFO = new Cls_FEFO();
            lo_MedioPago = new Cls_MedioPago();

            lo_Folio = new Cls_Folio();
            lo_NumeracionVenta = new Cls_NumeracionVenta();
            lo_NumeracionCompra = new Cls_NumeracionCompra();
            lo_NumeracionInventario = new Cls_NumeracionInventario();

            lo_PagaDetracciones = new Cls_PagoMasivoDetraccion();
            lo_GenerarAsiento = new Cls_GenerarAsiento();
            lo_ReclasificacionWIP = new Cls_ReclasificacionWIP();
            lo_StratSettings = new Cls_StratSettings();
            lo_Revalorizacion = new Cls_Revalorizacion();
            lo_MenuConfiguracion = new Cls_MenuConfiguracion();
            lo_GenerarAsientoProvision = new Cls_ReprocesarAsientoProvision();

            go_Localizacion_Init = new MetaData.Cls_Localizacion_Init();

            lo_ListaMateriales = new Cls_ListaMateriales();
            // Generar TXT de Orden de Venta
            //lo_GenerarTxt = new Cls_GenerarTxt();
            //snSolidario = new Cls_SNSolidario();
            sboMessageBox = new Cls_SBOMessageBox();

            //Adelanto de clientes
            lo_AdelantoCliente = new Cls_AdelantoCliente();

            //Seleccion masiva de series articulo
            lo_SeleccionSeriesArticulo = new Cls_SeleccionSeriesArticulo();

            //Descarga de inventario
            lo_DescargaInventario = new Cls_COM_DescargaInventario();

            //Proviciones
            lo_ProvisionesNoDomiciliado = new Cls_ProvisionesNoDomiciliado();

            //Provisiones No domiciliado
            lo_Provisiones = new Cls_Provisiones();

            //Fonde de Garantía
            lo_FondoDeGarantia = new Cls_FondoDeGarantia();

            sboMessageBox = new Cls_SBOMessageBox();
        }

        private void sb_SetApplication()
        {
            try
            {
                string ls_ConnectionString =
                    Environment.GetCommandLineArgs().
                    GetValue(Environment.GetCommandLineArgs().Length > 1 ? 1 : 0).ToString();

                Cls_Global.go_SBOGUIAPI = new SAPbouiCOM.SboGuiApi();
                Cls_Global.go_SBOGUIAPI.Connect(ls_ConnectionString);

                if (Cls_Global.go_SBOApplication == null)
                    Cls_Global.go_SBOApplication = Cls_Global.go_SBOGUIAPI.GetApplication(-1);

                if (Cls_Global.go_SBOApplication != null) sb_SetCompany();

                Cls_Global.go_TimerComprueba =
                    new THR.Timer(e =>
                    {
                        try
                        {
                            Cls_Global.go_SBOGUIAPI.GetApplication(-1);
                        }
                        catch (Exception)
                        {
                            Environment.Exit(0);
                        }
                    }, null, 0, 5000);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
        }

        private void sb_SetCompany()
        {
            try
            {
                Cls_Global.go_SBOCompany = Cls_Global.go_SBOApplication.Company.GetDICompany();

                //Cls_Global.go_SBOCompany.Server

                go_SBOApplication.StatusBar.SetText("Iniciando el Addon de Localizacion", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                if (Cls_Global.go_SBOCompany != null)
                {
                    //Clase Global
                    Cls_Global.go_ServerType = go_SBOCompany.DbServerType;
                    Cls_Global.go_SBObob = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);
                }
            }
            catch (Exception ex)
            {
                go_SBOApplication.
                    StatusBar.
                    SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                throw;
            }
        }

        private void sb_SetFilters()
        {
            SAPbouiCOM.EventFilters lo_EventFilters = new SAPbouiCOM.EventFilters();
            SAPbouiCOM.EventFilter lo_EventFilter = null;

            lo_EventFilter = lo_EventFilters.Add(SAPbouiCOM.BoEventTypes.et_ALL_EVENTS);

            lo_EventFilter.AddEx(lo_AnulCorrelativo.gs_FormName);
            lo_EventFilter.AddEx(lo_PagaDetracciones.gs_FormName);
            lo_EventFilter.AddEx(lo_GenerarAsiento.gs_FormName);
            lo_EventFilter.AddEx(lo_AsientoCtaDestino.gs_FormName);
            lo_EventFilter.AddEx(lo_StratSettings.gs_FormName);
            lo_EventFilter.AddEx(lo_MenuConfiguracion.gs_FormName); // Configuracion de Detracciones
            lo_EventFilter.AddEx(lo_AdelantoCliente.gs_FormName);
            lo_EventFilter.AddEx(lo_GenerarAsientoProvision.gs_FormName);

            lo_Folio.go_InternalFormID.ForEach(s => lo_EventFilter.AddEx(s));
            lo_NumeracionVenta.go_InternalFormID.ForEach(s => lo_EventFilter.AddEx(s));
            lo_NumeracionCompra.go_InternalFormID.ForEach(s => lo_EventFilter.AddEx(s));
            lo_NumeracionInventario.go_InternalFormID.ForEach(s => lo_EventFilter.AddEx(s));

            //Detraccion | Los demas formularios ya estan asigados
            lo_EventFilter.AddEx("134"); // Consulta RUC
            lo_EventFilter.AddEx("65211"); // Lista de Materiales 20/05/2022

            lo_EventFilter.AddEx("146");//MedioPago
            lo_EventFilter.AddEx("196");//MedioPago
            lo_EventFilter.AddEx("39724");//Solicitud de Compra
            lo_EventFilter.AddEx("39698");//Oferta de Compra
            //lo_EventFilter.AddEx("4142"); //Generador de Consulta 

            lo_EventFilter.AddEx("139");//Orden de Venta
            lo_EventFilter.AddEx("0");

            lo_EventFilter.AddEx("25");// Formulario de seleccion de numeros de serie
            lo_EventFilter.AddEx(lo_SeleccionSeriesArticulo.gs_FormName);

            lo_EventFilter.AddEx("1470000200");
            lo_EventFilter.AddEx("1250000940");


            lo_EventFilter.AddEx("143");//Entrada de Mercancia(Proveedores)
            lo_EventFilter.AddEx("170"); // Pagos recibidos 05/04/2022

            go_SBOApplication.SetFilter(lo_EventFilters);

            lo_EventFilter = lo_EventFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_LOAD);
            go_SBOApplication.SetFilter(lo_EventFilters);

            lo_EventFilter = lo_EventFilters.Add(SAPbouiCOM.BoEventTypes.et_MENU_CLICK);
            lo_EventFilter.AddEx("STR_mnu_Settings");

            //lo_EventFilter = lo_EventFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD);
            //lo_EventFilter.AddEx("143");

            go_SBOApplication.SetFilter(lo_EventFilters);
        }

        private void sb_AddMenu()
        {
            XmlDocument oMnuXML = new XmlDocument();
            go_SBOApplication.StatusBar.SetText("Addon Perú: Cargando funcionalidades de Localización...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            go_SBOApplication.Forms.GetFormByTypeAndCount(169, 1).Freeze(true);
            try
            {
                oMnuXML.LoadXml(Properties.Resources.Menu);
                go_SBOApplication.LoadBatchActions(oMnuXML.InnerXml);
                go_SBOApplication.StatusBar.SetText("El menu del Addon Localización fue cargado correctamente. ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
            catch (System.IO.FileNotFoundException)
            {
                go_SBOApplication.StatusBar.SetText("El recurso: Menu.xml, no fue encontrado...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            catch (Exception ex)
            {
                go_SBOApplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                go_SBOApplication.Forms.GetFormByTypeAndCount(169, 1).Freeze(false);
                go_SBOApplication.Forms.GetFormByTypeAndCount(169, 1).Update();
                oMnuXML = null;
            }
        }

        #endregion
    }
}
