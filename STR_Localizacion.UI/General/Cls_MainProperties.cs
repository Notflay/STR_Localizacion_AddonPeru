
using STR_Localizacion.MetaData;

namespace STR_Localizacion.UI
{
    partial class Cls_Main
    {
        /// Declaracion de objetos para luego instanciarlos y usarlos en esta clase
        private Cls_Localizacion_Init go_Localizacion_Init;

        //private Cls_Numeracion lo_Numeracion;
        private Cls_Folio lo_Folio;

        private Cls_NumeracionVenta lo_NumeracionVenta;
        private Cls_NumeracionCompra lo_NumeracionCompra;
        private Cls_NumeracionInventario lo_NumeracionInventario;
        private Cls_Detraccion lo_Detraccion;
        private Cls_RucSunat lo_RucSunat;

        private Cls_PagoMasivoDetraccion lo_PagaDetracciones;
        private Cls_GenerarAsiento lo_GenerarAsiento;
        private Cls_AnulCorrelativo lo_AnulCorrelativo;
        private Cls_AsientoCtaDestino lo_AsientoCtaDestino;
        private Cls_MedioPago lo_MedioPago;
        private Cls_ReclasificacionWIP lo_ReclasificacionWIP;
        private Cls_FEFO lo_FEFO;
        private Cls_StratSettings lo_StratSettings;
        private Cls_Revalorizacion lo_Revalorizacion;
        private Cls_MenuConfiguracion lo_MenuConfiguracion;
        private Cls_SeleccionSeriesArticulo lo_SeleccionSeriesArticulo;
        private Cls_COM_DescargaInventario lo_DescargaInventario;
        private Cls_ReprocesarAsientoProvision lo_GenerarAsientoProvision;

        private Cls_ListaMateriales lo_ListaMateriales;
        // Generar TXT de Orden de Venta
        //private Cls_GenerarTxt lo_GenerarTxt;
        //private Cls_SNSolidario snSolidario;
        private Cls_SBOMessageBox sboMessageBox;

        //Adelanto de clientes

        private Cls_AdelantoCliente lo_AdelantoCliente;
        private Cls_Provisiones lo_Provisiones;
        private Cls_ProvisionesNoDomiciliado lo_ProvisionesNoDomiciliado;
        private Cls_FondoDeGarantia lo_FondoDeGarantia;
    }
}
