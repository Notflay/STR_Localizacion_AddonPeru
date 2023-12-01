//using Microsoft.Office.Interop.Excel;
using SAPbouiCOM;
using STR_Localizacion.UTIL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

//using _excel = Microsoft.Office.Interop.Excel;

namespace STR_Localizacion.UI
{
    internal class Cls_SeleccionSeriesArticulo : Cls_PropertiesControl
    {
        private Form frmSeriesSAP = null;
        private List<ItemSeries> lstExlSeries = new List<ItemSeries>();

        public Cls_SeleccionSeriesArticulo()
        {
            gs_FormName = "FormSeleccionSeries";
            gs_FormPath = "Resources/Localizacion/Frm001.srf";
            lc_NameClass = "Cls_SeleccionSeriesArticulo";
        }

        public void sb_FormSAPLoad(Form form)
        {
            frmSeriesSAP = form;
            //Aniado un boton para llamar a este formulario de usuario
            go_Item = form.Items.Add("btnSlcExl", BoFormItemTypes.it_BUTTON);
            go_Item.Height = form.GetItem("4").Height;
            go_Item.Width = 100;
            go_Item.Visible = false; 
            go_Item.Top = form.GetItem("4").Top;
            go_Item.Left = form.GetItem("4").Left - 110;
            go_Button = go_Item.Specific;
            go_Button.Caption = "Seleccionar de excel";
            InitializeEvents();
        }

        private void sb_FormLoad()
        {
            try
            {
                if (go_SBOForm == null)
                {
                    go_SBOForm = Cls_Global.fn_CreateForm(gs_FormName, gs_FormPath);
                    ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
                    sb_DataFormLoad();
                }
            }
            catch (Exception exc)
            {
                go_SBOApplication.SetStatusBarMessage(exc.Message, BoMessageTime.bmt_Short, true);
            }
            finally { go_SBOForm.Visible = true; }
        }

        private void sb_DataFormLoad()
        {
            go_SBOForm.GetComboBox("cmbTpoDto").Select(0, BoSearchKey.psk_Index);
            char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToUpper().ToCharArray();
            var oitm = go_SBOForm.DataSources.DBDataSources.Item("OITM");
            int row = 0;
            alphabet.ToList().ForEach(l =>
            {
                oitm.InsertRecord(row);
                oitm.Offset = row;
                oitm.SetValue("ItemCode", row, l.ToString());
                row++;
            });
            go_Matrix = go_SBOForm.GetMatrix("mtxClmns");
            go_Matrix.LoadFromDataSourceEx();
        }

        private void InitializeEvents()
        {
            itemevent.Add(new sapitemevent(BoEventTypes.et_CLICK, "btnSlcExl", e =>
             {
                 if (e.BeforeAction) sb_FormLoad();
             }));
            itemevent.Add(new sapitemevent(BoEventTypes.et_CLICK, "btnSlcSre", e =>
             {
                 if (!e.BeforeAction)
                 {
                     try
                     {
                         frmSeriesSAP.Freeze(true);
                         Matrix go_MatrixAux = null;
                         go_SBOApplication.StatusBar.SetText("Iniciando la selección de series SAP..."
                        , BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                         go_Matrix = frmSeriesSAP.GetMatrix("3");
                         var SBDR = frmSeriesSAP.DataSources.DBDataSources.Item("SBDR");
                         for (int i = 0; i < SBDR.Size; i++)
                         {
                             var itemCode = SBDR.GetValue("ItemCode", i).Trim();
                             var whsCode = SBDR.GetValue("WhseCode", i).Trim();
                             go_Matrix.Columns.Item("0").Cells.Item(i + 1).Click(BoCellClickType.ct_Regular);
                             go_MatrixAux = frmSeriesSAP.GetMatrix("5");
                             var lstSeries = lstExlSeries.Where(s => s.WhsCode.Equals(whsCode) && s.ItemCode.Equals(itemCode))
                             .OrderBy(s => s.SerialNumber).Select(t => t.SerialNumber).ToList();
                             go_MatrixAux.Columns.Item("0").TitleObject.Sort(BoGridSortType.gst_Ascending);
                             for (int j = 0; j < go_MatrixAux.RowCount; j++)
                             {
                                 var sreArtSAP = ((EditText)go_MatrixAux.GetCellSpecific("19", j + 1)).Value;
                                 if (lstSeries.Contains(sreArtSAP))
                                     go_MatrixAux.Columns.Item("0").Cells.Item(j + 1).Click(BoCellClickType.ct_Regular, 4096);
                             }
                             frmSeriesSAP.GetItem("8").Click(BoCellClickType.ct_Regular);
                             if (frmSeriesSAP.Mode == BoFormMode.fm_UPDATE_MODE)
                                 frmSeriesSAP.GetItem("1").Click(BoCellClickType.ct_Regular);
                         }
                         go_SBOForm.Close();
                         go_SBOApplication.StatusBar.SetText("Operación finalizada con éxito..."
                        , BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                     }
                     catch { throw; }
                     finally { frmSeriesSAP.Freeze(false); }
                 }
             }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_CLICK, "btnValid", e =>
            {
                if (!e.BeforeAction)
                {
                    SAPbobsCOM.Recordset recSet = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    //_Application excel = new _excel.Application();
                    //Workbook wb = null;
                    //Worksheet ws = null;
                    //Range xlRange = null;
                    //List<ItemSeries> lstSrsSAP = new List<ItemSeries>();

                    //try
                    //{
                    //    go_SBOForm.GetItem("btnSlcSre").Enabled = false;
                    //    lstExlSeries.Clear();
                    //    var excelFilePath = go_SBOForm.DataSources.DBDataSources.Item("OADM").GetValue("ExcelPath", 0);
                    //    //if (string.IsNullOrWhiteSpace(excelFilePath)) throw new ArgumentNullException(nameof(excelFilePath));
                    //    wb = excel.Workbooks.Open(excelFilePath);
                    //    ws = wb.Worksheets[1];
                    //    xlRange = ws.UsedRange;
                    //    int rowCount = xlRange.Rows.Count;
                    //    int colCount = xlRange.Columns.Count;
                    //    //Se cargan las series a una lista
                    //    go_Matrix = go_SBOForm.GetMatrix("mtxClmns");
                    //    var clmExclItmCod = string.Empty;
                    //    var clmExclWhsCod = string.Empty;
                    //    var clmExclSrlNmb = string.Empty;
                    //    for (int i = 0; i < go_Matrix.RowCount; i++)
                    //    {
                    //        var mtxValue = ((ComboBox)go_Matrix.GetCellSpecific("clmFldID", i + 1)).Value;
                    //        if (mtxValue == "Número de Artículo")
                    //            clmExclItmCod = ((EditText)go_Matrix.GetCellSpecific("#", i + 1)).Value;
                    //        else if (mtxValue == "Código de almacén")
                    //            clmExclWhsCod = ((EditText)go_Matrix.GetCellSpecific("#", i + 1)).Value;
                    //        else if (mtxValue == "Número de serie")
                    //            clmExclSrlNmb = ((EditText)go_Matrix.GetCellSpecific("#", i + 1)).Value;
                    //    }
                    //    if (string.IsNullOrWhiteSpace(clmExclItmCod) || string.IsNullOrWhiteSpace(clmExclWhsCod) || string.IsNullOrWhiteSpace(clmExclSrlNmb))
                    //        throw new InvalidOperationException("Debe indicar las columnas del excel a procesar");

                    //    for (int i = 0; i < rowCount; i++)
                    //    {
                    //        var itemCode = ((Range)xlRange.Cells[i + 1, clmExclItmCod]).Value2?.ToString();
                    //        var whsCode = ((Range)xlRange.Cells[i + 1, clmExclWhsCod]).Value2?.ToString();
                    //        var serialNumber = ((Range)xlRange.Cells[i + 1, clmExclSrlNmb]).Value2?.ToString();
                    //        if (string.IsNullOrWhiteSpace(itemCode) || string.IsNullOrWhiteSpace(whsCode)
                    //        || string.IsNullOrWhiteSpace(serialNumber)) continue;
                    //        lstExlSeries.Add(new ItemSeries
                    //        {
                    //            ItemCode = itemCode,
                    //            WhsCode = whsCode,
                    //            SerialNumber = serialNumber
                    //        });
                    //    }
                    //    if (lstExlSeries.Count == 0)
                    //        go_SBOApplication.StatusBar.SetText("No fue posible extraer ningún dato desde el archivo excel"
                    //            , BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                    //    else
                    //    {
                    //        var SBDR = frmSeriesSAP.DataSources.DBDataSources.Item("SBDR");
                    //        for (int i = 0; i < SBDR.Size; i++)
                    //        {
                    //            var itemCode = SBDR.GetValue("ItemCode", i).Trim();
                    //            var whsCode = SBDR.GetValue("WhseCode", i).Trim();
                    //            var qry = @"select ""IntrSerial"",""ItemCode"",""WhsCode"" from OSRI TT3 where TT3.""ItemCode""
                    //            = '{0}' AND TT3.""WhsCode"" = '{1}' and TT3.""Status"" = '0'";
                    //            qry = string.Format(qry, itemCode, whsCode);
                    //            recSet.DoQuery(qry);
                    //            lstSrsSAP.AddRange(recSet.ToList(dc =>
                    //           {
                    //               return new ItemSeries
                    //               {
                    //                   ItemCode = dc["ItemCode"],
                    //                   WhsCode = dc["WhsCode"],
                    //                   SerialNumber = dc["IntrSerial"]
                    //               };
                    //           }));
                    //        }
                    //        var lstSrsNoSap = lstExlSeries.Except(lstSrsSAP).ToList();
                    //        if (lstSrsNoSap.Count > 0)
                    //        {
                    //            var msgSrsNoSAP = "Las siguientes series no fueron encontradas en sap: \n";
                    //            lstSrsNoSap.ForEach(s =>
                    //            {
                    //                msgSrsNoSAP += $"Articulo Nro:{s.ItemCode} ,Almacén :{s.WhsCode} ,Serie:{s.SerialNumber}\n";
                    //            });
                    //            go_SBOApplication.MessageBox(msgSrsNoSAP, 1, "OK");
                    //        }
                    //        else
                    //            go_SBOForm.GetItem("btnSlcSre").Enabled = true;
                    //    }
                    //}
                    //catch { throw; }
                    //finally
                    //{
                    //    wb?.Close(0);
                    //    excel.Application.Quit();
                    //    excel?.Quit();

                    //    if (ws != null) Marshal.ReleaseComObject(wb);
                    //    if (ws != null) Marshal.ReleaseComObject(ws);
                    //    if (excel != null) Marshal.ReleaseComObject(excel);

                    //    GC.Collect();
                    //    GC.WaitForPendingFinalizers();
                    //}
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, e =>
            {
                if (e.BeforeAction)
                {
                    go_SBOForm = null;
                    this.Dispose();
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "btnRtaExcl", e =>
             {
                 if (e.BeforeAction)
                 {
                     var rutaArchivo = Cls_Global.fn_WindowDialog("xls");
                     go_SBOForm.DataSources.DBDataSources.Item("OADM").SetValue("ExcelPath", 0, rutaArchivo);
                 }
             }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_CLICK, "mtxClmns", e =>
            {
                if (e.ColUID.Equals("clmFldID"))
                {
                    if (e.BeforeAction)
                    {
                        go_SBOForm.Freeze(true);
                        go_Matrix = go_SBOForm.GetMatrix("mtxClmns");
                        go_Combo = go_Matrix.GetCellSpecific("clmFldID", e.Row);
                        while (go_Combo.ValidValues.Count > 1)
                            go_Combo.ValidValues.Remove(1, BoSearchKey.psk_Index);
                        go_Combo.ValidValues.Add("Número de Artículo", "Número de Artículo");
                        go_Combo.ValidValues.Add("Código de almacén", "Código de almacén");
                        go_Combo.ValidValues.Add("Número de serie", "Número de serie");
                        go_Combo.ExpandType = BoExpandType.et_DescriptionOnly;
                        string[] arrSlcValues = new string[3];
                        for (int i = 0; i < go_Matrix.RowCount; i++)
                        {
                            var slcValue = ((ComboBox)go_Matrix.GetCellSpecific("clmFldID", i + 1)).Value;
                            if (string.IsNullOrWhiteSpace(slcValue)) continue;
                            go_Combo.ValidValues.Remove(slcValue, BoSearchKey.psk_ByValue);
                        }
                        go_SBOForm.Freeze(false);
                    }
                }
            }));
        }

        private class ItemSeries
        {
            public string ItemCode { get; set; }
            public string WhsCode { get; set; }
            public string SerialNumber { get; set; }

            public override bool Equals(object obj)
            {
                var itemSeries = obj as ItemSeries;
                if (itemSeries == null) return false;
                return ItemCode == itemSeries.ItemCode && WhsCode == itemSeries.WhsCode && SerialNumber == itemSeries.SerialNumber;
            }

            public override int GetHashCode()
            {
                return ItemCode.GetHashCode() ^ WhsCode.GetHashCode() ^ SerialNumber.GetHashCode();
            }
        }
    }
}