using STR_Localizacion.UTIL;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace STR_Localizacion.DL
{
    public static class Cls_QueryManager
    {
        private static SAPbobsCOM.Recordset go_RecSet = null;

        private static bool ishana
        {
            get { return (Cls_Global.go_ServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB); }
        }

        /// <summary>
        /// Ejecuta un procedimiento
        /// </summary>
        /// <param name="query_resource">Nombre del recurso a utilizar</param>
        /// <param name="prms">Parametros del recurso a ejecutar</param>
        /// <returns>Retorna el true cuando el proceso se ha realizado con éxito, caso contrario, false.</returns>
        public static bool Procesa(string query_resource, params object[] prms)
        {
            try
            {
                go_RecSet = Cls_Global.go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                go_RecSet.DoQuery(string.Format(Generar(query_resource), prms));
                return true;
            }
            catch (Exception ex)
            {
                Cls_Global.go_SBOApplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
            finally { go_RecSet = null; }
        }

        /// <summary>
        /// <parm>Ejecuta una funcion, procedimiento con resultado o consulta</parm>
        /// <parm>field, necesita ser especificado si el query tiene parametros</parm>
        /// <parm>field, necesita ser integer, string o null;</parm>
        /// <parm>generará una excepción de no ser así</parm>
        /// </summary>
        /// <param name="query_resource">Nombre del recurso a utilizar</param>
        /// <param name="field">Campo dentro a retornar</param>
        /// <param name="prms">Parametros del recurso a ejecutar</param>
        /// <returns>Cuando field es null, retorna una lista, caso contrario el valor del campo especificado</returns>
        public static dynamic Retorna(string query_resource, object field = null, params object[] prms)
        {
            try
            {/*
                if (field != null &&
                    (field.GetType() != typeof(string) || field.GetType() != typeof(Int32)))
                    throw new ArgumentException("El tipo de dato, del parámetro 'field' es invalido");*/

                go_RecSet = Cls_Global.go_SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                go_RecSet.DoQuery(string.Format(Generar(query_resource), prms));

                if (field == null)
                    return go_RecSet;
                else
                    return go_RecSet.Fields.Item(field).Value;
            }
            catch (Exception ex)
            {
                Cls_Global.go_SBOApplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return null;
            }
            finally { go_RecSet = null; }
        }

        public static void Consulta(this SAPbouiCOM.DataTable dt, string query_resource, params object[] prms)
        {
            try
            {
                var consul = string.Format(Generar(query_resource), prms);
                dt.ExecuteQuery(consul);
            }
            catch (Exception ex)
            {
                Cls_Global.go_SBOApplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }

        internal static string Generar(string sql)
        {
            var xdoc = new XDocument();
            using (StringReader s = new StringReader(Properties.Resources.Queries))
                xdoc = XDocument.Load(s);

            XElement xQRY = xdoc.
                Descendants("query").
                FirstOrDefault(s => s.Attribute("nameid").Value.Equals(sql));

            if (xQRY != null)
            {
                switch (xQRY.Attribute("definition").Value)
                {
                    case "D":
                        return xQRY.Element(ishana ? "hana" : "tsql").Value;

                    case "I":
                        return xQRY.Element("sql").Value;

                    case "P":
                        string query = xQRY.Element("sql").Value;

                        string parms = "";
                        if (xQRY.Element("params") != null)
                            parms = xQRY.Element("params").Value;

                        if (ishana)
                        {
                            string llamada = ("CALL " + query + " (" + parms + ")").Trim(); 
                            return ("CALL " + query + " (" + parms + ")").Trim();
                        }
                           
                        else
                            return ("EXEC " + query + " " + parms).Trim();
                }
            }

            return string.Empty;
        }
    }
}