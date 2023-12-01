CREATE PROCEDURE STR_SP_ObtTipoDefXForm
(
	@pv_u_bpp_form nvarchar(10)
)
as
begin

select 
top 1 
T0."U_BPP_TDTD" 
from "@BPP_TPODOC" T0 
inner join "@BPP_TIPOXFORM" T1 on T0."U_BPP_TDTD"=T1."U_BPP_Tipo" 
where 
T1."U_BPP_Form"=@pv_u_bpp_form
and T1."U_BPP_PorDefecto"='S'
;
end;