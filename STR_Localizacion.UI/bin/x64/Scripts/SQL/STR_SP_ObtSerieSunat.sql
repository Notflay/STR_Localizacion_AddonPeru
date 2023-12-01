create procedure STR_SP_ObtSerieSunat
(
	@pv_u_bpp_ndtd nvarchar(10),
	@pv_Usuario VARCHAR(25)
)
AS
begin

select 
"U_BPP_NDSD"
, "U_BPP_NDSD"
from 
"@BPP_NUMDOC"
where 
U_BPP_NDTD = @pv_u_bpp_ndtd
;
end;

