CREATE PROCEDURE STR_SP_InfoAsientoDestino
(
	@fIni NVARCHAR(50),
	@fFin NVARCHAR(50)
)
AS

BEGIN


	select 
	
	"Codigo",
	"Cuenta",
	"Descripcion",
	CASE when "Saldo">=0.0 then ABS("Saldo") else 0.0 end as "Debito",
	CASE when "Saldo"<0.0 then ABS("Saldo") else 0.0 end as "Credito"
	
	
	from
	(
		select 
		
		
(select top 1 "U_BPP_CdgCuenta" from "@BPP_CONFIG") as "Codigo", 
(select top 1 "U_BPP_FmtCuenta" from "@BPP_CONFIG") as "Cuenta", 
(select top 1 "U_BPP_NmbCuenta" from "@BPP_CONFIG") as "Descripcion", 

		SUM("Credito") as "Debito", 
		SUM("Debito") as "Credito",
		(SUM("Credito") - SUM("Debito")) as "Saldo"
		from (
			select 
			/*T1.AcctCode*/'' as "Codigo", /*T0.OcrCode3*/'' as "Cuenta", /*T1.AcctName*/'' as "Descripcion", SUM(T0."Debit") as "Debito", SUM(T0."Credit") as "Credito" 
			from 
			JDT1 T0 inner join OACT T1 on T0."OcrCode3"=T1."Segment_0" 
					inner join OJDT T2 on T0."TransId" = T2."TransId"
				    inner join OPCH T3 on T2."CreatedBy" = T3."DocEntry" and t3."DocType" = 'S' and t3."U_STR_ADP" = 'N'
			where 
			COALESCE(T0."OcrCode3", '')<>'' 
			and (T0."OcrCode3" like '9%' OR T0."OcrCode3" like '6%')
			and CAST(T0."RefDate" AS DATE) BETWEEN CAST(@fIni AS DATE) AND CAST(@fFin AS DATE)
			Group By "OcrCode3", T1."AcctName", T1."AcctCode"
			) CD
	
		UNION ALL
	
		select 
		T1."AcctCode" as "Codigo", 
		T0."OcrCode3" as "Cuenta", 
		T1."AcctName" as "Descripcion", 
		SUM(T0."Debit") as "Debito", 
		SUM(T0."Credit") as "Credito",
		(SUM(T0."Debit") - SUM(T0."Credit")) as "Saldo"
		from 
		JDT1 T0 inner join OACT T1 on T0."OcrCode3"=T1."Segment_0"
				inner join OJDT T2 on T0."TransId" = T2."TransId"
				inner join OPCH T3 on T2."CreatedBy" = T3."DocEntry" and t3."DocType" = 'S' and t3."U_STR_ADP" = 'N'		
		where 
		COALESCE(T0."OcrCode3", '')<>'' 
		and (T0."OcrCode3" like '9%' OR T0."OcrCode3" like '6%')
		and CAST(T0."RefDate" AS DATE) BETWEEN CAST(@fIni AS DATE) AND CAST(@fFin AS DATE)
		Group By "OcrCode3", T1."AcctName", T1."AcctCode" 
	) CtaDst;
END;