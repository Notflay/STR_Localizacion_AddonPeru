using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Tesseract;

namespace STR_Localizacion.DL
{
    public class Cls_Sunat
    {
        private static CookieContainer oCookie;
        public static string Ruc { get; private set; }
        public static string RazonSocial { get; private set; }
        public static string TipoContribuyente { get; private set; }
        public static string Direccion { get; private set; }
        public static string NroCalle { get; private set; }
        public static string Estado { get; private set; }
        public static string Habido { get; private set; }
        public static string Telefono { get; private set; }
        public static string Distrito { get; private set; }
        public static string Provincia { get; private set; }
        public static string Departamento { get; private set; }
        public static string AgRetencion { get; private set; }
        public static string AgPercepcion { get; private set; }
        public static string BuenContrib { get; private set; }
        public static string MensajeError { get; private set; }

        private Resul state;

        public enum Resul { Ok = 0, NoResul = 1, ErrorCapcha = 2, Error = 3 }

        public static int ObtenerDatosDesdeSUNAT(string ruc)
        {
            try
            {
                string captcha = LeerCaptcha();
                LlenarDatos(ruc, captcha);
            }
            catch (Exception e)
            {
                MensajeError = e.Message;
                return -1;
            }
            return 0;
        }

        private static void LlenarDatos(string ruc, string captcha)
        {
            string mensajeError = string.Empty;

            try
            {
                string response = ObtenerResponseSUNAT(ruc, captcha);
                string[] tables = response.Split(new string[] { "<table border=\"1\" cellpadding=\"2\" cellspacing=\"3\" width=\"100%\" class=\"form-table\">" }, StringSplitOptions.None);

                if (tables.Count() > 0)
                {
                    int indexCierre = tables[1].IndexOf("</table>", 0);
                    string xml = tables[1].Substring(0, indexCierre);
                    xml = "<table>" + xml + "</table>";
                    xml = xml.Replace("colspan=1", "").Replace("colspan=3", "").Replace("<br>", "");
                    XmlDocument oXml = new XmlDocument();
                    oXml.LoadXml(xml);

                    RazonSocial = ObtenerInformacionPorTag(oXml, "Número de RUC:");
                    ObtenerDatosDePadrones(ObtenerInformacionPorTag(oXml, "Padrones :"));
                    BuenContrib = ObtenerInformacionPorTag(oXml, "Estado del Contribuyente:");
                    Habido = ObtenerInformacionPorTag(oXml, "Condición del Contribuyente:");
                    Direccion = ObtenerInformacionPorTag(oXml, "Dirección del Domicilio Fiscal:");

                    SegmentarDireccion(Direccion);
                }
                else
                    throw new Exception("No existen resultados para la consulta");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static void ObtenerDatosDePadrones(string padrones)
        {
            if (padrones.ToUpper().Equals("NINGUNO"))
            {
                AgPercepcion = "N";
                AgRetencion = "N";
            }
            else
            {
                AgRetencion = padrones.Contains("Incorporado al Régimen de Agentes de Retención") ? "Y" : "N";
                AgPercepcion = padrones.Contains("Incorporado al Régimen de Agentes de Percepción") ? "Y" : "N";
            }
        }

        private static void SegmentarDireccion(string direccion)
        {
            try
            {
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                direccion = regex.Replace(direccion, " ");

                if (direccion.Equals("-"))
                {
                    Departamento = string.Empty;
                    Provincia = string.Empty;
                    Distrito = string.Empty;
                    Direccion = string.Empty;
                }
                else
                {
                    string[] array = direccion.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);

                    if (array.Length > 1)
                    {
                        int a = array.Length;
                        string DirTemp = array[a - 3].Trim();
                        DirTemp = DirTemp.TrimEnd(' ');
                        string[] ArrayDir = DirTemp.Split(' ');
                        int i = ArrayDir.Length;

                        Departamento = ArrayDir[i - 1].Trim();
                        Provincia = array[a - 2].Trim();
                        Distrito = array[a - 1].Trim();
                        Direccion = direccion.Substring(0, direccion.Length > 100 ? 100 : direccion.Length);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string ObtenerInformacionPorTag(XmlDocument xml, string tag)
        {
            try
            {
                XmlNodeList rows = xml.ChildNodes[0].ChildNodes;

                foreach (XmlNode data in rows)
                {
                    XmlNodeList nodes = data.ChildNodes;

                    foreach (XmlNode item in nodes)
                    {
                        if (item.InnerText.Trim().Equals(tag))
                        {
                            if (tag.Equals("Condición del Contribuyente:") || tag.Equals("Estado del Contribuyente:"))
                                return item.ParentNode.ChildNodes[1].InnerText.Trim();
                            else
                                return item.ParentNode.LastChild.InnerText.Trim();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return "";
        }

        private static string ObtenerResponseSUNAT(string ruc, string captcha)
        {
            string response = string.Empty;

            try
            {
                string myUrl = string.Format("http://www.sunat.gob.pe/cl-ti-itmrconsruc/jcrS00Alias?accion=consPorRuc&nroRuc={0}&codigo={1}", ruc, captcha);
                HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create(myUrl);
                myWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:23.0) Gecko/20100101 Firefox/23.0";
                myWebRequest.CookieContainer = oCookie;
                myWebRequest.Credentials = CredentialCache.DefaultCredentials;
                myWebRequest.Proxy = null;
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myWebRequest.GetResponse();

                Stream myStream = myHttpWebResponse.GetResponseStream();

                StreamReader myStreamReader = new StreamReader(myStream);
                response = HttpUtility.HtmlDecode(myStreamReader.ReadToEnd());

                string[] _split = response.Split(new char[] { '<', '>', '\n', '\r' });

                List<string> _result = new List<string>();
                for (int i = 0; i < _split.Length; i++)
                {
                    if (!string.IsNullOrEmpty(_split[i].Trim()))
                        _result.Add(_split[i].Trim());
                }

                if (_result.Count == 77)
                    throw new Exception("Captcha incorrecto");
                if (_result.Count == 147)
                    throw new Exception("No existen resultados para la consulta");
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }

        private static string LeerCaptcha()
        {
            try
            {
                using (var engine = new TesseractEngine(@".\Resources\tessdata\", "eng", EngineMode.Default))
                {
                    using (var image = new Bitmap(ObtenerImagenCaptcha()))
                    {
                        using (var pix = PixConverter.ToPix(image))
                        {
                            using (var page = engine.Process(pix))
                            {
                                var Porcentaje = String.Format("{0:P}", page.GetMeanConfidence());
                                string CaptchaTexto = page.GetText();
                                char[] eliminarChars = { '\n', ' ' };
                                CaptchaTexto = CaptchaTexto.TrimEnd(eliminarChars);
                                CaptchaTexto = CaptchaTexto.Replace(" ", string.Empty);
                                CaptchaTexto = Regex.Replace(CaptchaTexto, "[^a-zA-Z]+", string.Empty);

                                if (CaptchaTexto != string.Empty & CaptchaTexto.Length == 4)
                                    return CaptchaTexto.ToUpper();
                                else
                                    return LeerCaptcha();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Image ObtenerImagenCaptcha()
        {
            try
            {
                HttpWebRequest oWebRequest = (HttpWebRequest)WebRequest.Create("http://www.sunat.gob.pe/cl-ti-itmrconsruc/captcha?accion=image&magic=2");
                oCookie = new CookieContainer();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                oWebRequest.CookieContainer = oCookie;
                oWebRequest.Proxy = null;
                oWebRequest.Credentials = CredentialCache.DefaultCredentials;

                HttpWebResponse myWebResponse = (HttpWebResponse)oWebRequest.GetResponse();
                Stream myImgStream = myWebResponse.GetResponseStream();
                return Image.FromStream(myImgStream);
            }
            catch (Exception ex )
            {
                throw;
            }
        }

        public static bool EsRucValido(string ruc)
        {
            if (ruc.Length != 11 || !long.TryParse(ruc, out long parse))
                return false;

            int dig01 = int.Parse(ruc.Substring(0, 1)) * 5;
            int dig02 = int.Parse(ruc.Substring(1, 1)) * 4;
            int dig03 = int.Parse(ruc.Substring(2, 1)) * 3;
            int dig04 = int.Parse(ruc.Substring(3, 1)) * 2;
            int dig05 = int.Parse(ruc.Substring(4, 1)) * 7;
            int dig06 = int.Parse(ruc.Substring(5, 1)) * 6;
            int dig07 = int.Parse(ruc.Substring(6, 1)) * 5;
            int dig08 = int.Parse(ruc.Substring(7, 1)) * 4;
            int dig09 = int.Parse(ruc.Substring(8, 1)) * 3;
            int dig10 = int.Parse(ruc.Substring(9, 1)) * 2;
            int dig11 = int.Parse(ruc.Substring(10, 1));

            int suma = dig01 + dig02 + dig03 + dig04 + dig05 + dig06 + dig07 + dig08 + dig09 + dig10;
            int residuo = suma % 11;
            int resta = 11 - residuo;

            int digChk = 0;
            if (resta == 10)
                digChk = 0;
            else if (resta == 11)
                digChk = 1;
            else
                digChk = resta;

            if (dig11 == digChk)
                return true;

            return false;
        }


    }
}