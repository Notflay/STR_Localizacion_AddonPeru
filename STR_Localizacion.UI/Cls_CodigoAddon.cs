using SAPbouiCOM;
using STR_Localizacion.BL;
using STR_Localizacion.UTIL;
using System;
using System.IO;

namespace STR_Localizacion.UI
{
    public class Cls_CodigoAddon : Cls_PropertiesControl
    {
        private string gs_TxtKey = "txtKey";
        private string gs_BtnOk = "btnOK";
        private string gs_BtnOpnFile = "btnOpnFile";
        private string gs_BtnCancel = "2";
        public bool gb_Estado = false;

        public Cls_CodigoAddon()
        {
            gs_FormName = "LOC_FRM_COD_ADDON";
            gs_FormPath = "Resources/Localizacion/CodigoAddon.srf";
            lc_NameClass = "Cls_CodigoAddon";
            ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
            InitializeEvents();
        }

        ///<Declaración del constructor>
        ///Se hace uso del contructor para no perder los valores del Company de Clase Global
        ///<param name="lo_Cls_Global"></param>
        ///</>

        public void sb_FormLoad(string ps_Tipo)
        {
            try
            {
                if (go_SBOForm == null)
                {
                    go_SBOForm = Cls_Global.fn_CreateForm(gs_FormName, gs_FormPath);
                    InitializeEvents();
                }
            }
            catch (Exception ex)
            {
                go_SBOApplication.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Short, true);
            }
            finally
            {
                go_SBOForm.Visible = true;
            }
        }

        private void InitializeEvents()
        {
            itemevent.Add(BoEventTypes.et_ITEM_PRESSED,
                new sapitemevent(gs_BtnOk, s =>
                {
                    if (!s.BeforeAction)
                    {
                        go_Edit = go_SBOForm.Items.Item(gs_TxtKey).Specific;
                        Cls_CodigoAddon_BL.gs_cadena = go_Edit.Value;
                        if (Cls_CodigoAddon_BL.fn_VerificarCodigoAddOn())
                        {
                            gb_Estado = true;
                            go_SBOForm.Items.Item(gs_BtnCancel).Click();
                            System.Windows.Forms.Application.Restart();
                        }
                    }
                }),
                new sapitemevent(gs_BtnOpnFile, s =>
                {
                    if (!s.BeforeAction)
                        fn_MostrarVentanaBusqueda();
                }),

                new sapitemevent(gs_BtnCancel, s =>
                    {
                        if (s.BeforeAction)
                            if (!gb_Estado)
                                System.Windows.Forms.Application.Exit();
                    })
                );

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, s => Dispose()));
        }

        public bool IniciarAddon()
        {
            sb_GetHardwareKey();

            if (!Cls_CodigoAddon_BL.sb_IniciarAddOn())
            {
                if (Cls_CodigoAddon_BL.gs_hadkey == string.Empty)
                    go_SBOApplication.StatusBar.SetText("Ingrese el código del AddOn", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                else
                    go_SBOApplication.StatusBar.SetText("El código del AddOn es incorrecto", BoMessageTime.bmt_Short);

                try { go_SBOForm = go_SBOApplication.Forms.GetForm(gs_FormName, 0); }
                catch (Exception) { GC.Collect(); }

                if (go_SBOForm != null)
                    return false;

                System.Xml.XmlDocument lo_XMLForm = null;
                FormCreationParams lo_FrmCrtPrms = null;
                try
                {
                    lo_XMLForm = new System.Xml.XmlDocument();
                    lo_FrmCrtPrms = go_SBOApplication.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
                    lo_XMLForm.Load(gs_FormPath);
                    lo_FrmCrtPrms.XmlData = lo_XMLForm.InnerXml;
                    lo_FrmCrtPrms.FormType = gs_FormName;
                    lo_FrmCrtPrms.UniqueID = gs_FormName;

                    this.go_SBOForm = go_SBOApplication.Forms.AddEx(lo_FrmCrtPrms);
                }
                catch (Exception ex)
                {
                    go_SBOApplication.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                }
                finally { lo_XMLForm = null; lo_FrmCrtPrms = null; }

                return false;
            }
            return true;
        }

        #region Métodos privados

        /// <Obtiene HardwareKey>
        /// Aqui levanta el formulario de "Acerca de BO" de sap y tomamos el HK desde la ventana.
        /// </summary>
        /// <returns>Retorna el codigo obtenido</returns>
        private void sb_GetHardwareKey()
        {
            try
            {
                go_SBOApplication.Menus.Item("257").Activate();
                go_SBOForm = go_SBOApplication.Forms.GetForm("999999", 0);
                go_Edit = go_SBOForm.Items.Item("79").Specific;
                Cls_CodigoAddon_BL.gs_hadkey = go_Edit.Value;
                go_SBOForm.Close();
            }
            catch (Exception MsjExc) { go_SBOApplication.SetStatusBarMessage(MsjExc.Message, BoMessageTime.bmt_Short); }
            finally
            {
                go_SBOForm = null;
                GC.Collect();
            }
        }

        /// <Muestra el cuadro de diálogo para la búsqueda>
        /// </summary>
        /// <param name="po_ItemEvent"></param>
        /// <param name="po_SBOForm"></param>
        /// <returns></returns>
        private void fn_MostrarVentanaBusqueda()
        {
            StreamReader lo_stream = null;
            try
            {
                lo_stream = new StreamReader(Cls_Global.fn_WindowDialog("txt", true));
                go_Edit = go_SBOForm.Items.Item(gs_TxtKey).Specific;
                go_Edit.Value = lo_stream.ReadLine();
            }
            catch (Exception ex) { go_SBOApplication.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Short); }
            finally
            {
                if (lo_stream != null) { lo_stream.Close(); lo_stream.Dispose(); }
                lo_stream = null;
                GC.Collect();
            }
        }

        #endregion Métodos privados
    }
}