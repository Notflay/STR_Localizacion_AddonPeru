using SAPbobsCOM;
using SAPbouiCOM;
using STR_Localizacion.DL;
using STR_Localizacion.UTIL;
using System;

namespace STR_Localizacion.UI
{
    public partial class Cls_ReprocesarAsientoProvision : Cls_PropertiesControl
    {
        public Cls_ReprocesarAsientoProvision()
        {
            gs_FormName = "FrmAsientosProvision";
            gs_FormPath = "Resources/Localizacion/FrmAsientosProvision.srf";
            lc_NameClass = "Cls_ReprocesarAsientoProvision";
        }

        internal void sb_FormLoad()
        {
            try
            {
                if (go_SBOForm == null)
                {
                    go_SBOForm = Cls_Global.fn_CreateForm(gs_FormName, gs_FormPath);
                    ExceptionPrepared = new internalexception(lc_NameClass, lc_NameLayout);
                    if (!sb_DataFormLoad())
                        return;

                    InitializeEvents();
                }
                go_SBOForm.Visible = true;
            }
            catch (Exception exc)
            {
                go_SBOApplication.SetStatusBarMessage(exc.Message, BoMessageTime.bmt_Short, true);
            }
        }

        private bool sb_DataFormLoad()
        {
            go_SBOForm.Left = (go_SBOApplication.Desktop.Width - go_SBOForm.Width) / 2;
            go_SBOForm.Top = (go_SBOApplication.Desktop.Height - go_SBOForm.Height) / 2 - 100;

            AplicarFiltroChosseFromListClientes();
            CargarSeries();
            SetDocNum();
            CargarComboUsuarios();

            ColocarValoresPorDefecto();
            go_SBOForm.Visible = true;

            return true;
        }

        private void ColocarValoresPorDefecto()
        {
            go_SBOForm.SetHeaderDBValue("@ST_LC_OASPR", "U_ST_LC_USU", go_SBOCompany.UserSignature.ToString());
            go_SBOForm.SetHeaderDBValue("@ST_LC_OASPR", "U_ST_LC_FECHA", go_SBOCompany.GetDBServerDate().Date.ToString("yyyyMMdd"));
        }

        private void CargarSeries()
        {
            go_Combo = go_SBOForm.GetComboBox("Item_23");
            go_Combo.ValidValues.LoadSeries("BPP_LC_ASPROV", BoSeriesMode.sf_Add);
            go_Combo.SelectExclusive(0, BoSearchKey.psk_Index);
        }

        private void SetDocNum()
        {
            string serie = go_SBOForm.GetComboBox("Item_23").Value;
            go_SBOForm.SetHeaderDBValue("@ST_LC_OASPR", "DocNum", go_SBOForm.BusinessObject.GetNextSerialNumber(serie).ToString());
        }

        private void CargarComboUsuarios()
        {
            try
            {
                go_Combo = go_SBOForm.GetComboBox("Item_2");
                Recordset oRs = Cls_QueryManager.Retorna(Cls_Query.get_Usuarios);

                while (go_Combo.ValidValues.Count > 0)
                    go_Combo.ValidValues.Remove(0, BoSearchKey.psk_Index);

                while (!oRs.EoF)
                {
                    go_Combo.ValidValues.Add(oRs.Fields.Item(0).Value.ToString(), oRs.Fields.Item(1).Value.ToString());
                    oRs.MoveNext();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void AplicarFiltroChosseFromListClientes()
        {
            var cfl = go_SBOForm.ChooseFromLists.Item("CFL_OCRD1");
            cfl.SetConditions(null);
            var cnds = cfl.GetConditions();
            var cnd = cnds.Add();
            cnd.Alias = "CardType";
            cnd.Operation = BoConditionOperation.co_EQUAL;
            cnd.CondVal = "S";

            cnd.Relationship = BoConditionRelationship.cr_AND;

            cnd = cnds.Add();
            cnd.Alias = "frozenFor";
            cnd.Operation = BoConditionOperation.co_EQUAL;
            cnd.CondVal = "N";

            cfl.SetConditions(cnds);

            cfl = go_SBOForm.ChooseFromLists.Item("CFL_OCRD2");
            cfl.SetConditions(null);
            cfl.SetConditions(cnds);
        }

        internal void LoadAction()
        {
            go_Matrix = go_SBOForm.GetMatrix("Item_6");
            go_Matrix.Columns.Item("Col_0").Visible = false;

            go_SBOForm.GetItem("Item_23").Enabled = false;
            go_SBOForm.GetItem("Item_8").Enabled = false;
            go_SBOForm.GetItem("Item_9").Enabled = false;
            go_SBOForm.GetItem("Item_18").Enabled = false;
            go_SBOForm.GetItem("Item_19").Enabled = false;
            go_SBOForm.GetItem("Item_20").Enabled = false;
            go_SBOForm.GetItem("Item_21").Enabled = false;

            go_SBOForm.GetItem("Item_4").Visible = false;
            go_Matrix.AutoResizeColumns();
        }
    }
}