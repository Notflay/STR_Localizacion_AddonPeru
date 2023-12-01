CREATE PROCEDURE STR_SP_ValidaAnulSUNAT
(
	@TpSunat nvarchar(20),
	@Serie nvarchar(20)
)
AS
BEGIN
	SELECT top 1 "U_BPP_NDCD" FROM "@BPP_NUMDOC" WHERE "U_BPP_NDTD"=@TpSunat AND "U_BPP_NDSD"=@Serie;
END;