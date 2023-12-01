CREATE PROCEDURE STR_SP_ObtNumSiguiente
(
	@Codigo nvarchar(20),
	@Serie integer
)
AS
BEGIN
	SELECT CAST("NextNumber" AS NVARCHAR(20)) FROM NNM1 WHERE "ObjectCode"= @Codigo AND "Series"= @Serie;
END;