CREATE PROCEDURE STR_SP_ObtNumSerie
(
	IN CodigoObjeto NVARCHAR(20),
	IN Indicador NVARCHAR(10)
)
AS
BEGIN
	SELECT "Series", "SeriesName" FROM nnm1 WHERE "ObjectCode" = :CodigoObjeto AND "Indicator" = :Indicador AND
			"Locked" != 'Y' ;;
END;