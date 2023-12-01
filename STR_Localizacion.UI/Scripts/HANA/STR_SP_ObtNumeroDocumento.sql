CREATE PROCEDURE STR_SP_ObtNumeroDocumento(
pv_ObjType VARCHAR(20)
)
AS
BEGIN
SELECT "NextNumber" FROM NNM1 
	WHERE "ObjectCode" = :pv_ObjType AND "Locked" != 'Y'
		AND "Indicator" = (SELECT "Indicator" FROM OFPR WHERE CURRENT_DATE BETWEEN "F_RefDate" AND "T_RefDate");
END