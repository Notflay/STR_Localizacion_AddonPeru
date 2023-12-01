using SAPbobsCOM;
using STR_Localizacion.UTIL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STR_Localizacion.BL
{
    public static class Validacion
    {
        //public bool fn_getPreviaValidacion(int ps_addn)
        //{

        //    if ()

        //}

        public static bool fn_getComparacion(int pi_addnId)
        {

            try
            {
                SAPbouiCOM.Application lo_app = Cls_Global.go_SBOApplication;
                SAPbobsCOM.Recordset go_RecordSet = Cls_Global.go_SBOCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                int li_tipodb = Cls_Global.go_ServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB ? 1 : 0;

                if (li_tipodb == 1)
                    go_RecordSet.DoQuery($"SELECT \"U_STR_Clave\",\"U_STR_Activo\" FROM \"@STR_ADDONSPERU\" where \"Code\" = '{pi_addnId}'");
                else
                    go_RecordSet.DoQuery($"SELECT [U_STR_Clave],[U_STR_Activo] FROM \"@STR_ADDONSPERU\" where [Code] = '{pi_addnId}'");

                if (go_RecordSet.RecordCount > 0)
                {

                    string ls_code = go_RecordSet.Fields.Item(0).Value;
                    string ls_activo = go_RecordSet.Fields.Item(1).Value;

                    if (ls_code == null | ls_code == "" | fn_getValidacion(ls_code, pi_addnId) == false)
                    {
                        lo_app.MessageBox($"Formulario o Funcionalidad {(PeruAddon)pi_addnId} restringido, contacta con tu proveedor");
                        return false;

                    }
                    else if (ls_activo == "N")
                    {
                        if ((PeruAddon)pi_addnId == PeruAddon.TipoCambio)
                        {
                            return false;
                        }
                        else
                        {
                            lo_app.MessageBox($"Formulario o Funcionalidad {(PeruAddon)pi_addnId} se encuentra desactiva, si deseas activiarlo dirigite al Menu Addon Perú");
                            return false;
                        }

                    }
                    return true;
                }
                lo_app.MessageBox("La tabla @STR_ADDONSPERU no contiene está funcionalidad, contacta con tu proveedor");
                return false;
            }
            catch (Exception)
            {

                throw;
            }

        }


        public static bool fn_getValidacion(string ps_key, int ps_addn)
        {
            SAPbobsCOM.Company sboCompany = Cls_Global.go_SBOCompany;

            if (ps_key == string.Empty)
                return false;


            CodigoAddon codigoAddon = (CodigoAddon)ps_addn;

            string ls_nombDb = sboCompany.CompanyDB;
            string ls_nomAdd = codigoAddon.ToString();
            string ls_hardKey = Cls_Global.gs_hardwarek;

            var lst_serial = Code.generate(ls_nombDb, ls_nomAdd, ls_hardKey);

            string ls_key = string.Join("-", lst_serial);

            if (ps_key == ls_key)
                return true;
            return false;
        }
        /*
        public static string fn_getHrdKey()
        {

            try
            {
                SAPbouiCOM.Application app = Cls_Global.go_SBOApplication;

                app.Menus.Item("257").Activate();
                SAPbouiCOM.Form aboutSAP = app.Forms.GetForm("999999", 0);
                string a = ((SAPbouiCOM.EditText)aboutSAP.Items.Item("79").Specific).Value;
                aboutSAP.Close();

                return a;
            }
            catch (Exception)
            {
                throw;
            }
        }
        */

        public static string fn_getBits()
        {
            try
            {
                SAPbouiCOM.Application app = Cls_Global.go_SBOApplication;

                app.Menus.Item("257").Activate();
                SAPbouiCOM.Form aboutSAP = app.Forms.GetForm("999999", 0);
                string a = ((SAPbouiCOM.StaticText)aboutSAP.Items.Item("26").Specific).Caption;
                aboutSAP.Close();

                return a;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static string fn_getCodigo(int ps_addn)
        {
            int li_tipodb = Cls_Global.go_ServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB ? 1 : 0;

            SAPbobsCOM.Recordset go_Recorset = Cls_Global.go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            if (li_tipodb == 1)
                go_Recorset.DoQuery($"SELECT \"U_STR_Clave\" FROM \"@STR_ADDONSPERU\" WHERE \"Code\" = '{ps_addn}'");
            else
                go_Recorset.DoQuery($"SELECT [U_STR_Clave] FROM \"@STR_ADDONSPERU\" WHERE [Code] = '{ps_addn}'");


            if (go_Recorset.RecordCount > 0)
                return go_Recorset.Fields.Item(0).Value == null ? string.Empty : go_Recorset.Fields.Item(0).Value;
            else
                throw new Exception("No se encontró addon en la tabla @STR_ADDONSPERU");
        }
    }
}
