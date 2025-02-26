namespace STR_Localizacion.UI
{
    partial class Cls_PagoMasivoDetraccion
    {
        #region Declaraciones
        //*****************************************************************
        //                      Constantes generales del módulo
        //*****************************************************************
        private string ls_NroConst = string.Empty;
        private string ls_docEntry = string.Empty;
        private string ls_msjCancelacion = string.Empty;
        private int li_rsptCancelacion = 0;
        private int li_CountRowSelect = 0;
        private readonly string lrs_FchFormat = "yyyyMMdd";
        private readonly string lrs_ValEstado = "O";
        private readonly string lrs_ValAnulado = "Y";

        //*****************************************************************
        //                      DataSource
        //*****************************************************************
        private readonly string lrs_DtcPAYDTR = "@BPP_PAYDTR";
        private readonly string lrs_DtcPAYDTRDET = "@BPP_PAYDTRDET";
        //*****************************************************************
        //                      User Fields - @BPP_PAYDTR
        //*****************************************************************
        private readonly string lrs_UflDocEntry = "DocEntry";
        private readonly string lrs_UflSeries = "Series";
        private readonly string lrs_UflDocNum = "DocNum";
        private readonly string lrs_UflCrtDate = "CreateDate";
        private readonly string lrs_UflEstado = "Status";
        private readonly string lrs_UflCanceled = "Canceled";
        private readonly string lrs_UflLineId = "Line_ID";
        //private readonly string lrs_ = "Canceled";

        private readonly string lrs_UflPrvDd = "U_BPP_PvDd";
        private readonly string lrs_UflPrvHt = "U_BPP_PvHt";
        private readonly string lrs_UflFchCnDd = "U_BPP_FcDd";
        private readonly string lrs_UflFchCnHt = "U_BPP_FcHt";
        private readonly string lrs_UflFchVnDd = "U_BPP_FvDd";
        private readonly string lrs_UflFchVnHt = "U_BPP_FvHt";

        private readonly string lrs_UflTtlPg = "U_BPP_TtPg";
        private readonly string lrs_UflFchCnPg = "U_BPP_FcCt";

        private readonly string lrs_UflArtPr = "U_BPP_PtFC";
        private readonly string lrs_UflCtaTrs = "U_BPP_CtTr";
        private readonly string lrs_UflFchDp = "U_BPP_FcDp";
        private readonly string lrs_UflNumDp = "U_BPP_NmDp";
        //*****************************************************************
        //                      User Fields - @BPP_PAYDTRDET
        //*****************************************************************
        private readonly string lrs_UflCodPrv = "U_BPP_CgPv";

        private readonly string lrs_UflIdDtr = "U_BPP_DEAs";
        private readonly string lrs_UflCodPg = "U_BPP_DEPg";
        private readonly string lrs_UflNomPv = "U_BPP_NbPv";
        private readonly string lrs_UflNumDc = "U_BPP_NmDc";
        private readonly string lrs_UflNumPg = "U_BPP_NmPg";
        private readonly string lrs_UflDtrXPg = "U_BPP_SdAs"; 
        private readonly string lrs_Serie = "Series";
        private readonly string lrs_UflCodBn = "U_BPP_CdBn";
        private readonly string lrs_UflCodOpe = "U_BPP_CdOp";
        private readonly string lrs_UflTipObj = "U_BPP_TpOb";
        private readonly string lrs_UflNumCns = "U_BPP_Cnst";
        private readonly string lrs_UflFchDc = "U_BPP_FcDc";
        private readonly string lrs_UflCodDtr = "U_BPP_CdDt";
        //*****************************************************************
        //                      Controles UI
        //*****************************************************************
        //________________________ComboBox________________________
        private readonly string lrs_CmbSeries = "cbSeries";
        private readonly string lrs_CmbEstado = "cbStatus";
        private readonly string lrs_CmbArticulo = "cbCshFlw";
        //________________________EditText________________________
        private readonly string lrs_EdtDocEntry = "txDocEntry";

        private readonly string lrs_EdtPrvDd = "txPvDd";
        private readonly string lrs_EdtPrvHt = "txPvHt";
        private readonly string lrs_EdtFchCnDd = "txFcDd";
        private readonly string lrs_EdtFchCnHt = "txFcHt";
        private readonly string lrs_EdtFchVnDd = "txFvDd";
        private readonly string lrs_EdtFchVnHt = "txFvHt";

        private readonly string lrs_CmbSuc = "codSuc";

        private readonly string lrs_EdtDocNum = "txDocNum";
        private readonly string lrs_EdtFchCr = "txCDate";
        private readonly string lrs_EdtFchCn = "txFcCt";

        private readonly string lrs_EdtTtlPg = "txTtPg";

        private readonly string lrs_EdtCtaTrs = "txCtTr";
        private readonly string lrs_EdtFchDp = "txFcDp";
        private readonly string lrs_EdtNumDp = "txNmDp";
        //________________________Grid________________________
        private readonly string lrs_GrdPayDTRDET = "gdDtr";
        //________________________Matrix________________________
        private readonly string lrs_MtxPayDTRDET = "mxDtrDet";
        //________________________Columnas Matrix_______________
        private readonly string lrs_ClmMtxCodPrv = "clmCgPv";
        private readonly string lrs_ClmMtxIdDtr = "clmDEAs";
        private readonly string lrs_ClmMtxCodPg = "clmDEPg";
        private readonly string lrs_ClmMtxNomPv = "clmNbPv";
        private readonly string lrs_ClmMtxNumDc = "clmNmDc";
        private readonly string lrs_ClmMtxNumPg = "clmNmPg";
        private readonly string lrs_ClmMtxDtrXPg = "clmSdAs";
        private readonly string lrs_ClmMtxCodBn = "clmCdBn";
        private readonly string lrs_ClmMtxCodOpe = "clmCdOp";
        private readonly string lrs_ClmMtxTipObj = "clmTpOb";
        private readonly string lrs_ClmMtxNumCns = "clmNConst";
        private readonly string lrs_ClmMtxFchDc = "clmFchDoc";
        private readonly string lrs_ClmMtxCodDtr = "clmCdDt";
        //________________________Button________________________
        private readonly string lrs_BtnBuscar = "btBuscar";
        private readonly string lrs_BtnGnrPagos = "btPagar";
        private readonly string lrs_BtnCrear = "1";
        private readonly string lrs_BtnTXT= "btnTxt";
        //________________________Static________________________
        private readonly string lrs_lblDesCtaTrs = "lbCtTr";
        //________________________DataTable - Grid______________
        private readonly string lrs_DttPayDTRDET = "dtDetrac";
        private readonly string lrs_DttAux = "TblAux";
        //________________________Cabecera DataTable________________________
        private const string lrs_CabDttSelect = "Seleccion";
        private readonly string lrs_CabDttNomPrv = "Proveedor";
        private readonly string lrs_CabDttCodPrv = "Codigo_SN";
        private readonly string lrs_CabDttNomRs = "Razon_Social";
        private readonly string lrs_CabDttNumDoc = "NumeroDoc";
        private readonly string lrs_CabDttFchDoc = "Fecha de Documento";
        private readonly string lrs_CabDttCodBn = "Cod_Bien";
        private readonly string lrs_CabDttCodTrn = "Cod_Transaccion";
        private readonly string lrs_CabDttCodDtr = "Cod_Detraccion";
        private readonly string lrs_CabDttImpDtr = "Imp_Detraccion";
        private readonly string lrs_CabDttNumPg = "NumeroPago";
        private readonly string lrs_CabDttCodPg = "CodigoPago";
        private const string lrs_CabDttNumCns = "N de Constancia";
        private readonly string lrs_CabDttCodOpe = "Cod_Operacion";

        private readonly string lrs_CabDttCodObj = "Cod_Objeto";
        private readonly string lrs_CabDttImpDtr2 = "Imp_Detraccion2";

        private static string ls_CtaTransferSYS = string.Empty;
        private static bool lb_registrarCshFlw = false;
        #endregion
    }
}
