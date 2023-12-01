create procedure STR_SP_ObtSerieSunat
(
IN pv_u_bpp_ndtd VARCHAR(10),
IN pv_Usuario VARCHAR(25)
)
AS
BEGIN

SELECT 
"U_BPP_NDSD"
, "U_BPP_NDSD" 
FROM 
"@BPP_NUMDOC"
WHERE 
"U_BPP_NDTD" = :pv_u_bpp_ndtd --OR "U_BPP_NDTD" !='09'

AND SUBSTRING("U_BPP_USUARIOS", LOCATE("U_BPP_USUARIOS", :pv_Usuario), LENGTH(:pv_Usuario))= :pv_Usuario

union all
SELECT 
"U_BPP_NDSD"
, "U_BPP_NDSD" 
FROM 
"@BPP_NUMDOC"
WHERE 
LEFT("U_BPP_NDSD",1)not in ('F', 'B') and "U_BPP_NDTD" = :pv_u_bpp_ndtd and "U_BPP_NDTD" !='09';

END;

