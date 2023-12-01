CREATE PROCEDURE STR_SP_ObtLineaId
(
	@Nombre NVARCHAR(15),
	@Codigo integer
)
AS
BEGIN
	SELECT TOP 1 "Line_ID", "BPLId" FROM JDT1 WHERE "ShortName"=@Nombre and "TransId"=@Codigo;
END;