create PROCEDURE STR_SP_ObtNumSiguiente
(
	IN Codigo nvarchar(20),
	IN Serie integer
)
AS
BEGIN
	SELECT TO_VARCHAR("NextNumber") AS "NextNumber"
	 FROM NNM1 WHERE "ObjectCode"= :Codigo AND "Series"= :Serie;
END;