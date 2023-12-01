using SAPbobsCOM;
using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;

namespace STR_Localizacion.UI
{
    public class Cls_FEFO : Cls_PropertiesControl
    {
        public Cls_FEFO()
        {
            lc_NameClass = "Cls_FEFO";
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "1", s =>
            {
                if (s.FormMode == (int)BoFormMode.fm_ADD_MODE && !s.BeforeAction)
                {
                    go_SBOForm = go_SBOFormActive;

                    int li_Est = 0;
                    Button lo_Button;
                    Column lo_column;

                    ///<Permite la matrix, luego ubicar la columna>
                    go_Matrix = go_SBOForm.Items.Item("4").Specific;
                    lo_column = go_Matrix.Columns.Item("15");

                    ///<Permite ordenar la columna de la matrix, en este caso la fecha de vencimiento>
                    lo_column.TitleObject.Sort(BoGridSortType.gst_Ascending);

                    go_Matrix = go_SBOForm.Items.Item("3").Specific;

                    for (int i = 0; i < go_Matrix.RowCount; i++)
                    {
                        ///Recorre la matrix principal en el formulario
                        go_Matrix.Columns.Item("3").Cells.Item(i + 1).Click();

                        ///<Activación de botones en tiempo de ejecución>
                        if (li_Est == 0)
                        {
                            li_Est = 1;
                            //Boton de Consumo
                            lo_Button = go_SBOForm.Items.Item("16").Specific;
                            lo_Button.Item.Click(BoCellClickType.ct_Regular);

                            //Boton a Actualizar
                            lo_Button = go_SBOForm.Items.Item("1").Specific;
                            if (!(lo_Button.Caption == "OK"))
                            {
                                //Si el boton esta en Caption "OK", entonces no ejectuta el evento Click
                                lo_Button.Item.Click(BoCellClickType.ct_Regular);
                            }
                            li_Est = 0;
                        }
                    }
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, s => Dispose()));
        }

        #region "Validacion"

        public bool fn_validaSN()
        {
            lc_NameMethod = "fn_validaSN"; //Se asigna el nombre del método para la identificación del mismo
            string ls_cardCode;
            try
            {
                go_SBOForm = go_SBOFormActive;
                if (go_SBOForm.TypeEx == "42")
                {
                    return true;
                }

                switch (go_SBOForm.TypeEx)
                {
                    case "720":
                    case "940":
                    case "65213":
                        return true;
                }

                //Declaración de variables
                Recordset lo_recordSet;

                //Inicializacion de objetos
                lo_recordSet = go_SBOCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                go_Edit = go_SBOForm.Items.Item("4").Specific;
                ls_cardCode = string.IsNullOrEmpty(go_Edit.Value) ? "N" : go_Edit.Value;

                //Retorna los valores que obtiene

                //Retorna el resultado de la consulta
                return !((string)Cls_QueryManager.Retorna(Cls_Query.get_SocioNegocioFefo, (int)0, ls_cardCode) == "N");
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                ExceptionPrepared.SaveInLog(false);
                return false;
            } //Método para el manejo de las operaciones de Log
        }

        #endregion "Validacion"
    }
}