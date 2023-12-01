CREATE PROCEDURE STR_SP_ObtDetailData
(
	IN DocEntry VARCHAR(30),
	IN ParentKey VARCHAR(30) 
)
AS
	Filas INT;
BEGIN
	

DetailData=(
			SELECT TO_VARCHAR(ROW_NUMBER() OVER(ORDER BY "U_SN" DESC)) AS "ParentKey", 
			       '0' AS "LineNum",
			       "ItemCode",
			       "Dscription",
			       "Quantity",
			       '0.1' AS "Price",
			       CAST("PriceAfVAT" AS VARCHAR(20)) AS "U_CE_IMSL",
			       "Project" AS "U_STR_FILE"
			FROM
			(
			SELECT   T1."DocDate", T1."U_SN", T1."U_STR_DescripProv", T1."ItemCode", T1."Dscription", T1."PriceAfVAT",
			         T3."empID", T3."firstName", T0."Project", T1."Quantity"
			FROM ORDR T0 INNER JOIN RDR1 T1 ON T0."DocEntry"=T1."DocEntry"
			             INNER JOIN OITM T2 ON T1."ItemCode"=T2."ItemCode" AND T2."U_STR_Rubro"='04'
			             INNER JOIN OHEM T3 ON T1."U_SN"=T3."U_CE_PVAS"
			WHERE  T0."DocEntry" = :DocEntry --(CASE WHEN IFNULL(:DocEntry,'') = '' THEN T0."DocEntry" ELSE :DocEntry END) 
			)TT
		);

	SELECT * FROM :DetailData T0 
	WHERE  T0."ParentKey" = (CASE WHEN IFNULL(:ParentKey,'') = '' THEN T0."ParentKey" ELSE :ParentKey END);

	-- ================================================
	-- CALL STR_SP_ObtDetailData(115,'2');
END;