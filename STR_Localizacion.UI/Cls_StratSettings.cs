using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;

namespace STR_Localizacion.UI
{
    public class Cls_StratSettings : Cls_PropertiesControl
    {
        private const string ls_grabar = "btnGrabar";

        public Cls_StratSettings()
        {
            gs_FormName = "frmStratSettings";
            gs_FormPath = "Resources/Localizacion/StratSettings.srf";
            lc_NameClass = "Cls_StratSettings";
        }

        public void sb_FormLoad()
        {
            try
            {
                if (go_SBOForm == null)
                {
                    go_SBOForm = Cls_Global.fn_CreateForm(gs_FormName, gs_FormPath);
                    ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
                    sb_DataFormLoad();
                    InitializeEvents();
                }
            }
            catch (Exception exc)
            {
                go_SBOApplication.SetStatusBarMessage(exc.Message, BoMessageTime.bmt_Short, true);
            }
            finally { go_SBOForm.Visible = true; }
        }

        private void InitializeEvents()
        {
            itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, ls_grabar, s =>
            {
                if (!s.BeforeAction)
                {
                    string ls_params = string.Empty;
                    string ls_Concepto = string.Empty;
                    string ls_Estado = string.Empty;
                    for (int i = 0; i < go_Grid.Rows.Count; i++)
                    {
                        ls_Concepto = go_Grid.DataTable.GetValue("Concepto", i);
                        ls_Estado = go_Grid.DataTable.GetValue("Estado", i);
                        Cls_QueryManager.Procesa(Cls_Query.update_StratSettings, ls_Estado, ls_Concepto);
                    }

                    go_SBOApplication.StatusBar.SetText("Configuración actualizada correctamente.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, s => Dispose()));
        }

        public void sb_DataFormLoad()
        {
            lc_NameMethod = "sb_DataFormLoad";

            try
            {
                go_SBOForm.GetGrid("gdCom").DataTable.Consulta(Cls_Query.get_Configuracion);

                go_EditColumn = (EditTextColumn)go_Grid.Columns.Item("Estado");
                go_EditColumn.Type = BoGridColumnType.gct_CheckBox;
                go_EditColumn.Editable = true;
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                ExceptionPrepared.SaveInLog(false);
            }
        }

        /// <Consulta la configuración general del addon complementario de STRAT>
        /// Devuelve el valor de la configuración Strat
        /// </>
        /// <param name="ps_cardCode"></param>
        /// <param name="po_SBOCompany"></param>
        public bool fn_StratSettings(string ps_Code)
        {
            lc_NameMethod = "fn_StratSettings"; //Se asigna el nombre del método para la identificación del mismo
            //string ls_cardCode;
            try
            {
                go_SBOForm = go_SBOFormActive;
                return Cls_QueryManager.Retorna(Cls_Query.validate_StratSettings, 0, ps_Code) == "Y";
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod); ExceptionPrepared.SaveInLog(false);
                return false;
            } //Método para el manejo de las operaciones de Log
        }
    }
}