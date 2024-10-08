using SAPbobsCOM;
using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;
using System.Collections.Generic;

namespace STR_Localizacion.UI
{
    public class Cls_NumeracionBase : Cls_PropertiesControl
    {
        public List<string> go_InternalFormID;
        public List<string> go_MenuID;
        protected string ls_tablename;
        public bool isInitializingComboBox = false;

        public Cls_NumeracionBase()
        {
            itemevent.Add(BoEventTypes.et_COMBO_SELECT,
               //new sapitemevent("U_BPP_MDSD", s =>
               //{
               //    if (!s.BeforeAction)
               //    {
               //        go_SBOForm = go_SBOFormEvent;
               //        fn_obtCorrelativoXSerie();
               //    }
               //}),
               new sapitemevent("cbSerie", s =>
               {
                   if (!s.BeforeAction)
                   {
                       go_SBOForm = go_SBOFormEvent;
                       fn_obtCorrelativoXSerie();
                   }
               }),
               new sapitemevent("cbTpoTrnf", s =>
               {
                   if (!s.BeforeAction)
                   {
                       go_SBOForm = go_SBOFormEvent;
                       fn_MostrarTipoSerieNumero();
                   }
               }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, s => Dispose()));
        }

        /// <summary>
        /// Solo se usa en compras y ventas
        /// </summary>
        /// <param name="prm_Top"></param>
        /// <param name="prm_Left"></param>
        /// <param name="prm_Height"></param>
        /// <param name="prm_LeftSt"></param>
        protected void InitializeControl(out int prm_Top, out int prm_Left, out int prm_Height, out int prm_LeftSt)
        {
            prm_Top = 0;
            prm_Left = 0;
            prm_Height = 0;
            prm_LeftSt = 0;

            lc_NameMethod = "sb_crearControlNumeracion"; // Se asigna el nombre del método para la identificación del mismo

            try
            {
                // Congelar la pantalla para evitar redibujos mientras se realizan cambios
                go_SBOForm.Freeze(true);

                // Obtener referencias de los ítems necesarios
                SAPbouiCOM.Item item70 = go_SBOForm.GetItem("70");
                SAPbouiCOM.Item item3 = go_SBOForm.GetItem("3");
                SAPbouiCOM.Item item15 = go_SBOForm.GetItem("15");
                SAPbouiCOM.Item item5 = go_SBOForm.GetItem("5");
                SAPbouiCOM.Item item14 = go_SBOForm.GetItem("14");

                // Calcular las posiciones y tamaños
                prm_Top = item70.Top + item70.Height + 1;
                prm_Left = item3.Left;
                prm_Height = item15.Height;
                prm_LeftSt = item5.Left;

                // Ocultar controles de Número de folio
                go_SBOForm.SetVisibleControl(false, "84", "208", "210", "211");

                // Crear y configurar el ComboBox "cbTipo"
                CrearComboBox("cbTipo", 76, prm_Height, prm_Top, item14.Left - 60, true, true, true, ls_tablename, "U_BPP_MDTD");

                // Crear y configurar el StaticText "stNum"
                CrearStaticText("stNum", "N° Doc", 76, prm_Height, prm_Top, prm_LeftSt, true, true);
            }
            catch (Exception ex)
            {
                //Cls_Global.WriteToFile(ex.Message);
                //ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                //ExceptionPrepared.SaveInLog(false);
            }
            finally
            {
                // Descongelar la pantalla después de hacer los cambios
                go_SBOForm.Freeze(false);
            }
        }

        // Método auxiliar para crear y configurar un ComboBox
        private void CrearComboBox(string itemId, int width, int height, int top, int left, bool enabled, bool visible, bool show, string tableName, string dataField)
        {
            SAPbouiCOM.Item go_Item = go_SBOForm.Items.Add(itemId, BoFormItemTypes.it_COMBO_BOX);
            go_Item.SetDisplay(width, height, top, left);
            go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 9, BoModeVisualBehavior.mvb_False);
            go_Item.Enabled = enabled;
            go_Item.Visible = visible;
            SAPbouiCOM.ComboBox lo_ComboTipo = go_Item.Specific;
            lo_ComboTipo.DataBind.SetBound(true, tableName, dataField);
        }

        // Método auxiliar para crear y configurar un StaticText
        private void CrearStaticText(string itemId, string caption, int width, int height, int top, int left, bool enabled, bool visible)
        {
            SAPbouiCOM.Item go_Item = go_SBOForm.Items.Add(itemId, BoFormItemTypes.it_STATIC);
            go_Item.SetDisplay(width, height, top, left);
            go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 9, BoModeVisualBehavior.mvb_False);
            go_Item.Enabled = enabled;
            go_Item.Visible = visible;
            SAPbouiCOM.StaticText go_Static = go_Item.Specific;
            go_Static.Caption = caption;
        }

        /// <Actualiza la Serie Sunat>
        /// Primero carga el recordset ejecutando el script desde el Resource SQL o Hana.
        /// Luego carga el combo pasando como parametro el recordset antes cargado.
        /// </>
        protected void fn_actualizarSerieSUNAT()
        {
            GC.Collect(); //Libera la memoria
            string formCancelacion = string.Empty;
            bool esCancelado = false;
            string estado = "1";

            // Validar si el formulario es uno de cancelación
            formCancelacion = go_SBOForm.DataSources.DBDataSources.Item(0).GetValue("CANCELED", 0);

            try
            {
                estado = go_SBOForm.GetComboBox("81").Value;
            }
            catch (Exception)
            {

            }

            if (!string.IsNullOrEmpty(formCancelacion))
                esCancelado = formCancelacion == "C";

            string ls_ComboTipo;
            Recordset lo_recordSet;
            ComboBox
                lo_comboTipo = go_SBOForm.GetControl("cbTipo"),
                lo_comboSerie = go_SBOForm.GetControl("cbSerie");



            go_SBOForm.GetEditText("txNumero").Value = string.Empty;

            ls_ComboTipo = lo_comboTipo.Value.Trim();
            string usuario = go_SBOCompany.UserName;
            lo_recordSet = Cls_QueryManager.Retorna(Cls_Query.get_SerieXTipo, null, ls_ComboTipo, usuario);
            Cls_Global.sb_comboLlenar(lo_comboSerie, lo_recordSet);

            if (lo_recordSet.RecordCount > 0)
            {
                if (!esCancelado && estado != "6")
                {
                    lo_comboSerie.Select(0, BoSearchKey.psk_Index);
                }
            }
            else
                Cls_Global.sb_msjStatusBarSAP("No se ha configurado una serie de numeración para este tipo de documento SUNAT.", BoStatusBarMessageType.smt_Warning, go_SBOApplication);
        
    

            //if (!esCancelado && estado != "6")
            //{

            //    go_SBOForm.GetEditText("txNumero").Value = string.Empty;

            //    ls_ComboTipo = lo_comboTipo.Value.Trim();
            //    string usuario = go_SBOCompany.UserName;
            //    lo_recordSet = Cls_QueryManager.Retorna(Cls_Query.get_SerieXTipo, null, ls_ComboTipo, usuario);
            //    Cls_Global.sb_comboLlenar(lo_comboSerie, lo_recordSet);

            //    if (lo_recordSet.RecordCount > 0)
            //        lo_comboSerie.Select(0, BoSearchKey.psk_Index);
            //    else
            //        Cls_Global.sb_msjStatusBarSAP("No se ha configurado una serie de numeración para este tipo de documento SUNAT.", BoStatusBarMessageType.smt_Warning, go_SBOApplication);
            //}
            //else {
            //    go_SBOForm.GetEditText("txNumero").Value = string.Empty;
            //    ls_ComboTipo = lo_comboTipo.Value.Trim();
            //}
        }
        protected void fn_actualizarSerieSUNATRd()
        {
            GC.Collect(); //Libera la memoria

            string ls_ComboTipo;
            Recordset lo_recordSet;
            ComboBox
                lo_comboTipo = go_SBOForm.GetControl("U_BPP_MDTD"),
                lo_comboSerie = go_SBOForm.GetControl("U_BPP_MDSD");


                go_SBOForm.GetEditText("U_BPP_MDCD").Value = string.Empty;

                ls_ComboTipo = lo_comboTipo.Value.Trim();
                string usuario = go_SBOCompany.UserName;
                lo_recordSet = Cls_QueryManager.Retorna(Cls_Query.get_SerieXTipo, null, ls_ComboTipo, usuario);
                Cls_Global.sb_comboLlenar(lo_comboSerie, lo_recordSet);

                if (lo_recordSet.RecordCount > 0)
                    lo_comboSerie.Select(0, BoSearchKey.psk_Index);
                else
                    Cls_Global.sb_msjStatusBarSAP("No se ha configurado una serie de numeración para este tipo de documento SUNAT.", BoStatusBarMessageType.smt_Warning, go_SBOApplication);
        }

        /// <Obtiene correlativo de la serie>
        /// Primero carga el recordset ejecutando el script desde el Resource SQL o Hana.
        /// ubica el ultimo correlativo y captura el correlativo siguiente.
        /// Finalmente verfica que dicho correlativo no este anulado, de ser asi continua con el correlativo siguiente.
        /// </>
        protected void fn_obtCorrelativoXSerie()
        {
            string ls_ComboTipo, ls_ComboSerie;
            bool lb_IndCorrAnl = false;

            string ls_CorrDoc = string.Empty;
            int li_CorrDoc;

            GC.Collect(); //Libera la memoria
            go_Combo = go_SBOForm.GetItem("cbTipo").Specific;
            ls_ComboTipo = go_Combo.Value.ToString().Trim();
            go_Combo = go_SBOForm.GetItem("cbSerie").Specific;
            ls_ComboSerie = go_Combo.Value.ToString().Trim();
            go_Edit = go_SBOForm.GetItem("txNumero").Specific;

            /// Se cargan los datos en el recordset usando el script del recurso.
            go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_CorrelativoTipoSerie, null, ls_ComboTipo, ls_ComboSerie);
            if (go_RecordSet.RecordCount > 0)
            {
                /// Setea el valor del último número correlativo
                go_Edit.Value = go_RecordSet.Fields.Item("U_BPP_NDCD").Value;
                /// Captura el valor del correlativo siguiente
                ls_CorrDoc = go_Edit.Value;
            }

            ///verifica si el numero correlativo esta anulado, si es asi obtiene el correlativo habil siguiente.
            while (!lb_IndCorrAnl)
            {
                /// Se cargan los datos en el recordset usando el script desde el recurso.
                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_CorrelativoAnulado, null, ls_ComboTipo, ls_ComboSerie, ls_CorrDoc);
                if (go_RecordSet.RecordCount > 0)
                {
                    li_CorrDoc = int.Parse(ls_CorrDoc); li_CorrDoc++; ls_CorrDoc = (li_CorrDoc).ToString();
                    for (int li_j = li_CorrDoc.ToString().Length; li_j <= (go_Edit.Value.Length - 1); li_j++)
                    {
                        ls_CorrDoc = "0" + ls_CorrDoc;
                    }
                    lb_IndCorrAnl = false;
                }
                else
                    lb_IndCorrAnl = true;
            }
            go_Edit = go_SBOForm.GetItem("txNumero").Specific;
            go_Edit.Value = ls_CorrDoc;
        }
        protected void fn_obtCorrelativoXSerieRd()
        {
            string ls_ComboTipo, ls_ComboSerie;
            bool lb_IndCorrAnl = false;

            string ls_CorrDoc = string.Empty;
            int li_CorrDoc;

            GC.Collect(); //Libera la memoria
            go_Combo = go_SBOForm.GetItem("U_BPP_MDTD").Specific;
            ls_ComboTipo = go_Combo.Value.ToString().Trim();
            go_Combo = go_SBOForm.GetItem("U_BPP_MDSD").Specific;
            ls_ComboSerie = go_Combo.Value.ToString().Trim();
            go_Edit = go_SBOForm.GetItem("U_BPP_MDCD").Specific;

            /// Se cargan los datos en el recordset usando el script del recurso.
            go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_CorrelativoTipoSerie, null, ls_ComboTipo, ls_ComboSerie);
            if (go_RecordSet.RecordCount > 0)
            {
                /// Setea el valor del último número correlativo
                go_Edit.Value = go_RecordSet.Fields.Item("U_BPP_NDCD").Value;
                /// Captura el valor del correlativo siguiente
                ls_CorrDoc = go_Edit.Value;
            }

            ///verifica si el numero correlativo esta anulado, si es asi obtiene el correlativo habil siguiente.
            while (!lb_IndCorrAnl)
            {
                /// Se cargan los datos en el recordset usando el script desde el recurso.
                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_CorrelativoAnulado, null, ls_ComboTipo, ls_ComboSerie, ls_CorrDoc);
                if (go_RecordSet.RecordCount > 0)
                {
                    li_CorrDoc = int.Parse(ls_CorrDoc); li_CorrDoc++; ls_CorrDoc = (li_CorrDoc).ToString();
                    for (int li_j = li_CorrDoc.ToString().Length; li_j <= (go_Edit.Value.Length - 1); li_j++)
                    {
                        ls_CorrDoc = "0" + ls_CorrDoc;
                    }
                    lb_IndCorrAnl = false;
                }
                else
                    lb_IndCorrAnl = true;
            }
            go_Edit = go_SBOForm.GetItem("U_BPP_MDCD").Specific;
            go_Edit.Value = ls_CorrDoc;
        }

        /// <Muestra u oculta los controles de numeración de Inventario>
        /// Segun sea el tipo de salida (Externa o Interna)
        /// muestra u oculta los controles del tipo, serie y número.
        /// </>
        protected void fn_MostrarTipoSerieNumero()
        {
            go_Combo = go_SBOForm.GetItem("cbTpoTrnf").Specific;
            //Pregunta por el tipo de serie
            if (go_Combo.Value == "TSE")
            {   //Muestra los controles
                go_SBOForm.SetVisibleControl(true, "stNum", "cbTipo", "cbSerie", "txNumero");
                go_Combo = go_SBOForm.GetItem("cbTipo").Specific;

                /// Asignar tipo de documento SUNAT por Defecto
                this.sb_asignarTipoSunat();
                this.sb_asignarTipoXDefecto();
            }
            else if (go_Combo.Value == "TSI")
            {
                if (go_SBOForm.TypeEx == "720")
                {
                    go_Edit = go_SBOForm.GetItem("7").Specific;
                    go_Edit.Value = go_Edit.Value;
                }
                else
                {
                    go_Edit = go_SBOForm.GetItem("22").Specific;
                    go_Edit.Value = go_Edit.Value;
                }
                //Oculta los controles
                go_SBOForm.SetVisibleControl(false, "stNum", "cbTipo", "cbSerie", "txNumero");
            }
        }

        /// <Asigna el tipo Sunat>
        /// Primero carga el recordset ejecutando el script desde el Resource SQL o Hana.
        /// Luego carga el combo pasando como parametro el recordset antes cargado.
        /// </>
        //protected void sb_asignarTipoSunat()
        //{
        //    lc_NameMethod = "sb_asignarTipoSunat";
        //    try
        //    {
        //        isInitializingComboBox = true;
        //        // Recuperar el recordset
        //        go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_TipoXFormulario, null, go_SBOForm.TypeEx);

        //        // Llenar todos los controles en un solo recorrido del Recordset
        //        if (go_RecordSet != null && go_RecordSet.RecordCount > 0)
        //        {
        //            // Crear listas temporales para los valores a agregar
        //            var tipos = new List<Tuple<string, string, string>>();
        //            var series = new List<Tuple<string, string>>();
        //            string numero = null;

        //            while (!go_RecordSet.EoF)
        //            {
        //                string _type = go_RecordSet.Fields.Item(0).Value;
        //                string _a = go_RecordSet.Fields.Item(1).Value;
        //                string _b = go_RecordSet.Fields.Item(2).Value;
        //                string _c = go_RecordSet.Fields.Item(3).Value;

        //                if (_type == "TPO")
        //                {
        //                    tipos.Add(new Tuple<string, string, string>(_a, _b, _c));
        //                }
        //                else if (_type == "SER")
        //                {
        //                    series.Add(new Tuple<string, string>(_b, _b));
        //                }
        //                else if (_type == "COR")
        //                {
        //                    numero = _b;
        //                }

        //                go_RecordSet.MoveNext();
        //            }

        //            // Llenar ComboBox y EditText con los datos recopilados
        //            Cls_Global.LlenarComboBox(go_SBOForm.GetComboBox("cbTipo"), tipos, false);
        //            Cls_Global.LlenarComboBox(go_SBOForm.GetComboBox("cbSerie"), series, false);
        //            go_SBOForm.GetEditText("txNumero").Value = numero;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Cls_Global.WriteToFile(ex.Message);
        //        ExceptionPrepared.inner(ex.Message, lc_NameMethod);
        //        ExceptionPrepared.SaveInLog(false);
        //    }
        //    finally {
        //        isInitializingComboBox = false; // Desactivar la bandera después de terminar
        //    }
        //}

        /// <Asigna el tipo por Defecto>
        /// Primero carga el recordset ejecutando el script desde el Resource SQL o Hana.
        /// Luego selecciona el item inicial para mostrase en el combo anteriormente cargado.
        /// </>
        protected void sb_asignarTipoSunat()
        {
            lc_NameMethod = "sb_asignarTipoSunat"; //Se asigna el nombre del método para la identificación del mismo
            try
            {
                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_TipoXFormulario, null, go_SBOForm.TypeEx);
                Cls_Global.sb_comboLlenar(go_SBOForm.GetComboBox("cbTipo"), go_RecordSet);
            }
            catch (Exception ex) { Cls_Global.WriteToFile(ex.Message); ExceptionPrepared.inner(ex.Message, lc_NameMethod); ExceptionPrepared.SaveInLog(false); } //Método para el manejo de las operaciones de Log
        }
        protected void sb_asignarTipoXDefecto()
        {
            lc_NameMethod = "sb_asignarTipoXDefecto"; //Se asigna el nombre del método para la identificación del mismo
            try
            {
                go_SBOForm = go_SBOFormEvent;
                string ls_ByValue = Cls_QueryManager.Retorna(Cls_Query.get_TipoXDefecto, (int)0, go_SBOForm.TypeEx);

                if (!string.IsNullOrEmpty(ls_ByValue))
                    go_SBOForm.GetComboBox("cbTipo").Select(ls_ByValue, BoSearchKey.psk_ByValue);
            }
            catch (Exception ex) { Cls_Global.WriteToFile(ex.Message);  ExceptionPrepared.inner(ex.Message, lc_NameMethod); ExceptionPrepared.SaveInLog(false); } //Método para el manejo de las operaciones de Log
        }
    }
}