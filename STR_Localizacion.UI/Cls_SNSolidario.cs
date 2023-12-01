using STR_Localizacion.UTIL;
using System;

namespace STR_Localizacion.UI
{
    internal class Cls_SNSolidario : Cls_PropertiesControl
    {
        private SAPbobsCOM.BusinessPartners businessPartner = null;
        private SAPbobsCOM.Documents document = null;
        private string snInvoice = string.Empty;
        private Cls_SBOMessageBox sboMessageBox = null;

        public Cls_SBOMessageBox SetSBOMessageBox { set => sboMessageBox = value; }

        public void formLoad()
        {
            itemevent.Add(new sapitemevent(SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED, "1", e =>
            {
                if (e.BeforeAction)
                {
                    var form = go_SBOApplication.Forms.Item(e.FormUID);
                    updateSNConsolidating(form);
                }
                else
                    cleanSNConsolidation();
            }));
        }

        private bool updateSNConsolidating(SAPbouiCOM.Form form)
        {
            try
            {
                document = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                businessPartner = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
                var snConsolidating = form.GetEditText("U_Solidario").Value;
                snInvoice = form.GetEditText("4").Value;
                if (!string.IsNullOrEmpty(snConsolidating) && snConsolidating != snInvoice)
                {
                    if (businessPartner.GetByKey(snInvoice))
                    {
                        form.Freeze(true);
                        businessPartner.FatherCard = snConsolidating;
                        businessPartner.FatherType = SAPbobsCOM.BoFatherCardTypes.cPayments_sum;
                        if (businessPartner.Update() != 0)
                            throw new InvalidOperationException($"{go_SBOCompany.GetLastErrorCode()} - {go_SBOCompany.GetLastErrorDescription()}");
                        sboMessageBox.ApplyAction = true;
                        sboMessageBox.Answer = false;
                        form.GetEditText("4").Value = "C99999999999";
                        form.GetItem("54").Click(SAPbouiCOM.BoCellClickType.ct_Regular);
                        form.GetEditText("4").Value = snInvoice;
                        form.GetItem("54").Click(SAPbouiCOM.BoCellClickType.ct_Regular);
                        sboMessageBox.ApplyAction = false;
                        return true;
                    }
                    throw new InvalidOperationException($"No se pudo recuperar informacion de SN con ID: {snInvoice}");
                }
                return false;
            }
            catch { throw; }
            finally { form.Freeze(false); }
        }

        private bool cleanSNConsolidation()
        {
            try
            {
                businessPartner = go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
                businessPartner.GetByKey(snInvoice);
                businessPartner.FatherCard = string.Empty;
                if (businessPartner.Update() != 0)
                    throw new InvalidOperationException($"{go_SBOCompany.GetLastErrorCode()} - {go_SBOCompany.GetLastErrorDescription()}");
                return true;
            }
            catch { throw; }
        }
    }
}