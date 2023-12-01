CREATE PROCEDURE STR_SP_ObtCodigoDetrac
(
	IN CodigoFormato NVARCHAR(210)
)
AS
BEGIN
	SELECT top 1 "AcctCode" FROM OACT WHERE "FormatCode" = :CodigoFormato;
END;