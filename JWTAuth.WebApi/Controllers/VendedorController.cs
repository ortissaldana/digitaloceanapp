using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWTAuth.WebApi.Interface;
using JWTAuth.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace JWTAuth.WebApi.Controllers
{
    [Authorize]


    [Route("api")]
    [ApiController]
    public class VendedorController : ControllerBase
    {
        private readonly IVendedor _IUser;
        private readonly IAVentas _IVenta;

        private readonly DatabaseContext _context;

        public VendedorController(IVendedor IEmployee, DatabaseContext context)
        {
            _IUser = IEmployee;
            _context = context;
        }

        // GET: api/user
        [Route("showAll")]
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Vendedor>>> Get()
        {
            return await Task.FromResult(_IUser.GetVendedorDetails());
        }
        





        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<LoginUserModelo2>> Post(User _userData)
        {
            LoginUserModelo2 respuesta = new();



            if (_userData != null && _userData.Email != null && _userData.Passsword != null)
            {
                var user1 = await GetUser(_userData.Email);

                if (user1 != null)
                {
                    user1 = await GetUser2(_userData.Email, _userData.Passsword);

                    if (user1 != null)
                    {

                        var user2 = await GetUser(_userData.Email);



                        respuesta.Estatus = "Credenciales exitosas";
                        respuesta.user_id = user2.Id;
                        return await Task.FromResult(respuesta);
                    }

                    else
                    {
                        respuesta.Estatus = "contraseña incorecta";
                        return await Task.FromResult(respuesta);
                    }
                }
                else
                {
                    respuesta.Estatus = "Usuario no existe";
                    return await Task.FromResult(respuesta);
                }
            }
            else
            {
                respuesta.Estatus = "Datos incorecctos";
                return await Task.FromResult(respuesta);
            }
        }




        [Route("add")]
        [HttpPost]
        public async Task<ActionResult<LoginUserModelo>> Post2(Vendedor user)
        {
            LoginUserModelo respuesta = new();

            _IUser.AddVendedor(user);
            //return await Task.FromResult(CreatedAtAction("GetEmployees", new { id = employee.EmployeeID }, employee));
            respuesta.Estatus = "Añadido exitosamente ";
            return await Task.FromResult(respuesta);
        }
        public class TarjetaModel
        {
            public string Messages { get; set; }
           
        }
        public class TarjetaModelResponse
        {
            public string Tarjeta { get; set; }
            public decimal Saldo { get; set; }
            public string Error { get; set; }
        }


        public class VentasPorVendedorResponse
        {
            public string Tarjeta { get; set; }
            public string boletos { get; set; }
            public string kiosko { get; set; }
            public string app { get; set; }
            public string appListas { get; set; }

        }

        public class VentasPorVendedorResponseAdmin
        {
            public string Email { get; set; }
            public string contra { get; set; }
            public string contra2 { get; set; }


            public string vendedor { get; set; }
        }

        public class VentasPorVendedorResponseAdminVendedor
        {
            public string contra { get; set; }
      
        }

        public class VentasPorVendedorResponseAdminVendedorContraseñaTelegram
        {
            public string contra { get; set; }
            public string chatId { get; set; }

        }

        public class VentasPorVendedorPorTarjeta
        {
            public string contraseña { get; set; }
            public string tarjeta { get; set; }


        }
        public class VentasPorVendedorResponseAdmin2
        {
            public string Email { get; set; }
            public string contra { get; set; }
            public string contra2 { get; set; }
            public string instru { get; set; }
            public decimal num { get; set; }

            public string vendedor { get; set; }
        }



        [Route("add2")]
        [HttpPost]
        public async Task<ActionResult<LoginUserModelo>> GetUserñ(User id)

        {

            LoginUserModelo respuesta = new();

            var user1 = await GetUser(id.Email);

            if (user1 != null)
            {
                respuesta.Estatus = user1.Email;
                return await Task.FromResult(respuesta);


            }
            else
            {
                respuesta.Estatus = "Datos incorecctos";
                return await Task.FromResult(respuesta);
            }
        }
        [Route("add4")]
        [HttpPost]
        public ActionResult<TarjetaModel> VerificarSaldo(Vendedor vendedor)
        {
            var vendedorEncontrado = _IUser.GetVendedorDetails().FirstOrDefault(v => v.contraseña == vendedor.contraseña);

            TarjetaModel tarjetaModel = new TarjetaModel();

            if (vendedorEncontrado != null)
            {

                var vendedorIdParam = new SqlParameter("@VendedorID", vendedorEncontrado.id);
                var resultadoParam = new SqlParameter("@Resultado", SqlDbType.Int);
                resultadoParam.Direction = ParameterDirection.Output;

                _context.Database.ExecuteSqlRaw("CompararAcumulados @VendedorID, @Resultado OUTPUT", vendedorIdParam, resultadoParam);

                int resultado = (int)resultadoParam.Value;

                if (resultado == 1)
                {
                    tarjetaModel.Messages = "Has alcanzado el límite permitido. Por favor, contacta al soporte para obtener ayuda.";
                }
                else if (resultado == -1)
                {

                    var saldoSolicitadoParam = new SqlParameter("@saldoSolicitado", vendedor.acumulado);
                    var resultadoBusquedaParam = new SqlParameter("@resultadoBusqueda", SqlDbType.VarChar, -1);
                    resultadoBusquedaParam.Direction = ParameterDirection.Output;

                    _context.Database.ExecuteSqlRaw("BuscarSaldoDisponible @vendedorId, @saldoSolicitado, @resultadoBusqueda OUTPUT", vendedorIdParam, saldoSolicitadoParam, resultadoBusquedaParam);

                    var resultadoBusqueda = resultadoBusquedaParam.Value as string;

                    if (!string.IsNullOrEmpty(resultadoBusqueda))
                    {
                        tarjetaModel.Messages = resultadoBusqueda;
                    }
                    else
                    {
                        // La cadena es nula o vacía, así que establece un valor predeterminado
                        tarjetaModel.Messages = "4";
                    }
                }
                else
                {
                    tarjetaModel.Messages = "Has alcanzado el límite permitido. Por favor, contacta al soporte para obtener ayuda.";
                }
            }
            else
            {
                tarjetaModel.Messages = "Ha ocurrido un error. Por favor, contacta al soporte para obtener más información.";
            }

            return tarjetaModel;
        }

        [Route("add12")]
        [HttpPost]
        public ActionResult<TarjetaModel> VerificarSaldo12(Vendedor vendedor)
        {
            var vendedorEncontrado = _IUser.GetVendedorDetails().FirstOrDefault(v => v.contraseña == vendedor.contraseña);

            TarjetaModel tarjetaModel = new TarjetaModel();

            if (vendedorEncontrado != null)
            {

                var vendedorIdParam = new SqlParameter("@VendedorID", vendedorEncontrado.id);
                var resultadoParam = new SqlParameter("@Resultado", SqlDbType.Int);
                resultadoParam.Direction = ParameterDirection.Output;

                _context.Database.ExecuteSqlRaw("CompararAcumulados @VendedorID, @Resultado OUTPUT", vendedorIdParam, resultadoParam);

                int resultado = (int)resultadoParam.Value;

                if (resultado == 1)
                {
                    tarjetaModel.Messages = "Has alcanzado el límite permitido. Por favor, contacta al soporte para obtener ayuda.";
                }
                else if (resultado == -1)
                {

                    var saldoSolicitadoParam = new SqlParameter("@saldoSolicitado", vendedor.acumulado);
                    var saldoSolicitadoParamListoParaApp = new SqlParameter("@listosparaapp", vendedor.nombre);

                    var resultadoBusquedaParam = new SqlParameter("@resultadoBusqueda", SqlDbType.VarChar, -1);
                    resultadoBusquedaParam.Direction = ParameterDirection.Output;

                    _context.Database.ExecuteSqlRaw("BuscarSaldoDisponibleparaapp @vendedorId, @saldoSolicitado,@listosparaapp, @resultadoBusqueda OUTPUT", vendedorIdParam, saldoSolicitadoParam, saldoSolicitadoParamListoParaApp, resultadoBusquedaParam);

                    var resultadoBusqueda = resultadoBusquedaParam.Value as string;

                    if (!string.IsNullOrEmpty(resultadoBusqueda))
                    {
                        tarjetaModel.Messages = resultadoBusqueda;
                    }
                    else
                    {
                        // La cadena es nula o vacía, así que establece un valor predeterminado
                        tarjetaModel.Messages = "4";
                    }
                }
                else
                {
                    tarjetaModel.Messages = "Has alcanzado el límite permitido. Por favor, contacta al soporte para obtener ayuda.";
                }
            }
            else
            {
                tarjetaModel.Messages = "Ha ocurrido un error. Por favor, contacta al soporte para obtener más información.";
            }

            return tarjetaModel;
        }

        [Route("add2Kioskos")]
        [HttpPost]
        public ActionResult<TarjetaModel> VerificarSaldo2(Vendedor vendedor)
        {
            var vendedorEncontrado = _IUser.GetVendedorDetails().FirstOrDefault(v => v.contraseña == vendedor.contraseña);

            TarjetaModel tarjetaModel = new TarjetaModel();

            if (vendedorEncontrado != null)
            {

                var vendedorIdParam = new SqlParameter("@VendedorID", vendedorEncontrado.id);
                var resultadoParam = new SqlParameter("@Resultado", SqlDbType.Int);
                resultadoParam.Direction = ParameterDirection.Output;

                _context.Database.ExecuteSqlRaw("CompararAcumuladosKioskos @VendedorID, @Resultado OUTPUT", vendedorIdParam, resultadoParam);

                int resultado = (int)resultadoParam.Value;

                if (resultado == 1)
                {
                    tarjetaModel.Messages = "Has alcanzado el límite permitido. Por favor, contacta al soporte para obtener ayuda.";
                }
                else if (resultado == -1)
                {

                    var saldoSolicitadoParam = new SqlParameter("@saldoSolicitado", vendedor.acumulado);
                    var resultadoBusquedaParam = new SqlParameter("@resultadoBusqueda", SqlDbType.VarChar, -1);
                    resultadoBusquedaParam.Direction = ParameterDirection.Output;

                    _context.Database.ExecuteSqlRaw("BuscarSaldoDisponibleKioskos @vendedorId, @saldoSolicitado, @resultadoBusqueda OUTPUT", vendedorIdParam, saldoSolicitadoParam, resultadoBusquedaParam);

                    var resultadoBusqueda = resultadoBusquedaParam.Value as string;

                    if (!string.IsNullOrEmpty(resultadoBusqueda))
                    {
                        tarjetaModel.Messages = resultadoBusqueda;
                    }
                    else
                    {
                        // La cadena es nula o vacía, así que establece un valor predeterminado
                        tarjetaModel.Messages = "4";
                    }
                }
                else
                {
                    tarjetaModel.Messages = "Has alcanzado el límite permitido. Por favor, contacta al soporte para obtener ayuda.";
                }
            }
            else
            {
                tarjetaModel.Messages = "Ha ocurrido un error. Por favor, contacta al soporte para obtener más información.";
            }

            return tarjetaModel;
        }


        [HttpPost]
        [Route("obtenerVentasVendedor")]
        public ActionResult<VentasPorVendedorResponse> ObtenerVentasPorVendedor(Vendedor vendedor)
        {
            var vendedorEncontrado = _IUser.GetVendedorDetails().FirstOrDefault(v => v.contraseña == vendedor.contraseña);

            VentasPorVendedorResponse response = new VentasPorVendedorResponse();

            if (vendedorEncontrado != null)
            {
                var vendedorIdParam = new SqlParameter("@vendedor_id", vendedorEncontrado.id);
                var resultadoParam = new SqlParameter("@Resultado", SqlDbType.NVarChar, -1);
                resultadoParam.Direction = ParameterDirection.Output;

                _context.Database.ExecuteSqlRaw("ObtenerVentasPorVendedor @vendedor_id, @Resultado OUTPUT", vendedorIdParam, resultadoParam);



                var vendedorIdParam2 = new SqlParameter("@vendedor_id", vendedorEncontrado.id);
                var resultadoParam2 = new SqlParameter("@resultadoBusqueda", SqlDbType.NVarChar, -1);
                resultadoParam2.Direction = ParameterDirection.Output;

                _context.Database.ExecuteSqlRaw("obtenerEntradas @vendedor_id, @resultadoBusqueda OUTPUT", vendedorIdParam2, resultadoParam2);

                var vendedorIdParamm = new SqlParameter("@vendedor_id", vendedorEncontrado.id);
                var resultadoParamm = new SqlParameter("@resultadoBusqueda", SqlDbType.NVarChar, -1);
                resultadoParamm.Direction = ParameterDirection.Output;

                _context.Database.ExecuteSqlRaw("ObtenerVentasPorVendedorKioskos @vendedor_id, @resultadoBusqueda OUTPUT", vendedorIdParamm, resultadoParamm);



                var vendedorIdParamm120 = new SqlParameter("@vendedor_id", vendedorEncontrado.id);
                var resultadoParamm120 = new SqlParameter("@resultadoBusqueda", SqlDbType.NVarChar, -1);
                resultadoParamm120.Direction = ParameterDirection.Output;

                _context.Database.ExecuteSqlRaw("ObtenerVentasPorVendedorparaApp @vendedor_id, @resultadoBusqueda OUTPUT", vendedorIdParamm120, resultadoParamm120);

                var vendedorIdParammAppCorreo = new SqlParameter("@vendedor_id", vendedorEncontrado.id);
                var resultadoParammAppCorreo = new SqlParameter("@resultadoBusqueda", SqlDbType.NVarChar, -1);
                resultadoParammAppCorreo.Direction = ParameterDirection.Output;

                _context.Database.ExecuteSqlRaw("ObtenerVentasPorVendedorparaAppQueYaEstabanListasConCorreo @vendedor_id, @resultadoBusqueda OUTPUT", vendedorIdParammAppCorreo, resultadoParammAppCorreo);


                response.Tarjeta = resultadoParam.Value as string;

                response.boletos = resultadoParam2.Value as string;

                response.kiosko = resultadoParamm.Value as string;

                response.app = resultadoParamm120.Value as string;

                response.appListas = resultadoParammAppCorreo.Value as string;



            }
            else
            {
                response.Tarjeta = "";
            }

            return response;
        }

        public class Pases
        {
            public string contraseña { get; set; }
            public decimal numeros { get; set; }
            public string boletos { get; set; }

        }



        [HttpPost]
        [Route("obtenerboletos")]
        public ActionResult<TarjetaModel> VentasPases(Pases vendedor)
        {
            var vendedorEncontrado = _IUser.GetVendedorDetails().FirstOrDefault(v => v.contraseña == vendedor.contraseña);

            TarjetaModel tarjetaModel = new TarjetaModel();

            if (vendedorEncontrado != null)
            {
                var vendedorIdParam = new SqlParameter("@vendedor_id", vendedorEncontrado.id);
                var numeroBoletosParam = new SqlParameter("@numeroBoletos", vendedor.numeros);
                var numeroBoletosParam2 = new SqlParameter("@pelicula", vendedor.boletos);


                var resultadoParam = new SqlParameter("@resultadostarjetas", SqlDbType.NVarChar, -1);
                resultadoParam.Direction = ParameterDirection.Output;

                _context.Database.ExecuteSqlRaw("tarjetas2 @vendedor_id, @resultadostarjetas OUTPUT, @numeroBoletos, @pelicula", vendedorIdParam, resultadoParam, numeroBoletosParam, numeroBoletosParam2);

                tarjetaModel.Messages = resultadoParam.Value as string;
            }
            else
            {
                tarjetaModel.Messages = "Para obtener ayuda contacta al administrador";
            }

            return tarjetaModel;
        }


        [HttpPost]
        [Route("ventasPorVendedor")]
        public ActionResult<VentasPorVendedorResponse> ObtenerVentasPorVendedor2(Vendedor vendedor)
        {
            var vendedorEncontrado = _IUser.GetVendedorDetails().FirstOrDefault(v => v.contraseña == vendedor.contraseña);

            VentasPorVendedorResponse response = new VentasPorVendedorResponse();

            if (vendedorEncontrado != null)
            {
                var vendedorIdParam = new SqlParameter("@vendedor_id", vendedorEncontrado.id);
                var resultadoParam = new SqlParameter("@Resultado", SqlDbType.NVarChar, -1);
                resultadoParam.Direction = ParameterDirection.Output;

                _context.Database.ExecuteSqlRaw("ObtenerVentasPorVendedor2 @vendedor_id, @Resultado OUTPUT", vendedorIdParam, resultadoParam);

                response.Tarjeta = resultadoParam.Value as string;

            }
            else
            {
                response.Tarjeta = "Vendedor no encontrado";
            }

            return response;
        }

        public class Reporte
        {
            public string contraseña { get; set; }
            public string version { get; set; }
            public string tarjeta { get; set; }



        }

        [HttpPost]
        [Route("agregarReporte")]
        public ActionResult<VentasPorVendedorResponse> Agregarreporte(Reporte vendedor)
        {

            VentasPorVendedorResponse response = new VentasPorVendedorResponse();

           
                var vendedorIdParam = new SqlParameter("@contraseña", vendedor.contraseña);

                var vendedorIdParamversion = new SqlParameter("@version",vendedor.version);

                var vendedorIdParamtarjeta = new SqlParameter("@tarjeta", vendedor.tarjeta);

                var resultadoParam = new SqlParameter("@resultadoBusqueda", SqlDbType.NVarChar, -1);


                resultadoParam.Direction = ParameterDirection.Output;

            _context.Database.ExecuteSqlRaw("InsertarEnReporteDeTarjeta @contraseña, @version, @tarjeta, @resultadoBusqueda OUTPUT", vendedorIdParam, vendedorIdParamversion, vendedorIdParamtarjeta, resultadoParam);

                response.Tarjeta = resultadoParam.Value as string;

         

            return response;
        }

        [HttpPost]
        [Route("ventasPorVendedorPorTarjeta")]
        public ActionResult<VentasPorVendedorResponse> ObtenerVentasPorVendedorPorTarjeta(VentasPorVendedorPorTarjeta vendedor)
        {
            VentasPorVendedorResponse response = new VentasPorVendedorResponse();

                var vendedorIdParam = new SqlParameter("@numeroTarjeta", vendedor.tarjeta);
                var vendedorIdParamcontra = new SqlParameter("@contraseña", vendedor.contraseña);
                var resultadoParam = new SqlParameter("@resultadoBusqueda", SqlDbType.NVarChar, -1);
                resultadoParam.Direction = ParameterDirection.Output;

                _context.Database.ExecuteSqlRaw("RevisarTarjetasporVendedorConbaseSusVentas @numeroTarjeta,@contraseña, @resultadoBusqueda OUTPUT", vendedorIdParam, vendedorIdParamcontra, resultadoParam);

                response.Tarjeta = resultadoParam.Value as string;

                
          
            return response;
        }


        [HttpPost]
        [Route("ventasPorVendedorPorTarjetaObtenerPin")]
        public ActionResult<VentasPorVendedorResponse> ObtenerVentasPorVendedorPorTarjetaObtenerPin(VentasPorVendedorPorTarjeta vendedor)
        {
            VentasPorVendedorResponse response = new VentasPorVendedorResponse();

            var vendedorIdParam = new SqlParameter("@numeroTarjeta", vendedor.tarjeta);
            var vendedorIdParamcontra = new SqlParameter("@contraseña", vendedor.contraseña);
            var resultadoParam = new SqlParameter("@resultadoBusqueda", SqlDbType.NVarChar, -1);
            resultadoParam.Direction = ParameterDirection.Output;

            _context.Database.ExecuteSqlRaw("RevisarTarjetasporVendedorConbaseSusVentasAñadirPindelaTarjeta @numeroTarjeta,@contraseña, @resultadoBusqueda OUTPUT", vendedorIdParam, vendedorIdParamcontra, resultadoParam);

            response.Tarjeta = resultadoParam.Value as string;



            return response;
        }

        [HttpPost]
        [Route("ventasPorVendedorPorTarjetaObtenerPinChatPagina")]
        public ActionResult<VentasPorVendedorResponse> ObtenerVentasPorVendedorPorTarjetaObtenerPinChatPagina(VentasPorVendedorResponseAdminVendedor vendedor)
        {
            VentasPorVendedorResponse response = new VentasPorVendedorResponse();

            var vendedorIdParamcontra = new SqlParameter("@contraseña", vendedor.contra);
            var resultadoParam = new SqlParameter("@resultadoBusqueda", SqlDbType.NVarChar, -1);
            resultadoParam.Direction = ParameterDirection.Output;

            _context.Database.ExecuteSqlRaw("RevisarTarjetasporVendedorConbaseSusVentasTelegramChatId @contraseña, @resultadoBusqueda OUTPUT", vendedorIdParamcontra, resultadoParam);

            response.Tarjeta = resultadoParam.Value as string;



            return response;
        }



        [HttpPost]

        [Route("ventasAdminporvendedores")]
        public async Task<ActionResult<VentasPorVendedorResponse>> ObtenerVentasPorVendedor4Async(VentasPorVendedorResponseAdmin vendedor)
        {
            VentasPorVendedorResponse response = new VentasPorVendedorResponse();

            if (vendedor != null && vendedor.Email != null && vendedor.contra != null)
            {
                var user = await GetUser4(vendedor.Email, vendedor.contra);

                if (user != null)
                {

                    var vendedorEncontrado = _IUser.GetVendedorDetails().FirstOrDefault(v => v.contraseña == vendedor.contra2);


                    if (vendedorEncontrado.id == 4)
                    {
                        var vendedorEncontrado2 = _IUser.GetVendedorDetails().FirstOrDefault(v => v.contraseña == vendedor.vendedor);





                        if (vendedorEncontrado2 != null)
                        {



                            var vendedorIdParam = new SqlParameter("@vendedor_id", vendedorEncontrado2.id);
                            var resultadoParam = new SqlParameter("@Resultado", SqlDbType.NVarChar, -1);
                            resultadoParam.Direction = ParameterDirection.Output;

                            _context.Database.ExecuteSqlRaw("ActualizarSaldos @vendedor_id, @Resultado OUTPUT", vendedorIdParam, resultadoParam);

                            response.Tarjeta = resultadoParam.Value as string;

                        }
                        else
                        {
                            response.Tarjeta = "2";
                            return response;
                        }


                    }
                    else
                    {
                        response.Tarjeta = "4";
                        return response;
                    }

                    return response;
                }
                else
                {
                    response.Tarjeta = "revisar información";
                    return response;
                }


            }
            else
            {

                response.Tarjeta = "revisar información";
                return response;
            }
        }








        [HttpPost]

        [Route("ventasAdminporvendedoresVendedor")]
        public async Task<ActionResult<VentasPorVendedorResponse>> ObtenerVentasPorVendedor4AsyncVendedores(VentasPorVendedorResponseAdminVendedor vendedor)
        {
            VentasPorVendedorResponse response = new VentasPorVendedorResponse();

      

                
                    var vendedorEncontrado = _IUser.GetVendedorDetails().FirstOrDefault(v => v.contraseña == vendedor.contra);


                    if (vendedorEncontrado!= null) 
                    {
                        response.Tarjeta = "16";
                        return response;

                    }
                    else
                    {
                        response.Tarjeta = "4";
                        return response;
                    }

     

        }



        [HttpPost]

        [Route("ventasAdminporvendedoresVendedorTelegram")]
        public async Task<ActionResult<VentasPorVendedorResponse>> ObtenerVentasPorVendedor4AsyncVendedoresChatTelegram(VentasPorVendedorResponseAdminVendedor vendedor)
        {
            VentasPorVendedorResponse response = new VentasPorVendedorResponse();




            var vendedorIdParam = new SqlParameter("@chatIdTelegram", vendedor.contra);
            var resultadoParam = new SqlParameter("@resultadoBusqueda", SqlDbType.NVarChar, -1);

            resultadoParam.Direction = ParameterDirection.Output;

            _context.Database.ExecuteSqlRaw("ObtenerContrasenaPorChatIdTelegram @chatIdTelegram, @resultadoBusqueda OUTPUT", vendedorIdParam, resultadoParam);

            response.Tarjeta = resultadoParam.Value as string;

            return response;



        }






        [HttpPost]

        [Route("ventasAdminporvendedoresVendedorTelegramChat")]
        public async Task<ActionResult<VentasPorVendedorResponse>> ObtenerVentasPorVendedor4AsyncVendedoresChatTelegramContraseña(VentasPorVendedorResponseAdminVendedorContraseñaTelegram vendedor)
        {
            VentasPorVendedorResponse response = new VentasPorVendedorResponse();


            var vendedorIdParamContraseña = new SqlParameter("@contraseña", vendedor.contra);

            var vendedorIdParam = new SqlParameter("@chatIdTelegram", vendedor.chatId);

            var resultadoParam = new SqlParameter("@resultadoBusqueda", SqlDbType.NVarChar, -1);

            resultadoParam.Direction = ParameterDirection.Output;

            _context.Database.ExecuteSqlRaw("AñadirChatIdPorContrasenaParaTelegram @contraseña, @chatIdTelegram, @resultadoBusqueda OUTPUT", vendedorIdParamContraseña, vendedorIdParam, resultadoParam);

            response.Tarjeta = resultadoParam.Value as string;

            return response;



        }





        [HttpPost]

        [Route("ventasAdminporvendedores2")]
        public async Task<ActionResult<VentasPorVendedorResponse>> ObtenerVentasPorVendedor4Async2(VentasPorVendedorResponseAdmin2 vendedor)
        {
            VentasPorVendedorResponse response = new VentasPorVendedorResponse();

            if (vendedor != null && vendedor.Email != null && vendedor.contra != null)
            {
                var user = await GetUser4(vendedor.Email, vendedor.contra);

                if (user != null)
                {

                    var vendedorEncontrado = _IUser.GetVendedorDetails().FirstOrDefault(v => v.contraseña == vendedor.contra2);



                    if (vendedorEncontrado.id == 4)
                    {
                        var vendedorEncontrado2 = _IUser.GetVendedorDetails().FirstOrDefault(v => v.contraseña == vendedor.vendedor);


                        if (vendedor.instru == "Establecer")
                        {



                            if (vendedorEncontrado2 != null)
                            {



                                var vendedorIdParam = new SqlParameter("@vendedor_id", vendedorEncontrado2.id);
                                var resultadoParam = new SqlParameter("@Resultado", SqlDbType.NVarChar, -1);
                                var nuevoSaldoParam = new SqlParameter("@nuevo_saldo", SqlDbType.Decimal);
                                nuevoSaldoParam.Value = vendedor.num; // Aquí establece el valor del nuevo saldo

                                resultadoParam.Direction = ParameterDirection.Output;

                                _context.Database.ExecuteSqlRaw("ActualizarSaldos2 @vendedor_id, @nuevo_saldo, @Resultado OUTPUT", vendedorIdParam, nuevoSaldoParam, resultadoParam);

                                response.Tarjeta = resultadoParam.Value as string;

                                return response;


                            }
                            else
                            {
                                response.Tarjeta = "2";
                                return response;
                            }







                        }


                        if (vendedor.instru == "Sumar")
                        {



                            if (vendedorEncontrado2 != null)
                            {



                                var vendedorIdParam = new SqlParameter("@vendedor_id", vendedorEncontrado2.id);
                                var resultadoParam = new SqlParameter("@Resultado", SqlDbType.NVarChar, -1);
                                var nuevoSaldoParam = new SqlParameter("@nuevo_saldo", SqlDbType.Decimal);
                                nuevoSaldoParam.Value = vendedor.num; // Aquí establece el valor del nuevo saldo

                                resultadoParam.Direction = ParameterDirection.Output;

                                _context.Database.ExecuteSqlRaw("ActualizarSaldos26 @vendedor_id, @nuevo_saldo, @Resultado OUTPUT", vendedorIdParam, nuevoSaldoParam, resultadoParam);

                                response.Tarjeta = resultadoParam.Value as string;

                                return response;


                            }
                            else
                            {
                                response.Tarjeta = "2";
                                return response;
                            }







                        }

                        if (vendedor.instru == "Tarjetas")
                        {



                            if (vendedorEncontrado2 != null)
                            {



                                var vendedorIdParam = new SqlParameter("@vendedor_id", vendedorEncontrado2.id);
                                var resultadoParam = new SqlParameter("@Resultado", SqlDbType.NVarChar, -1);

                                resultadoParam.Direction = ParameterDirection.Output;

                                _context.Database.ExecuteSqlRaw("ObtenerVentasPorVendedor4 @vendedor_id, @Resultado OUTPUT", vendedorIdParam, resultadoParam);

                                response.Tarjeta = resultadoParam.Value as string;

                                return response;


                            }
                            else
                            {
                                response.Tarjeta = "2";
                                return response;
                            }







                        }

                        if (vendedor.instru == "Revisar")
                        {



                            if (vendedorEncontrado2 != null)
                            {



                                var vendedorIdParam = new SqlParameter("@Password", vendedorEncontrado2.contraseña);
                                var resultadoParam = new SqlParameter("@Resultado", SqlDbType.NVarChar, -1);

                                resultadoParam.Direction = ParameterDirection.Output;

                                _context.Database.ExecuteSqlRaw("VerificarAcumuladoParaVendedor2 @Password, @Resultado OUTPUT", vendedorIdParam, resultadoParam);

                                response.Tarjeta = resultadoParam.Value as string;

                                return response;


                            }
                            else
                            {
                                response.Tarjeta = "2";
                                return response;
                            }







                        }
                        if (vendedor.instru == "Boletos")
                        {



                            if (vendedorEncontrado2 != null)
                            {



                                var vendedorIdParam = new SqlParameter("@vendedor_id", vendedorEncontrado2.id);
                                var resultadoParam = new SqlParameter("@resultadoBusqueda", SqlDbType.NVarChar, -1);

                                resultadoParam.Direction = ParameterDirection.Output;

                                _context.Database.ExecuteSqlRaw("borrarboletosvendedor @vendedor_id, @resultadoBusqueda OUTPUT", vendedorIdParam, resultadoParam);

                                response.Tarjeta = resultadoParam.Value as string;

                                return response;


                            }
                            else
                            {
                                response.Tarjeta = "2";
                                return response;
                            }







                        }
                        if (vendedor.instru == "Kioskos")
                        {



                            if (vendedorEncontrado2 != null)
                            {



                                var vendedorIdParam = new SqlParameter("@vendedor_id", vendedorEncontrado2.id);
                                var resultadoParam = new SqlParameter("@resultadoBusqueda", SqlDbType.NVarChar, -1);

                                resultadoParam.Direction = ParameterDirection.Output;

                                _context.Database.ExecuteSqlRaw("ActualizarSaldosKioskos @vendedor_id, @resultadoBusqueda OUTPUT", vendedorIdParam, resultadoParam);

                                response.Tarjeta = resultadoParam.Value as string;

                                return response;


                            }
                            else
                            {
                                response.Tarjeta = "2";
                                return response;
                            }







                        }
                        if (vendedor.instru == "app")
                        {



                            if (vendedorEncontrado2 != null)
                            {



                                var vendedorIdParam = new SqlParameter("@vendedor_id", vendedorEncontrado2.id);
                                var resultadoParam = new SqlParameter("@resultadoBusqueda", SqlDbType.NVarChar, -1);

                                resultadoParam.Direction = ParameterDirection.Output;

                                _context.Database.ExecuteSqlRaw("ActualizarSaldosapp @vendedor_id, @resultadoBusqueda OUTPUT", vendedorIdParam, resultadoParam);

                                response.Tarjeta = resultadoParam.Value as string;

                                return response;


                            }
                            else
                            {
                                response.Tarjeta = "2";
                                return response;
                            }







                        }

                        if (vendedor.instru == "all")
                        {



                            if (vendedorEncontrado2 != null)
                            {

                                var vendedorIdParam = new SqlParameter("@vendedor_id", vendedorEncontrado2.id);
                                var resultadoParam = new SqlParameter("@resultadoBusqueda", SqlDbType.NVarChar, -1);

                                resultadoParam.Direction = ParameterDirection.Output;


                                // ActualizarSaldosTarjetas

                                _context.Database.ExecuteSqlRaw("ActualizarSaldos @vendedor_id, @resultadoBusqueda OUTPUT", vendedorIdParam, resultadoParam);

                                response.Tarjeta += resultadoParam.Value as string;

                                // ActualizarSaldosKioskos
                                _context.Database.ExecuteSqlRaw("ActualizarSaldosKioskos @vendedor_id, @resultadoBusqueda OUTPUT", vendedorIdParam, resultadoParam);
                                response.Tarjeta += resultadoParam.Value as string;

                                // borrarboletosvendedor
                                _context.Database.ExecuteSqlRaw("borrarboletosvendedor @vendedor_id, @resultadoBusqueda OUTPUT", vendedorIdParam, resultadoParam);
                                response.Tarjeta += resultadoParam.Value as string;

                                // ActualizarSaldosapp
                                _context.Database.ExecuteSqlRaw("ActualizarSaldosapp @vendedor_id, @resultadoBusqueda OUTPUT", vendedorIdParam, resultadoParam);
                                response.Tarjeta += resultadoParam.Value as string;

                                return response;



                            }
                            else
                            {
                                response.Tarjeta = "2";
                                return response;
                            }







                        }
                        else
                        {
                            response.Tarjeta = "4";
                            return response;
                        }





                    }
                    else
                    {
                        response.Tarjeta = "4";
                        return response;
                    }

                    return response;
                }
                else
                {
                    response.Tarjeta = "revisar información";
                    return response;
                }


            }
            else
            {

                response.Tarjeta = "revisar información";
                return response;
            }
        }


        private async Task<User> GetUser(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }


        private async Task<User> GetUser2(string email, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Passsword == password);
        }



        private async Task<Administradores> GetUser4(string email, string password)
        {
            return await _context.Administradoress.FirstOrDefaultAsync(u => u.Email == email && u.Passsword == password);
        }

    }
}