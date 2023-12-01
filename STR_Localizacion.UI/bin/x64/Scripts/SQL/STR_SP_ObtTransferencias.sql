CREATE PROCEDURE STR_SP_ObtTransferencias(
 @DocEntry INT
)
AS
BEGIN
	SELECT 
		 CAST(T2."DocDate" AS DATE) AS "DocDate"
		,CAST(T2."TaxDate" AS DATE) AS "TaxDate"
		,1 AS "Valor"
		,T0."ItemCode"
		,"Dscription"
		,"WhsCode"
		,"unitMsr"
		,"Quantity"
		,"Quantity" * T0."U_STR_Flete" AS "Debito/Credito"
		,(SELECT "U_STR_Descripcion" FROM "@STR_SETTINGS" WHERE "Code"='CTADESTINO' AND "U_STR_Valor"='1') AS "Cta Aumenta"
		,(SELECT "U_STR_Descripcion" FROM "@STR_SETTINGS" WHERE "Code"='CTADESTINO' AND "U_STR_Valor"='1') AS "Cta Disminuye" 
		,T2."Comments" AS "Comentario"
		,T0."OcrCode"  AS "LineaNegocio"
		,T0."OcrCode2" AS "UnidadEconomica"
		,T0."OcrCode3" AS "CtaDestino"
		,T0."OcrCode4" AS "CentroCosto"
		,T0."OcrCode5" AS "CS"
	FROM WTR1 T0 INNER JOIN OITM T1 ON T0."ItemCode" = T1."ItemCode"
				 INNER JOIN OWTR T2 ON T0."DocEntry" = T2."DocEntry"
	WHERE T0."DocEntry" = @DocEntry AND T0."U_STR_Flete" > 0 AND T1."U_STR_Costeo"='Y';
END