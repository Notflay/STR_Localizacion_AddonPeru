create procedure STR_SP_VerNumAnul
(
	@pv_u_bpp_docsnt nvarchar(5),
	@pv_u_bpp_serie nvarchar(10),
	@pv_u_bpp_nmcr nvarchar(15)
)
as
begin

select 'existe' 
from "@BPP_ANULCORRDET" T0 
inner join "@BPP_ANULCORR" T1 on T0."DocEntry"=T1."DocEntry"
where 
T1."U_BPP_DocSnt" = @pv_u_bpp_docsnt
and T1."U_BPP_Serie" = @pv_u_bpp_serie
and T0."U_BPP_NmCr" = @pv_u_bpp_nmcr;
end;