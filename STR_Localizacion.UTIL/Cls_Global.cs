using System;
using System.Collections.Generic;
using System.Resources;
using System.Windows.Forms;

namespace STR_Localizacion.UTIL
{
    public static class Cls_Global
    {
        //Declaracion de variables que seran utilizadas por todo el proyecto

        public static System.Threading.Timer go_TimerComprueba = null;
        public static SAPbobsCOM.Company go_SBOCompany = null;
        public static SAPbouiCOM.Application go_SBOApplication = null;
        public static SAPbobsCOM.BoDataServerTypes go_ServerType;
        public static SAPbobsCOM.SBObob go_SBObob = null;
        public static SAPbouiCOM.SboGuiApi go_SBOGUIAPI = null;
        //Objeto para CopnayService
        public static SAPbobsCOM.CompanyService go_SBOCompanyService;

        //Declaración del recordSet Global
        public static SAPbobsCOM.Recordset go_RecordSet;

        //Verificador para la inicialización del add-on
        public static bool gb_Ver = true;

        //Verificador global para el inicio del add-on
        public static bool gb_bol;

        //Indica que el formulario es Modal: para el inicio del add-on
        public static bool gb_Modal;

        //Esta variable contendrá los mensajes devueltos durante los procesos de migración
        public static string gs_msjInterfaz = string.Empty;

        //Esta variable contendrá el código del add-on
        public static string gs_addon = "LOC";

        //Aqui carga la ruta de Resource a Hana o SQL
        public static string gss_baseDefault = string.Empty;

        //La variable almacenará la linea final de codigo de los procedimientos dependiendo si es Hana o SQL
        public static string gss_finLine = string.Empty;

        //Indicará en que ruta estan los scripts, según el gestor de BD
        public static string gs_rutaScript;

        //Indicador de conexion correcta del AddOn
        public static bool gb_IndConexionAddOn = true;

        //Filtros para las acciones del formulario
        public static SAPbouiCOM.EventFilters go_Filters;

        public static SAPbouiCOM.EventFilter go_Filter;

        public static SAPbouiCOM.UserDataSource go_UserDataSource;

        //Declaración de un objeto SBOApplication auxiliar para el uso del Hilo utilizado en la lógica para el Codigo de Add-On
        public static SAPbouiCOM.Application go_AuxSBOApplication;

        //Para manejo del hilo
        public static bool gb_manejoHilo;

        public static ResXResourceSet gso_resxSet = null;

        //Variables generales del modulo
        public static string ms_nomMet = string.Empty;

        public static string[] gs_array = null;
        public static int gi_lenArray;

        private static string gs_Path;

        //Constantes generales del modulo
        private const string c_nomMod = "Cls_Global";

        private const string c_nomCap = "STR_Addon.UTIL";

        //Variables de la configuración
        public static string metCalculoTC = string.Empty;

        public static string fuenteTC = string.Empty;
        public static string metAsientoDestino = string.Empty;
        public static string APDifGain = string.Empty;
        public static string APDifLoss = string.Empty;
        public static string ProvisionNDActivo = string.Empty;
        public static string ProvisionNDCodigosRetencion = string.Empty;
        public static string ProvisionNDCuentaDebito = string.Empty;
        public static string ProvisionNDCuentaCredito = string.Empty;
        public static string ProvisionNDFormatoCuentaDebito = string.Empty;
        public static string ProvisionNDFormatoCuentaCredito = string.Empty;
        public static string RetencionDeGarantiaActivo = string.Empty;
        public static string ImpuestoRetencionDeGarantia = string.Empty;
        public static string ReconciliacionActivo = string.Empty;
        public static string ReconciliacionCuenta = string.Empty;

        #region "Metodos Comunes"

        /// <Devuelve un número de espacios>
        /// </>
        /// <param name="pby_n"></param>
        /// <returns></returns>
        public static string fnSpace(byte pby_n = 1)
        {
            return new string(' ', pby_n);
        }

        public static string IIf(bool Expression, object TruePart, object FalsePart)
        {
            return (Expression ? TruePart : FalsePart).ToString();
        }

        public static string fnStringDB(string ps_msj)
        {
            return ps_msj.Replace("'", "");
        }

        public static bool fnFindElement(string ps_element, string[] ps_array = null)
        {
            ps_array = ps_array ?? gs_array;
            for (int i = 0; i < ps_array.Length; i++)
            {
                if (ps_array[i] == ps_element) return true;
            }
            return false;
        }

        #endregion "Metodos Comunes"

        #region "Metodos Globales"

        /// <Defin n parametros para query>
        /// Este método permite definir n parametros y arma la cadena lista para la ejecución.
        /// </>
        /// <param name="ps_cadena"></param>
        /// <param name="po_param"></param>
        /// <returns>Query armado incluido parametros</returns>
        /// 

        public static string fn_defineParametros(string ps_cadena, params object[] po_param)
        {
            try
            {
                string ls_query = string.Empty;
                List<string> ls_listaCadena = null;

                ls_listaCadena = new List<string>(); //Inicializa lista de cadena
                ms_nomMet = "A";
                ls_listaCadena.AddRange(ps_cadena.Split('?')); //Retorna la cadena que se encuentra antes del carácter ingresado
                ms_nomMet = "B";
                int j = 0;
                for (int i = 0; i < ls_listaCadena.Count; i++)
                {
                    if (j < ls_listaCadena.Count - 1)
                    {
                        ls_query += ls_listaCadena[i].ToString() + po_param[j]; //Concatena la cadena con los parámetros enviados
                        j++;
                    }
                    else
                        ls_query += ls_listaCadena[i].ToString(); //Concatena el resultado final de la cadena
                }
                ms_nomMet = "C";
                return ls_query;
            }
            catch (Exception ex)
            {
                ms_nomMet += ex.Message;
                sb_msjStatusBarSAP(ms_nomMet + " - " + ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error, go_AuxSBOApplication);
                return null;
            }
        }

        /// <Se inicializa el recordSet>
        /// </summary>
        public static void sb_iniRecordSet()
        {
            string ls_nomMet = "sb_asgMontAplicTotal";
            try
            {
                go_RecordSet = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            }
            catch (Exception ex)
            {
                string ls_msgExc = "Excepción: " + ex.Message + " - Módulo: " + c_nomMod + " - Capa: " + c_nomCap + " - Método: " + ls_nomMet;
            }
        }

        /// <Generá el UID para formularios>
        /// </summary>
        /// <returns></returns>
        public static int fn_GetNextFormUID()
        {
            string ls_nomMet = "fn_GetNextFormUID"; //Se asigna el nombre del metodo para la identificación del mismo
            try
            {
                Random lo_randomD = new Random();
                int li_valor;
                li_valor = lo_randomD.Next(999999999);
                return li_valor;
            }
            catch (Exception ex)
            {
                string ls_msgExc = "Excepcíon: " + ex.Message + " - Modulo: " + c_nomMod + " - Capa: " + c_nomCap + " - Metodo: " + ls_nomMet;
                return -1;
            }
        }

        /// <Llena un ComboBox sin espacio vacío>
        /// Invoca al método sb_comboLlenar y envía un parámetro en false para indicar que no
        /// se agregará con un item vacío
        /// </>
        /// <param name="po_Combo"></param>
        /// <param name="lo_Recordset"></param>
        public static void sb_comboLlenar(SAPbouiCOM.ComboBox po_Combo, SAPbobsCOM.Recordset lo_Recordset)
        {
            sb_comboLlenar(po_Combo, lo_Recordset, false);
        }

        /// <Llena ComboBox>
        /// Llena el control ComboBox preguntando antes si se agregará un espacio item vacío
        /// </>
        /// <param name="po_Combo"></param>
        /// <param name="po_Recordset"></param>
        /// <param name="pb_AgregarItemVacio"></param>
        public static void sb_comboLlenar(SAPbouiCOM.ComboBox po_Combo, SAPbobsCOM.Recordset po_Recordset, Boolean pb_AgregarItemVacio)
        {
            sb_comboLimpiar(po_Combo);
            if (pb_AgregarItemVacio)
            { //Pregunta si se agregará item vacío
                po_Combo.ValidValues.Add(string.Empty, string.Empty); //Inserta item vacío
            }

            while (!po_Recordset.EoF)
            {
                //Agrega nuevo item
                string caaa = po_Recordset.Fields.Item(0).Value;
                string daaa = po_Recordset.Fields.Item(1).Value;
                po_Combo.ValidValues.Add(po_Recordset.Fields.Item(0).Value, po_Recordset.Fields.Item(1).Value);
                po_Recordset.MoveNext(); //Pasa a la siguiente fila del recordSet
            }
        }

        /// <Remueve todos los items en un ComboBox>
        /// Quita todos los elementos dentro del comboBox
        /// </>
        /// <param name="po_combo"></param>
        public static void sb_comboLimpiar(SAPbouiCOM.ComboBox po_combo)
        {
            string ls_nomMet = "sb_comboLimpiar";
            try
            {
                //for (int li_i = 0; li_i <= po_combo.ValidValues.Count - 1; li_i++)
                int li_count = po_combo.ValidValues.Count;
                for (int li_i = 0; li_i < li_count; li_i++)
                    po_combo.ValidValues.Remove(0, SAPbouiCOM.BoSearchKey.psk_Index);//Remueve los items dentro del comboBox
            }
            catch (Exception ex)
            {
                string ls_msgExc = "Excepcíon: " + ex.Message + " - Modulo: " + c_nomMod + " - Capa: " + c_nomCap + " - Metodo: " + ls_nomMet;
            }
        }

        /// <Asigna valor a una posición de una tabla>
        /// Se envía como parámetro el nombre de la tabla y campo, también el número de fila y el valor
        /// a insertar en dicha posición.
        /// </>
        /// <param name="po_SBOForm"></param>
        /// <param name="ps_tabla"></param>
        /// <param name="ps_campo"></param>
        /// <param name="ps_valor"></param>
        /// <param name="pi_fila"></param>
        public static void sb_FormSetValueToDBDataSource(SAPbouiCOM.Form po_SBOForm, string ps_tabla, string ps_campo, string ps_valor, int pi_fila)
        {
            po_SBOForm.DataSources.DBDataSources.Item(ps_tabla).SetValue(ps_campo, pi_fila, ps_valor.Trim());
        }

        /// <Obtiene el valor de una posición de una tabla>
        /// Se envía como parámetro el nombre de la tabla y campo para poder recuperar el valor
        /// </>
        /// <param name="po_SBOForm"></param>
        /// <param name="ps_tabla"></param>
        /// <param name="ps_campo"></param>
        /// <returns></returns>
        public static string sb_FormGetValueFromDBDataSource(SAPbouiCOM.Form po_SBOForm, string ps_tabla, string ps_campo)
        {
            return po_SBOForm.DataSources.DBDataSources.Item(ps_tabla).GetValue(ps_campo, 0).Trim();
        }

        public static void sb_msjStatusBarSAP(string ps_mensaje, SAPbouiCOM.BoStatusBarMessageType pi_tipoMsj, SAPbouiCOM.Application po_SBOApplication)
        {
            po_SBOApplication.StatusBar.SetText(ps_mensaje, SAPbouiCOM.BoMessageTime.bmt_Medium, pi_tipoMsj);
        }

        /// <Busca un cadena dentro de una arreglo>
        /// </>
        /// <param name="ps_cadena"></param>
        /// <param name="ps_arrString"></param>
        /// <returns></returns>
        public static string fn_buscarEnArreglo(string ps_cadena, string[] ps_arrString)
        {
            for (int li_i = 0; li_i <= (ps_arrString.Length - 1); li_i++)
            {
                if (ps_cadena == ps_arrString[li_i]) //Compara si valor de la cadena es igual al del arreglo
                    return ps_cadena; //Retorna la cadena si ambos valores son iguales
            }
            return string.Empty;
        }

        /// <Maneja las transacciones>
        /// Inicializa la transacción, realiza Commit y Rollback
        /// </>
        /// <param name="pi_tipTran"></param>
        /// <param name="ps_nomTran"></param>
        /// <param name="po_SBOCompany"></param>
        /// <returns></returns>
        public static string fn_handlesqltransaction(int pi_tipTran, string ps_nomTran)
        {
            try
            {
                //Se asigna el tipo de transaccion
                switch (pi_tipTran)
                {
                    case 0: //Begin Transaction
                        go_SBOCompany.StartTransaction();
                        break;

                    case 1: //Commit Transaction
                        go_SBOCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        break;

                    case 2: //Rollback Transaction
                        go_SBOCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        break;
                }
                //Si no ocurrió ningun error
                return string.Empty;
            }
            catch (Exception ex)
            {
                //Se registrará la excepción en la tabla de log
                string ls_msgExc = "Excepción: " + ex.Message + " - Módulo: Cls_Global - Capa: STR_Addon.UTIL - Método: fn_handlesqltransaction";
                return ls_msgExc;
            }
        }

        public static void sb_CargaCombo(SAPbouiCOM.ComboBox po_ComboBox, SAPbobsCOM.Recordset po_RecordSet, bool pb_AddInitValue = false)
        {
            try
            {
                while (po_ComboBox.ValidValues.Count > 0)
                    po_ComboBox.ValidValues.Remove(0, SAPbouiCOM.BoSearchKey.psk_Index);

                if (pb_AddInitValue)
                    po_ComboBox.ValidValues.Add(" - - ", " - - ");

                while (!po_RecordSet.EoF)
                {
                    po_ComboBox.ValidValues.Add(po_RecordSet.Fields.Item(0).Value, po_RecordSet.Fields.Item(1).Value);
                    po_RecordSet.MoveNext();
                }
            }
            catch (Exception MsjExc) { go_SBOApplication.SetStatusBarMessage(MsjExc.Message, SAPbouiCOM.BoMessageTime.bmt_Short); }
            finally { po_RecordSet = null; po_ComboBox = null; }
        }

        public static string sb_ObtenerMonedaLocal()
        {
            SAPbobsCOM.SBObob lo_SBObob;
            try
            {
                lo_SBObob = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);
                return (string)lo_SBObob.GetLocalCurrency().Fields.Item(0).Value;
            }
            finally { lo_SBObob = null; }
        }

        public static double sb_ObtenerTipodeCambioXDia(string ps_CodMnd, DateTime po_Fch)
        {
            double ld_TpoCmb = 0.0;
            int li_CodErr = 0;
            string ls_Des = string.Empty;
            try
            {
                SAPbobsCOM.SBObob lo_SBObob = null;
                lo_SBObob = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);
                ld_TpoCmb = (double)lo_SBObob.GetCurrencyRate(ps_CodMnd, po_Fch).Fields.Item(0).Value;

                go_SBOCompany.GetLastError(out li_CodErr, out ls_Des);

                if (!(li_CodErr == 0 && ls_Des == string.Empty))
                    go_SBOApplication.SetStatusBarMessage(ls_Des, SAPbouiCOM.BoMessageTime.bmt_Short);

                lo_SBObob = null;
            }
            catch (Exception ex) { go_SBOApplication.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short); }

            return ld_TpoCmb;
        }

        public static SAPbouiCOM.Form fn_CreateForm(string ps_NomForm, string ps_RutaForm)
        {
            System.Xml.XmlDocument lo_XMLForm = null;
            SAPbouiCOM.FormCreationParams lo_FrmCrtPrms = null;

            try
            {
                lo_XMLForm = new System.Xml.XmlDocument();
                lo_XMLForm.Load(ps_RutaForm);

                lo_FrmCrtPrms = go_SBOApplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
                lo_FrmCrtPrms.XmlData = lo_XMLForm.InnerXml;
                lo_FrmCrtPrms.FormType = ps_NomForm;
                lo_FrmCrtPrms.UniqueID = ps_NomForm;
                return go_SBOApplication.Forms.AddEx(lo_FrmCrtPrms);
            }
            catch (Exception ex)
            {
                go_SBOApplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                throw;
            }
            finally { lo_XMLForm = null; lo_FrmCrtPrms = null; }
        }

        public static void fn_FormSetValueToDBDataSource(SAPbouiCOM.Form po_SBOForm, string ps_tabla, string ps_campo, string ps_valor, int pi_fila)
        {
            po_SBOForm.DataSources.DBDataSources.Item(ps_tabla).SetValue(ps_campo, pi_fila, ps_valor.Trim());
        }

        public static string fn_FormGetValueFromDBDataSource(SAPbouiCOM.Form po_SBOForm, string ps_tabla, string ps_campo)
        {
            return po_SBOForm.DataSources.DBDataSources.Item(ps_tabla).GetValue(ps_campo, 0).Trim();
        }

        public static void sb_msgStatusBarSAP(string ps_mensaje, SAPbouiCOM.BoStatusBarMessageType po_MsgType = SAPbouiCOM.BoStatusBarMessageType.smt_Error, SAPbouiCOM.BoMessageTime po_MsgTime = SAPbouiCOM.BoMessageTime.bmt_Short)
        {
            go_SBOApplication.StatusBar.SetText(ps_mensaje, po_MsgTime, po_MsgType);
        }

        public static string fn_WindowDialog(string fileType, bool pb_OpenFile = true)
        {
            System.Threading.Thread lt_Thread = null;
            try
            {
                //Ask for value gb_Dialog and initialite Thread
                if (pb_OpenFile) lt_Thread = new System.Threading.Thread(() => { sb_OpenFile(fileType); });
                else lt_Thread = new System.Threading.Thread(sb_FolderBrowser);

                if (lt_Thread.ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    lt_Thread.SetApartmentState(System.Threading.ApartmentState.STA);
                    lt_Thread.Start();
                }
                else if (lt_Thread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    lt_Thread.Start();
                    lt_Thread.Join();
                }
                while (lt_Thread.ThreadState == System.Threading.ThreadState.Running)
                {
                    Application.DoEvents();
                }
                return gs_Path;
            }
            catch (Exception ex)
            {
                sb_msgStatusBarSAP(ex.Message);
                return string.Empty;
            }
            finally
            {
                lt_Thread = null;
            }
        }

        /// <Retorna un valor de cadena en fecha>
        /// </>
        /// <param name="ps_Fecha"></param>
        /// <returns></returns>
        public static DateTime fn_Format_StringToDate(string ps_Fecha)
        {
            return new DateTime(int.Parse(ps_Fecha.Substring(0, 4)), int.Parse(ps_Fecha.Substring(4, 2)), int.Parse(ps_Fecha.Substring(6, 2)));
        }

        /// <Devuelve un número de espacios>
        /// </>
        /// <param name="pi_n"></param>
        /// <returns></returns>
        public static string fn_space(int pi_n)
        {
            string ls_space = new string(' ', pi_n);
            return ls_space;
        }

        /// <Devuelve el código de la moneda del local>
        /// </>
        /// <returns></returns>
        public static string fdi_ObtenerMonedaLocal()
        {
            SAPbobsCOM.SBObob lo_SBObob;
            lo_SBObob = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);
            return lo_SBObob.GetLocalCurrency().Fields.Item(0).Value; //Obtiene el tipo de la moneda
        }

        /// <Devuelve el código de la moneda del sistema>
        /// </>
        /// <returns></returns>
        public static string fdi_ObtenerMonedaSistema()
        {
            SAPbobsCOM.SBObob lo_SBObob;
            lo_SBObob = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);
            return lo_SBObob.GetSystemCurrency().Fields.Item(0).Value; //Obtiene el tipo de la moneda
        }

        #endregion "Metodos Globales"

        #region "Metodos Privados"

        private static void sb_OpenFile(string fileType)
        {
            OpenFileDialog OpenFileDialog = null;
            System.Diagnostics.Process MyProcess = null;
            WindowWrapper MyWindow = null;
            try
            {
                fileType = fileType.Equals("txt") ? "TXT Files (*.txt)|*.txt" : fileType.Equals("xls") ? "Excel Files|*.xls;*.xlsx;*.xlsm" : "All Files|*.* ";
                OpenFileDialog = new OpenFileDialog();
                OpenFileDialog.DefaultExt = "txt";
                OpenFileDialog.Filter = fileType;
                OpenFileDialog.RestoreDirectory = true;

                OpenFileDialog.InitializeLifetimeService();
                MyProcess = System.Diagnostics.Process.GetProcessesByName("SAP Business One")[0];

                MyWindow = new WindowWrapper(MyProcess.MainWindowHandle);
                if (OpenFileDialog.ShowDialog(MyWindow) == DialogResult.OK)
                    gs_Path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(OpenFileDialog.FileName), OpenFileDialog.FileName);
            }
            catch (Exception ex) { go_SBOApplication.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short); }
            finally
            {
                OpenFileDialog.Dispose(); MyWindow = null; MyProcess = null;
                Application.ExitThread();
            }
        }

        private static void sb_FolderBrowser()
        {
            FolderBrowserDialog FolderBrowser = null;
            System.Diagnostics.Process MyProcess = null;
            WindowWrapper MyWindow = null;
            try
            {
                FolderBrowser = new FolderBrowserDialog();

                FolderBrowser.InitializeLifetimeService();
                MyProcess = System.Diagnostics.Process.GetProcessesByName("SAP Business One")[0];

                MyWindow = new WindowWrapper(MyProcess.MainWindowHandle);
                if (FolderBrowser.ShowDialog(MyWindow) == DialogResult.OK) gs_Path = FolderBrowser.SelectedPath + @"\";
            }
            catch (Exception ex) { go_SBOApplication.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short); }
            finally
            {
                FolderBrowser.Dispose(); MyWindow = null;
                Application.ExitThread();
            }
        }

        #endregion "Metodos Privados"
    }

    public enum PeruAddon
    {
        Localizacion = 1,
        Sire = 2,
        CCEAR = 3,
        Letras = 4,
        TipoCambio = 5
    }

    public enum CodigoAddon
    {
        RAMOLOCALI = 1,
        RAMOSIRE = 2,
        RAMOEAR = 3,
        RAMOLETRAS = 4,
        RAMOCAMBIO = 5
    }

    partial class WindowWrapper : IWin32Window
    {
        private IntPtr _hwnd;

        //Property
        public virtual IntPtr Handle { get { return _hwnd; } }

        //Constructor
        public WindowWrapper(IntPtr handle) { _hwnd = handle; }
    }
}