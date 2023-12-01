CREATE PROCEDURE STR_SP_UpdEstadoAnulado
(
	IN Codigo VARCHAR(5)	
)
AS
	Options nvarchar(5);
BEGIN
	SELECT "U_STR_MCTC" INTO  Options FROM "@STR_LC_CONF" WHERE "Code" = '01';
	
	UPDATE "@BPP_PAYDTR" SET "Status" = 'A' WHERE "DocEntry" = :Codigo 
		AND (SELECT TOP 1 "Canceled" FROM "@BPP_PAYDTR" WHERE "DocEntry" = Codigo) = 'Y';
		
	IF Options='O2' THEN
		UPDATE "OPCH" SET "U_STR_DetraccionPago"='N' WHERE "DocEntry" IN( SELECT "U_BPP_DEAs" FROM "@BPP_PAYDTRDET" WHERE "DocEntry"=:Codigo);
	END IF;
END;
