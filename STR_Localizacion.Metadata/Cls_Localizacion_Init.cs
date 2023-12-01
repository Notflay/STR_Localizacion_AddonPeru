using STR_Localizacion.UTIL;
using System;
using System.Windows.Forms;

namespace STR_Localizacion.MetaData
{
    public class Cls_Localizacion_Init : Cls_Properties
    {
        private int li_IndInstal = 1;
        private string ls_Path = string.Empty;

        public Cls_Localizacion_Init()
        {
            ls_Path = Application.StartupPath;
        }

        public void sb_VerificarInstalacion()
        {
            SAPbobsCOM.Recordset lo_RecSet = null;
            SAPbobsCOM.UserTablesMD lo_UsrTblMD = null;
            SAPbobsCOM.UserTable lo_UsrTbl = null;
            string ls_Qry = string.Empty;
            try
            {
                lo_RecSet = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lo_UsrTblMD = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
                if (lo_UsrTblMD.GetByKey("STR_LOC_SYS"))
                {
                    lo_UsrTbl = go_SBOCompany.UserTables.Item("STR_LOC_SYS");
                    ls_Qry = @"SELECT ""U_LO_ID"" FROM ""@STR_LOC_SYS""";
                    lo_RecSet.DoQuery(ls_Qry);
                    if (lo_RecSet.EoF)
                    {
                        lo_UsrTbl.Code = "001";
                        lo_UsrTbl.Name = "001";
                        lo_UsrTbl.UserFields.Fields.Item("U_LO_ID").Value = "1";
                        if (lo_UsrTbl.Add() != 0)
                            go_SBOApplication.SetStatusBarMessage(go_SBOCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Short);
                        return;
                    }
                    else
                    {
                        if (Convert.ToInt32(lo_RecSet.Fields.Item(0).Value) < li_IndInstal)
                        {
                            if (lo_UsrTbl.GetByKey("001"))
                            {
                                lo_UsrTbl.UserFields.Fields.Item("U_LO_ID").Value = li_IndInstal.ToString();
                                if (lo_UsrTbl.Update() != 0)
                                    go_SBOApplication.SetStatusBarMessage(go_SBOCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Short);
                            }
                            else
                                throw new Exception("No se ha configurado el código 001 en la tabla STR_LOC_SYS");
                        }
                        else
                            return;
                    }
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(lo_RecSet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(lo_UsrTblMD);
                if (lo_UsrTbl != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(lo_UsrTbl);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                lo_RecSet = null;
                lo_UsrTblMD = null;
                lo_UsrTbl = null;
                sb_CreateAddonResources();
            }
            catch (Exception ex)
            {
                go_SBOApplication.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short);
            }
        }

        private void sb_CreateAddonResources()
        {
            go_SBOApplication.MetadataAutoRefresh = false; 

            sb_CreateUserTables();
            sb_CreateUserFields();
            sb_CreateUserDefinedObjects();
            sb_CreateStoreProcedures();
            go_SBOApplication.MetadataAutoRefresh = true;
        }

        private void sb_CreateUserTables()
        {
            SAPbobsCOM.UserTablesMD lo_UsrTblMD = null;
            SAPbouiCOM.ProgressBar lo_PrgssBar = null;
            string ls_DscErr = string.Empty;
            string ls_PathFile = string.Empty;
            int li_CodErr = 0;
            int li_CntTbls = 0;
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                go_SBOApplication.StatusBar.SetText("Localizacion: Iniciando la creación de tablas de usuario...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                Cursor.Current = Cursors.WaitCursor;
                ls_PathFile = ls_Path + @"\BO\UT.vte";
                li_CntTbls = go_SBOCompany.GetXMLelementCount(ls_PathFile);
                lo_PrgssBar = go_SBOApplication.StatusBar.CreateProgressBar("Creando tablas...", li_CntTbls, false);
                for (int i = 0; i < li_CntTbls; i++)
                {
                    lo_UsrTblMD = go_SBOCompany.GetBusinessObjectFromXML(ls_PathFile, i);
                    lo_PrgssBar.Text = "Creando tabla: " + lo_UsrTblMD.TableName;
                    if (lo_UsrTblMD.Add() != 0)
                    {
                        go_SBOCompany.GetLastError(out li_CodErr, out ls_DscErr);
                        if (li_CodErr != -2035)
                            go_SBOApplication.SetStatusBarMessage(li_CodErr + " - " + ls_DscErr, SAPbouiCOM.BoMessageTime.bmt_Short);
                    }
                    lo_UsrTblMD = null;
                    GC.Collect();
                    lo_PrgssBar.Value += 1;
                }
                go_SBOApplication.StatusBar.SetText("Tablas de usuario creadas correctamente...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
            catch (Exception ex)
            {
                go_SBOApplication.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                lo_PrgssBar.Stop();
                lo_PrgssBar = null;
                lo_UsrTblMD = null;
            }
        }

        private void sb_CreateUserFields()
        {
            SAPbobsCOM.UserFieldsMD lo_UsrFldMD = null;
            SAPbouiCOM.ProgressBar lo_PrgssBar = null;
            string ls_DscErr = string.Empty;
            string ls_PathFile = string.Empty;
            int li_CodErr = 0;
            int li_CntFlds = 0;
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                go_SBOApplication.StatusBar.SetText("Localizacion: Iniciando la creación de campos de usuario...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                Cursor.Current = Cursors.WaitCursor;
                ls_PathFile = ls_Path + @"\BO\UF.vte";
                li_CntFlds = go_SBOCompany.GetXMLelementCount(ls_PathFile);
                lo_PrgssBar = go_SBOApplication.StatusBar.CreateProgressBar("Creando Campos...", li_CntFlds, false);
                for (int i = 0; i < li_CntFlds; i++)
                {
                    lo_UsrFldMD = go_SBOCompany.GetBusinessObjectFromXML(ls_PathFile, i);
                    lo_PrgssBar.Text = "Creando campo: " + lo_UsrFldMD.Name + " de tabla: " + lo_UsrFldMD.TableName;
                    if (lo_UsrFldMD.Add() != 0)
                    {
                        go_SBOCompany.GetLastError(out li_CodErr, out ls_DscErr);
                        if (li_CodErr != -2035 && li_CodErr != -5002)
                            go_SBOApplication.SetStatusBarMessage(li_CodErr + " - " + ls_DscErr, SAPbouiCOM.BoMessageTime.bmt_Short);
                    }
                    lo_UsrFldMD = null;
                    GC.Collect();
                    lo_PrgssBar.Value += 1;
                }
                go_SBOApplication.StatusBar.SetText("Campos de usuario creados correctamente...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
            catch (Exception ex)
            {
                go_SBOApplication.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                lo_PrgssBar.Stop();
                lo_PrgssBar = null;
                lo_UsrFldMD = null;
            }

        }

        private void sb_CreateUserDefinedObjects()
        {
            SAPbobsCOM.UserObjectsMD lo_UsrObjMD = null;
            SAPbobsCOM.UserObjectsMD lo_UsrObjMDAux = null;
            SAPbouiCOM.ProgressBar lo_PrgssBar = null;
            string ls_DscErr = string.Empty;
            string ls_PathFile = string.Empty;
            int li_CodErr = 0;
            int li_CntFlds = 0;
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                lo_UsrObjMDAux = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);
                go_SBOApplication.StatusBar.SetText("Localizacion: Iniciando la creación de objetos de usuario...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                Cursor.Current = Cursors.WaitCursor;
                ls_PathFile = ls_Path + @"\BO\UO.vte";
                li_CntFlds = go_SBOCompany.GetXMLelementCount(ls_PathFile);
                lo_PrgssBar = go_SBOApplication.StatusBar.CreateProgressBar("Creando Objetos...", li_CntFlds, false);
                for (int i = 0; i < li_CntFlds; i++)
                {
                    lo_UsrObjMD = go_SBOCompany.GetBusinessObjectFromXML(ls_PathFile, i);
                    lo_PrgssBar.Text = "Creando objeto de usuario: " + lo_UsrObjMD.Name;
                    if (!lo_UsrObjMDAux.GetByKey(lo_UsrObjMD.Code))
                    {
                        if (lo_UsrObjMD.Add() != 0)
                        {
                            go_SBOCompany.GetLastError(out li_CodErr, out ls_DscErr);
                            if (li_CodErr != -2035)
                                go_SBOApplication.SetStatusBarMessage(li_CodErr + " - " + ls_DscErr, SAPbouiCOM.BoMessageTime.bmt_Short);
                        }
                    }
                    lo_UsrObjMD = null;
                    GC.Collect();
                    lo_PrgssBar.Value += 1;
                }
                go_SBOApplication.StatusBar.SetText("Objetos de usuario creados correctamente...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
            catch (Exception ex)
            {
                go_SBOApplication.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short);
            }
            finally
            {
                lo_UsrObjMDAux = null;
                Cursor.Current = Cursors.Default;
                lo_PrgssBar.Stop();
                lo_PrgssBar = null;
            }
        }

        private void sb_CreateStoreProcedures()
        {
            SAPbobsCOM.Recordset lo_RecSet = null;
            SAPbobsCOM.Recordset lo_RevSetAux = null;
            string[] lo_ArrFiles = null;
            string ls_Qry = string.Empty;
            string ls_Tipo = string.Empty;
            string ls_TipoSQL = string.Empty;
            string ls_NmbFile = string.Empty;
            System.IO.StreamReader lo_StrmRdr = null;
            string ls_StrFile = string.Empty;
            string[] lo_ArrTpoScrpt = null;

            lo_RecSet = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            lo_RevSetAux = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            if (Cls_Global.go_ServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
            {
                lo_ArrFiles = System.IO.Directory.GetFiles(ls_Path + @"\Scripts\HANA\", "*.sql");
            }
            else
            {
                lo_ArrFiles = System.IO.Directory.GetFiles(ls_Path + @"\Scripts\SQL\", "*.sql");
            }

            for (int i = 0; i < lo_ArrFiles.GetUpperBound(0) + 1; i++)
            {
                lo_StrmRdr = new System.IO.StreamReader(lo_ArrFiles[i]);
                ls_StrFile = lo_StrmRdr.ReadToEnd();
                lo_ArrTpoScrpt = ls_StrFile.Substring(0, 50).Split(new char[] { ' ' });
                ls_NmbFile = System.IO.Path.GetFileName(lo_ArrFiles[i]);
                ls_NmbFile = ls_NmbFile.Substring(0, ls_NmbFile.Length - 4);

                if (lo_ArrTpoScrpt[1].Trim() == "PROCEDURE")
                {
                    ls_Tipo = "el procedimiento ";
                    ls_TipoSQL = "= 'P'";
                }
                else if (lo_ArrTpoScrpt[1].Trim() == "VIEW")
                {
                    ls_Tipo = "la vista ";
                    ls_TipoSQL = "= 'V'";
                }
                else if (lo_ArrTpoScrpt[1].Trim() == "FUNCTION")
                {
                    ls_Tipo = "la funcion ";
                    ls_TipoSQL = "in (N'FN', N'IF', N'TF', N'FS', N'FT')";
                }
                if (Cls_Global.go_ServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    ls_Qry = @"SELECT COUNT('A') FROM ""SYS"".""OBJECTS"" WHERE ""OBJECT_NAME"" ='" + ls_NmbFile.Trim().ToUpper() + @"' AND ""SCHEMA_NAME"" = '" + go_SBOCompany.CompanyDB + "'";
                }
                else
                {
                    ls_Qry = @"SELECT COUNT(*) FROM sys.all_objects WHERE type " + ls_TipoSQL + " and name = '" + ls_NmbFile.Trim().ToUpper() + "'";
                }

                lo_RecSet.DoQuery(ls_Qry);
                if (!lo_RecSet.EoF)
                {
                    if (Convert.ToInt32(lo_RecSet.Fields.Item(0).Value) != 0)
                    {
                        try
                        {
                            ls_Qry = @"DROP " + lo_ArrTpoScrpt[1].Trim() + " " + ls_NmbFile;
                            lo_RecSet.DoQuery(ls_Qry);
                            lo_RecSet.DoQuery(ls_StrFile);
                            go_SBOApplication.StatusBar.SetText("Se creo/actualizo " + ls_Tipo + ls_NmbFile, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                        }
                        catch (Exception ex)
                        {
                            go_SBOApplication.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short);
                        }
                    }
                    else
                    {
                        try
                        {
                            lo_RecSet.DoQuery(ls_StrFile);
                            go_SBOApplication.StatusBar.SetText("Se creo/actualizo " + ls_Tipo + ls_NmbFile, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                        }
                        catch (Exception ex)
                        {
                            go_SBOApplication.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short);
                        }
                    }
                }
            }
        }
    }
}
