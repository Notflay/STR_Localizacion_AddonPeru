CREATE PROCEDURE STR_SP_ObtNombreCuenta
(
	IN CodigoFormato NVARCHAR(210)
)
AS
BEGIN
	SELECT "AcctName" FROM OACT WHERE "FormatCode"= :CodigoFormato;
END;