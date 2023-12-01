using STR_Localizacion.UTIL;
using System;

namespace STR_Localizacion.BL
{
    public class Cls_CodigoAddon_BL // : Cls_Properties
    {
        #region Declaración de variables

        private static SAPbobsCOM.Company go_SBOCompany = Cls_Global.go_SBOCompany;

        private static System.Data.DataTable lo_tbl_1 = new System.Data.DataTable();
        private static System.Data.DataTable lo_tbl_2 = new System.Data.DataTable();
        private static System.Data.DataRow lo_row1 = null;
        private static System.Data.DataRow lo_row2 = null;

        public static string gs_hadkey;
        public static string gs_cadena;
        public static bool gb_initAddon;

        #endregion Declaración de variables

        public static bool sb_IniciarAddOn()
        {
            sb_InsertarUpdateCodigo();

            return gs_cadena == fn_GenerarEncriptacion();
        }

        #region "Validar Codigo del Addon"

        /// <Encripta el codigo ingresado>
        /// Funcion que permite encriptar el codigo, pasa por varios procesos de conversion como convertir a un entero sin decimales, etc
        /// </summary>
        /// <returns>codigo final</returns>
        public static string fn_GenerarEncriptacion()
        {
            //Limpia los valores de la tabla
            lo_tbl_1.Rows.Clear();
            lo_tbl_2.Rows.Clear();
            lo_tbl_1.Columns.Clear();
            lo_tbl_2.Columns.Clear();
            //Declara variables del método
            string ls_key = gs_hadkey;
            bool lb_bol = false;
            bool lb_bol2 = false;
            string ls_cad = string.Empty;
            string ls_cadfin = string.Empty;

            string ls_cadgen = Cls_Global.fnSpace(1) + "21487145789hu44rryasd2134214cdpql88mxkz23f427b8056tcdpqswglm000dpqswglmio345v7793en54268j68ñ651274753654z";
            string ls_AbcdX, ls_AbcdY, ls_cadnum2, ls_cadnum1;
            int li_ppar, li_ppub, li_upar, li_upub, li_pimp, li_piub, li_uimp, li_uiub, li_ngen, li_i, li_j;
            //Agrega columnas de tipo string y integer
            lo_tbl_1.Columns.Add(new System.Data.DataColumn("Caracter", typeof(string)));
            lo_tbl_1.Columns.Add(new System.Data.DataColumn("Posicion", typeof(int)));
            lo_tbl_2.Columns.Add(new System.Data.DataColumn("Caracter", typeof(string)));
            lo_tbl_2.Columns.Add(new System.Data.DataColumn("Posicion", typeof(int)));

            li_uimp = 0;
            li_pimp = 0;
            li_upub = 0;
            li_ppar = 0;
            li_piub = 0;
            li_uiub = 0;
            li_ppub = 0;
            li_upar = 0;

            if (int.Parse(ls_key.Substring(1, 1)) >= 2)
            {
                ls_key = ls_key.Substring(0, 1) + "1" + ls_key.Substring(2, ls_key.Length - 2);
            }

            for (li_i = 0; (li_i <= (ls_key.Length - 1)); li_i++)
            {
                //if (Information.IsNumeric(ls_key.Substring(li_i, 1)))
                if (char.IsNumber(Convert.ToChar(ls_key.Substring(li_i, 1))))
                {
                    lo_row1 = lo_tbl_1.NewRow();

                    lo_row1["Caracter"] = ls_key.Substring(li_i, 1);
                    lo_row1["Posicion"] = (li_i + 1);
                    lo_tbl_1.Rows.Add(lo_row1);
                }
                else
                {
                    lo_row2 = lo_tbl_2.NewRow();
                    lo_row2["Caracter"] = ls_key.Substring(li_i, 1);
                    lo_row2["Posicion"] = li_i + 1;
                    lo_tbl_2.Rows.Add(lo_row2);
                }
            }

            for (li_i = 0; (li_i <= (lo_tbl_1.Rows.Count - 1)); li_i++)
            {
                if (Convert.ToInt32(lo_tbl_1.Rows[li_i][0]) % 2 == 0 && (lb_bol == false))
                {
                    li_ppar = Convert.ToInt32(lo_tbl_1.Rows[li_i][0]);
                    li_ppub = Convert.ToInt32(lo_tbl_1.Rows[li_i][1]);
                    lb_bol = true;
                }
                else if (Convert.ToInt32(lo_tbl_1.Rows[li_i][0]) % 2 == 0)
                {
                    li_upar = Convert.ToInt32(lo_tbl_1.Rows[li_i][0]);
                    li_upub = Convert.ToInt32(lo_tbl_1.Rows[li_i][1]);
                }
                if (Convert.ToInt32(lo_tbl_1.Rows[li_i][0]) % 2 != 0 && (lb_bol2 == false))
                {
                    li_pimp = Convert.ToInt32(lo_tbl_1.Rows[li_i][0]);
                    li_piub = Convert.ToInt32(lo_tbl_1.Rows[li_i][1]);
                    lb_bol2 = true;
                }
                else if (Convert.ToInt32(lo_tbl_1.Rows[li_i][0]) % 2 != 0)
                {
                    li_uimp = Convert.ToInt32(lo_tbl_1.Rows[li_i][0]);
                    li_uiub = Convert.ToInt32(lo_tbl_1.Rows[li_i][1]);
                }
                ls_cad += lo_tbl_1.Rows[li_i][0];
            }

            if (li_uimp == 0)
            {
                li_uimp = li_pimp;
            }
            if (li_pimp == 0)
            {
                li_uimp = 1;
            }

            //Obtencion de numero entero, sin decimales
            li_ngen = ((int.Parse(ls_cad) + li_pimp + li_upub + li_ppar) + (int.Parse(ls_cad) % li_uimp)) / li_uimp;
            ls_cadnum1 = Convert.ToString(li_ngen);

            ls_AbcdX = Cls_Global.fnSpace(1) + ls_cadgen.Substring(ls_cadgen.IndexOf(Convert.ToString(lo_tbl_2.Rows[0][0]).ToLower()) + 1, (ls_cadgen.Length - ls_cadgen.IndexOf(Convert.ToString(lo_tbl_2.Rows[0][0]).ToLower()) - 1));

            ls_AbcdY = Cls_Global.fnSpace(1) + ls_cadgen.Substring(1, (ls_cadgen.IndexOf(Convert.ToString(lo_tbl_2.Rows[0][0]).ToLower()) - 1));

            ls_cadnum2 = Convert.ToString(li_piub.ToString() + li_ppar.ToString() + li_uiub.ToString() + li_pimp.ToString() + li_ppub.ToString() + li_uimp.ToString() + li_upub.ToString() + li_upar.ToString());
            if (ls_AbcdX.Trim().Length != 0)
            {
                for (li_i = 0; (li_i <= (ls_cadnum1.ToString().Length - 1)); li_i++)
                {
                    ls_cadfin += ls_AbcdX.Substring(Convert.ToInt32(Cls_Global.IIf(Convert.ToUInt32(ls_cadnum1.Substring(li_i, 1)) == 0, 1, ls_cadnum1.Substring(li_i, 1))), 1);
                }
            }
            if (ls_AbcdY.Trim().Length != 0)
            {
                for (li_j = 0; (li_j <= ls_cadnum2.Length - 1); li_j++)
                {
                    ls_cadfin += ls_AbcdY.Substring(Convert.ToInt32(Cls_Global.IIf(Convert.ToUInt32(ls_cadnum2.Substring(li_j, 1)) == 0, 1, ls_cadnum2.Substring(li_j, 1))), 1);
                }
            }
            return ls_cadfin; //Retorna el Codigo del Addon
        }

        /// <Insercion de codigo addon>
        /// El metodo recibe un parametro, el cual es pasado en la ejecucion de la sentencia de insercion ubicado en el RESOURCE
        public static void sb_InsertarUpdateCodigo(bool pb_Update = false)
        {
            SAPbobsCOM.UserTablesMD lo_UsrTblMD = null;
            SAPbobsCOM.UserTable lo_UsrTbl = null;
            SAPbobsCOM.Recordset lo_RecSet = null;
            string ls_Qry = string.Empty;
            try
            {
                lo_RecSet = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lo_UsrTblMD = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
                if (lo_UsrTblMD.GetByKey("BPP_CONFIG"))
                {
                    lo_UsrTbl = go_SBOCompany.UserTables.Item("BPP_CONFIG");
                    ls_Qry = @"SELECT ""U_BPP_CdgAddon"" FROM ""@BPP_CONFIG""";
                    lo_RecSet.DoQuery(ls_Qry);
                    if (lo_RecSet.EoF)
                    {
                        lo_UsrTbl.Code = "1";
                        lo_UsrTbl.Name = "1";
                        lo_UsrTbl.UserFields.Fields.Item("U_BPP_CdgAddon").Value = gs_cadena;
                        if (lo_UsrTbl.Add() != 0)
                        {
                            Cls_Global.go_SBOApplication.SetStatusBarMessage(go_SBOCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Short);
                        }
                    }
                    else if (pb_Update)
                    {
                        lo_UsrTbl.GetByKey("1");
                        lo_UsrTbl.UserFields.Fields.Item("U_BPP_CdgAddon").Value = gs_cadena;
                        if (lo_UsrTbl.Update() != 0)
                        {
                            Cls_Global.go_SBOApplication.SetStatusBarMessage(go_SBOCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Short);
                        }
                    }
                    else { gs_cadena = lo_RecSet.Fields.Item("U_BPP_CdgAddon").Value; }
                }
            }
            catch (Exception MsjExc)
            {
                Cls_Global.go_SBOApplication.StatusBar.SetText(MsjExc.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(lo_RecSet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(lo_UsrTblMD);
                if (lo_UsrTbl != null) { System.Runtime.InteropServices.Marshal.ReleaseComObject(lo_UsrTbl); }
                lo_RecSet = null;
                lo_UsrTblMD = null;
                lo_UsrTbl = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <Verificación del Addon>
        /// Aqui si la verficación del addon es correcta, entonces llama finalmente a "Iniciar Addon" de la clase Cls_Inicializacion
        /// </summary>
        /// <param name="po_SBOApplication"></param>
        /// <param name="po_SBOForm"></param>
        /// <param name="po_ItemEvent"></param>
        /// <param name="po_CodAddon"></param>
        /// <returns></returns>
        public static bool fn_VerificarCodigoAddOn()
        {
            try
            {
                if (gs_cadena == fn_GenerarEncriptacion())
                {
                    sb_InsertarUpdateCodigo(true);
                    return true;
                }
                else
                {
                    Cls_Global.go_SBOApplication.StatusBar.SetText("El codigo de Addon es incorrecto", SAPbouiCOM.BoMessageTime.bmt_Medium);
                    return false;
                }
            }
            catch (Exception) { return false; }
        }

        #endregion "Validar Codigo del Addon"
    }
}