create procedure STR_SP_ObtCorrSunatTxtTipSer
(
	@pv_u_bpp_ndtd nvarchar(5),
	@pv_u_bpp_ndsd nvarchar(10)
)
as
begin

select 
top 1 "U_BPP_NDCD"
from "@BPP_NUMDOC" 
where 
"U_BPP_NDTD" = @pv_u_bpp_ndtd
and "U_BPP_NDSD" = @pv_u_bpp_ndsd;
end;