using SAPbouiCOM;
using STR_Localizacion.UTIL;
using System;
using System.Collections.Generic;

namespace STR_Localizacion.UI
{
    public class Cls_Folio : Cls_PropertiesControl
    {
        private const string F1 = "65080";
        private const string F2 = "65081";

        public List<string> go_InternalFormID = new List<string> { F1, F2 };

        public Cls_Folio()
        {
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_LOAD, string.Empty, s =>
            {
                if (s.BeforeAction)
                {
                    go_SBOForm = go_SBOFormEvent;

                    if (F1 == s.FormType.ToString())
                    {
                        try
                        {   //Recupera los valores de los controles del Formulario
                            go_Edit = go_SBOForm.Items.Item("3").Specific;
                            go_Edit.Value = "1";
                            //go_SBOForm.SetEnabledControl(false, "3", "7");
                            go_SBOForm.Items.Item("7").Enabled = false;
                            go_SBOForm.Items.Item("3").Enabled = false;

                            //Obtiene los valores del Botón y simila un Click
                            go_Item = go_SBOForm.Items.Item("4");
                            go_Item.Click(BoCellClickType.ct_Regular);
                        }
                        catch (Exception)
                        {   //Agrega un nuevo Item tipo Button y asigna valores a sus propiedades
                            go_Item = go_SBOForm.Items.Add("txNo", BoFormItemTypes.it_EDIT);
                            go_Item.Left = 10;
                            go_Item.Top = 10;
                            go_Item.Width = 1;
                            go_Item.AffectsFormMode = false;
                            //Recupera los valores del EditText del Formulario
                            //?go_Edit = go_Item.Specific;
                            go_Item.Click();

                            //go_SBOForm.SetEnabledControl(false, "3", "7");
                            go_SBOForm.Items.Item("7").Enabled = false;
                            go_SBOForm.Items.Item("3").Enabled = false;

                            go_Item = go_SBOForm.Items.Item("4");
                            go_Item.Click();
                        }
                    }
                    else
                    {
                        go_Item = go_SBOForm.Items.Item("4");
                        go_Item.Click(BoCellClickType.ct_Regular);
                    }
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, s => Dispose()));
        }
    }
}