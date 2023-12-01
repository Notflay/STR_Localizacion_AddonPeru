CREATE PROCEDURE STR_SP_ValidarRegistroSAP
(
@pv_TipoSunat NVARCHAR(15),
@pv_SerieSunat NVARCHAR(15),
@pv_NumSunat NVARCHAR(15)
)
AS
BEGIN

SELECT TOP 1 'No es posible anular el número ' + @pv_TipoSunat + '-' + @pv_SerieSunat + '-' + @pv_NumSunat + ' porque éste ya se encuentra registrado en un documento del sistema o ya está anulado.'
	FROM (
		SELECT "DocNum" AS "DocNum" FROM OINV WHERE "U_BPP_MDCD"=@pv_NumSunat AND COALESCE("U_BPP_MDCD", '')<>'' AND "U_BPP_MDSD"=@pv_SerieSunat AND COALESCE("U_BPP_MDSD", '')<>'' AND "U_BPP_MDTD"= @pv_TipoSunat AND COALESCE("U_BPP_MDTD", '')<>''
		UNION ALL
		SELECT "DocNum" AS "DocNum" FROM ORIN WHERE "U_BPP_MDCD"=@pv_NumSunat AND COALESCE("U_BPP_MDCD", '')<>'' AND "U_BPP_MDSD"=@pv_SerieSunat AND COALESCE("U_BPP_MDSD", '')<>'' AND "U_BPP_MDTD"= @pv_TipoSunat AND COALESCE("U_BPP_MDTD", '')<>''
		UNION ALL
		SELECT "DocNum" AS "DocNum" FROM ODLN WHERE "U_BPP_MDCD"=@pv_NumSunat AND COALESCE("U_BPP_MDCD", '')<>'' AND "U_BPP_MDSD"=@pv_SerieSunat AND COALESCE("U_BPP_MDSD", '')<>'' AND "U_BPP_MDTD"= @pv_TipoSunat AND COALESCE("U_BPP_MDTD", '')<>''
		UNION ALL
		SELECT "DocNum" AS "DocNum" FROM ORDN WHERE "U_BPP_MDCD"=@pv_NumSunat AND COALESCE("U_BPP_MDCD", '')<>'' AND "U_BPP_MDSD"=@pv_SerieSunat AND COALESCE("U_BPP_MDSD", '')<>'' AND "U_BPP_MDTD"= @pv_TipoSunat AND COALESCE("U_BPP_MDTD", '')<>''
		UNION ALL
		SELECT "DocNum" AS "DocNum" FROM ODPI WHERE "U_BPP_MDCD"=@pv_NumSunat AND COALESCE("U_BPP_MDCD", '')<>'' AND "U_BPP_MDSD"=@pv_SerieSunat AND COALESCE("U_BPP_MDSD", '')<>'' AND "U_BPP_MDTD"= @pv_TipoSunat AND COALESCE("U_BPP_MDTD", '')<>''
		UNION ALL					
		SELECT "DocNum" AS "DocNum" FROM OIGE WHERE "U_BPP_MDCD"=@pv_NumSunat AND COALESCE("U_BPP_MDCD", '')<>'' AND "U_BPP_MDSD"=@pv_SerieSunat AND COALESCE("U_BPP_MDSD", '')<>'' AND "U_BPP_MDTD"= @pv_TipoSunat AND COALESCE("U_BPP_MDTD", '')<>''
		UNION ALL
		SELECT "DocNum" AS "DocNum" FROM OWTR WHERE "U_BPP_MDCD"=@pv_NumSunat AND COALESCE("U_BPP_MDCD", '')<>'' AND "U_BPP_MDSD"=@pv_SerieSunat AND COALESCE("U_BPP_MDSD", '')<>'' AND "U_BPP_MDTD"= @pv_TipoSunat AND COALESCE("U_BPP_MDTD", '')<>''
		UNION ALL				
		SELECT "DocNum" AS "DocNum" FROM OVPM WHERE "U_BPP_PTCC"=@pv_NumSunat AND COALESCE("U_BPP_PTCC", '')<>'' AND "U_BPP_PTSC"=@pv_SerieSunat AND COALESCE("U_BPP_PTSC", '')<>'' AND "U_BPP_MDTD"= @pv_TipoSunat AND COALESCE("U_BPP_MDTD", '')<>''
		UNION ALL
		SELECT "Code" AS "DocNum" FROM "@BPP_NROANUL" WHERE "U_BPP_TpoDoc"='Venta' AND "U_BPP_Correlativo"=@pv_NumSunat AND COALESCE("U_BPP_Correlativo", '')<>'' AND "U_BPP_Serie"=@pv_SerieSunat AND COALESCE("U_BPP_Serie", '')<>'' AND "U_BPP_TpoSUNAT"=@pv_TipoSunat AND COALESCE("U_BPP_TpoSUNAT", '')<>''
		UNION ALL
		SELECT "DocNum" AS "DocNum" FROM "@BPP_ANULCORR" T1 INNER JOIN "@BPP_ANULCORRDET" T2 on
		T1."DocEntry" = T2."DocEntry" WHERE "U_BPP_TpDoc" = 'Venta' AND "U_BPP_NmCr" = @pv_NumSunat AND COALESCE("U_BPP_NmCr",'')<>'' AND "U_BPP_DocSnt" = @pv_TipoSunat AND 
		COALESCE("U_BPP_DocSnt",'')<>'' AND "U_BPP_Serie" = @pv_SerieSunat AND COALESCE("U_BPP_Serie",'') <>'') DC;
END;