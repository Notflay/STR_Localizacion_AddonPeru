CREATE PROCEDURE STR_SP_ObtFacturasPendientesAD
(
	@fIni NVARCHAR(50),
	@fFin NVARCHAR(50),
	@method nvarchar(1)
)
AS

BEGIN

	IF @method = 'E'
		BEGIN
		SELECT T2."CreatedBy"
		FROM JDT1 AS T0
		INNER JOIN OOCR AS T1 ON T0."OcrCode3" = T1."OcrCode"
		INNER JOIN OJDT AS T2 ON T0."TransId" = T2."TransId" 
		INNER JOIN OPCH AS T4 ON T2."CreatedBy" = T4."DocEntry" AND T4."DocType" = 'S' AND T4."U_STR_ADP" = 'N'
		WHERE CAST(T0."RefDate" AS DATE) BETWEEN 
									CASE WHEN @fIni = '' THEN CAST('19000101' AS DATE) ELSE CAST(@fIni AS DATE) END 
									AND 
									CASE WHEN @fFin = '' THEN GETDATE() ELSE CAST(@fFin AS DATE) 
									END
	END	
	ELSE
	BEGIN
		SELECT T2."CreatedBy"
		FROM JDT1 AS T0
		INNER JOIN OOCR AS T1 ON T0."OcrCode3" = T1."OcrCode"
		INNER JOIN OJDT AS T2 ON T0."TransId" = T2."TransId" 
		INNER JOIN OACT AS T3 ON T3."AcctCode" = T1."U_STR_LC_CDST"
		INNER JOIN OPCH AS T4 ON T2."CreatedBy" = T4."DocEntry" AND T4."DocType" = 'S' AND T4."U_STR_ADP" = 'N'
		WHERE CAST(T0."RefDate" AS DATE) BETWEEN 
									CASE WHEN @fIni = '' THEN CAST('19000101' AS DATE) ELSE CAST(@fIni AS DATE) END 
									AND 
									CASE WHEN @fFin = '' THEN GETDATE() ELSE CAST(@fFin AS DATE) END;
	END
END



