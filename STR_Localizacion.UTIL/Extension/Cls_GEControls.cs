using System.Linq;
using SAPbouiCOM;

namespace STR_Localizacion.UTIL
{
    public static class Cls_GEControls
    {

        public static bool ItemExists(this Form form, string controlname)
        {
            return form.Items.OfType<Item>().Any(s => s.UniqueID.Equals(controlname));
        }

        public static BoFormItemTypes GetItemType(this Form form, string controlname)
        {
            return form.Items.Item(controlname).Type;
        }

        public static Item GetItem(this Form form, string controlname)
        {
            return form.Items.Item(controlname);
        }

        public static void SetEnabledControl(this Form f, bool value, params string[] controlname)
        {
            for (int i = 0; i < controlname.Length; i++)
                f.GetControl(controlname[i]).Enabled = value;
        }

        public static void SetVisibleControl(this Form f, bool value, params string[] controlname)
        {
            for (int i = 0; i < controlname.Length; i++)
            {
                var item = f.Items.Item(controlname[i]);
                if (item != null)
                    item.Visible = value;
            }
        }

        public static void SetSize(this Item it, int Width, int Height)
        {
            it.Width = Width;
            it.Height = Height;
        }

        public static void SetPosition(this Item it, int Top, int Left)
        {
            it.Top = Top;
            it.Left = Left;
        }


        /// <summary>
        /// Permite establecer las propiedades de tamaño y margen
        /// </summary>
        /// <param name="it">Item</param>
        /// <param name="Width">Especificar ancho</param>
        /// <param name="Height">Especificar alto</param>
        /// <param name="Top">Especificar margen superior</param>
        /// <param name="Left">Especificar margen izquierdo</param>
        /// <param name="DisplayDesc">
        /// True: Muestra la descripción del valor válido.
        /// <para />False: Muestra el valor válido.
        /// </param>
        public static void SetDisplay(this Item it, int Width, int Height, int Top, int Left, bool DisplayDesc = false)
        {
            it.Width = Width;
            it.Height = Height;
            it.Top = Top;
            it.Left = Left;
            it.DisplayDesc = DisplayDesc;
        }

        #region Obtener controles
        /// <summary>
        /// Obtiene un control de tipo dinamico, es decir, retornará el tipo del mismo
        /// <para />Será usado cuando el control sea asignado a una variable con el mismo tipo
        /// <para />Ejemplo:
        /// <para />SAPbouiCOM.ComboBox var_name = form.GetControl(name_combobox)
        /// </summary>
        /// <param name="controlname">nombre del control que será buscado</param>
        /// <returns></returns>
        public static dynamic GetControl(this Form form, string controlname)
        {
            return form.Items.Item(controlname).Specific;
        }

        public static StaticText GetStaticText(this Form form, string controlname)
        {
            return (StaticText)form.Items.Item(controlname).Specific;
        }
        public static ComboBox GetComboBox(this Form form, string controlname)
        {
            return (ComboBox)form.Items.Item(controlname).Specific;
        }
        public static CheckBox GetCheckBox(this Form form, string controlname)
        {
            return (CheckBox)form.Items.Item(controlname).Specific;
        }
        public static EditText GetEditText(this Form form, string controlname)
        {
            return (EditText)form.Items.Item(controlname).Specific;
        }
        public static Button GetButton(this Form form, string controlname)
        {
            return (Button)form.Items.Item(controlname).Specific;
        }
        public static LinkedButton GetLinkedButton(this Form form, string controlname)
        {
            return (LinkedButton)form.Items.Item(controlname).Specific;
        }
        public static OptionBtn GetOptionButton(this Form form, string controlname)
        {
            return (OptionBtn)form.Items.Item(controlname).Specific;
        }
        public static Grid GetGrid(this Form form, string controlname)
        {
            return (Grid)form.Items.Item(controlname).Specific;
        }
        public static Matrix GetMatrix(this Form form, string controlname)
        {
            return (Matrix)form.Items.Item(controlname).Specific;
        }

        public static string GetHeaderDBValue(this Form form, string datasource, string fieldID)
        {
            return form.DataSources.DBDataSources.Item(datasource).GetValue(fieldID, 0).Trim();
        }

        public static string GetBodyDBValue(this Form form, string datasource, string fieldID, int nroRow)
        {
            return form.DataSources.DBDataSources.Item(datasource).GetValue(fieldID, nroRow).Trim();
        }


        public static void SetHeaderDBValue(this Form form, string datasource, string fieldID, string value)
        {
            form.DataSources.DBDataSources.Item(datasource).SetValue(fieldID, 0, value);
        }

        public static void SetBodyDBValue(this Form form, string datasource, string fieldID, int nroRow,string value)
        {
            form.DataSources.DBDataSources.Item(datasource).SetValue(fieldID, nroRow,value);
        }


        #endregion
    }
}
