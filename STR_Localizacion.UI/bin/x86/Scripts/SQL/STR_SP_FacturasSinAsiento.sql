CREATE PROCEDURE STR_SP_FacturasSinAsiento
@ProvD	VARCHAR(50),
@ProvH	VARCHAR(50),
@FcCntD VARCHAR(50),
@FcCntH VARCHAR(50),
@FcVncD VARCHAR(50),
@FcVncH VARCHAR(50),
@Moneda VARCHAR(3),
@Tpo	VARCHAR(2)
AS
BEGIN
DECLARE @lv_union CHAR(1) = CASE WHEN @Tpo = 'DP' THEN NULL 
								 WHEN @Tpo = 'D'  THEN 0
								 WHEN @Tpo = 'P'  THEN 1
								 WHEN @Tpo = 'F'  THEN 2 END

SET @ProvD  = NULLIF(@ProvD, '')
SET @ProvH  = NULLIF(@ProvH, '')
SET @FcCntD = NULLIF(@FcCntD, '')
SET @FcCntH = NULLIF(@FcCntH, '')
SET @FcVncD = NULLIF(@FcVncD, '')
SET @FcVncH = NULLIF(@FcVncH, '')
SET @Moneda = NULLIF(@Moneda, '')
						
SELECT
	'N'						AS "Seleccion"
	,T1."CardCode"			AS "CodSN"
	,T1."CardName"			AS "NomSN"
	,T1."TransId"			AS "CodTransaccion"
	,T1."DocEntry"			AS "DocEntry"
	,CASE WHEN T1."ObjType" = 018 THEN 'Factura de Proveedores' 
		  WHEN T1."ObjType" = 019 THEN 'Nota de Credito Proveedores' 
		  WHEN T1."ObjType" = 204 THEN 'Fac. Anticipo de Proveedores'
	END						AS "TipDocumento"

	,T1."U_BPP_MDTD"+'-'+T1."U_BPP_MDSD" +'-'+T1."U_BPP_MDCD" AS "NroDocumento"
	,T1."TaxDate"			AS "FechaDocumento"
	,(OW."WTCode"+'-'+OW."WTName")	AS "ConceptoRet"
	,T2."WTAmnt"			AS "ImpRetencion"
	,T3."Project"			AS "CodProyecto"
	,OP."PrjName"			AS "DesProyecto"
	,'Pendiente'			AS "EstDocumento"
	,''						AS "AsientoGnr"
	FROM 
		(SELECT * FROM OPCH UNION ALL
		 SELECT * FROM ODPO UNION ALL
		 SELECT * FROM ORPC) T1
		INNER JOIN (SELECT * FROM PCH5 UNION ALL
					SELECT * FROM DPO5 UNION ALL
					SELECT * FROM RPC5) T2 ON T1."ObjType" = T2."ObjType" AND T1."DocEntry" = T2."AbsEntry"
		LEFT  JOIN (SELECT * FROM PCH1 UNION ALL
					SELECT * FROM DPO1 UNION ALL
					SELECT * FROM RPC1) T3 ON T1."ObjType" = T3."ObjType" AND T1."DocEntry" = T3."DocEntry" AND T3."VisOrder" = 0
		LEFT  JOIN OWHT OW ON T2."WTCode" = OW."WTCode"
		LEFT  JOIN OPRJ OP ON T3."Project" = OP."PrjCode"
	WHERE ISNULL(T1."U_BPP_AstDetrac",'') = '' AND LEFT(T2."WTCode",1) = 'D' AND T1."CANCELED" = 'N'
	AND T1."U_BPP_MDCD" NOT LIKE 'AN%' AND T1."U_BPP_MDSD" NOT LIKE 'AN%' AND T2."WTAmnt">0

	AND ((@ProvD IS NULL AND @ProvH IS NULL) OR (T1."CardCode" BETWEEN @ProvD AND @ProvH))
	AND (@FcCntD IS NULL OR T1."DocDate" >= @FcCntD)
	AND (@FcCntH IS NULL OR T1."DocDate" <= @FcCntH)
	AND (@FcVncD IS NULL OR T1."DocDate" >= @FcVncD)
	AND (@FcVncH IS NULL OR T1."DocDate" <= @FcVncH)
	AND NULLIF(@lv_union, 0) IS NULL

UNION ALL

SELECT
	'N'						AS "Seleccion"
	,T1."CardCode"			AS "CodSN"
	,T1."CardName"			AS "NomSN"
	,T1."TransId"			AS "CodTransaccion"
	,T1."DocEntry"			AS "DocEntry"
	,CASE WHEN T1."ObjType" = 013 THEN 'Factura de Clientes' 
		  WHEN T1."ObjType" = 014 THEN 'Anticipo de Clientes' 
		  WHEN T1."ObjType" = 203 THEN 'Nota de Credito de Clientes'
	 END					AS "TipDocumento"

	,T1."U_BPP_MDTD"+'-'+T1."U_BPP_MDSD" +'-'+T1."U_BPP_MDCD" AS "NroDocumento"
	,T1."TaxDate"			AS "FechaDocumento"
	,'Percepcion'			AS "ConceptoRet"
	,T1."U_BPP_TtPc"		AS "ImpRetencion"
	,''						AS "CodProyecto"
	,''						AS "DesProyecto"
	,'Pendiente'			AS "EstDocumento"
	,''						AS "AsientoGnr"
	FROM 
		(SELECT * FROM OINV UNION ALL
		 SELECT * FROM ODPI UNION ALL
		 SELECT * FROM ORIN) T1
	WHERE ISNULL(CONVERT(VARCHAR(10), T1."U_BPP_TtPc"),'')<>'' AND T1."U_BPP_TtPc" > 0.0 
	  AND ISNULL(T1."U_BPP_AsPc",'')='' AND T1."CANCELED" = 'N'

	AND ((@ProvD IS NULL AND @ProvH IS NULL) OR (T1."CardCode" BETWEEN @ProvD AND @ProvH))
	AND (@FcCntD IS NULL OR T1."DocDate" >= @FcCntD)
	AND (@FcCntH IS NULL OR T1."DocDate" <= @FcCntH)
	AND (@FcVncD IS NULL OR T1."DocDate" >= @FcVncD)
	AND (@FcVncH IS NULL OR T1."DocDate" <= @FcVncH)
	AND NULLIF(@lv_union, 1) IS NULL

UNION ALL 
	
SELECT DISTINCT 
	'N'						AS "Seleccion"
	,T1."CardCode"			AS "CodSN"
	,T1."CardName"			AS "NomSN"
	,T1."TransId"			AS "CodTransaccion"
	,T1."DocEntry"			AS "DocEntry"
	,'Anticipo de Clientes' AS "TipoDocumento"
	,T1."U_BPP_MDTD"+'-'+T1."U_BPP_MDSD"+'-'+T1."U_BPP_MDCD" AS "NroDocumento"
	,T1."TaxDate"			AS "FechaDocumento"
	,'Factura Negociable'	AS "ConceptoRet"
	,T1."DocTotal"			AS "ImpRetencion"
	,''						AS "CodProyecto"
	,''						AS "DesProyecto"
	,'Pendiente'			AS "EstDocumento"
	,''						AS "AsientoGnr"
	FROM ODPI T1 WHERE T1."DocStatus" = 'O' 
		AND T1."DocEntry" NOT IN (SELECT ISNULL(TS0."U_STR_BsEnt",'0') FROM JDT1 TS0 WHERE  TS0."U_STR_BsTyp" = T1."ObjType") 
		AND T1."U_STR_FacNeg" = 'Y'

	AND ((@ProvD IS NULL AND @ProvH IS NULL) OR (T1."CardCode" BETWEEN @ProvD AND @ProvH))
	AND (@FcCntD IS NULL OR T1."DocDate" >= @FcCntD)
	AND (@FcCntH IS NULL OR T1."DocDate" <= @FcCntH)
	AND (@FcVncD IS NULL OR T1."DocDate" >= @FcVncD)
	AND (@FcVncH IS NULL OR T1."DocDate" <= @FcVncH)
	AND (@Moneda IS NULL OR T1."DocCur"   = @Moneda)
	AND ISNULL(@lv_union,'') = 2
	
UNION ALL

SELECT DISTINCT
		'N'						AS "Seleccion"
		,T1."CardCode"			AS "CodSN"
		,T1."CardName"			AS "NomSN"
		,T1."TransId"			AS "CodTransaccion"
		,T1."DocEntry"			AS "DocEntry"
		,'Factura de Clientes'	AS "Tipo de Documento"
		,T1."U_BPP_MDTD"+'-'+T1."U_BPP_MDSD"+'-'+T1."U_BPP_MDCD" AS "NroDocumento"
		,T1."TaxDate"			AS "FechaDocumento"
		,'Factura Negociable'	AS "ConceptoRet"
		,CASE WHEN T1.DocCur <> 'SOL' THEN T1.DocTotalFC - T1.PaidFC ELSE T1.DocTotal -T1.PaidToDate END AS "ImpRetencion"
		,''						AS "CodProyecto"
		,''						AS "DesProyecto"
		,'Pendiente'			AS "EstDocumento"
		,''						AS "AsientoGnr"
	FROM OINV T1 WHERE T1."DocStatus" = 'O' 
		AND T1."DocEntry" NOT IN (SELECT DISTINCT ISNULL(TS0."U_STR_BsEnt",'0') FROM JDT1 TS0 LEFT JOIN OJDT TS1 ON TS0."TransId" = TS1."StornoToTr"
				WHERE TS0."U_STR_BsTyp" = 13 AND TS1."StornoToTr" IS NULL AND (SELECT "StornoToTr" FROM OJDT TS2 WHERE TS2."TransId" = TS0."TransId") IS NULL) 
		AND T1."U_STR_FacNeg" = 'Y'

	AND ((@ProvD IS NULL AND @ProvH IS NULL) OR (T1."CardCode" BETWEEN @ProvD AND @ProvH))
	AND (@FcCntD IS NULL OR T1."DocDate" >= @FcCntD)
	AND (@FcCntH IS NULL OR T1."DocDate" <= @FcCntH)
	AND (@FcVncD IS NULL OR T1."DocDate" >= @FcVncD)
	AND (@FcVncH IS NULL OR T1."DocDate" <= @FcVncH)
	AND (@Moneda IS NULL OR T1."DocCur"   = @Moneda)
	AND ISNULL(@lv_union,'') = 2

END