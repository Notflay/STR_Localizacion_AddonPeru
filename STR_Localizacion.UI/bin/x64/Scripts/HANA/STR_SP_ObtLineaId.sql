CREATE PROCEDURE STR_SP_ObtLineaId
(
	IN Nombre NVARCHAR(15),
	IN Codigo integer
)
AS
BEGIN
	SELECT TOP 1 "Line_ID" FROM JDT1 WHERE "ShortName"=:Nombre and "TransId"=:Codigo;
END;