CREATE PROCEDURE STR_SP_ActualizaPagoDetraccion
(
	@Codigo integer
)
AS
BEGIN
	DECLARE @Options nvarchar(5);
	SELECT @Options="U_STR_MCTC" FROM "@STR_LC_CONF" WHERE "Code" = '01';

	update "@BPP_PAYDTRDET"
	set "@BPP_PAYDTRDET"."U_BPP_DEPg" = CAST("OVPM"."DocEntry" AS INTEGER)
	, "@BPP_PAYDTRDET"."U_BPP_NmPg" = CAST("OVPM"."DocNum" AS INTEGER)
	from
	"@BPP_PAYDTRDET" 
	inner join "VPM2" on "@BPP_PAYDTRDET"."U_BPP_DEAs"="VPM2"."DocEntry"
	inner join "OVPM" on "VPM2"."DocNum" ="OVPM"."DocEntry"
	where
	"OVPM"."CardCode"="@BPP_PAYDTRDET"."U_BPP_CgPv"
	and COALESCE("OVPM"."Canceled", '') <> 'Y'
	and "@BPP_PAYDTRDET"."DocEntry"=@Codigo;
	
	IF @Options='O2' BEGIN
		UPDATE "OPCH" SET "U_STR_DetraccionPago"='Y' WHERE "DocEntry"=( SELECT "U_BPP_DEAs" FROM "@BPP_PAYDTRDET" WHERE "DocEntry"=@Codigo);
	END;

END;