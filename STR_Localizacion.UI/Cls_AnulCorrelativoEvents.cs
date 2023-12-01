using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;

namespace STR_Localizacion.UI
{
    partial class Cls_AnulCorrelativo
    {
        private void InitializeEvents()
        {
            menuevent.Add(new sapmenuevent("1282", s =>
            {
                //Carga datos en el comboBox
                go_SBOForm = go_SBOFormEvent;
                go_Combo = go_SBOForm.GetControl("cboSeries");
                go_Combo.ValidValues.LoadSeries(go_SBOForm.BusinessObject.Type, BoSeriesMode.sf_Add);

                //Si en comboBox no se ha seleccionado ningún item, pero éste tiene datos, se asigna por defecto el item de índice 0, 
                if (go_Combo.Selected == null && go_Combo.ValidValues.Count > 0)
                    Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, "@BPP_ANULCORR", "Series", go_Combo.ValidValues.Item(0).Value, 0);
                else
                    serie = go_Combo.Selected.Value;

                //Asigna el número siguiente de la serie actual en la tabla de Anulaccón de correlativo
                Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, "@BPP_ANULCORR", "DocNum", go_SBOForm.BusinessObject.GetNextSerialNumber(serie, go_SBOForm.BusinessObject.Type).ToString(), 0);
                //Asigna valores al editText del formulario
                go_SBOForm.GetEditText("edtTpDoc").Value = "Venta";
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "1", s =>
            {
                if (go_SBOFormEvent.Mode == BoFormMode.fm_ADD_MODE) //Pregunta por el modo del formulario
                {
                    go_SBOForm = go_SBOFormEvent;
                    if (s.BeforeAction)
                        sb_DetalleAnulacion(); //Ingresa el detalle en el formulario
                    else
                    {
                        //Carga valores en el ComboBox
                        go_Combo = go_SBOForm.Items.Item("cboSeries").Specific;
                        go_Combo.ValidValues.LoadSeries(go_SBOForm.BusinessObject.Type, BoSeriesMode.sf_Add);
                        if (go_Combo.Selected == null && go_Combo.ValidValues.Count > 0)
                            Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, "@BPP_ANULCORR", "Series", go_Combo.ValidValues.Item(0).Value, 0);
                        else
                            serie = go_Combo.Value;
                        Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, "@BPP_ANULCORR", "DocNum", go_SBOForm.BusinessObject.GetNextSerialNumber(serie, go_SBOForm.BusinessObject.Type).ToString(), 0);
                        go_Edit = go_SBOForm.Items.Item("edtTpDoc").Specific;
                        go_Edit.Value = "Venta";
                    }
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_COMBO_SELECT, "cbTpSUNAT", s =>
                {
                    if (!s.BeforeAction) {
                        go_SBOForm = go_SBOFormEvent;
                        fn_ActualizarSerieSunat();
                    } 
                }));

            itemevent.Add(BoEventTypes.et_CLICK,
                new sapitemevent("pnlGeneral", s => { if (!s.BeforeAction) go_SBOFormEvent.PaneLevel = 1; }),
                new sapitemevent("pnlDetalle", s => { if (!s.BeforeAction) go_SBOFormEvent.PaneLevel = 2; }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_LOST_FOCUS, "txFchAnul", s =>
                {
                    if (!s.BeforeAction)
                    {
                        go_SBOForm = go_SBOFormEvent;

                        string serie;
                        
                        if (Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, "@BPP_ANULCORR", "U_BPP_FchAnl") != string.Empty)
                        {
                            go_Combo = go_SBOForm.GetControl("cboSeries");
                            go_Edit = go_SBOForm.GetControl("txFchAnul");

                            if (go_Edit.Value != go_Combo.Selected.Description)
                            {
                                //Almacena el número de serie según los parámetros enviados
                                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_NumeroSerie, null,
                                    go_SBOForm.BusinessObject.Type,
                                    Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, "@BPP_ANULCORR", "U_BPP_FchAnl").Substring(0, 4));

                                //Válida que la serie haya sido creada
                                if (go_RecordSet.RecordCount == 0)
                                {
                                    go_SBOApplication.StatusBar.SetText("No existen series creadas", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                                    return;
                                }
                                //Llena el ValidValues global
                                go_Validvalues = go_Combo.ValidValues;

                                while (go_Validvalues.Count > 0)
                                {
                                    go_Validvalues.Remove(0, BoSearchKey.psk_Index);
                                }
                                while (!go_RecordSet.EoF)
                                {
                                    go_Validvalues.Add(go_RecordSet.Fields.Item("Series").Value, go_RecordSet.Fields.Item("SeriesName").Value);
                                    go_RecordSet.MoveNext(); //Pasa al siguiente registro del recordSet
                                }
                                go_Combo.Select(0, BoSearchKey.psk_Index);
                                //Recupera el número de serie, seleccionado en el formulario
                                serie = Cls_Global.sb_FormGetValueFromDBDataSource(go_SBOForm, "@BPP_ANULCORR", "Series");

                                //Se llena el RecordSet
                                go_RecordSet = Cls_QueryManager.Retorna(
                                    Cls_Query.get_NumeroSiguiente, null,
                                    go_SBOForm.BusinessObject.Type, serie.ToString());

                                Cls_Global.sb_FormSetValueToDBDataSource(go_SBOForm, "@BPP_ANULCORR", "DocNum", go_RecordSet.Fields.Item("NextNumber").Value.ToString(), 0);
                            }
                        }
                    }
                }));
            //Maneja el evento de Foco perdido)

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, s => Dispose()));
        }
    }
}
