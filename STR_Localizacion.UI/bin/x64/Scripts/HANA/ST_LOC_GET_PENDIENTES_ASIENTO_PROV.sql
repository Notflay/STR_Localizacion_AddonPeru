CREATE PROCEDURE ST_LOC_GET_PENDIENTES_ASIENTO_PROV
(
	IN ClienteIni nvarchar(30),
	IN ClienteFin nvarchar(30),
	IN FechaContIni date,
	IN FechaContFin date,
	IN FechaVencIni date,
	IN FechaVencFin date
)
AS
BEGIN

	DECLARE FiltroSocio int;
	DECLARE FiltroFecha1 int;
	DECLARE FiltroFecha2 int;

	IF(ClienteIni <> '' AND ClienteFin <> '') THEN FiltroSocio := 1;ELSE FiltroSocio := 0; END IF;
	IF(FechaVencIni = '' AND FechaVencFin = '') THEN FiltroFecha1 := 0; END IF;
	IF(FechaVencIni <> '' AND FechaVencFin <> '') THEN FiltroFecha1 := 1; END IF;
	IF(FechaVencIni = '' AND FechaVencFin <> '') THEN FiltroFecha1 := 2; END IF;
	IF(FechaVencIni <> '' AND FechaVencFin = '') THEN FiltroFecha1 := 3; END IF;
	
	IF(FechaContIni = '' AND FechaContFin = '') THEN FiltroFecha2 := 0; END IF;
	IF(FechaContIni <> '' AND FechaContFin <> '') THEN FiltroFecha2 := 1; END IF;
	IF(FechaContIni = '' AND FechaContFin <> '') THEN FiltroFecha2 := 2; END IF;
	IF(FechaContIni <> '' AND FechaContFin = '') THEN FiltroFecha2 := 3; END IF;

	SELECT 
			T0."DocEntry" AS "U_ST_LC_DE",
			T0."TaxDate" AS "U_ST_LC_FCONT",
			T0."DocDueDate" AS "U_ST_LC_FVENC",
			T0."DocNum" AS "U_ST_LC_DOCNUM",
			T0."CardCode" AS "U_ST_LC_CODSOC",
			T0."CardName" AS "U_ST_LC_NOMSOC",
			SUM(T1."LineTotal") AS "U_ST_LC_TOTAL",
			0 AS "U_ST_LC_TRIDAP",
			'' as "U_ST_LC_MSJERR"
			 
	FROM OPDN AS T0
	INNER JOIN PDN1 AS T1 ON T0."DocEntry" = T1."DocEntry"
	INNER JOIN OITM AS T2 ON T1."ItemCode" = T2."ItemCode" AND T2."InvntItem" = 'N' AND T2."U_GEN_PROV" = 'Y'
	WHERE "DocStatus" = 'O' AND
	IFNULL(T0."U_ST_EP_ASPR", 0) = 0 AND
	((:FiltroSocio = 1 AND T0."CardCode" between ClienteIni and ClienteFin) OR :FiltroSocio = 0) AND
	((:FiltroFecha1 = 3 AND T0."DocDueDate" >= FechaVencIni) OR (:FiltroFecha1 = 2 AND T0."DocDueDate" <= FechaVencFin) OR (:FiltroFecha1 = 1 AND T0."DocDueDate" between FechaVencIni and FechaVencFin) OR (:FiltroFecha1 = 0)) AND
	((:FiltroFecha2 = 3 AND T0."TaxDate" >= FechaContIni) OR (:FiltroFecha2 = 2 AND T0."TaxDate" <= FechaContFin) OR (:FiltroFecha2 = 1 AND T0."TaxDate" between FechaContIni and FechaContFin) OR (:FiltroFecha2 = 0))
	
	GROUP BY 
	T0."DocEntry",
			T0."TaxDate",
			T0."DocDueDate",
			T0."DocNum",
			T0."CardCode",
			T0."CardName"
	
	;
END;