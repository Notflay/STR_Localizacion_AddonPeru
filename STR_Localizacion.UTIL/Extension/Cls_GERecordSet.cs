
using System.Collections.Generic;

namespace STR_Localizacion.UTIL
{
    public delegate T SAPRecodSetData<T>(Dictionary<string, string> dc);

    public static class Cls_GERecordSet
    {
        public static dynamic GetValue(this SAPbobsCOM.Recordset rs, string item)
        {
            return rs.Fields.Item(item).Value;
        }

        public static dynamic GetValue(this SAPbobsCOM.Recordset rs, int item)
        {
            return rs.Fields.Item(item).Value;
        }

        public static List<T> ToList<T>(this SAPbobsCOM.Recordset rs, SAPRecodSetData<T> rd)
        {
            try
            {
                Dictionary<string, string> dcValues;
                List<T> lstValues = new List<T>();
                while (!rs.EoF)
                {
                    dcValues = new Dictionary<string, string>();
                    for (int i = 0; i < rs.Fields.Count; i++)
                        dcValues[rs.Fields.Item(i).Name] = rs.GetValue(i).ToString().Trim();
                    lstValues.Add(rd(dcValues));
                    rs.MoveNext();
                }
                return lstValues;
            }
            catch { throw; }
        }
    }
}
