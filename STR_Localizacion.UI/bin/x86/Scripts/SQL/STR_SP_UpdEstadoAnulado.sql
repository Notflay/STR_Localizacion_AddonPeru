CREATE PROCEDURE STR_SP_UpdEstadoAnulado(
	@Codigo VARCHAR(5)	
)
AS
BEGIN
	DECLARE @Options nvarchar(5);
	SELECT @Options="U_STR_MCTC" FROM "@STR_LC_CONF" WHERE "Code" = '01';

	UPDATE "@BPP_PAYDTR" SET "Status" = 'A' WHERE "DocEntry" = @Codigo 
		AND (SELECT TOP 1 "Canceled" FROM "@BPP_PAYDTR" WHERE "DocEntry" = @Codigo) = 'Y';

	IF @Options='O2' BEGIN
		UPDATE "OPCH" SET "U_STR_DetraccionPago"='N' WHERE "DocEntry"=( SELECT "U_BPP_DEAs" FROM "@BPP_PAYDTRDET" WHERE "DocEntry"=@Codigo);
	END;

END;
