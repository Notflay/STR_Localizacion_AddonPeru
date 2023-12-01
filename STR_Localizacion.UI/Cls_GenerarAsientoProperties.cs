
namespace STR_Localizacion.UI
{
    partial class Cls_GenerarAsiento
    {
        #region Declaraciones
        //*****************************************************************
        //                      Constantes generales del módulo
        //*****************************************************************
        private string ls_msjCancelacion = string.Empty;
        private readonly string lrs_FchFormat = "yyyyMMdd";
        private readonly string lrs_ValEstado = "O";
        private readonly string lrs_ValAnulado = "Y";

        //*****************************************************************
        //                      DataSource
        //*****************************************************************
        private readonly string lrs_DtcBATDTR = "@BPP_BATDTR";
        private readonly string lrs_DtcBATDTRDET = "@BPP_BATDTRDET";
        //*****************************************************************
        //                      UserDataSource
        //*****************************************************************
        private readonly string lrs_UdsTotal = "UDS_Total";
        private readonly string lrs_UdsImpTotal = "UDS_ImpTot";
        //*****************************************************************
        //                      User Fields - @BPP_BATDTR
        //*****************************************************************
        private readonly string lrs_UflSeries = "Series";
        private readonly string lrs_UflDocNum = "DocNum";
        private readonly string lrs_UflCrtDate = "CreateDate";
        private readonly string lrs_UflEstado = "Status";
        private readonly string lrs_UflCanceled = "Canceled";
        private readonly string lrs_UflCardCode = "CardCode";
        private readonly string lrs_UflDocEtr = "DocEntry";
        //private readonly string lrs_ = "Canceled";

        private readonly string lrs_UflPrvDd = "U_BPP_PrvD";
        private readonly string lrs_UflPrvHt = "U_BPP_PrvH";
        private readonly string lrs_UflFchCnDd = "U_BPP_FchCntD";
        private readonly string lrs_UflFchCnHt = "U_BPP_FchCntH";
        private readonly string lrs_UflFchVnDd = "U_BPP_FchVncD";
        private readonly string lrs_UflFchVnHt = "U_BPP_FchVncH";

        private readonly string lrs_UflTotPag = "U_BPP_TotPag";

        private readonly string lrs_UflBnkPais = "U_STR_BnkPais";
        private readonly string lrs_UflBnkCdgo = "U_STR_BnkCdgo";
        private readonly string lrs_UflBnkCta = "U_STR_BnkCnta";
        private readonly string lrs_UflBnkMnd = "U_STR_BnkMnda";

        private readonly string lrs_UflTipoAsn = "U_STR_TpoAsnt";
        private readonly string lrs_UflMntPorte = "U_STR_MtoPrts";
        private readonly string lrs_UflMntCmsn = "U_STR_MtoCmsn";
        private readonly string lrs_UflFchEje = "U_STR_FchEjec";
        private readonly string lrs_UflAsnRsp = "U_STR_AsnRsp";

        //*****************************************************************
        //                      User Fields - @BPP_BATDTRDET
        //*****************************************************************
        private readonly string lrs_UflSelec = "U_BPP_Selec";
        private readonly string lrs_UflCodProv = "U_BPP_CodProv";
        private readonly string lrs_UflRznScl = "U_BPP_RznScl";
        private readonly string lrs_UflCodTrans = "U_BPP_CodTrans";
        private readonly string lrs_UflDocEntry = "U_BPP_DocEntry";
        private readonly string lrs_UflTpDoc = "U_BPP_TpDoc";
        private readonly string lrs_UflNroDoc = "U_BPP_NroDoc";
        private readonly string lrs_UflFchDoc = "U_BPP_FchDoc";
        private readonly string lrs_UflCptoRet = "U_BPP_CncDet";
        private readonly string lrs_UflImpDet = "U_BPP_ImpDet";
        private readonly string lrs_UflCodPry = "U_BPP_CodPry";
        private readonly string lrs_UflDesPry = "U_BPP_DscPry";
        private readonly string lrs_UflEstDoc = "U_BPP_EstDoc";
        private readonly string lrs_UflAsDtr = "U_BPP_AsDtr";

        private readonly string lrs_UflAstPercepcion = "U_BPP_AsPc";
        private readonly string lrs_UflAstDetraccion = "U_BPP_AstDetrac";

        //*****************************************************************
        //                      Sql Fields - Listado Detalle
        //*****************************************************************
        private readonly string lrs_SqlSelec = "Seleccion";
        private readonly string lrs_SqlCodProv = "CodSN";
        private readonly string lrs_SqlRznScl = "NomSN";
        private readonly string lrs_SqlCodTrans = "CodTransaccion";
        private readonly string lrs_SqlDocEntry = "DocEntry";
        private readonly string lrs_SqlTpDoc = "TipDocumento";
        private readonly string lrs_SqlNroDoc = "NroDocumento";
        private readonly string lrs_SqlFchDoc = "FechaDocumento";
        private readonly string lrs_SqlCptoRet = "ConceptoRet";
        private readonly string lrs_SqlImpDet = "ImpRetencion";
        private readonly string lrs_SqlCodPry = "CodProyecto";
        private readonly string lrs_SqlDesPry = "DesProyecto";
        private readonly string lrs_SqlEstDoc = "EstDocumento";
        private readonly string lrs_SqlAsDtr = "AsientoGnr";

        //*****************************************************************
        //                      UserDataSource
        //*****************************************************************
        private readonly string lrs_UDS_TipAsn = "UDS_TipAsn";
        private readonly string lrs_UDS_ChkPrc = "UDS_ChkPrc";

        //*****************************************************************
        //                      Controles UI
        //*****************************************************************
        //________________________ChooseFromList________________________
        private readonly string lrs_CflSocioDsd = "CFL_SN1";
        private readonly string lrs_CflSocioHst = "CFL_SN2";
        //________________________ComboBox________________________
        private readonly string lrs_CbxSeries = "cbxSeries";
        private readonly string lrs_CbxEstado = "cbxStatus";
        private readonly string lrs_CbxBnkPais = "cbxBnkPais";
        private readonly string lrs_CbxBnkCdgo = "cbxBnkCdgo";
        private readonly string lrs_CbxBnkCta = "cbxBnkCta";
        //________________________Label___________________________
        private readonly string lrs_LblBnkPais = "lblBnkPais";
        private readonly string lrs_LblBnkCdgo = "lblBnkCdgo";
        private readonly string lrs_LblBnkCta = "lblBnkCta";
        private readonly string lrs_LblBnkMnd = "lblBnkMnd";

        private readonly string lrs_LblMntTotal = "lblTotal";
        private readonly string lrs_LblMntPorte = "lblPortes";
        private readonly string lrs_LblMntCmns = "lblCmsion";
        private readonly string lrs_LblMntImporte = "lblImporte";

        private readonly string lrs_LblAsnRspn = "lblAstRspn";


        //________________________EditText________________________
        private readonly string lrs_EdtPrvDd = "txtPrvD";
        private readonly string lrs_EdtPrvHt = "txtPrvH";
        private readonly string lrs_EdtFchCnDd = "txtFchCntD";
        private readonly string lrs_EdtFchCnHt = "txtFchCntH";
        private readonly string lrs_EdtFchVnDd = "txtFchVncD";
        private readonly string lrs_EdtFchVnHt = "txtFchVncH";

        private readonly string lrs_EdtDocNum = "txtDocNum";
        private readonly string lrs_EdtFchEje = "txtFchEje";

        private readonly string lrs_EdtBnkMnd = "txtBnkMnd";
        private readonly string lrs_EdtAsnRsp = "txtAsnRsp";
        private readonly string lrs_EdtDocEnt = "txtDocEnt";

        private readonly string lrs_EdtMntTotal = "txtTotal";
        private readonly string lrs_EdtMntPorte = "txtPortes";
        private readonly string lrs_EdtMntCmns = "txtCmsion";
        private readonly string lrs_EdtMntImporte = "txtImporte";

        //________________________CheckBox________________________
        private readonly string lrs_RbtDtr = "rbtDtr";
        private readonly string lrs_RbtPrc = "rbtPrc";
        private readonly string lrs_RbtFNg = "rbtFNg";
        //________________________Matrix________________________
        private readonly string lrs_MtxBatDTR = "mtxDetalle";
        //________________________Columnas Matrix_______________
        private readonly string lrs_ClmMtxSelec = "clmSelec";
        private readonly string lrs_ClmMtxCodPrv = "clmCodPrv";
        private readonly string lrs_ClmMtxRzSocial = "clmRazSc";
        private readonly string lrs_ClmMtxCodTrans = "clmCodTrns";
        private readonly string lrs_ClmMtxTpDoc = "clmTpDoc";
        private readonly string lrs_ClmMtxNrDoc = "clmNrDoc";
        private readonly string lrs_ClmMtxDocEnt = "clmDocEnt";
        private readonly string lrs_ClmMtxFchDoc = "clmFchDoc";
        private readonly string lrs_ClmMtxCptoRet = "clmCptoRet";
        private readonly string lrs_ClmMtxImpRet = "clmImpRet";
        private readonly string lrs_ClmMtxCodPry = "clmCodPry";
        private readonly string lrs_ClmMtxDesPry = "clmDesPry";
        private readonly string lrs_ClmMtxEstDoc = "clmEstado";
        private readonly string lrs_ClmMtxAsGnr = "clmAsDtr";
        //________________________Button________________________
        private readonly string lrs_BtnBuscar = "btnBuscar";
        private readonly string lrs_BtnCrear = "1";

        #endregion
    }

    partial class AsientoProperties
    {
        public AsientoCabecera AsCabecera { get; set; }
        public AsientoDetalle AsDetalle { get; set;}

        public class AsientoCabecera
        {
            public System.DateTime FechaContabilizacion {get; set;}
            public System.DateTime FechaVencimiento { get; set; }
            public System.DateTime FechaDocumento { get; set; }
            public string Comentarios { get; set; }
            public string CodigoTransaccion { get; set; }
            public string Referencia { get; set; }
            public string Referencia2 { get; set; }
            public string Referencia3 { get; set; }
            public System.Collections.Generic.List<AsientoDetalle> AsientoDetalle { get; set; }
            public string FCMoneda { get; set; }
        }
        public class AsientoDetalle
        {
            public string CodigoCuenta { get;  set; }
            public string NombreCuenta { get;  set; }
            public double Debito { get;  set; }
            public double Credito { get;  set; }
            public string Referencia2 { get;  set; }
            public string Referencia3 { get;  set; }
            public int BaseType { get;  set; }
            public int BaseEntry { get;  set; }
            public double FCDebito { get;  set; }
            public double FCCredito { get;  set; }
            public string FCMoneda { get;  set; }
        }
        public class LineData
        {
            public string CardCode { get;  set; }
            public int LineID { get;  set; }
        }
    }
}
