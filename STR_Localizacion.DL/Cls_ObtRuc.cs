using Microsoft.Office.Core;
using SAPbouiCOM;
using STR_Localizacion.UTIL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace STR_Localizacion.DL
{
    public class Cls_ObtRuc : Cls_PropertiesControl
    {
            Form  oform = null;

        public async void ConsultarRucMedianteNumeroRandom(string ruc, Form go_SBOForm)
        {

            try
            {
                int tipoRespuesta = 2;
                string mensajeRespuesta = "";
                SAPbouiCOM.ComboBox comboBox = null;
                string Departamento = string.Empty;

                CuTexto oCuTexto = new CuTexto();
                CookieContainer cookies = new CookieContainer();
                HttpClientHandler controladorMensaje = new HttpClientHandler();
                controladorMensaje.CookieContainer = cookies;
                controladorMensaje.UseCookies = true;
        
                using (HttpClient cliente = new HttpClient(controladorMensaje))
                {
                    cliente.DefaultRequestHeaders.Add("Host", "e-consultaruc.sunat.gob.pe");
                    cliente.DefaultRequestHeaders.Add("sec-ch-ua",
                        " \" Not A;Brand\";v=\"99\", \"Chromium\";v=\"90\", \"Google Chrome\";v=\"90\"");
                    cliente.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                    cliente.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
                    cliente.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
                    cliente.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
                    cliente.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
                    cliente.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                    cliente.DefaultRequestHeaders.Add("User-Agent",
                        "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36");
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 |
                                                           SecurityProtocolType.Tls12;
                    await Task.Delay(100);

                    string url =
                        "https://e-consultaruc.sunat.gob.pe/cl-ti-itmrconsruc/jcrS00Alias";
                    using (HttpResponseMessage resultadoConsulta = await cliente.GetAsync(new Uri(url)))
                    {
                        if (resultadoConsulta.IsSuccessStatusCode)
                        {
                            await Task.Delay(100);
                            cliente.DefaultRequestHeaders.Remove("Sec-Fetch-Site");

                            cliente.DefaultRequestHeaders.Add("Origin", "https://e-consultaruc.sunat.gob.pe");
                            cliente.DefaultRequestHeaders.Add("Referer", url);
                            cliente.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");

                            string numeroDNI = "12345678"; // cualquier número DNI que exista en SUNAT. Pueden aprovechar este "bug" para consultar también mediante DNI a la SUNAT
                            var lClaveValor = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("accion", "consPorTipdoc"),
                            new KeyValuePair<string, string>("razSoc", ""),
                            new KeyValuePair<string, string>("nroRuc", ""),
                            new KeyValuePair<string, string>("nrodoc", numeroDNI),
                            new KeyValuePair<string, string>("contexto", "ti-it"),
                            new KeyValuePair<string, string>("modo", "1"),
                            new KeyValuePair<string, string>("search1", ""),
                            new KeyValuePair<string, string>("rbtnTipo", "2"),
                            new KeyValuePair<string, string>("tipdoc", "1"),
                            new KeyValuePair<string, string>("search2", numeroDNI),
                            new KeyValuePair<string, string>("search3", ""),
                            new KeyValuePair<string, string>("codigo", ""),
                        };
                            FormUrlEncodedContent contenido = new FormUrlEncodedContent(lClaveValor);

                            url = "https://e-consultaruc.sunat.gob.pe/cl-ti-itmrconsruc/jcrS00Alias";
                            using (HttpResponseMessage resultadoConsultaRandom = await cliente.PostAsync(url, contenido))
                            {
                                if (resultadoConsultaRandom.IsSuccessStatusCode)
                                {
                                    await Task.Delay(100);
                                    string contenidoHTML = await resultadoConsultaRandom.Content.ReadAsStringAsync();
                                    string numeroRandom = oCuTexto.ExtraerContenidoEntreTagString(contenidoHTML, 0, "name=\"numRnd\" value=\"", "\">");

                                    lClaveValor = new List<KeyValuePair<string, string>>
                                {
                                    new KeyValuePair<string, string>("accion", "consPorRuc"),
                                    new KeyValuePair<string, string>("actReturn", "1"),
                                    new KeyValuePair<string, string>("nroRuc", ruc),
                                    new KeyValuePair<string, string>("numRnd", numeroRandom),
                                    new KeyValuePair<string, string>("modo", "1")
                                };
                                    // Por si cae en el primer intento por el código "Unauthorized", en el buble se va a intentar hasta 3 veces "nConsulta"
                                    int cConsulta = 0;
                                    int nConsulta = 3;
                                    HttpStatusCode codigoEstado = HttpStatusCode.Unauthorized;
                                    while (cConsulta < nConsulta && codigoEstado == HttpStatusCode.Unauthorized)
                                    {
                                        contenido = new FormUrlEncodedContent(lClaveValor);
                                        using (HttpResponseMessage resultadoConsultaDatos =
                                        await cliente.PostAsync(url, contenido))
                                        {
                                            codigoEstado = resultadoConsultaDatos.StatusCode;
                                            if (resultadoConsultaDatos.IsSuccessStatusCode)
                                            {
                                                contenidoHTML = await resultadoConsultaDatos.Content.ReadAsStringAsync();
                                                contenidoHTML = WebUtility.HtmlDecode(contenidoHTML);

                                                #region Obtener los datos del RUC
                                                EnSUNAT oEnSUNAT = ObtenerDatos(contenidoHTML);
                                                if (oEnSUNAT.TipoRespuesta == 1)
                                                {

                                                    go_SBOForm.GetEditText("7").Value = oEnSUNAT.RazonSocial.Substring(14).Trim(); //oEnSUNAT.RazonSocial.Split('-')[1].Trim();
                                                    string tipoPersona = ruc.Substring(0, 2).Equals("20") ? "TPJ" : (ruc.Substring(0, 2).Equals("10") ? "TPN" : string.Empty);

                                                    if (tipoPersona != string.Empty)
                                                        go_SBOForm.GetComboBox("cbxTipPrs").SelectExclusive(tipoPersona, BoSearchKey.psk_ByValue);

                                                    go_SBOForm.GetComboBox("cbxHabido").SelectExclusive(oEnSUNAT.CondicionContribuyente.Equals("HABIDO") ? "Y" : "N", BoSearchKey.psk_ByValue);


                                                    go_SBOForm.GetComboBox("cbxAgtRet").SelectExclusive("N", BoSearchKey.psk_ByValue);
                                                    go_SBOForm.GetComboBox("cbxAgtPer").SelectExclusive("N", BoSearchKey.psk_ByValue);

                                                    oEnSUNAT.Padrones = oEnSUNAT.Padrones == null ? string.Empty : oEnSUNAT.Padrones;

                                                    if (oEnSUNAT.Padrones.Contains("Incorporado al Régimen de Agentes de Retención"))
                                                        go_SBOForm.GetComboBox("cbxAgtRet").SelectExclusive("Y", BoSearchKey.psk_ByValue);

                                                    if (oEnSUNAT.Padrones.Contains("Incorporado al Régimen de Agentes de Percepción"))
                                                        go_SBOForm.GetComboBox("cbxAgtPer").SelectExclusive("Y", BoSearchKey.psk_ByValue);

                                                   
                                                    go_SBOForm.GetComboBox("cbxBnCntr").SelectExclusive(oEnSUNAT.EstadoContribuyente.Equals("ACTIVO") ? "Y" : "N", BoSearchKey.psk_ByValue);

                                                    // SegmentarDireccion(oEnSUNAT.DomicilioFiscal);
                                                    go_SBOForm.PaneLevel = 7;
                                                    //DIRECCION
                                                    Matrix matrix = go_SBOForm.GetMatrix("178");
                                                    matrix.Columns.Item("1").Cells.Item(1).Specific.Value = "SUNAT";

                                                    RegexOptions options = RegexOptions.None;
                                                    Regex regex = new Regex("[ ]{2,}", options);
                                                    oEnSUNAT.DomicilioFiscal = regex.Replace(oEnSUNAT.DomicilioFiscal, " ");

                                                    if (oEnSUNAT.DomicilioFiscal.Equals("-"))
                                                    {

                                                        Departamento = string.Empty;
                                                        matrix.Columns.Item("4").Cells.Item(1).Specific.Value = string.Empty;
                                                        matrix.Columns.Item("3").Cells.Item(1).Specific.Value = string.Empty;
                                                        matrix.Columns.Item("2").Cells.Item(1).Specific.Value = string.Empty;

                                                        /* Departamento = string.Empty;
                                                         matrix.Columns.Item("4").Cells.Item(1).Specific.Value  = string.Empty; //PROVINCIA
                                                         matrix.Columns.Item("3").Cells.Item(1).Specific.Value = string.Empty;//DISTRITO
                                                         matrix.Columns.Item("6").Cells.Item(1).Specific.Value = string.Empty;*/
                                                    }
                                                    else
                                                    {
                                                        string[] array = oEnSUNAT.DomicilioFiscal.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);

                                                        if (array.Length > 1)
                                                        {
                                                            int a = array.Length;
                                                            string DirTemp = array[a - 3].Trim();
                                                            DirTemp = DirTemp.TrimEnd(' ');
                                                            string[] ArrayDir = DirTemp.Split(' ');
                                                            int i = ArrayDir.Length;

                                                            Departamento = ArrayDir[i - 1].Trim();
                                                            /* Provincia = array[a - 2].Trim();
                                                             Distrito = array[a - 1].Trim();*/

                                                            matrix.Columns.Item("1").Cells.Item(1).Specific.Value = "SUNAT";
                                                            matrix.Columns.Item("3").Cells.Item(1).Specific.Value = array[a - 1].Trim();//DISTRITO
                                                            matrix.Columns.Item("4").Cells.Item(1).Specific.Value = array[a - 2].Trim(); //PROVINCIA
                                                            matrix.Columns.Item("2").Cells.Item(1).Specific.Value = oEnSUNAT.DomicilioFiscal.Substring(0, oEnSUNAT.DomicilioFiscal.Length > 100 ? 100 : oEnSUNAT.DomicilioFiscal.Length); //DIRECCION 

                                                            comboBox = matrix.GetCellSpecific("7", 1);
                                                            var depa = comboBox.ValidValues.OfType<IValidValue>().Where(item => item.Description.Contains(Departamento)).FirstOrDefault();
                                                            if (depa != null)
                                                                comboBox.SelectExclusive(depa.Value, BoSearchKey.psk_ByValue);
                                                        }
                                                    }

                                                    oform = go_SBOApplication.Forms.GetForm("-134", 1);
                                                    oform.Select();

                                                    oform.Items.Item("U_STR_EST_CNTR").Specific.Value = oEnSUNAT.CondicionContribuyente.Equals("ACTIVO") ? "ACTIVO" : 
                                                        oEnSUNAT.EstadoContribuyente.Length > 30 ? oEnSUNAT.EstadoContribuyente.Substring(0, 30) : oEnSUNAT.EstadoContribuyente;
                                                    oform.Items.Item("U_STR_FCH_TRAN").Specific.Value = System.DateTime.Today.ToString("yyyyMMdd");

                                                    go_SBOApplication.StatusBar.SetText(string.Format("Se realizó exitosamente la consulta del número de RUC {0}",
                                                            ruc), BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

                                                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oform);
                                                    GC.Collect();
                                                }
                                                else
                                                {
                                                    go_SBOApplication.StatusBar.SetText(string.Format(
                                                        "No se pudo realizar la consulta del número de RUC {0}.\r\nDetalle: {1}",
                                                        ruc,
                                                        oEnSUNAT.MensajeRespuesta), BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);

                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                throw new Exception(string.Format(
                                                        "Ocurrió un inconveniente al consultar los datos del RUC {0}.\r\nDetalle:{1}",
                                                        ruc, mensajeRespuesta));

                                            }
                                        }

                                        cConsulta++;
                                    }

                                }
                                else
                                {
                                    mensajeRespuesta = await resultadoConsultaRandom.Content.ReadAsStringAsync();

                                    throw new Exception(string.Format(
                                            "Ocurrió un inconveniente al consultar el número random del RUC {0}.\r\nDetalle:{1}",
                                            ruc, mensajeRespuesta));

                                }
                            }
                        }
                        else
                        {
                            mensajeRespuesta = await resultadoConsulta.Content.ReadAsStringAsync();
                            mensajeRespuesta =
                                string.Format(
                                    "Ocurrió un inconveniente al consultar la página principal {0}.\r\nDetalle:{1}",
                                    ruc, mensajeRespuesta);
                            throw new Exception(mensajeRespuesta);

                        }
                    }
                }
            }
            catch (Exception ex )
            {

                go_SBOApplication.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
          
        }
        

        private static EnSUNAT ObtenerDatos(string contenidoHTML)
        {
            CuTexto oCuTexto = new CuTexto();
            EnSUNAT oEnSUNAT = new EnSUNAT();
            string nombreInicio = "<HEAD><TITLE>";
            string nombreFin = "</TITLE></HEAD>";
            string contenidoBusqueda = oCuTexto.ExtraerContenidoEntreTagString(contenidoHTML, 0, nombreInicio, nombreFin);
            if (contenidoBusqueda == ".:: Pagina de Mensajes ::.")
            {
                nombreInicio = "<p class=\"error\">";
                nombreFin = "</p>";
                oEnSUNAT.TipoRespuesta = 2;
                oEnSUNAT.MensajeRespuesta = oCuTexto.ExtraerContenidoEntreTagString(contenidoHTML, 0, nombreInicio, nombreFin);
            }
            else if (contenidoBusqueda == ".:: Pagina de Error ::.")
            {
                nombreInicio = "<p class=\"error\">";
                nombreFin = "</p>";
                oEnSUNAT.TipoRespuesta = 3;
                oEnSUNAT.MensajeRespuesta = oCuTexto.ExtraerContenidoEntreTagString(contenidoHTML, 0, nombreInicio, nombreFin);
            }
            else
            {
                oEnSUNAT.TipoRespuesta = 2;
                nombreInicio = "<div class=\"list-group\">";
                nombreFin = "<div class=\"panel-footer text-center\">";
                contenidoBusqueda = oCuTexto.ExtraerContenidoEntreTagString(contenidoHTML, 0, nombreInicio, nombreFin);
                if (contenidoBusqueda == "")
                {
                    nombreInicio = "<strong>";
                    nombreFin = "</strong>";
                    oEnSUNAT.MensajeRespuesta = oCuTexto.ExtraerContenidoEntreTagString(contenidoHTML, 0, nombreInicio, nombreFin);
                    if (oEnSUNAT.MensajeRespuesta == "")
                        oEnSUNAT.MensajeRespuesta = "No se encuentra las cabeceras principales del contenido HTML";
                }
                else
                {
                    contenidoHTML = contenidoBusqueda;
                    oEnSUNAT.MensajeRespuesta = "Mensaje del inconveniente no especificado";
                    nombreInicio = "<h4 class=\"list-group-item-heading\">";
                    nombreFin = "</h4>";
                    int resultadoBusqueda = contenidoHTML.IndexOf(nombreInicio, 0, StringComparison.OrdinalIgnoreCase);
                    if (resultadoBusqueda > -1)
                    {
                        // Modificar cuando el estado del Contribuyente es "BAJA DE OFICIO", porque se agrega un elemento con clase "list-group-item"
                        resultadoBusqueda += nombreInicio.Length;
                        string[] arrResultado = oCuTexto.ExtraerContenidoEntreTag(contenidoHTML, resultadoBusqueda,
                            nombreInicio, nombreFin);
                        if (arrResultado != null)
                        {
                            oEnSUNAT.RazonSocial = arrResultado[1];

                            // Tipo Contribuyente
                            nombreInicio = "<p class=\"list-group-item-text\">";
                            nombreFin = "</p>";
                            arrResultado = oCuTexto.ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                nombreInicio, nombreFin);
                            if (arrResultado != null)
                            {
                                oEnSUNAT.TipoContribuyente = arrResultado[1];

                                // Nombre Comercial
                                arrResultado = oCuTexto.ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                    nombreInicio, nombreFin);
                                if (arrResultado != null)
                                {
                                    oEnSUNAT.NombreComercial = arrResultado[1].Replace("\r\n", "").Replace("\t", "").Trim();

                                    // Fecha de Inscripción
                                    arrResultado = oCuTexto.ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                        nombreInicio, nombreFin);
                                    if (arrResultado != null)
                                    {
                                        oEnSUNAT.FechaInscripcion = arrResultado[1];

                                        // Fecha de Inicio de Actividades: 
                                        arrResultado = oCuTexto.ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                            nombreInicio, nombreFin);
                                        if (arrResultado != null)
                                        {
                                            oEnSUNAT.FechaInicioActividades = arrResultado[1];

                                            // Estado del Contribuyente
                                            arrResultado = oCuTexto.ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                            nombreInicio, nombreFin);
                                            if (arrResultado != null)
                                            {
                                                oEnSUNAT.EstadoContribuyente = arrResultado[1].Trim();

                                                // Condición del Contribuyente
                                                arrResultado = oCuTexto.ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                                    nombreInicio, nombreFin);
                                                if (arrResultado != null)
                                                {
                                                    oEnSUNAT.CondicionContribuyente = arrResultado[1].Trim();

                                                    // Domicilio Fiscal
                                                    arrResultado = oCuTexto.ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                                        nombreInicio, nombreFin);
                                                    if (arrResultado != null)
                                                    {
                                                        oEnSUNAT.DomicilioFiscal = arrResultado[1].Trim();

                                                        // Actividad(es) Económica(s)
                                                        nombreInicio = "<tbody>";
                                                        nombreFin = "</tbody>";
                                                        arrResultado = oCuTexto.ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                                            nombreInicio, nombreFin);
                                                        if (arrResultado != null)
                                                        {
                                                            oEnSUNAT.ActividadesEconomicas = arrResultado[1].Replace("\r\n", "").Replace("\t", "").Trim();

                                                            // Comprobantes de Pago c/aut. de impresión (F. 806 u 816)
                                                            arrResultado = oCuTexto.ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                                                nombreInicio, nombreFin);
                                                            if (arrResultado != null)
                                                            {
                                                                oEnSUNAT.ComprobantesPago = arrResultado[1].Replace("\r\n", "").Replace("\t", "").Trim();

                                                                // Sistema de Emisión Electrónica
                                                                arrResultado = oCuTexto.ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                                                    nombreInicio, nombreFin);
                                                                if (arrResultado != null)
                                                                {
                                                                    oEnSUNAT.SistemaEmisionComprobante = arrResultado[1].Replace("\r\n", "").Replace("\t", "").Trim();

                                                                    // Afiliado al PLE desde
                                                                    nombreInicio = "<p class=\"list-group-item-text\">";
                                                                    nombreFin = "</p>";
                                                                    arrResultado = oCuTexto.ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                                                        nombreInicio, nombreFin);
                                                                    if (arrResultado != null)
                                                                    {
                                                                        oEnSUNAT.AfiliadoPLEDesde = arrResultado[1];

                                                                        // Padrones 
                                                                        nombreInicio = "<tbody>";
                                                                        nombreFin = "</tbody>";
                                                                        arrResultado = oCuTexto.ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                                                            nombreInicio, nombreFin);
                                                                        if (arrResultado != null)
                                                                        {
                                                                            oEnSUNAT.Padrones = arrResultado[1].Replace("\r\n", "").Replace("\t", "").Trim();
                                                                        }
                                                                    }

                                                                    oEnSUNAT.TipoRespuesta = 1;
                                                                    oEnSUNAT.MensajeRespuesta = "Ok";
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return oEnSUNAT;
        }
    }
}
