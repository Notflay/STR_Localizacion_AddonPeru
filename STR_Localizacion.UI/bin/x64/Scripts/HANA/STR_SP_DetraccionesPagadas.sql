CREATE PROCEDURE STR_SP_DetraccionesPagadas
(
	IN docentry integer
)
AS
BEGIN
	SELECT  
	"U_BPP_CgPv" || ' - ' || "U_BPP_NbPv" AS "Proveedor"
	,"U_BPP_CgPv" AS "Codigo_SN"
	,"U_BPP_NbPv" AS "Razon_Social"
	,"U_BPP_DEAs" AS "Cod_Transaccion"
	,"U_BPP_NmDc" AS "NumeroDoc"
	,"U_BPP_FcDc" AS "Fecha de Documento"
	,"U_BPP_CdBn" AS "Codigo de Bien"
	,"U_BPP_CdOp" AS "Cod_Operacion"
	,"U_BPP_CdDt" AS "Cod_Detraccion"
	,ROUND("U_BPP_SdAs",0) AS "Imp_Detraccion"
	,"U_BPP_NmPg" AS "NumeroPago"
	,"U_BPP_DEPg" AS "CodigoPago"
	,"U_BPP_Cnst" AS "N de Constancia"
	FROM "@BPP_PAYDTRDET"
	WHERE "DocEntry" = :docentry;
END;