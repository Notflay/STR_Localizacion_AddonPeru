CREATE PROCEDURE STR_SP_ObtHeaderData
(
	IN DocEntry VARCHAR(30)
)
AS
BEGIN

HeadData=(
		SELECT  ROW_NUMBER() OVER(ORDER BY "U_SN" DESC) AS "DocNum", 
		       CASE WHEN "DocType"='I' THEN 0 ELSE 1 END AS "DocType",
		       "DocDate" AS "DocDate",
		       "DocDate" AS "DocDueDate",
		       "DocDate" AS "TaxDate",
		       "DocDate" AS "RequriedDate",
		       
		      "empID" AS "Requester",
		       "firstName" AS "RequesterName",
		       171 AS "ReqType",
		       "Project" AS "U_STR_FILE",
		       'USD' AS "U_CE_MNDA",
		       0 AS "SendNotification"
		       
		FROM
		(
		SELECT   T1."DocDate", T1."U_SN",T0."DocType", T1."U_STR_DescripProv", T1."ItemCode", T1."Dscription", T1."PriceAfVAT",
		         T3."empID", T3."firstName", T0."Project"
		FROM ORDR T0 INNER JOIN RDR1 T1 ON T0."DocEntry"=T1."DocEntry"
		             INNER JOIN OITM T2 ON T1."ItemCode"=T2."ItemCode" AND T2."U_STR_Rubro"='04'
		             INNER JOIN OHEM T3 ON T1."U_SN"=T3."U_CE_PVAS"
		WHERE T0."DocEntry"=:DocEntry
		)TT
	);
	
	SELECT  * FROM :HeadData;
-- ================================================
-- CALL STR_SP_ObtHeaderData('115');

END;
