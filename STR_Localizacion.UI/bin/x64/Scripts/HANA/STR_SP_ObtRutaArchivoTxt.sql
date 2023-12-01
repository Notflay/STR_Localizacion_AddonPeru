CREATE PROCEDURE STR_SP_ObtRutaArchivoTxt
(
	 IN Tabla VARCHAR(30)
)
AS
BEGIN
	-- Generar archivo de txt de Orden de Venta 
	SELECT "U_STR_Descripcion",'OrdenVentaCab.txt','OrdenVentaDet.txt' AS "TituloCab" FROM "@STR_SETTINGS" WHERE "U_STR_Concepto"='GenerarTxt';
END;