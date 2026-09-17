using Dapper;
using System.Data;
using TreskelRest.Data.Base;
using TreskelRest.Data.Models;

namespace TreskelRest.Data
{
    public class TreskelRepository : ITreskelRepository
    {
        private readonly TreskelContext _context;
        private IDbConnection _connexion;
        private IDbTransaction _transaction;
        private bool _roolBack = false;

        public TreskelRepository(TreskelContext context)
        {
            _context = context;
            _connexion = _context.CreateConnection();
        }

        public TreskelRepository(TreskelContext context, bool UsaTransaction, bool autoRoolback = false)
        {
            _roolBack = autoRoolback;
            _context = context;
            _connexion = _context.CreateConnection();
            _connexion.Open();

            if (UsaTransaction)
            {
                _transaction = _connexion.BeginTransaction();
            }
        }

        private void Commit()
        {
            if (_transaction != null)
            {
                if (_roolBack)
                {
                    _transaction.Rollback();
                }
                else
                    _transaction.Commit();
            }

            Close();
        }

        private void Close()
        {
            if (_transaction != null)
            {
                _transaction?.Dispose();
            }

            if (_connexion != null)
            {
                _connexion?.Close();
                _connexion?.Dispose();
            }
        }

        public async Task<UsuarioEmpresa> login(UsuarioRequest usuario)
        {
            UsuarioEmpresa? result;
            try
            {
                string query = "SELECT * FROM sp_login(@empruc, @empusuario, @empguid)";

                result = await _connexion.QueryFirstOrDefaultAsync<UsuarioEmpresa>(query,
                    new
                    {
                        empruc = usuario.Ruc,
                        empusuario = usuario.Usuario,
                        empguid = usuario.EmpresaId
                    });
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los usuarios", ex);
            }

            return result;
        }

        public async Task<UsuarioEmpresa> actualizaCliente(UsuarioEmpresa usuario)
        {
            UsuarioEmpresa result;
            try
            {
                string query = "SELECT * FROM sp_actualiza_cliente(@empruc, @empguid, @usamongo, @creaonhd, @ambiente, @email, @emplogo)";

                result = await _connexion.QueryFirstOrDefaultAsync<UsuarioEmpresa>(query,
                    new
                    {
                        empruc = usuario.Ruc,
                        empguid = usuario.EmpresaId,
                        usamongo = usuario.UsaMongoDB,
                        creaonhd = usuario.CreateOnHd,
                        ambiente = usuario.Environment,
                        email = usuario.email,
                        emplogo = usuario.Logotipo,
                    });

                Commit();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al grabar los datos del cliente", ex);
            }

            return result;
        }

        public async Task<UsuarioEmpresa> getEmpresa(string empresaid)
        {
            UsuarioEmpresa? result;
            try
            {
                string query = "SELECT * FROM sp_get_empresa(@p_empresaid)";

                result = await _connexion.QueryFirstOrDefaultAsync<UsuarioEmpresa>(query,
                    new
                    {
                        p_empresaid = empresaid
                    });
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los datos de la empresa", ex);
            }

            return result;
        }

        public async Task<Facturacion> grabarFactura(Facturacion facturacion, string mongoId)
        {
            Facturacion? result;
            try
            {
                string query = "SELECT * FROM sp_facturacion_grabar(" +
                               "@p_id::bigint, @p_venta::character varying, @p_ruc::character varying, " +
                               "@p_empresaid::character varying, @p_serial::character varying, @p_codigoLocal::character varying," +
                               "@p_nombreLocal::character varying, @p_idCliente::character varying, @p_nombreCliente::character varying, " +
                               "@p_emailCliente::character varying, @p_xmlfile::bytea, " +
                               "@p_procesado::boolean, @p_pdfGenerado::boolean, @p_enviadoCliente::boolean, " +
                               "@p_autorizacion::character varying, @p_pcname::character varying, " +
                               "@p_ipaddress::character varying, @p_clave::character varying, " +
                               "@p_clave_caduca::timestamp without time zone, " +
                               "@p_archivoid::character varying " +
                               ")";

                var dbParams = new Dapper.DynamicParameters();
                dbParams.Add("p_id", facturacion.id);
                dbParams.Add("p_venta", facturacion.secuencial);
                dbParams.Add("p_ruc", facturacion.ruc);
                dbParams.Add("p_empresaId", facturacion.empresaid);
                dbParams.Add("p_serial", facturacion.serial);
                dbParams.Add("p_codigoLocal", facturacion.codigoLocal ?? "");
                dbParams.Add("p_nombreLocal", facturacion.nombreLocal ?? "");
                dbParams.Add("p_idCliente", facturacion.idCliente);
                dbParams.Add("p_nombreCliente", facturacion.nombreCliente); 
                dbParams.Add("p_emailCliente", facturacion.emailCliente ?? "");
                dbParams.Add("p_xmlfile", null, dbType: System.Data.DbType.String);
                dbParams.Add("p_procesado", facturacion.procesado ?? false);
                dbParams.Add("p_pdfGenerado", facturacion.pdfGenerado ?? false);
                dbParams.Add("p_enviadoCliente", facturacion.enviadoCliente ?? false);
                dbParams.Add("p_autorizacion", facturacion.claveAcceso);
                dbParams.Add("p_pcname", facturacion.pcName ?? "");
                dbParams.Add("p_ipaddress", facturacion.ipAddress ?? "");
                dbParams.Add("p_clave", facturacion.claveDownload ?? "");
                dbParams.Add("p_clave_caduca", DateTime.Now.AddDays(15));
                dbParams.Add("p_archivoid", mongoId);

                result = await _connexion.QueryFirstOrDefaultAsync<Facturacion>(query, dbParams);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al grabar la factura", ex);
            }
            return result;
        }

        public async Task<Facturacion> actualizaFactura(Facturacion facturacion)
        {
            Facturacion? result;
            try
            {
                string query = "SELECT sp_facturacion_actualiza(" +
                               "@p_id, @p_estado, @p_estadomail, " +
                               "@p_mensaje, @p_xmlfile::bytea, @p_pdffile::bytea) AS resultado";

                var dbParams = new Dapper.DynamicParameters();
                dbParams.Add("p_id", facturacion.id);
                dbParams.Add("p_estado", facturacion.estadoCodigo);
                dbParams.Add("p_estadomail", facturacion.enviadoCliente == true ? "A": "N");
                dbParams.Add("p_mensaje", facturacion.mensaje);
                dbParams.Add("p_xmlfile", null);
                dbParams.Add("p_pdffile", null);
                
                var respuesta = await _connexion.QueryFirstOrDefaultAsync<bool>(query, dbParams);

                result = facturacion;
                if (respuesta)
                {
                    result.estado = facturacion.estadoCodigo;
                    result.estadoEnvioMail = facturacion.enviadoCliente == true ? "A" : "N";
                    result.mensaje = facturacion.mensaje;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la factura", ex);
            }
            return result;
        }

        public async Task<Facturacion> obtenerFactura(Facturacion facturacion, string tipo)
        {
            Facturacion? result;
            try
            {
                string query = generaQueryUpdate(tipo);
                
                var dbParams = new Dapper.DynamicParameters();
                dbParams.Add("p_empresaid", facturacion.empresaid);
                dbParams.Add("p_ventascl", facturacion.secuencial);
                dbParams.Add("p_autorizacion", facturacion.claveAcceso);
                
                result = await _connexion.QueryFirstOrDefaultAsync<Facturacion>(query, dbParams);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los datos de la factura", ex);
            }
            return result;
        }

        //--------------------------------------------------------------------------------------------------------------
        public async Task<Retencion> grabarRetencion(Retencion retencion, string mongoId)
        {
            Retencion? result;
            try
            {
                string query = "SELECT * FROM sp_retencion_grabar(" +
                               "@p_id::bigint, @p_retencion::character varying, @p_ruc::character varying, " +
                               "@p_empresaid::character varying, @p_serial::character varying, @p_codigoLocal::character varying," +
                               "@p_nombreLocal::character varying, @p_idCliente::character varying, @p_nombreCliente::character varying, " +
                               "@p_emailCliente::character varying, @p_xmlfile::bytea, " +
                               "@p_procesado::boolean, @p_pdfGenerado::boolean, @p_enviadoCliente::boolean, " +
                               "@p_autorizacion::character varying, @p_pcname::character varying, " +
                               "@p_ipaddress::character varying, @p_clave::character varying, " +
                               "@p_clave_caduca::timestamp without time zone, " +
                               "@p_archivoid::character varying " +
                               ")";

                var dbParams = new Dapper.DynamicParameters();
                dbParams.Add("p_id", retencion.id);
                dbParams.Add("p_retencion", retencion.secuencial);
                dbParams.Add("p_ruc", retencion.ruc);
                dbParams.Add("p_empresaId", retencion.empresaid);
                dbParams.Add("p_serial", retencion.serial);
                dbParams.Add("p_codigoLocal", retencion.codigoLocal ?? "");
                dbParams.Add("p_nombreLocal", retencion.nombreLocal ?? "");
                dbParams.Add("p_idCliente", retencion.idCliente);
                dbParams.Add("p_nombreCliente", retencion.nombreCliente);
                dbParams.Add("p_emailCliente", retencion.emailCliente ?? "");
                dbParams.Add("p_xmlfile", null, dbType: System.Data.DbType.String);
                dbParams.Add("p_procesado", retencion.procesado ?? false);
                dbParams.Add("p_pdfGenerado", retencion.pdfGenerado ?? false);
                dbParams.Add("p_enviadoCliente", retencion.enviadoCliente ?? false);
                dbParams.Add("p_autorizacion", retencion.claveAcceso);
                dbParams.Add("p_pcname", retencion.pcName ?? "");
                dbParams.Add("p_ipaddress", retencion.ipAddress ?? "");
                dbParams.Add("p_clave", retencion.claveDownload ?? "");
                dbParams.Add("p_clave_caduca", DateTime.Now.AddDays(15));
                dbParams.Add("p_archivoid", mongoId);

                result = await _connexion.QueryFirstOrDefaultAsync<Retencion>(query, dbParams);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al grabar la retención", ex);
            }
            return result;
        }

        public async Task<Retencion> obtenerRetencion(Retencion retencion, string tipo)
        {
            Retencion? result;
            try
            {
                string query = generaQueryUpdate(tipo);
                
                var dbParams = new Dapper.DynamicParameters();
                dbParams.Add("p_empresaid", retencion.empresaid);
                dbParams.Add("p_ventascl", retencion.secuencial);
                dbParams.Add("p_autorizacion", retencion.claveAcceso);

                result = await _connexion.QueryFirstOrDefaultAsync<Retencion>(query, dbParams);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los datos de la retencion", ex);
            }
            return result;
        }

        public async Task<Retencion> actualizaRetencion(Retencion retencion)
        {
            Retencion? result;
            try
            {
                string query = "SELECT sp_retencion_actualiza(" +
                               "@p_id, @p_estado, @p_estadomail, " +
                               "@p_mensaje, @p_xmlfile::bytea, @p_pdffile::bytea) AS resultado";

                var dbParams = new Dapper.DynamicParameters();
                dbParams.Add("p_id", retencion.id);
                dbParams.Add("p_estado", retencion.estadoCodigo);
                dbParams.Add("p_estadomail", retencion.enviadoCliente == true ? "A" : "N");
                dbParams.Add("p_mensaje", retencion.mensaje);
                dbParams.Add("p_xmlfile", null);
                dbParams.Add("p_pdffile", null);

                var respuesta = await _connexion.QueryFirstOrDefaultAsync<bool>(query, dbParams);

                result = retencion;
                if (respuesta)
                {
                    result.estado = retencion.estadoCodigo;
                    result.estadoEnvioMail = retencion.enviadoCliente == true ? "A" : "N";
                    result.mensaje = retencion.mensaje;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la retención", ex);
            }
            return result;
        }

        //--------------------------------------------------------------------------------------------------------------
        public async Task<NotaCredito> grabarNotaCredito(NotaCredito notacredito, string mongoId)
        {
            NotaCredito? result;
            try
            {
                string query = "SELECT * FROM sp_retencion_grabar(" +
                               "@p_id::bigint, @p_retencion::character varying, @p_ruc::character varying, " +
                               "@p_empresaid::character varying, @p_serial::character varying, @p_codigoLocal::character varying," +
                               "@p_nombreLocal::character varying, @p_idCliente::character varying, @p_nombreCliente::character varying, " +
                               "@p_emailCliente::character varying, @p_xmlfile::bytea, " +
                               "@p_procesado::boolean, @p_pdfGenerado::boolean, @p_enviadoCliente::boolean, " +
                               "@p_autorizacion::character varying, @p_pcname::character varying, " +
                               "@p_ipaddress::character varying, @p_clave::character varying, " +
                               "@p_clave_caduca::timestamp without time zone, " +
                               "@p_archivoid::character varying " +
                               ")";

                var dbParams = new Dapper.DynamicParameters();
                dbParams.Add("p_id", notacredito.id);
                dbParams.Add("p_retencion", notacredito.secuencial);
                dbParams.Add("p_ruc", notacredito.ruc);
                dbParams.Add("p_empresaId", notacredito.empresaid);
                dbParams.Add("p_serial", notacredito.serial);
                dbParams.Add("p_codigoLocal", notacredito.codigoLocal ?? "");
                dbParams.Add("p_nombreLocal", notacredito.nombreLocal ?? "");
                dbParams.Add("p_idCliente", notacredito.idCliente);
                dbParams.Add("p_nombreCliente", notacredito.nombreCliente);
                dbParams.Add("p_emailCliente", notacredito.emailCliente ?? "");
                dbParams.Add("p_xmlfile", null, dbType: System.Data.DbType.String);
                dbParams.Add("p_procesado", notacredito.procesado ?? false);
                dbParams.Add("p_pdfGenerado", notacredito.pdfGenerado ?? false);
                dbParams.Add("p_enviadoCliente", notacredito.enviadoCliente ?? false);
                dbParams.Add("p_autorizacion", notacredito.claveAcceso);
                dbParams.Add("p_pcname", notacredito.pcName ?? "");
                dbParams.Add("p_ipaddress", notacredito.ipAddress ?? "");
                dbParams.Add("p_clave", notacredito.claveDownload ?? "");
                dbParams.Add("p_clave_caduca", DateTime.Now.AddDays(15));
                dbParams.Add("p_archivoid", mongoId);

                result = await _connexion.QueryFirstOrDefaultAsync<NotaCredito>(query, dbParams);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al grabar la nota de crédito", ex);
            }
            return result;
        }

        public async Task<NotaCredito> obtenerNotaCredito(NotaCredito notacredito, string tipo)
        {
            NotaCredito? result;
            try
            {
                string query = generaQueryUpdate(tipo);

                var dbParams = new Dapper.DynamicParameters();
                dbParams.Add("p_empresaid", notacredito.empresaid);
                dbParams.Add("p_ventascl", notacredito.secuencial);
                dbParams.Add("p_autorizacion", notacredito.claveAcceso);

                result = await _connexion.QueryFirstOrDefaultAsync<NotaCredito>(query, dbParams);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los datos de la nota de crédito", ex);
            }
            return result;
        }

        public async Task<NotaCredito> actualizaNotaCredito(NotaCredito notaCredito)
        {
            NotaCredito? result;
            try
            {
                string query = "SELECT sp_notacredito_actualiza(" +
                               "@p_id, @p_estado, @p_estadomail, " +
                               "@p_mensaje, @p_xmlfile::bytea, @p_pdffile::bytea) AS resultado";

                var dbParams = new Dapper.DynamicParameters();
                dbParams.Add("p_id", notaCredito.id);
                dbParams.Add("p_estado", notaCredito.estadoCodigo);
                dbParams.Add("p_estadomail", notaCredito.enviadoCliente == true ? "A" : "N");
                dbParams.Add("p_mensaje", notaCredito.mensaje);
                dbParams.Add("p_xmlfile", null);
                dbParams.Add("p_pdffile", null);

                var respuesta = await _connexion.QueryFirstOrDefaultAsync<bool>(query, dbParams);

                result = notaCredito;
                if (respuesta)
                {
                    result.estado = notaCredito.estadoCodigo;
                    result.estadoEnvioMail = notaCredito.enviadoCliente == true ? "A" : "N";
                    result.mensaje = notaCredito.mensaje;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la nota de crédito", ex);
            }
            return result;
        }

        // -------------------------------------------------------------------------------------------------
        private string generaQueryUpdate(string tipo)
        {
            var tabla = string.Empty;
            var secid = string.Empty;

            switch (tipo)
            {
                case "F":
                    tabla = "svrfacturacion";
                    secid = "ventasec";
                    break;
                case "R":
                    tabla = "svrretencion";
                    secid = "retesec";
                    break;
                case "C":
                    tabla = "svrnotacredito";
                    secid = "ntcrsec";
                    break;
                case "D":
                    tabla = "svrnotadebito";
                    secid = "ntdbsec";
                    break;
            }

            string query = $"SELECT  id, empresaid, {secid} AS secuencial, serial, estado, " +
                               $"estadomail AS estadoEnvioMail, mensaje, ruta_archivo AS archivoPath, " +
                               $"archivo_id AS archivoid  " +
                               $"FROM {tabla} " +
                               "WHERE   empresaid = @p_empresaid " +
                               $"        AND {secid} = @p_ventascl " +
                               $"        AND autorizacion = @p_autorizacion " +
                               "LIMIT 1";

            return query;
        }
    }
}
