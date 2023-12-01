using SAPbobsCOM;
using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;
using System.Collections.Generic;

namespace STR_Localizacion.UI
{
    public class Cls_Detraccion : Cls_PropertiesControl
    {
        private string ls_cuenta = string.Empty;
        private string ls_tablename;
        public List<string> go_InternalFormID = new List<string>() { "134", "141", "181", "65301" };

        public Cls_Detraccion()
        {
            lc_NameClass = "Cls_Detraccion";
            ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
            InitializeEvents();
        }

        private void SetTableName(string ps_CdgForm)
        {
            switch (ps_CdgForm)
            {
                case "141":
                    ls_tablename = "OPCH"; break;
                case "181":
                    ls_tablename = "ORPC"; break;
                case "65301":
                    ls_tablename = "ODPO"; break;
            }
        }

        private void InitializeEvents()
        {
            menuevent.Add(new sapmenuevent(
                s => sb_cargarcontroles(s), "2308", "2309", "2314", "2315", "2317"));

            //Devengado
            //menuevent.Add(new sapmenuevent(
            //    s => sb_validarBoton(s), "1288", "1289", "1290", "1291"));

            menuevent.Add(new sapmenuevent(
                s => sb_cargarcontroles(s), "2561"));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_LOAD, string.Empty,
                s => sb_cargarcontroles(s)));

            itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "lkAsDtr", s =>
            {
                if (!s.BeforeAction)
                {
                    go_LinkButton = go_SBOFormEvent.Items.Item("lkAsDtr").Specific;
                    go_Edit = go_SBOFormEvent.Items.Item("txAsDtr").Specific;
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_CHOOSE_FROM_LIST, "txtCuenta", s =>
            {
                go_SBOForm = go_SBOFormEvent;

                SAPbouiCOM.ChooseFromList lo_choosefromlist;
                IChooseFromListEvent lo_cflEvent;

                if (s.BeforeAction)
                {
                    //Declara condiciones
                    Conditions lo_Conditions;
                    Condition lo_Condition;
                    Conditions lo_emptyCon = new Conditions();
                    lo_cflEvent = (IChooseFromListEvent)s;
                    lo_choosefromlist = go_SBOForm.ChooseFromLists.Item(lo_cflEvent.ChooseFromListUID);
                    lo_choosefromlist.SetConditions(lo_emptyCon);
                    //Asigna los valores a las propiedades de la condición
                    lo_Conditions = lo_choosefromlist.GetConditions();
                    lo_Condition = lo_Conditions.Add();
                    lo_Condition.Alias = "LocManTran";
                    lo_Condition.Operation = BoConditionOperation.co_EQUAL;
                    lo_Condition.CondVal = "Y";
                    lo_choosefromlist.SetConditions(lo_Conditions);
                }
                else
                {
                    //Recupera los valores del po_ItemEvent
                    DataTable lo_datatable;
                    lo_cflEvent = (IChooseFromListEvent)s;
                    lo_choosefromlist = go_SBOForm.ChooseFromLists.Item(lo_cflEvent.ChooseFromListUID);
                    lo_datatable = lo_cflEvent.SelectedObjects; //Ingresa los valores de la lista en el DataTable
                    if (lo_datatable == null) throw new InvalidOperationException();
                    ls_cuenta = lo_datatable.GetValue("FormatCode", 0); //Recupera la cuenta
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_ACTIVATE, string.Empty, s =>
            {
                if (ls_cuenta != string.Empty)
                {
                    go_SBOFormEvent.GetEditText("txtCuenta").Value = ls_cuenta;
                    ls_cuenta = string.Empty;
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_RESIZE, string.Empty, s =>
            {
                if (!s.BeforeAction && go_SBOFormEvent.TypeEx == "134")
                {
                    go_SBOForm = go_SBOFormEvent;
                    switch (go_SBOForm.State)
                    {
                        case BoFormStateEnum.fs_Maximized:
                        case BoFormStateEnum.fs_Restore:
                            go_SBOForm.GetItem("277").Left = go_SBOForm.GetItem("258").Left;
                            go_SBOForm.GetItem("278").Left = go_SBOForm.GetItem("258").Left;

                            go_SBOForm.GetItem("294").SetPosition(
                                go_SBOForm.GetItem("277").Top + go_SBOForm.GetItem("277").Height + 2,
                                go_SBOForm.GetItem("258").Left);

                            go_SBOForm.GetItem("295").SetPosition(
                                go_SBOForm.GetItem("259").Top + (go_SBOForm.GetItem("259").Height * 3) + 1,
                                go_SBOForm.GetItem("259").Left);

                            if (!go_SBOForm.ItemExists("stAsDtr2")) return;

                            go_SBOForm.GetItem("stAsDtr2").SetPosition(
                                go_SBOForm.GetItem("294").Top + go_SBOForm.GetItem("294").Height + 50,
                                go_SBOForm.GetItem("294").Left);

                            go_SBOForm.GetItem("txtCuenta").SetPosition(
                                go_SBOForm.GetItem("295").Top + (go_SBOForm.GetItem("295").Height) + 50,
                                go_SBOForm.GetItem("295").Left);
                            break;
                    }
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_FORM_UNLOAD, string.Empty, s => Dispose()));

            itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "cbDeven", s =>
            {
                if (!s.BeforeAction)
                {
                    go_CheckBox = go_SBOFormEvent.Items.Item("cbDeven").Specific;

                    if (go_CheckBox.Checked == true)
                    {
                        go_Button = go_SBOFormEvent.Items.Item("btnDeven").Specific;
                        go_Button.Item.Enabled = true;
                    }
                    else
                    {
                        go_Button = go_SBOFormEvent.Items.Item("btnDeven").Specific;
                        go_Button.Item.Enabled = false;
                    }
                }
            }));

            itemevent.Add(new sapitemevent(BoEventTypes.et_ITEM_PRESSED, "btnDeven", s =>
            {
                if (!s.BeforeAction)
                {
                    if (s.FormMode != 3) // Modo actualizacion
                    {
                        Form ls_Proveedor = go_SBOApplication.Forms.Item(s.FormUID);
                        int li_docnum;
                        string numref;
                        string ls_tipo = string.Empty;

                        var dato = ls_Proveedor.DataSources.DBDataSources.Item("OPCH").GetValue("ObjType", 0);
                        if (dato == "18")
                        {
                            ls_tipo = "TT";
                        }

                        li_docnum = int.Parse(ls_Proveedor.Items.Item("8").Specific.Value); // Docnum de documento (Fact. Proveedor)
                        numref = ls_Proveedor.Items.Item("14").Specific.Value; // Docnum de documento (Fact. Proveedor)
                        // Abrir formulario de Contabilizacion periodida
                        go_SBOApplication.OpenForm(BoFormObjectEnum.fo_RecurringTransactions, string.Empty, go_SBOCompany.GetNewObjectKey());

                        ls_Proveedor = go_SBOApplication.Forms.ActiveForm;
                        ls_Proveedor.Mode = BoFormMode.fm_ADD_MODE;
                        // Asignar informacion a nuevo formulario
                        //EditText li_codigo = x.Items.Item("3").Specific;
                        //li_codigo.Value = li_docnum.ToString();

                        EditText ls_ref1 = ls_Proveedor.Items.Item("19").Specific;
                        ls_ref1.Value = ls_tipo.ToString();

                        EditText ls_ref2 = ls_Proveedor.Items.Item("20").Specific;
                        ls_ref2.Value = numref.ToString();

                        EditText ls_ref3 = ls_Proveedor.Items.Item("540000098").Specific;
                        ls_ref3.Value = li_docnum.ToString();
                    }
                    else
                    {
                        Cls_Global.sb_msjStatusBarSAP("Esta opcion esta habilitado para documentos ya creados", BoStatusBarMessageType.smt_Error, go_SBOApplication);
                    }
                }
            }));
        }

        private void sb_validarBoton(MenuEvent po_Event)
        {
            if (!po_Event.BeforeAction)
            {
                go_CheckBox = go_SBOFormEvent.Items.Item("cbDeven").Specific;

                if (go_CheckBox.Checked == true)
                {
                    go_Button = go_SBOFormEvent.Items.Item("btnDeven").Specific;
                    go_Button.Item.Enabled = true;
                }
                else
                {
                    go_Button = go_SBOFormEvent.Items.Item("btnDeven").Specific;
                    go_Button.Item.Enabled = false;
                }
            }
        }

        #region Metodos del Negocio

        private void sb_cargarcontroles(dynamic po_Event)
        {
            if (!po_Event.BeforeAction)
            {
                go_SBOForm = go_SBOFormEvent;

                string ls_FormTypeEx = go_SBOForm.TypeEx;
                this.SetTableName(ls_FormTypeEx);

                switch (ls_FormTypeEx)
                {
                    case "141":
                    case "181":
                    case "65301":
                        if (go_SBOForm.ItemExists("stAsDtr")) return;
                        go_Item = go_SBOForm.Items.Add("stAsDtr", BoFormItemTypes.it_STATIC);
                        go_Item.FromPane = 7;
                        go_Item.ToPane = 7;
                        go_Item.SetDisplay(
                            go_SBOForm.GetItem("156").Width,
                            go_SBOForm.GetItem("156").Height,
                            go_SBOForm.GetItem("2010").Top,
                            go_SBOForm.GetItem("156").Left);

                        go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 9, BoModeVisualBehavior.mvb_False);
                        go_Item.Enabled = true;
                        go_Static = go_Item.Specific;
                        go_Static.Caption = "Asiento Detraccion";

                        go_Item = go_SBOForm.Items.Add("txAsDtr", BoFormItemTypes.it_EDIT);
                        go_Item.FromPane = 7;
                        go_Item.ToPane = 7;
                        go_Item.SetDisplay(
                            go_SBOForm.GetItem("157").Width,
                            go_SBOForm.GetItem("157").Height,
                            go_SBOForm.GetItem("2010").Top,
                            go_SBOForm.GetItem("157").Left);
                        go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                        go_Item.Enabled = false;
                        go_Edit = go_Item.Specific;
                        go_Edit.DataBind.SetBound(true, ls_tablename, "U_BPP_AstDetrac");

                        go_Item = go_SBOForm.Items.Add("lkAsDtr", BoFormItemTypes.it_LINKED_BUTTON);
                        go_Item.FromPane = 7;
                        go_Item.ToPane = 7;
                        go_Item.SetPosition(
                            go_SBOForm.GetItem("2010").Top,
                            go_SBOForm.GetItem("157").Left - 20);
                        go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 9, BoModeVisualBehavior.mvb_True);
                        go_Item.Enabled = true;
                        go_Item.LinkTo = "txAsDtr";

                        go_LinkButton = go_Item.Specific;
                        go_LinkButton.LinkedObject = BoLinkedObject.lf_JournalPosting;

                        go_Edit = go_SBOForm.GetItem("txAsDtr").Specific;
                        go_Edit.Value = string.Empty;

                        // ----- Devengado ---------

                        //if (go_SBOForm.ItemExists("btnDeven")) return;
                        ////Boton Devengado
                        //go_Item = go_SBOForm.Items.Add("btnDeven", BoFormItemTypes.it_BUTTON);
                        //go_Item.Width = 60;
                        //go_Item.Height = go_SBOForm.GetItem("498").Height;
                        //go_Item.FromPane = 7;
                        //go_Item.ToPane = 7;
                        //go_Item.SetPosition(
                        //    go_SBOForm.GetItem("498").Top + 15,
                        //    go_SBOForm.GetItem("498").Left);
                        //go_Item.Enabled = false;
                        //go_Button = go_Item.Specific;
                        //go_Button.Caption = "Devengado";
                        //go_Item.Visible = false;
                        //// go_Button.Item.Visible = false;

                        //go_Item = go_SBOForm.Items.Add("cbDeven", BoFormItemTypes.it_CHECK_BOX);
                        //go_Item.FromPane = 7;
                        //go_Item.ToPane = 7;
                        //go_Item.Width = 70;
                        //go_Item.Height = go_SBOForm.GetItem("2044").Height;
                        //go_Item.SetPosition(
                        //    go_SBOForm.GetItem("2044").Top + 15,
                        //    go_SBOForm.GetItem("2044").Left);

                        //go_Item.Enabled = true;

                        //go_CheckBox = go_Item.Specific;
                        //go_CheckBox.Caption = "Devengar";
                        //go_CheckBox.DataBind.SetBound(true, "OPCH", "U_STR_Devengar");

                        //go_SBOForm.PaneLevel = 1;

                        break;

                    case "134": //De lo contrario se genera en el formulario 134
                        if (go_SBOForm.ItemExists("stAsDtr2")) return;
                        go_Item = go_SBOForm.Items.Add("stAsDtr2", BoFormItemTypes.it_STATIC);
                        go_Item.FromPane = 10;
                        go_Item.ToPane = 10;
                        go_Item.Width = go_SBOForm.GetItem("294").Width;
                        go_Item.SetPosition(
                            go_SBOForm.GetItem("294").Top + go_SBOForm.GetItem("294").Height + 50,
                            go_SBOForm.GetItem("294").Left);

                        go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, (int)BoAutoFormMode.afm_Ok, BoModeVisualBehavior.mvb_False);
                        go_Item.Enabled = true;
                        go_Static = go_Item.Specific;
                        go_Static.Caption = "Cuenta asociada detracción";

                        go_Item = go_SBOForm.Items.Add("txtCuenta", BoFormItemTypes.it_EDIT);
                        go_Item.FromPane = 10;
                        go_Item.ToPane = 10;
                        go_Item.Width = go_SBOForm.GetItem("295").Width;
                        go_Item.SetPosition(
                            go_SBOForm.GetItem("295").Top + go_SBOForm.GetItem("295").Height + 50,
                            go_SBOForm.GetItem("295").Left);

                        go_Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
                        go_Item.Enabled = true;
                        go_Edit = go_Item.Specific;

                        ChooseFromListCreationParams cflCreationParams;
                        cflCreationParams = go_SBOApplication.CreateObject(BoCreatableObjectType.cot_ChooseFromListCreationParams);
                        cflCreationParams.MultiSelection = false;
                        cflCreationParams.UniqueID = "cflDetrac";
                        cflCreationParams.ObjectType = "1";
                        ChooseFromListCollection cflCollection;
                        cflCollection = go_SBOForm.ChooseFromLists;
                        cflCollection.Add(cflCreationParams);

                        go_Edit.DataBind.SetBound(true, "OCRD", "U_BPP_CtaDetrac");

                        go_Edit.ChooseFromListUID = "cflDetrac";
                        go_Edit.ChooseFromListAlias = "FormatCode";

                        break;
                }
            }
        }

        public void sb_generarAsientoDetraccion()
        {
            lc_NameMethod = "sb_generarAsientoDetraccion"; //Se asigna el nombre del método para la identificación del mismo
            try
            {
                if (Cls_Global.metCalculoTC == "O2") // no se crean asientos de detracción, se aplica a la factura
                    return;

                //Variables del metodo
                Documents lo_Doc;
                JournalEntries lo_Jdt;
                //JournalEntries lo_JdtDoc;
                //BusinessPartners lo_BPn;
                //int li_intCtdLineas = 0;
                //double ldb_SumDetrD = 0;
                //double ldb_SumDetrH = 0;
                //double ldb_SumDetrDME = 0;
                //double ldb_SumDetrCME = 0;
                string ls_Result;

                //go_RecordSet = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                go_SBOForm = go_SBOFormEvent;
                //Pregunta por el TypeEx del formulario
                switch (go_SBOForm.TypeEx)
                {
                    case "141":
                    case "60092":
                    case "65306":
                    case "frmCargarDocumentos":
                        lo_Doc = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseInvoices); break;
                    case "65301":
                        lo_Doc = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseDownPayments); break;
                    case "181":
                        lo_Doc = go_SBOCompany.GetBusinessObject(BoObjectTypes.oPurchaseCreditNotes); break;
                    default:
                        return;//controlar
                }

                lo_Jdt = ObtenerAsientoDetraccion(lo_Doc);

                //lo_Jdt = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
                //lo_JdtDoc = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
                //lo_BPn = go_SBOCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                //string ls_DocEntry =go_SBOForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0);
                //lo_Doc.GetByKey(int.Parse(ls_DocEntry));

                //if (lo_Doc.WithholdingTaxData.Count > 0)
                //{
                //    lo_Doc.WithholdingTaxData.SetCurrentLine(0);
                //    lo_JdtDoc.GetByKey(lo_Doc.TransNum);
                //    lo_BPn.GetByKey(lo_Doc.CardCode);

                //    //Si WTCode es diferente de "D" (Inicial del codigo de detraccion) no continua
                //    if (!lo_Doc.WithholdingTaxData.WTCode.StartsWith("D"))
                //    {
                //        ExceptionPrepared.inner(
                //            internalexception.TCatch.Info,
                //            "Para generar el asiento de detracción debe seleccionar el código  al crear la factura (formulario de retención de impuestos).",
                //            lc_NameMethod);
                //        ExceptionPrepared.SaveInLog();
                //        return;
                //    }

                //    //Ejecuto el Procedimiento en el go_recordSet
                //    go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_NumeroDias);

                //    lo_Jdt.ReferenceDate = lo_Doc.DocDate;
                //    DateTime ld_FchVencAs;
                //    if (lo_Doc.DocDueDate.Month == 12)
                //        ld_FchVencAs = lo_Doc.DocDueDate.AddYears(1).AddMonths(-lo_Doc.DocDueDate.Month + 1).AddDays(-lo_Doc.DocDueDate.Day + go_RecordSet.Fields.Item("NDiasDetrac").Value);
                //    else
                //        ld_FchVencAs = lo_Doc.DocDueDate.AddMonths(1).AddDays(-lo_Doc.DocDueDate.Day + go_RecordSet.Fields.Item("NDiasDetrac").Value);

                //    //Llenando las propiedades del JournalEntries
                //    lo_Jdt.DueDate = ld_FchVencAs;
                //    lo_Jdt.TaxDate = lo_Doc.TaxDate;
                //    lo_Jdt.Reference = lo_JdtDoc.Reference;
                //    lo_Jdt.Reference2 = lo_Doc.UserFields.Fields.Item("U_BPP_MDTD").Value + "-" + lo_Doc.UserFields.Fields.Item("U_BPP_MDSD").Value + "-" + lo_Doc.UserFields.Fields.Item("U_BPP_MDCD").Value;
                //    lo_Jdt.Reference3 = lo_Doc.WithholdingTaxData.WTCode;
                //    lo_Jdt.Memo = "Detraccion Factura - " + lo_Doc.UserFields.Fields.Item("U_BPP_MDTD").Value + "-" + lo_Doc.UserFields.Fields.Item("U_BPP_MDSD").Value + "-" + lo_Doc.UserFields.Fields.Item("U_BPP_MDCD").Value;
                //    lo_Jdt.TransactionCode = "DTR";
                //    lo_Jdt.UserFields.Fields.Item("U_BPP_DocKeyDest").Value = lo_Doc.DocEntry.ToString();
                //    lo_Jdt.UserFields.Fields.Item("U_BPP_CtaTdoc").Value = lo_Doc.DocObjectCodeEx.ToString();
                //    lo_Jdt.UserFields.Fields.Item("U_BPP_SubTDoc").Value = lo_Doc.DocumentSubType.ToString();

                //    while (li_intCtdLineas < lo_JdtDoc.Lines.Count)
                //    {
                //        lo_JdtDoc.Lines.SetCurrentLine(li_intCtdLineas);
                //        if (lo_JdtDoc.Lines.AccountCode == lo_Doc.WithholdingTaxData.GLAccount)
                //        {
                //            ldb_SumDetrD += lo_JdtDoc.Lines.Debit;
                //            ldb_SumDetrH += lo_JdtDoc.Lines.Credit;

                //            //if ((lo_JdtDoc.Lines.FCDebit + lo_JdtDoc.Lines.FCCredit) > 0)
                //            //{
                //            //    ldb_SumDetrDME += lo_JdtDoc.Lines.FCDebit;
                //            //    ldb_SumDetrCME += lo_JdtDoc.Lines.FCCredit;
                //            //}
                //        }
                //        li_intCtdLineas++;
                //    }

                //    if ((ldb_SumDetrD + ldb_SumDetrH) == 0) return;

                //    lo_Jdt.Lines.AccountCode = lo_Doc.WithholdingTaxData.GLAccount;
                //    lo_Jdt.Lines.Debit = ldb_SumDetrH;
                //    lo_Jdt.Lines.Credit = ldb_SumDetrD;
                //    // Ingresa cuando la moneda es Dolares
                //    //if ((ldb_SumDetrDME + ldb_SumDetrCME) > 0)
                //    //{
                //    //    lo_Jdt.Lines.FCDebit = ldb_SumDetrCME;
                //    //    lo_Jdt.Lines.FCCredit = ldb_SumDetrDME;
                //    //    lo_Jdt.Lines.FCCurrency = lo_Doc.DocCurrency;
                //    //}
                //    lo_Jdt.Lines.Add();

                //    //Ejecuto el Procedimiento en el go_recordSet
                //    go_RecordSet = Cls_QueryManager.Retorna(
                //        Cls_Query.get_CodigoFormato, null,
                //        lo_BPn.UserFields.Fields.Item("U_BPP_CtaDetrac").Value);

                //    lo_Jdt.Lines.AccountCode = go_RecordSet.Fields.Item("AcctCode").Value;
                //    lo_Jdt.Lines.ShortName = lo_BPn.CardCode;
                //    lo_Jdt.Lines.Debit = ldb_SumDetrD;
                //    lo_Jdt.Lines.Credit = ldb_SumDetrH;

                //    // Ingresa cuando la moneda es Dolares
                //    //if ((ldb_SumDetrDME + ldb_SumDetrCME) > 0)
                //    //{
                //    //    lo_Jdt.Lines.FCDebit = ldb_SumDetrDME;
                //    //    lo_Jdt.Lines.FCCredit = ldb_SumDetrCME;
                //    //    lo_Jdt.Lines.FCCurrency = lo_Doc.DocCurrency;
                //    //}

                //    lo_Jdt.Lines.Add();
                ls_Result = lo_Jdt.Add().ToString();//.ToString();

                if (ls_Result != "0") //Si el resultado indica que la operacion no fue exitosa, muestra un mensaje
                {
                    Cls_Global.sb_msjStatusBarSAP(Cls_Global.go_SBOCompany.GetLastErrorDescription(), BoStatusBarMessageType.smt_Warning, go_SBOApplication);
                    return;
                }

                string sKey = Cls_Global.go_SBOCompany.GetNewObjectKey();
                lo_Doc.UserFields.Fields.Item("U_BPP_AstDetrac").Value = sKey;
                ls_Result = lo_Doc.Update().ToString();

                if (ls_Result != "0") //Si el resultado indica que la operacion no fue exitosa, muestra un mensaje
                {
                    Cls_Global.sb_msjStatusBarSAP(Cls_Global.go_SBOCompany.GetLastErrorDescription(), BoStatusBarMessageType.smt_Warning, go_SBOApplication);
                    return;
                }
                // }
            }
            catch (Exception ex)
            {
                ExceptionPrepared.inner(ex.Message, lc_NameMethod);
                ExceptionPrepared.SaveInLog(false);
            } //Método para el manejo de las operaciones de Log
        }

        private JournalEntries ObtenerAsientoDetraccion(Documents FacturaProveedores)
        {
            try
            {
                JournalEntries oAsientoDetraccion = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);

                JournalEntries lo_JdtDoc = go_SBOCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
                BusinessPartners lo_BPn = go_SBOCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                string docEntry = go_SBOForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0);
                FacturaProveedores.GetByKey(int.Parse(docEntry));

                lo_JdtDoc.GetByKey(FacturaProveedores.TransNum); // ASIENTO DE FP
                lo_BPn.GetByKey(FacturaProveedores.CardCode); // PROVEEDOR

                SetearValoresCabecera(ref oAsientoDetraccion, ref FacturaProveedores, ref lo_JdtDoc);
                SetearValoresDetalle(ref oAsientoDetraccion, ref FacturaProveedores, ref lo_JdtDoc, ref lo_BPn);

                return oAsientoDetraccion;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetearValoresCabecera(ref JournalEntries oAsientoDetraccion, ref Documents facturaProveedores, ref JournalEntries lo_JdtDoc)
        {
            try
            {
                //Ejecuto el Procedimiento en el go_recordSet
                go_RecordSet = Cls_QueryManager.Retorna(Cls_Query.get_NumeroDias);

                oAsientoDetraccion.ReferenceDate = facturaProveedores.DocDate;
                DateTime ld_FchVencAs;
                if (facturaProveedores.DocDueDate.Month == 12)
                    ld_FchVencAs = facturaProveedores.DocDueDate.AddYears(1).AddMonths(-facturaProveedores.DocDueDate.Month + 1).AddDays(-facturaProveedores.DocDueDate.Day + go_RecordSet.Fields.Item("NDiasDetrac").Value);
                else
                    ld_FchVencAs = facturaProveedores.DocDueDate.AddMonths(1).AddDays(-facturaProveedores.DocDueDate.Day + go_RecordSet.Fields.Item("NDiasDetrac").Value);

                //Llenando las propiedades del JournalEntries
                oAsientoDetraccion.DueDate = ld_FchVencAs;
                oAsientoDetraccion.TaxDate = facturaProveedores.TaxDate;
                oAsientoDetraccion.Reference = lo_JdtDoc.Reference;
                oAsientoDetraccion.Reference2 = facturaProveedores.UserFields.Fields.Item("U_BPP_MDTD").Value + "-" + facturaProveedores.UserFields.Fields.Item("U_BPP_MDSD").Value + "-" + facturaProveedores.UserFields.Fields.Item("U_BPP_MDCD").Value;
                oAsientoDetraccion.Reference3 = facturaProveedores.WithholdingTaxData.WTCode;
                oAsientoDetraccion.Memo = "Detraccion Factura - " + facturaProveedores.UserFields.Fields.Item("U_BPP_MDTD").Value + "-" + facturaProveedores.UserFields.Fields.Item("U_BPP_MDSD").Value + "-" + facturaProveedores.UserFields.Fields.Item("U_BPP_MDCD").Value;
                oAsientoDetraccion.TransactionCode = "DTR";
                oAsientoDetraccion.UserFields.Fields.Item("U_BPP_DocKeyDest").Value = facturaProveedores.DocEntry.ToString();
                oAsientoDetraccion.UserFields.Fields.Item("U_BPP_CtaTdoc").Value = facturaProveedores.DocObjectCodeEx.ToString();
                oAsientoDetraccion.UserFields.Fields.Item("U_BPP_SubTDoc").Value = facturaProveedores.DocumentSubType.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetearValoresDetalle(ref JournalEntries oAsientoDetraccion, ref Documents facturaProveedores, ref JournalEntries lo_JdtDoc, ref BusinessPartners lo_BPn)
        {
            //if (Cls_Global.metCalculoTC == "O2") // OPCIÓN 2
            //{
            //    double tasaDetraccion = double.Parse(facturaProveedores.UserFields.Fields.Item("U_STR_TasaDTR").Value.ToString());

            //    //DEBIT
            //    oAsientoDetraccion.Lines.AccountCode = "_SYS00000044339"; //DEBUG ONLY
            //    //oAsientoDetraccion.Lines.ShortName = lo_BPn.CardCode;
            //    //oAsientoDetraccion.Lines.Debit = Math.Round((facturaProveedores.DocTotal - facturaProveedores.VatSum) * (tasaDetraccion / 100), 0);

            //    oAsientoDetraccion.Lines.Debit = Math.Round((facturaProveedores.DocTotal) * (tasaDetraccion / 100), 0);

            //    oAsientoDetraccion.Lines.Add();

            //    //CREDIT
            //    //Ejecuto el Procedimiento en el go_recordSet
            //    go_RecordSet = Cls_QueryManager.Retorna(
            //        Cls_Query.get_CodigoFormato, null,
            //        lo_BPn.UserFields.Fields.Item("U_BPP_CtaDetrac").Value);

            //    oAsientoDetraccion.Lines.AccountCode = go_RecordSet.Fields.Item("AcctCode").Value;
            //    oAsientoDetraccion.Lines.ShortName = lo_BPn.CardCode;

            //    //oAsientoDetraccion.Lines.Credit = Math.Round((facturaProveedores.DocTotal - facturaProveedores.VatSum) * (tasaDetraccion / 100), 0);
            //    oAsientoDetraccion.Lines.Credit = Math.Round((facturaProveedores.DocTotal) * (tasaDetraccion / 100), 0);

            //    oAsientoDetraccion.Lines.Add();
            //}
            //else
            //{

            SAPbobsCOM.Recordset recordset = null; // Nuevo
            string Sucursal = string.Empty;
            // ------ Validar SI Sociedad tiene configuracion de Sucursales
            recordset = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string query = $"SELECT \"MltpBrnchs\" FROM OADM";
            recordset.DoQuery(query);
            Sucursal = recordset.Fields.Item(0).Value;
            //-------------------------------------------------------


            if (facturaProveedores.WithholdingTaxData.Count > 0) //FUNCIÓN ESTÁNDAR Y OPCIÓN 1
            {
                int li_intCtdLineas = 0;
                double ldb_SumDetrD = 0;
                double ldb_SumDetrH = 0;
                double ldb_SumDetrDME = 0;
                double ldb_SumDetrCME = 0;

                facturaProveedores.WithholdingTaxData.SetCurrentLine(0);

                //Si WTCode es diferente de "D" (Inicial del codigo de detraccion) no continua
                if (!facturaProveedores.WithholdingTaxData.WTCode.StartsWith("D"))
                {
                    ExceptionPrepared.inner(
                        internalexception.TCatch.Info,
                        "Para generar el asiento de detracción debe seleccionar el código  al crear la factura (formulario de retención de impuestos).",
                        lc_NameMethod);

                    ExceptionPrepared.SaveInLog();
                    return;
                }

                while (li_intCtdLineas < lo_JdtDoc.Lines.Count)
                {
                    lo_JdtDoc.Lines.SetCurrentLine(li_intCtdLineas);
                    if (lo_JdtDoc.Lines.AccountCode == facturaProveedores.WithholdingTaxData.GLAccount)
                    {
                        ldb_SumDetrD += lo_JdtDoc.Lines.Debit;
                        ldb_SumDetrH += lo_JdtDoc.Lines.Credit;
                        if (Cls_Global.metCalculoTC == "E") // Opcion Standar
                        {
                            if ((lo_JdtDoc.Lines.FCDebit + lo_JdtDoc.Lines.FCCredit) > 0)
                            {
                                ldb_SumDetrDME += lo_JdtDoc.Lines.FCDebit;
                                ldb_SumDetrCME += lo_JdtDoc.Lines.FCCredit;
                            }
                        }
                    }
                    li_intCtdLineas++;
                }

                if ((ldb_SumDetrD + ldb_SumDetrH) == 0) return;

                oAsientoDetraccion.Lines.AccountCode = facturaProveedores.WithholdingTaxData.GLAccount;
                oAsientoDetraccion.Lines.Debit = ldb_SumDetrH;
                oAsientoDetraccion.Lines.Credit = ldb_SumDetrD;
                if (Cls_Global.metCalculoTC == "E") // Opcion Standar
                {  // Ingresa cuando la moneda es Dolares
                    if ((ldb_SumDetrDME + ldb_SumDetrCME) > 0)
                    {
                        oAsientoDetraccion.Lines.FCDebit = ldb_SumDetrCME;
                        oAsientoDetraccion.Lines.FCCredit = ldb_SumDetrDME;
                        oAsientoDetraccion.Lines.FCCurrency = facturaProveedores.DocCurrency;
                    }
                }

                if (Sucursal.Equals("Y"))// Agregado 21/01/2022
                    oAsientoDetraccion.Lines.BPLID = facturaProveedores.BPL_IDAssignedToInvoice;
                oAsientoDetraccion.Lines.Reference2 = facturaProveedores.NumAtCard; // Agregado 25/01/2022

                oAsientoDetraccion.Lines.Add(); // Linea 1

                //Ejecuto el Procedimiento en el go_recordSet
                go_RecordSet = Cls_QueryManager.Retorna(
                    Cls_Query.get_CodigoFormato, null,
                    lo_BPn.UserFields.Fields.Item("U_BPP_CtaDetrac").Value);

                oAsientoDetraccion.Lines.AccountCode = go_RecordSet.Fields.Item("AcctCode").Value;
                oAsientoDetraccion.Lines.ShortName = lo_BPn.CardCode;
                oAsientoDetraccion.Lines.Debit = ldb_SumDetrD;
                oAsientoDetraccion.Lines.Credit = ldb_SumDetrH;
                if (Cls_Global.metCalculoTC == "E") // Opcion Standar
                {
                    // Ingresa cuando la moneda es Dolares
                    if ((ldb_SumDetrDME + ldb_SumDetrCME) > 0)
                    {
                        oAsientoDetraccion.Lines.FCDebit = ldb_SumDetrDME;
                        oAsientoDetraccion.Lines.FCCredit = ldb_SumDetrCME;
                        oAsientoDetraccion.Lines.FCCurrency = facturaProveedores.DocCurrency;
                    }
                }

                if (Sucursal.Equals("Y"))// Agregado 21/01/2022
                    oAsientoDetraccion.Lines.BPLID = facturaProveedores.BPL_IDAssignedToInvoice;

                oAsientoDetraccion.Lines.Reference2 = facturaProveedores.NumAtCard; // Agregado 25/01/2022
                oAsientoDetraccion.Lines.Add(); // Linea 2
            }
            //}
        }

        #endregion Metodos del Negocio
    }
}