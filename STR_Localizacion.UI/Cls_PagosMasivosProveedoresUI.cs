using SAPbobsCOM;
using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STR_Localizacion.UI
{
    public class Cls_PagosMasivosProveedoresUI : Cls_PropertiesControl
    {
        public Cls_PagosMasivosProveedoresUI()
        {
            gs_FormName = "UDO_FT_BPP_PAGM1";
            gs_FormPath = "Resources/Localizacion/frmParam1.srf";
            lc_NameClass = "Cls_PagosMasivosProveedoresUI";
        }

        private void InitializeEvents()
        {
        }

        public void Sb_FormLoad()
        {
            try
            {
                if (go_SBOForm == null)
                {
                    CrearFormulario();
                    //CargarDatos();
                    ConfigurarEventos();
                }

                go_SBOForm.Visible = true;
            }
            catch (Exception exc)
            {
                go_SBOApplication.SetStatusBarMessage(exc.Message, BoMessageTime.bmt_Short, true);
            }
        }

        private void ConfigurarEventos()
        {
            try
            {
                itemevent.Add(new sapitemevent(BoEventTypes.et_CHOOSE_FROM_LIST, "txtCodprov", e => { if (!e.BeforeAction) cflCodigoSN(ref e); }));
                itemevent.Add(new sapitemevent(BoEventTypes.et_CHOOSE_FROM_LIST, "txtCuenban", e => { if (!e.BeforeAction) cflCuentaBanco(ref e); }));
                itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "btncom", e =>
                {
                    if (!e.BeforeAction)
                    {
                        if (e.FormMode == 1 || e.FormMode == 2 || e.FormMode == 3)
                        {
                            //string TablaHana = "EW_PERDATA";
                            //string matrixDetalle = "matDet1";

                            //if (validardatostablausuario(TablaHana))
                            //{
                            //    removecolumns(matrixDetalle);
                            //    createcolumns(matrixDetalle);

                            //    cargardatostablausuario(matrixDetalle, TablaHana);
                            //}
                            //else
                            //{
                            //    removecolumns(matrixDetalle);
                            //    createcolumns(matrixDetalle);
                            //}

                            //habilitarcamposanexos();
                        }
                    }

                }));
                itemevent.Add(new sapitemevent(BoEventTypes.et_KEY_DOWN, "matDet1", e =>
                {
                    if (e.BeforeAction)
                    {
                        if ((e.ItemUID == "matDet1" || e.ItemUID == "matDet2") && (e.FormMode == 3 || e.FormMode == 1 || e.FormMode == 2))
                        {
                            //if (!validarbotonmatrix(pVal.ItemUID))
                            //{

                            //    //SAPMain.SBO_Application.MessageBox("Debe cargar los períodos.", 1, "Ok", "");

                            //    SAPMain.MensajeError("Debe cargar los períodos, de lo contrario no se guardaran sus datos.", true);
                            //    BubbleEvent = false;
                            //    return;

                            //}

                        }
                    }
                }));
                itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "btnConsult", e =>
                {
                    if (e.BeforeAction && e.FormMode == 3)
                    {
                        string oBanco = go_SBOForm.Items.Item("txtCuenban").Specific.Value;
                        if (oBanco.Equals(""))
                        {
                            go_SBOApplication.SetStatusBarMessage("Debe seleccionar un Banco.", BoMessageTime.bmt_Medium);
                        }
                        else
                        {
                            Matrix oMatrix = (Matrix)go_SBOForm.Items.Item("matDet1").Specific;
                            oMatrix.Columns.Item("colCheck").Editable = true;
                            cargarAsientos();
                        }
                    }
                }));
                itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "btnArch", e =>
                {
                    if (e.BeforeAction && e.FormMode == 1)
                    {
                        string oBanco = go_SBOForm.Items.Item("txtCodban").Specific.Value;
                        Recordset oRecordSet = (SAPbobsCOM.Recordset)go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        string query = string.Format("SELECT \"BankCode\" \"BankCode\" FROM DSC1 WHERE \"BankCode\" = '{0}' AND  \"U_BPP_GENTXT\" = 'Y' ", oBanco);
                        oRecordSet.DoQuery(query);
                        string codBanco = oRecordSet.Fields.Item("BankCode").Value.ToString();
                        if (!codBanco.Equals(""))
                        {
                            int rpta = go_SBOApplication.MessageBox("Se generará el archivo txt, la plantilla sera bloqueado. ¿Desea continuar?", 1, "Si", "No", "");
                            if (rpta == 1) generarArchivo();
                        }
                        else
                        {
                            go_SBOApplication.SetStatusBarMessage("No existe formato txt del banco seleccionado.", BoMessageTime.bmt_Medium, false);
                        }
                    }
                }));

                itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "btnProc", e =>
                {
                    if (e.BeforeAction && e.FormMode == 1)
                    {
                        int rpta = go_SBOApplication.MessageBox("Se generarán los pagos, ¿Desea continuar?", 1, "Si", "No", "");
                        if (rpta == 1)
                        {
                            if (generarAsientos())
                            {
                                go_SBOForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE;
                            }
                        }
                    }
                }));
                itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "1", e =>
                {
                    if (e.BeforeAction && e.FormMode == 2)
                        actualizarNumeroSunat();
                }));
                itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "1", e =>
                {
                    if (e.BeforeAction && e.FormMode == 3)
                        eliminarFilasNoSeleccionadas();
                }));

            }
            catch (Exception)
            {
                throw;
            }
        }



        private void CrearFormulario()
        {
            try
            {
                go_SBOForm = Cls_Global.fn_CreateForm(gs_FormName, gs_FormPath);
                cargarLogo();
                cargarFormDefault();
                cflCodigoSN();
                cflCuentaBanco();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void cflCodigoSN()
        {
            try
            {
                ChooseFromListCollection oCFLs = go_SBOForm.ChooseFromLists;
                SAPbouiCOM.ChooseFromList oCFL = null;
                ChooseFromListCreationParams oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(go_SBOApplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));

                oCFLCreationParams.MultiSelection = false;
                oCFLCreationParams.ObjectType = "2";
                oCFLCreationParams.UniqueID = "cflCodigoSN";
                oCFL = oCFLs.Add(oCFLCreationParams);

                Conditions oCons = new Conditions();
                Condition oCon = oCons.Add();
                oCons = new Conditions();
                oCon = oCons.Add();
                oCon.Alias = "CardType";
                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCon.CondVal = "S";

                oCFL.SetConditions(oCons);

                EditText txtCodeSN = ((EditText)go_SBOForm.Items.Item("txtCodprov").Specific);
                txtCodeSN.ChooseFromListUID = "cflCodigoSN";
                txtCodeSN.ChooseFromListAlias = "CardCode";
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        private void cflCuentaBanco()
        {
            try
            {
                ChooseFromListCollection oCFLs = go_SBOForm.ChooseFromLists;
                SAPbouiCOM.ChooseFromList oCFL = null;
                ChooseFromListCreationParams oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(go_SBOApplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));

                oCFLCreationParams.MultiSelection = false;
                oCFLCreationParams.ObjectType = "231";
                oCFLCreationParams.UniqueID = "cflCuentaBn";
                oCFL = oCFLs.Add(oCFLCreationParams);

                Conditions oCons = new Conditions();
                Condition oCon = oCons.Add();
                oCons = new Conditions();

                oCon = oCons.Add();

                EditText txtCodeSN = ((EditText)go_SBOForm.Items.Item("txtCuenban").Specific);
                txtCodeSN.ChooseFromListUID = "cflCuentaBn";
                txtCodeSN.ChooseFromListAlias = "Account";
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        private void cargarFormDefault()
        {
            SAPbouiCOM.ComboBox oComboSerie = go_SBOForm.Items.Item("txtSerie").Specific;
            oComboSerie.ValidValues.Add(DateTime.Now.Year.ToString(), DateTime.Now.Year.ToString());

            go_SBOForm.Items.Item("txtFeccrea").Specific.Value = DateTime.Now.ToString("yyyyMMdd");
            ((SAPbouiCOM.ComboBox)(go_SBOForm.Items.Item("22_U_E").Specific)).Select("Creado");
            ((SAPbouiCOM.ComboBox)(go_SBOForm.Items.Item("txtSerie").Specific)).Select(DateTime.Now.Year.ToString());

        }

        private void cargarSeries()
        {
            try
            {

                SAPbouiCOM.ComboBox oCombo = (SAPbouiCOM.ComboBox)go_SBOForm.Items.Item("Item_4").Specific;

                Recordset oRecordSet = (Recordset)go_SBOCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string query = "SELECT DISTINCT \"Indicator\" FROM OFPR ";


                oRecordSet.DoQuery(query);
                string code = string.Empty;

                for (int i = 0; i < oRecordSet.RecordCount; i++)
                {
                    code = oRecordSet.Fields.Item("Indicator").Value.ToString();
                    oCombo.ValidValues.Add(code, code);

                    oRecordSet.MoveNext();
                }


                oCombo.Select(DateTime.Now.Year.ToString(), BoSearchKey.psk_ByValue);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void cargarLogo()
        {
            string sPath = System.Windows.Forms.Application.StartupPath.ToString();
            SAPbouiCOM.Button oButton = (SAPbouiCOM.Button)go_SBOForm.Items.Item("btnLogo").Specific;
            oButton.Type = BoButtonTypes.bt_Image;
            oButton.Image = sPath + "\\Resources\\Imgs\\logo_empresa_1.png";
            oButton = (SAPbouiCOM.Button)go_SBOForm.Items.Item("btnArch").Specific;
            oButton.Type = BoButtonTypes.bt_Image;
            oButton.Image = sPath + "\\Resources\\Imgs\\boton_archivo_1.png";
            oButton = (SAPbouiCOM.Button)go_SBOForm.Items.Item("btnProc").Specific;
            oButton.Type = BoButtonTypes.bt_Image;
            oButton.Image = sPath + "\\Resources\\Imgs\\boton_procesar_1.png";
        }
        private void cflCodigoSN(ref SAPbouiCOM.ItemEvent pVal)
        {
            SAPbouiCOM.DataTable dtSelect = null;
            try
            {
                IChooseFromListEvent oCFLEvento = (IChooseFromListEvent)pVal;

                if (!oCFLEvento.Before_Action && oCFLEvento.ChooseFromListUID == "cflCodigoSN")
                {
                    dtSelect = oCFLEvento.SelectedObjects;

                    if (dtSelect != null)
                    {
                        EditText oEditCode = ((EditText)go_SBOForm.Items.Item("txtCodprov").Specific);
                        EditText oEditName = ((EditText)go_SBOForm.Items.Item("txtNomprov").Specific);
                        try { oEditCode.Value = dtSelect.GetValue("CardCode", 0).ToString(); } catch { }
                        try { oEditName.Value = dtSelect.GetValue("CardName", 0).ToString(); } catch { }
                        //try { oEditContacto.Value = dtSelect.GetValue("CntctPrsn", 0).ToString(); } catch { }
                        //try { oEditAddres.Value = dtSelect.GetValue("MailAddres", 0).ToString(); } catch { }

                    }
                }


            }
            catch (Exception ex)
            {
                throw;
            }



        }

        private void cflCuentaBanco(ref SAPbouiCOM.ItemEvent pVal)
        {
            SAPbouiCOM.DataTable dtSelect = null;
            try
            {
                IChooseFromListEvent oCFLEvento = (IChooseFromListEvent)pVal;

                if (!oCFLEvento.Before_Action && oCFLEvento.ChooseFromListUID == "cflCuentaBn")
                {
                    dtSelect = oCFLEvento.SelectedObjects;

                    if (dtSelect != null)
                    {
                        EditText oEditCuenban = ((EditText)go_SBOForm.Items.Item("txtCuenban").Specific);
                        EditText oEditCuencon = ((EditText)go_SBOForm.Items.Item("txtCuencon").Specific);
                        EditText oEditMoneda = ((EditText)go_SBOForm.Items.Item("txtMoneda").Specific);
                        EditText oEditNomban = ((EditText)go_SBOForm.Items.Item("txtNomban").Specific);
                        EditText oEditCodban = ((EditText)go_SBOForm.Items.Item("txtCodban").Specific);
                        try { oEditCuenban.Value = dtSelect.GetValue("Account", 0).ToString(); } catch { }
                        try { oEditCuencon.Value = dtSelect.GetValue("GLAccount", 0).ToString(); } catch { }
                        try { oEditMoneda.Value = dtSelect.GetValue("Branch", 0).ToString(); } catch { }
                        try { oEditNomban.Value = dtSelect.GetValue("AcctName", 0).ToString(); } catch { }
                        try { oEditCodban.Value = dtSelect.GetValue("BankCode", 0).ToString(); } catch { }


                    }
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void cargarAsientos()
        {
            Matrix oMatrix = (Matrix)go_SBOForm.Items.Item("matDet1").Specific;
            string oDocEntry = go_SBOForm.Items.Item("0_U_E").Specific.Value;
            string ofechaini = go_SBOForm.Items.Item("txtFecini").Specific.Value;
            string ofechafin = go_SBOForm.Items.Item("txtFecfin").Specific.Value;
            string ofechavenini = go_SBOForm.Items.Item("txtFecvini").Specific.Value;
            string ofechavenfin = go_SBOForm.Items.Item("txtFecvfin").Specific.Value;
            string oCodprov = go_SBOForm.Items.Item("txtCodprov").Specific.Value;
            string oMoneda = go_SBOForm.Items.Item("txtMoneda").Specific.Value;

            try
            {
                //oForm.Freeze(true);

                DBDataSource oDBDataSource = go_SBOForm.DataSources.DBDataSources.Item("@BPP_PAGM_DET1");
                oDBDataSource.Clear();

                Recordset oRecordSet = (Recordset)go_SBOCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = string.Empty;

                if (go_SBOCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    query = string.Format("CALL SP_BPP_CONSULTAR_PGM_PROVEEDORES ('{0}','{1}','{2}','{3}','{4}','{5}')  ", ofechaini, ofechafin, oCodprov, oMoneda, ofechavenini, ofechavenfin);
                }
                else
                {
                    query = string.Format("EXEC SP_BPP_CONSULTAR_PGM_PROVEEDORES '{0}','{1}','{2}','{3}','{4}','{5}'  ", ofechaini, ofechafin, oCodprov, oMoneda, ofechavenini, ofechavenfin);
                }

                oRecordSet.DoQuery(query);


                double total = 0;
                for (int i = 0; i < oRecordSet.RecordCount; i++)
                {

                    oDBDataSource.InsertRecord(oDBDataSource.Size);
                    oDBDataSource.SetValue("U_BPP_CODPROV", oDBDataSource.Size - 1, oRecordSet.Fields.Item("CodigoProveedor").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_RUC", oDBDataSource.Size - 1, oRecordSet.Fields.Item("RUC").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_NOMPROV", oDBDataSource.Size - 1, oRecordSet.Fields.Item("NombreProveedor").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_TIPODOC", oDBDataSource.Size - 1, oRecordSet.Fields.Item("TipoDocumento").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_NUMDOC", oDBDataSource.Size - 1, oRecordSet.Fields.Item("NumeroDocumento").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_FECDOC", oDBDataSource.Size - 1, oRecordSet.Fields.Item("FechaDocumento").Value.ToString("yyyyMMdd"));
                    oDBDataSource.SetValue("U_BPP_FECCONT", oDBDataSource.Size - 1, oRecordSet.Fields.Item("FechaContabilizacion").Value.ToString("yyyyMMdd"));
                    oDBDataSource.SetValue("U_BPP_FECVEN", oDBDataSource.Size - 1, oRecordSet.Fields.Item("FechaVencimiento").Value.ToString("yyyyMMdd"));
                    oDBDataSource.SetValue("U_BPP_SALDO", oDBDataSource.Size - 1, oRecordSet.Fields.Item("Saldo").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_MONTOPAG", oDBDataSource.Size - 1, oRecordSet.Fields.Item("MontoPago").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_CUENBAN", oDBDataSource.Size - 1, oRecordSet.Fields.Item("CuentaBanco").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_NOMBAN", oDBDataSource.Size - 1, oRecordSet.Fields.Item("NombreBanco").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_MONBAN", oDBDataSource.Size - 1, oRecordSet.Fields.Item("MonedaBanco").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_MONEDA", oDBDataSource.Size - 1, oRecordSet.Fields.Item("Moneda").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_IMPORTE", oDBDataSource.Size - 1, oRecordSet.Fields.Item("ImporteDoc").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_OBJTYPE", oDBDataSource.Size - 1, oRecordSet.Fields.Item("ObjType").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_NUMSAP", oDBDataSource.Size - 1, oRecordSet.Fields.Item("NumeroSAP").Value.ToString());

                    total = total + double.Parse(oRecordSet.Fields.Item("Saldo").Value.ToString());


                    oRecordSet.MoveNext();
                }
                go_SBOForm.Items.Item("txtTotdet").Specific.Value = total.ToString();
                oMatrix.Clear();
                oMatrix.LoadFromDataSource();
                oMatrix.AutoResizeColumns();

                go_SBOForm.Freeze(false);

                go_SBOApplication.SetStatusBarMessage("Se cargo satisfactoriamente los documentos.", BoMessageTime.bmt_Medium, false);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void generarArchivo()
        {
            try
            {
                string ofechaini = go_SBOForm.Items.Item("txtFecini").Specific.Value;
                string ofechafin = go_SBOForm.Items.Item("txtFecfin").Specific.Value;
                string oNumeroOper = go_SBOForm.Items.Item("txtPagodet").Specific.Value;
                string oDocEntry = go_SBOForm.Items.Item("0_U_E").Specific.Value;
                Recordset oRecordSet = (Recordset)go_SBOCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = string.Empty;

                if (go_SBOCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    query = string.Format("CALL SP_BPP_ARCHIVO_PAGOS_MASIVOS ('{0}')  ", oDocEntry);
                }
                else
                {
                    query = string.Format("EXEC SP_BPP_ARCHIVO_PAGOS_MASIVOS '{0}'  ", oDocEntry);
                }


                oRecordSet.DoQuery(query);
                StringBuilder cadena = new StringBuilder();
                FileStream fs = null;
                for (int i = 0; i < oRecordSet.RecordCount; i++)
                {

                    cadena.Append(oRecordSet.Fields.Item("Texto").Value.ToString());
                    cadena.AppendLine();
                    oRecordSet.MoveNext();

                }
                if (!Cls_Global.rutaPagos.Equals(""))
                {
                    string fileName = Cls_Global.rutaPagos + ofechaini + "-PGM-" + oNumeroOper + ".txt";

                    fs = File.Create(fileName);
                    fs.Close();

                    StreamWriter writer = new StreamWriter(fileName);
                    writer.Write(cadena.ToString());
                    writer.Close();

                    oRecordSet = (Recordset)go_SBOCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                    query = string.Format("UPDATE \"@BPP_PAGM_CAB\" SET  U_BPP_RUTATXT =  '{1}' WHERE \"DocEntry\"  = {0} ", oDocEntry, fileName);
                    oRecordSet.DoQuery(query);

                    go_SBOForm.Mode = BoFormMode.fm_ADD_MODE;
                    //go_SBOForm.Items.Item("txtRuta").Specific.Value = fileName;

                    go_SBOApplication.SetStatusBarMessage("Se genero Satisfactoriamente el archivo txt.", BoMessageTime.bmt_Medium, false);
                }
                else
                {
                    go_SBOApplication.SetStatusBarMessage("Debe definir una ruta para generar el archivo txt.", BoMessageTime.bmt_Medium);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private bool generarAsientos()
        {

            try
            {
                Recordset oRecordSet = (SAPbobsCOM.Recordset)go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                JournalEntries oAsiento = null;
                Payments oPago = null;
                Matrix oMatrix = (Matrix)go_SBOForm.Items.Item("matDet1").Specific;

                string ofechacrea = go_SBOForm.Items.Item("txtFeccrea").Specific.Value;
                string oCuenta = go_SBOForm.Items.Item("txtCuencon").Specific.Value;
                string oNumoper = go_SBOForm.Items.Item("txtPagodet").Specific.Value;
                string oDocEntry = go_SBOForm.Items.Item("0_U_E").Specific.Value;
                string ofechaeje = go_SBOForm.Items.Item("txtFeceje").Specific.Value;


                DBDataSource oDBDataSource = this.go_SBOForm.DataSources.DBDataSources.Item("@BPP_PAGM_DET1");
                string docEntryDoc;
                string cardCode;
                string cardName;
                double monto;
                string monedaDoc;
                string lineID;

                string objeto;
                List<string> listidPagos = new List<string>();

                if (oNumoper.Equals(""))
                {
                    go_SBOApplication.SetStatusBarMessage("Debe ingresar un numero de operación.", BoMessageTime.bmt_Medium);

                }
                else
                {
                    if (!go_SBOCompany.InTransaction)
                    {
                        go_SBOCompany.StartTransaction();
                    }

                    go_SBOApplication.SetStatusBarMessage("Creando las Pagos. Espere por favor...", BoMessageTime.bmt_Short, false);

                    for (int i = 0; i < oDBDataSource.Size; i++)
                    {

                        lineID = oDBDataSource.GetValue("LineId", i).ToString().Trim();
                        docEntryDoc = oDBDataSource.GetValue("U_BPP_NUMSAP", i).ToString().Trim();
                        cardCode = oDBDataSource.GetValue("U_BPP_CODPROV", i).ToString().Trim();
                        cardName = oDBDataSource.GetValue("U_BPP_NOMPROV", i).ToString().Trim();
                        monto = double.Parse(oDBDataSource.GetValue("U_BPP_MONTOPAG", i).ToString().Trim());
                        monedaDoc = oDBDataSource.GetValue("U_BPP_MONEDA", i).ToString().Trim();
                        objeto = oDBDataSource.GetValue("U_BPP_OBJTYPE", i).ToString().Trim();
                        oPago = (Payments)go_SBOCompany.GetBusinessObject(BoObjectTypes.oVendorPayments);
                        oPago.DocType = BoRcptTypes.rSupplier;
                        oPago.DocDate = DateTime.ParseExact(ofechaeje, "yyyyMMdd", null);
                        oPago.TaxDate = DateTime.ParseExact(ofechaeje, "yyyyMMdd", null);
                        oPago.DueDate = DateTime.ParseExact(ofechaeje, "yyyyMMdd", null);

                        oPago.JournalRemarks = "Pago Masivos Nro. " + oDocEntry;
                        oPago.CardCode = cardCode;
                        oPago.CardName = cardName;
                        oPago.UserFields.Fields.Item("U_BPP_NUMPAGO").Value = oNumoper;
                        oPago.UserFields.Fields.Item("U_BPP_TRAN").Value = "003";
                        oPago.UserFields.Fields.Item("U_BPP_TARJ").Value = "000";
                        oPago.UserFields.Fields.Item("U_BPP_LETR").Value = "000";
                        oPago.UserFields.Fields.Item("U_BPP_EFEC").Value = "000";
                        oPago.UserFields.Fields.Item("U_BPP_CHEQ").Value = "000";

                        oPago.TransferAccount = oCuenta;
                        oPago.TransferReference = oNumoper;
                        oPago.TransferDate = DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd", null);

                        oPago.Invoices.DocEntry = int.Parse(docEntryDoc);
                        switch (objeto)
                        {
                            case "18":
                                oPago.Invoices.InvoiceType = BoRcptInvTypes.it_PurchaseInvoice;
                                break;
                            case "204":
                                oPago.Invoices.InvoiceType = BoRcptInvTypes.it_PurchaseDownPayment;
                                break;
                        }


                        if (monedaDoc == "SOL")
                        {
                            oPago.Invoices.SumApplied = monto;
                        }
                        else
                        {
                            oPago.Invoices.AppliedFC = monto;
                        }

                        oPago.DocCurrency = monedaDoc;
                        oPago.TransferSum = monto;
                        if (oPago.Add() != 0)
                        {
                            if (go_SBOCompany.InTransaction)
                            {
                                go_SBOCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                            }
                            //oForm.Freeze(false);

                            string error = string.Format("{0}-{1}", go_SBOCompany.GetLastErrorCode(), go_SBOCompany.GetLastErrorDescription());
                            go_SBOApplication.SetStatusBarMessage(error, BoMessageTime.bmt_Medium, true);
                            return false;
                        }
                        else
                        {
                            string idPago = go_SBOCompany.GetNewObjectKey();

                            listidPagos.Add(idPago + "|" + lineID);
                        }

                    }
                    oMatrix.LoadFromDataSource();

                    if (go_SBOCompany.InTransaction)
                    {
                        go_SBOCompany.EndTransaction(BoWfTransOpt.wf_Commit);

                        oRecordSet = (Recordset)go_SBOCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                        string query = string.Format("UPDATE \"@BPP_PAGM_CAB\" SET  U_BPP_ESTADO = 'Procesado' WHERE \"DocEntry\"  = {0} ", oDocEntry);
                        oRecordSet.DoQuery(query);


                        for (int k = 0; k < listidPagos.Count; k++)
                        {
                            string[] datos = listidPagos[k].Split('|');
                            query = string.Format("UPDATE \"@BPP_PAGM_DET1\" SET \"U_BPP_PAGO\" = " + datos[0] + " WHERE \"DocEntry\" = " + oDocEntry + " AND \"LineId\" = " + datos[1]);
                            oRecordSet.DoQuery(query);


                        }

                        return true;
                    }
                }





                return false;

            }
            catch (Exception ex)
            {
                if (go_SBOCompany.InTransaction)
                {
                    go_SBOCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                    go_SBOApplication.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Medium, true);
                    //oForm.Freeze(false);
                }
                return false;
            }
        }


        private void actualizarNumeroSunat()
        {
            try
            {
                Matrix oMatrix = (Matrix)go_SBOForm.Items.Item("matDet1").Specific;

                for (int i = 1; i <= oMatrix.RowCount; i++)
                {
                    EditText oEditTextpag = (EditText)oMatrix.Columns.Item("colNumpag").Cells.Item(i).Specific;
                    EditText oEditTextsap = (EditText)oMatrix.Columns.Item("colNumsap").Cells.Item(i).Specific;

                    string numeropago = oEditTextpag.Value;
                    string numerosap = oEditTextsap.Value;

                    Recordset oRecordSet = (Recordset)go_SBOCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                    string query = string.Format("UPDATE OPCH SET \"U_BPP_DPNM\" =  '{0}' WHERE \"DocEntry\" = {1} ", numeropago, numerosap);
                    oRecordSet.DoQuery(query);
                }
            }
            catch (Exception ex)
            {
                go_SBOApplication.SetStatusBarMessage(ex.Message.ToString(), BoMessageTime.bmt_Short, true);
            }
        }

        private void eliminarFilasNoSeleccionadas()
        {
            try
            {
                go_SBOForm.Freeze(true);

                Item oItem = go_SBOForm.Items.Item("matDet1");
                Matrix oMatrix = (SAPbouiCOM.Matrix)oItem.Specific;
                SAPbouiCOM.CheckBox oCheckBox = null;

                int j = 0;
                double total = 0;
                for (int i = 1; i <= oMatrix.RowCount; i += j)
                {
                    oCheckBox = (SAPbouiCOM.CheckBox)oMatrix.Columns.Item("colCheck").Cells.Item(i).Specific;

                    if (!oCheckBox.Checked)
                    {
                        oMatrix.DeleteRow(i);

                        j = 0;
                    }
                    else
                    {
                        j = 1;
                        total = total + double.Parse(oMatrix.Columns.Item("colSaldo").Cells.Item(i).Specific.Value);
                    }
                }
                go_SBOForm.Freeze(false);

                go_SBOForm.Items.Item("txtTotdet").Specific.Value = total.ToString();

            }
            catch (Exception ex)
            {
                go_SBOApplication.SetStatusBarMessage(ex.Message.ToString(), BoMessageTime.bmt_Medium);
                go_SBOForm.Freeze(false);
            }

        }

    }
}
