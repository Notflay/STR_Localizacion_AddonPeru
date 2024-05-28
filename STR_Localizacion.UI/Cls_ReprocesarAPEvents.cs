using SAPbobsCOM;
using SAPbouiCOM;
using STR_Localizacion.UTIL;
using System;

namespace STR_Localizacion.UI
{
    partial class Cls_ReprocesarAsientoProvision
    {
        private void InitializeEvents()
        {
            itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "Item_4", e => BuscarDocumentos(e)));
            itemevent.Add(new sapitemevent(BoEventTypes.et_COMBO_SELECT, "Item_23", e => { if (!e.BeforeAction) SetDocNum(); }));
            itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "1", e => Crear(e)));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, s => Dispose()));
        }

        private void Crear(ItemEvent e)
        {
            if (e.BeforeAction && go_SBOForm.Mode == BoFormMode.fm_ADD_MODE)
            {
                int rpta = go_SBOApplication.MessageBox("Está a punto de generar los asientos de provisión de los documentos seleccionados. ¿Desea continuar?", 1, "Sí", "No");
                if (rpta == 1)
                {
                    int filasSeleccionadas = ObtenerNumeroDeSeleccionados();
                    if (filasSeleccionadas > 0)
                    {
                        EliminarFilasNoSeleccionadas(filasSeleccionadas);
                        CrearAsientosDeProvision();
                    }
                    else
                        throw new Exception("Debe seleccionar al menos un documento");
                }
                else
                    throw new Exception("Acción cancelada");
            }
        }

        private void CrearAsientosDeProvision()
        {
            Matrix matrix = go_SBOForm.GetMatrix("Item_6");
            Cls_Provisiones provision = new Cls_Provisiones();
            Documents entradaMercaderia = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);

            ProgressBar pb = go_SBOApplication.StatusBar.CreateProgressBar("Creando asientos de provisión", matrix.RowCount, false);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                int docEntry = Convert.ToInt32(matrix.GetCellSpecific("Col_1", i + 1).Value);
                entradaMercaderia.GetByKey(docEntry);
                JournalEntries asientoProvision = provision.ObtenerAsientoProvisiones(entradaMercaderia);

                pb.Text = $"Creando asiento de provisión para la entrada N° {docEntry}";
                pb.Value = pb.Value + 1;

                if (go_SBOCompany.InTransaction)
                    go_SBOCompany.EndTransaction(BoWfTransOpt.wf_RollBack);

                try
                {
                    go_SBOCompany.StartTransaction();

                    if (asientoProvision.Add() == 0)
                    {
                        matrix.GetCellSpecific("Col_6", i + 1).Value = go_SBOCompany.GetNewObjectKey();
                        entradaMercaderia.UserFields.Fields.Item("U_ST_EP_ASPR").Value = Convert.ToInt32(go_SBOCompany.GetNewObjectKey());
                        entradaMercaderia.Update();
                        go_SBOCompany.EndTransaction(BoWfTransOpt.wf_Commit);
                    }
                    else
                    {
                        matrix.GetCellSpecific("Col_9", i + 1).Value = go_SBOCompany.GetLastErrorDescription();
                    }
                }
                catch (Exception ex)
                {
                    matrix.GetCellSpecific("Col_6", i + 1).Value = string.Empty;
                    matrix.GetCellSpecific("Col_9", i + 1).Value = ex.Message;

                    if (go_SBOCompany.InTransaction)
                        go_SBOCompany.EndTransaction(BoWfTransOpt.wf_RollBack);

                    continue;
                }
            }

            pb.Stop();

            matrix.FlushToDataSource();
            matrix.LoadFromDataSource();
        }

        private void EliminarFilasNoSeleccionadas(int filasSeleccionadas)
        {
            try
            {
                go_SBOForm.Freeze(true);
                Matrix matrix = go_SBOForm.GetMatrix("Item_6");

                while (matrix.RowCount != filasSeleccionadas)
                {
                    for (int i = 1; i <= matrix.RowCount; i++)
                    {
                        if (!matrix.GetCellSpecific("Col_0", i).Checked)
                        {
                            matrix.DeleteRow(i);
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            { go_SBOForm.Freeze(false); }
        }

        private int ObtenerNumeroDeSeleccionados()
        {
            int filasSeleccionadas = 0;
            go_Matrix = go_SBOForm.GetMatrix("Item_6");

            for (int i = 0; i < go_Matrix.RowCount; i++)
            {
                go_CheckBox = go_Matrix.GetCellSpecific("Col_0", i + 1);
                if (go_CheckBox.Checked)
                    filasSeleccionadas++;
            }

            return filasSeleccionadas;
        }

        private void BuscarDocumentos(ItemEvent e)
        {
            if (!e.BeforeAction)
            {
                try
                {
                    go_SBOForm.Freeze(true);

                    go_Matrix = go_SBOForm.GetMatrix("Item_6");
                    string socioDesde, socioHasta, FContDesde, FContHasta, FVencDesde, FVencHasta;

                    socioDesde = go_SBOForm.GetHeaderDBValue("@ST_LC_OASPR", "U_ST_LC_CODD");
                    socioHasta = go_SBOForm.GetHeaderDBValue("@ST_LC_OASPR", "U_ST_LC_CODH");
                    FContDesde = go_SBOForm.GetHeaderDBValue("@ST_LC_OASPR", "U_ST_LC_FCNTD");
                    FContHasta = go_SBOForm.GetHeaderDBValue("@ST_LC_OASPR", "U_ST_LC_FCNTH");
                    FVencDesde = go_SBOForm.GetHeaderDBValue("@ST_LC_OASPR", "U_ST_LC_FVNCD");
                    FVencHasta = go_SBOForm.GetHeaderDBValue("@ST_LC_OASPR", "U_ST_LC_FVNCH");

                    string query = string.Empty;
                    if (go_SBOCompany.DbServerType == BoDataServerTypes.dst_HANADB)
                        query = $"CALL ST_LOC_GET_PENDIENTES_ASIENTO_PROV('{socioDesde}','{socioHasta}','{FContDesde}','{FContHasta}','{FVencDesde}','{FVencHasta}')";
                    else
                        query = $"EXECUTE ST_LOC_GET_PENDIENTES_ASIENTO_PROV '{socioDesde}','{socioHasta}','{FContDesde}','{FContHasta}','{FVencDesde}','{FVencHasta}'";

                    DataTable dt = go_SBOForm.DataSources.DataTables.Item("DT_DOCS");
                    DBDataSource ds = go_SBOForm.DataSources.DBDataSources.Item("@ST_LC_ASPR1");
                    Cls_Global.WriteToFile(query);
                    dt.ExecuteQuery(query);

                    if (!dt.IsEmpty)
                    {
                        go_SBOForm.DataSources.UserDataSources.Item("UD_0").Value = "N";
                        //RelacionarDataTable(go_Matrix, "DT_DOCS");

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            ds.InsertRecord(i);
                            string docEntry = dt.GetValue("U_ST_LC_DE", i).ToString();
                            ds.SetValue("U_ST_LC_DE", i, docEntry);
                            ds.SetValue("U_ST_LC_DOCNUM", i, dt.GetValue("U_ST_LC_DOCNUM", i).ToString());
                            ds.SetValue("U_ST_LC_CODSOC", i, dt.GetValue("U_ST_LC_CODSOC", i).ToString());
                            ds.SetValue("U_ST_LC_NOMSOC", i, dt.GetValue("U_ST_LC_NOMSOC", i).ToString());
                            ds.SetValue("U_ST_LC_TOTAL", i, dt.GetValue("U_ST_LC_TOTAL", i).ToString());
                            ds.SetValue("U_ST_LC_FCONT", i, dt.GetValue("U_ST_LC_FCONT", i).ToString("yyyyMMdd"));
                            ds.SetValue("U_ST_LC_FVENC", i, dt.GetValue("U_ST_LC_FVENC", i).ToString("yyyyMMdd"));
                        }

                        ds.RemoveRecord(ds.Size - 1);
                    }
                    else
                    {
                        go_SBOApplication.StatusBar.SetText("No se encontraron documentos pendientes", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                        go_SBOForm.DataSources.DBDataSources.Item("@ST_LC_ASPR1").Clear();
                    }
                    go_Matrix.LoadFromDataSource();
                    ConfigurarMatrix(go_Matrix);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    go_SBOForm.Freeze(false);
                }
            }
        }

        private void ConfigurarMatrix(Matrix matrix)
        {
            matrix.AutoResizeColumns();
        }

        private void RelacionarDataTable(Matrix matrix, string idDataTable)
        {
            matrix.Columns.Item("Col_1").DataBind.Bind(idDataTable, "U_ST_LC_DE");
            matrix.Columns.Item("Col_2").DataBind.Bind(idDataTable, "U_ST_LC_DOCNUM");
            matrix.Columns.Item("Col_3").DataBind.Bind(idDataTable, "U_ST_LC_CODSOC");
            matrix.Columns.Item("Col_4").DataBind.Bind(idDataTable, "U_ST_LC_NOMSOC");
            matrix.Columns.Item("Col_5").DataBind.Bind(idDataTable, "U_ST_LC_TOTAL");
            matrix.Columns.Item("Col_7").DataBind.Bind(idDataTable, "U_ST_LC_FCONT");
            matrix.Columns.Item("Col_8").DataBind.Bind(idDataTable, "U_ST_LC_FVENC");
            matrix.Columns.Item("Col_6").DataBind.Bind(idDataTable, "U_ST_LC_TRIDAP");
            matrix.Columns.Item("Col_9").DataBind.Bind(idDataTable, "U_ST_LC_MSJERR");
        }
    }
}