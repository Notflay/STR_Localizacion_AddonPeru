CREATE PROCEDURE STR_SP_ObtNumSerie
(
	@CodigoObjeto NVARCHAR(20),
	@Indicador NVARCHAR(10)
)
AS
BEGIN
	SELECT "Series", "SeriesName" FROM nnm1 WHERE "ObjectCode" = @CodigoObjeto AND "Indicator" = @Indicador AND
			"Locked" != 'Y' ;;
END;