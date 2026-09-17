using System.Net;
using System.ServiceModel;
using System.Xml;
using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Models.Responses;

namespace TreskelRest.Biz.Servicios
{
    public class SriWebService: ISriWebService{
        private enum eProceso{
            PreValidacion = 0,
            Validacion = 1,
            PreAutorizacion = 2,
            Autorizacion = 3,
            ConfirmaAutorizacion = 4
        }

        private eProceso control = eProceso.PreValidacion;

        public async Task<RespuestaSRI> validaDocumento(string environment, byte[] xml, string claveAcceso){
            RespuestaSRI respuesta = new RespuestaSRI();
            
            try{
                control = eProceso.PreValidacion;
                var valida = await ValidaXML(xml, environment);
                
                if (valida.estado != "DEVUELTA"){
                    respuesta.Estado = "VALIDADO";
                    control = eProceso.PreAutorizacion;
                    respuesta = await ProcesarAutorizacionAsync(claveAcceso, environment);
                    return respuesta;
                }
                else{
                    if (valida.comprobante == null)
                    {
                        respuesta.Estado = "DEVUELTA";
                        respuesta.Mensaje = "Devuelta";
                        respuesta.Tipo = "Error";

                        return respuesta;
                    }


                    if (valida.comprobante.Mensajes.Any(
                        m => m.identificador == 43 || 
                        m.mensaje.Contains("DUPLICADO") ||
                        m.mensaje.Contains("REGISTRADA"))){
                        respuesta = await ProcesarAutorizacionAsync(claveAcceso, environment);
                        control = eProceso.ConfirmaAutorizacion;
                    }

                    if (respuesta.Estado == "AUTORIZADO"){
                        return respuesta;
                    };

                    var mensaje = valida.comprobante.Mensajes[0].mensaje + System.Environment.NewLine
                                  + valida.comprobante.Mensajes[0].informacionAdicional;

                    respuesta.Mensaje = mensaje;
                    respuesta.Tipo = "Error";
                    return respuesta;
                }
            }
            catch (Exception e){
                if (control == eProceso.PreValidacion){
                    respuesta.Estado = "NOPROCESADO";
                }
                else if (control == eProceso.Validacion){
                    respuesta.Estado = "DEVUELTA";
                }
                else if (control == eProceso.PreAutorizacion){
                    respuesta.Estado = "PENDIENTE";
                }
                else if (control == eProceso.Autorizacion){
                    respuesta.Estado = "PENDIENTE";
                }  
                
                respuesta.Mensaje = e.Message;
                respuesta.Tipo = "Error";
                return respuesta;
            }
        }

        private async Task<RespuestaSRI> ProcesarAutorizacionAsync(string claveAcceso, string environment){
            RespuestaSRI respuesta = new RespuestaSRI();

            var autorizado = await AutorizaXML(claveAcceso, environment);

            respuesta.FechaAutorizacion = autorizado.autorizacion.FechaAutorizacion;
            respuesta.Estado = autorizado.autorizacion.Estado;
            respuesta.NumeroAutorizacion = autorizado.ClaveAcceso;


            if (autorizado.autorizacion.Mensajes.Count > 0){
                var mensaje = autorizado.autorizacion.Mensajes[0].mensaje + System.Environment.NewLine
                    + autorizado.autorizacion.Mensajes[0].informacionAdicional;

                respuesta.Estado = "PENDIENTE";
                respuesta.Mensaje = mensaje;
                respuesta.Tipo = autorizado.autorizacion.Mensajes[0].tipo;
            }
            else{
                respuesta.Estado = "AUTORIZADO";
                respuesta.Mensaje = "SUCESS";
                respuesta.Tipo = "SUCCESS";
            }

            return respuesta;
        }

        private async Task<SriValidacionResponse> ValidaXML(byte[] xml, string environment)
        {
            return await EjecutarConReintentoAsync(async () =>
            {
                if (environment.Contains("PRO"))
                {
                    var wsProd = new WS_PROD_RC.RecepcionComprobantesOfflineClient();
                    CrearClienteSRI((BasicHttpBinding)wsProd.Endpoint.Binding);

                    try
                    {
                        WS_PROD_RC.validarComprobanteResponse respuesta = await wsProd.validarComprobanteAsync(xml);

                        return generaRespuesta(respuesta);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                    finally
                    {
                        CerrarCliente(wsProd);
                    }
                }
                else
                {
                    var wsPrue = new WS_PRUE_RC.RecepcionComprobantesOfflineClient();

                    CrearClienteSRI((BasicHttpBinding)wsPrue.Endpoint.Binding);

                    try
                    {
                        WS_PRUE_RC.validarComprobanteResponse respuesta = await wsPrue.validarComprobanteAsync(xml);

                        return generaRespuesta(respuesta);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                    finally
                    {
                        CerrarCliente(wsPrue);
                    }
                }
            });
        }

        private void CerrarCliente(ICommunicationObject cliente)
        {
            try
            {
                if (cliente.State == CommunicationState.Faulted)
                    cliente.Abort();
                else
                    cliente.Close();
            }
            catch
            {
                cliente.Abort();
            }
        }

        private async Task<SriAutorizacionResponse> AutorizaXML(string numero, string environment)
        {
            return await EjecutarConReintentoAsync(async () =>
            {
                if (environment.Contains("PRO"))
                {
                    var wsProd = new WS_PROD_AC.AutorizacionComprobantesOfflineClient();
                    CrearClienteSRI((BasicHttpBinding)wsProd.Endpoint.Binding);

                    try
                    {
                        WS_PROD_AC.autorizacionComprobanteResponse respuesta = await wsProd.autorizacionComprobanteAsync(numero);

                        return generaRespuestaAut(respuesta);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                    finally
                    {
                        CerrarCliente(wsProd);
                    }
                }
                else
                {
                    var wsPrue = new WS_PRUE_AC.AutorizacionComprobantesOfflineClient();
                    CrearClienteSRI((BasicHttpBinding)wsPrue.Endpoint.Binding);

                    try
                    {
                        WS_PRUE_AC.autorizacionComprobanteResponse respuesta1 = await wsPrue.autorizacionComprobanteAsync(numero);

                        return generaRespuestaAut(respuesta1);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        CerrarCliente(wsPrue);
                    }
                }
            });
        }

        private SriValidacionResponse generaRespuesta<TRespuesta>(TRespuesta respuestaSolicitud)
        {
            try
            {
                if (respuestaSolicitud == null)
                    throw new ArgumentNullException(nameof(respuestaSolicitud));

                dynamic respuesta = respuestaSolicitud;

                Comprobante comprobante = null;

                if (respuesta.comprobantes != null &&
                    respuesta.comprobantes.Length > 0)
                {
                    dynamic c = respuesta.comprobantes[0];

                    comprobante = new Comprobante
                    {
                        ClaveAcceso = c.claveAcceso,
                        Mensajes = new List<Mensaje>()
                    };

                    if (c.mensajes != null)
                    {
                        foreach (dynamic m in c.mensajes)
                        {
                            comprobante.Mensajes.Add(new Mensaje
                            {
                                identificador = m.identificador,
                                mensaje = m.mensaje1,
                                informacionAdicional = m.informacionAdicional,
                                tipo = m.tipo
                            });
                        }
                    }
                }

                return new SriValidacionResponse
                {
                    estado = respuesta.estado,
                    comprobante = comprobante
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private SriValidacionResponse generaRespuesta(WS_PRUE_RC.validarComprobanteResponse respuesta)
        {
            return generaRespuesta(respuesta.RespuestaRecepcionComprobante);
        }

        private SriValidacionResponse generaRespuesta(WS_PROD_RC.validarComprobanteResponse respuesta)
        {
            return generaRespuesta(respuesta.RespuestaRecepcionComprobante);
        }

        private SriAutorizacionResponse generaRespuestaAut<TRespuesta>(TRespuesta respuestaSolicitud)
        {
            try
            {
                if (respuestaSolicitud == null)
                    throw new ArgumentNullException(nameof(respuestaSolicitud));

                dynamic respuesta = respuestaSolicitud;

                Autorizacion autorizacion = new Autorizacion();
                autorizacion.FechaAutorizacion = DateTime.Now;
                autorizacion.Estado = "AUTORIZADO";
                autorizacion.Ambiente = "PRODUCCION";
                autorizacion.Mensajes = new List<Mensaje>();

                if (respuesta.autorizaciones != null &&
                    respuesta.autorizaciones.Length > 0)
                {
                    dynamic a = respuesta.autorizaciones[0];

                    autorizacion = new Autorizacion
                    {
                        Estado = a.estado,
                        Ambiente = a.ambiente,
                        Comprobante = a.comprobante,
                        FechaAutorizacion = a.fechaAutorizacion,
                        Mensajes = new List<Mensaje>()
                    };

                    if (a.mensajes != null)
                    {
                        foreach (dynamic m in a.mensajes)
                        {
                            autorizacion.Mensajes.Add(new Mensaje
                            {
                                identificador = m.identificador,
                                mensaje = m.mensaje1,
                                informacionAdicional = m.informacionAdicional,
                                tipo = m.tipo
                            });
                        }
                    }
                }

                return new SriAutorizacionResponse
                {
                    ClaveAcceso = respuesta.claveAccesoConsultada,
                    NumeroComprobantes = int.Parse(respuesta.numeroComprobantes),
                    autorizacion = autorizacion
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private SriAutorizacionResponse generaRespuestaAut(WS_PROD_AC.autorizacionComprobanteResponse respuesta)
        {
            return generaRespuestaAut(respuesta.RespuestaAutorizacionComprobante);
        }

        private SriAutorizacionResponse generaRespuestaAut(WS_PRUE_AC.autorizacionComprobanteResponse respuesta)
        {
            return generaRespuestaAut(respuesta.RespuestaAutorizacionComprobante);
        }

        private void CrearClienteSRI(BasicHttpBinding binding)
        {
            binding.OpenTimeout = TimeSpan.FromSeconds(30);
            binding.CloseTimeout = TimeSpan.FromSeconds(30);
            binding.SendTimeout = TimeSpan.FromSeconds(60);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(60);

            // Tamaño máximo de mensajes
            binding.MaxReceivedMessageSize = 10 * 1024 * 1024; // 10 MB
            binding.MaxBufferSize = 10 * 1024 * 1024;          // 10 MB

            binding.ReaderQuotas = new XmlDictionaryReaderQuotas
            {
                MaxDepth = 64,
                MaxStringContentLength = 10 * 1024 * 1024,
                MaxArrayLength = 10 * 1024 * 1024,
                MaxBytesPerRead = 4096,
                MaxNameTableCharCount = 16384
            };
        }

        private async Task<T> EjecutarConReintentoAsync<T>(Func<Task<T>> funcion, int intentos = 3){
            Exception ultimoError = null;

            for (int i = 1; i <= intentos; i++){
                try{
                    return await funcion();
                }
                catch (QuotaExceededException ex)
                {
                    ultimoError = ex;
                }
                catch (TimeoutException ex){
                    ultimoError = ex;
                }
                catch (CommunicationException ex)
                {
                    ultimoError = ex;
                }
                catch (WebException ex){
                    ultimoError = ex;
                }
                /*catch (Exception ex){
                    ultimoError = ex;
                }*/

                if (i < intentos)
                    await Task.Delay(3000 * i);
            }

            throw ultimoError;
        }
    }
}
