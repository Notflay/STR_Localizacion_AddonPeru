CREATE PROCEDURE STR_SP_FechaAnulacion
(
	IN sTpSunat nvarchar(50),
	IN sSerie nvarchar(50) 
)
AS
BEGIN
	SELECT top 1 "U_BPP_FchAnl" FROM "@BPP_ANULCORR" WHERE "U_BPP_DocSnt"=:sTpSunat AND "U_BPP_Serie"=:sSerie ORDER BY "DocEntry" DESC;
END;