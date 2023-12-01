using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace STR_Localizacion.UI
{
    public class Cls_ListaMateriales : Cls_PropertiesControl
    {
        public List<string> go_InternalFormID = new List<string>() { "65211" };

        public Cls_ListaMateriales()
        {
            lc_NameClass = "Cls_ListaMateriales";
            ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            menuevent.Add(new sapmenuevent(
                s => sb_cargarcontroles(s), "2561"));

            menuevent.Add(new sapmenuevent(s => sb_cargarcontroles(s), "1282"));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_LOAD, string.Empty,
                s => sb_cargarcontroles(s)));
            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, s => Dispose()));

            itemevent.Add(new sapitemevent(BoEventTypes.et_CHOOSE_FROM_LIST, "78", e =>
            {
                if (go_SBOForm is null) go_SBOForm = go_SBOFormActive;

                try
                {
                    go_SBOForm.Freeze(true);
                    var cflEvnt = (SAPbouiCOM.ChooseFromListEvent)e;
                    if (!cflEvnt.BeforeAction && cflEvnt.SelectedObjects is DataTable dtbl)
                    {
                        var WhsCode = dtbl.GetValue("WhsCode", 0);
                        var Matrix = go_SBOForm.GetMatrix("37"); //go_SBOForm.Items.Item("37").Specific; //
                        var ItemCode = go_SBOForm.GetEditText("6").Value;
                        var itemLine = string.Empty;

                        Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(2000);
                            /*do //ESTA SECCION LIMPIA LA MATRIX, ES UN CAMBIO SOLICITADO POR GAVILON, QUE HASTA EL MOMENTO NO ES VALIDADO - ESTA PENDIENTE POR SOPORTE 
                            {
                                Matrix.Columns.Item("0").Cells.Item(1).Click(BoCellClickType.ct_Regular);
                                itemLine = ((SAPbouiCOM.EditText)Matrix.GetCellSpecific("4", 1)).Value;
                                if (go_SBOApplication.Menus.Item("1293").Enabled) go_SBOApplication.ActivateMenuItem("1293");

                            } while (Matrix.RowCount > 1 && !string.IsNullOrWhiteSpace(itemLine));*/

                            int li_record = 1;
                            go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_ListaMateriales, null, ItemCode, WhsCode);
                            while (!go_RecordSet.EoF)
                            {
                                try
                                {
                                    ((SAPbouiCOM.EditText)Matrix.GetCellSpecific("4", li_record)).Value = go_RecordSet.Fields.Item("U_STR_ItemCode").Value.ToString();
                                    ((SAPbouiCOM.EditText)Matrix.GetCellSpecific("10", li_record)).Value = go_RecordSet.Fields.Item("U_STR_WhsCode").Value.ToString();
                                    ((SAPbouiCOM.EditText)Matrix.GetCellSpecific("14", li_record)).Value = go_RecordSet.Fields.Item("U_STR_Quantity").Value.ToString();

                                    li_record++;
                                    go_RecordSet.MoveNext();
                                }
                                catch (Exception ex2)
                                {
                                    if (ex2.Message.Contains("Form"))
                                        go_SBOApplication.Forms.ActiveForm.GetItem("1").Click(BoCellClickType.ct_Regular);
                                }
                            }
                        });
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally { go_SBOForm.Freeze(false); }
               
            }));
                       
        }


        private void sb_cargarcontroles(dynamic po_Event)
        {
            if (!po_Event.BeforeAction)
            {/*
                try
                {
                    go_SBOForm = go_SBOFormEvent;
                    go_SBOForm.Freeze(true);
                    string ls_FormTypeEx = go_SBOForm.TypeEx;

                    if (go_SBOForm.Mode == BoFormMode.fm_ADD_MODE || go_SBOForm.Mode == BoFormMode.fm_UPDATE_MODE || go_SBOForm.Mode == BoFormMode.fm_OK_MODE || go_SBOForm.Mode == BoFormMode.fm_EDIT_MODE)
                    {
                        if (ls_FormTypeEx.Equals("65211"))
                        {
                            if (go_SBOForm.ItemExists("btnListar"))
                                return;

                            //go_SBOForm.PaneLevel = 7;

                            go_Item = go_SBOForm.GetItem("41");
                            go_Item.Width = go_SBOForm.GetItem("41").Width - 40;

                            //Boton Listar
                            go_Item = go_SBOForm.Items.Add("btnListar", BoFormItemTypes.it_BUTTON);
                            go_Item.Width = 37;
                            go_Item.Height = go_SBOForm.GetItem("41").Height;
                            go_Item.SetPosition(
                                go_SBOForm.GetItem("30").Top,
                                go_SBOForm.GetItem("41").Left + go_SBOForm.GetItem("41").Width + 5);
                            go_Button = go_Item.Specific;
                            go_Button.Caption = "Listar";

                            //go_SBOForm.PaneLevel = 1;
                        }
                    }
                }
                finally { go_SBOForm.Freeze(false); }
                */
            }
        }
    }
}