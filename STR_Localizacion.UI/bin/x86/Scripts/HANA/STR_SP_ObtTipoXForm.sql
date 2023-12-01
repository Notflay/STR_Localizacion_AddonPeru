create procedure STR_SP_ObtTipoXForm
(
in ps_formTypeEx nvarchar(10)
)
as
begin

select distinct 
T0."U_BPP_TDTD"
, T0."U_BPP_TDDD" 
from "@BPP_TPODOC" T0 
inner join "@BPP_TIPOXFORM" T1 on T0."U_BPP_TDTD"=T1."U_BPP_Tipo"
where 
T1."U_BPP_Form" = ps_formTypeEx
;
end;