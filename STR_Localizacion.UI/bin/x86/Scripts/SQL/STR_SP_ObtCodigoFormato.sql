CREATE PROCEDURE STR_SP_obtCodigoFormato
(
	@CodigoFormato NVARCHAR(210)
)
AS
BEGIN
	SELECT TOP 1 "AcctCode" FROM OACT WHERE "FormatCode" = @CodigoFormato;
END;