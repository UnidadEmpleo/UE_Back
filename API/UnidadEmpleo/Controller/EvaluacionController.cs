using API.Persistence;
using API.Seguridad.Application.Core;
using API.Seguridad.Application.Seguridad.Usuarios.Queries;
using API.Seguridad.Controllers;
using API.Seguridad.Controllers.Seguridad;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.DTOs.Seguridad;
using API.Seguridad.Infrastructure.Authorization;
using API.UnidadEmpleo.Application.Evln;
using API.UnidadEmpleo.Application.ReferenciaApp;
using API.UnidadEmpleo.Application.SolicitudSvs;
using API.UnidadEmpleo.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;


namespace API.UnidadEmpleo.Controller
{
    public class EvaluacionController : BaseApiController
    {
        private readonly UserManager<AppUserIdentity> _userManager;
        private readonly SignInManager<AppUserIdentity> _signInManager;
        private readonly ILogger<AuthController> _logger;

        public EvaluacionController(UserManager<AppUserIdentity> userManager,
            SignInManager<AppUserIdentity> signInManager,ILogger<AuthController> logger){
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create(EvaluacionCreate.Command command)
        {
            LoginDto loginDto = new LoginDto { Username = command.Username, Password = command.Password };

            //FALTA VALIDAR QUE SOLO SEAN LOS PERFILES Y DE ACUERDO A SU TIPO PARA CREAR EL USUARIO
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null) {             
                _logger.LogError(null, $"Tratao de crear una evaluación {loginDto.Username}, fallo el identificador de autenticación");
                return Unauthorized("Firma Usuario y/o contraseña no validos");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded){
                _logger.LogError(null, $"Tratao de crear una evaluación {loginDto.Username}, fallo su firma de autenticación");
                return Unauthorized("Usuario o contraseña no validos ");
            }

            var resultUser = await Mediator.Send(new EvaluacionValidaUsuario.Query { Id = user.Id });

            if (!resultUser.IsSuccess)
                return HandleResult(resultUser);
            
            command.UsuarioIngreso = user.Id;

            return HandleResult(await Mediator.Send(command));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTermino(int id, EvaluacionUpdate.Command command)
        {

            LoginDto loginDto = new LoginDto { Username = command.Username, Password = command.Password };

            //FALTA VALIDAR QUE SOLO SEAN LOS PERFILES Y DE ACUERDO A SU TIPO PARA CREAR EL USUARIO
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
            {
                _logger.LogError(null, $"Tratao de crear una evaluación {loginDto.Username}, fallo el identificador de autenticación");
                return Unauthorized("Firma Usuario y/o contraseña no validos");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                _logger.LogError(null, $"Tratao de crear una evaluación {loginDto.Username}, fallo su firma de autenticación");
                return Unauthorized("Usuario o contraseña no validos ");
            }

            var resultUser = await Mediator.Send(new EvaluacionValidaUsuario.Query { Id = user.Id });

            if (!resultUser.IsSuccess)
                return HandleResult(resultUser);

            command.Salida = DateTime.Now;
            command.UsuarioSalida = user.Id;

            command.IdRequest = id;
            command.opcion = 1; //asigna termino

            return HandleResult(await Mediator.Send(command));
        }

        [HttpPut("evaluo/{id}")]
        public async Task<IActionResult> UpdateEvaluo(int id, EvaluacionUpdate.Command command)
        {
            LoginDto loginDto = new LoginDto { Username = command.Username, Password = command.Password };

            //FALTA VALIDAR QUE SOLO SEAN LOS PERFILES Y DE ACUERDO A SU TIPO PARA CREAR EL USUARIO -- front end lo trabaja
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
            {
                _logger.LogError(null, $"Tratao de crear una evaluación {loginDto.Username}, fallo el identificador de autenticación");
                return Unauthorized("Firma Usuario y/o contraseña no validos");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                _logger.LogError(null, $"Tratao de crear una evaluación {loginDto.Username}, fallo su firma de autenticación");
                return Unauthorized("Usuario o contraseña no validos ");
            }

            var resultUser = await Mediator.Send(new EvaluacionValidaUsuario.Query { Id = user.Id });

            if (!resultUser.IsSuccess)
                return HandleResult(resultUser);
            
            command.NombreUsuarioEvaluo = resultUser.Value.NombreCompleto;
            command.UsuarioEvaluo = user.Id;
            
            command.IdRequest = id;
            command.opcion = 2; 

            var resultCommand = await Mediator.Send(command);

            var resultEvaluacion = await Mediator.Send(new GetEvaluacionListBySolicitud.Query { idsolicitud = command.IdSoliciud });
            if (resultEvaluacion.IsSuccess)
            {
                var resultAspEval = await Mediator.Send(new GetSolicitud.Query { Id = command.IdSoliciud });
                int noEvals = resultAspEval.Value.Aspirante.Sexo == Sexo.Femenino ? 5 : 4;
                List<Evaluacion> evals = resultEvaluacion.Value;
                if (evals.Count == noEvals)
                {
                    Boolean apto = true;
                    Boolean revalorado = false;
                    foreach (Evaluacion e in evals)
                    {
                        if (!e.Resultado) apto = false;
                        if (e.revalorable) revalorado = true;
                    }

                    var resultUpdAptitud = await Mediator.Send(
                        new SolicitudUpdateStatus.Command { IdSolicitud = command.IdSoliciud, Revalorable = revalorado, Status = apto ? StatusSolicitud.Apto : StatusSolicitud.No_apto });

                }
            }


            return HandleResult(resultCommand);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return HandleResult(await Mediator.Send(new EvaluacionDelete.Command { Id = id }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecordId(int id)
        {
            return HandleResult(await Mediator.Send(new GetEvaluacion.Query { Id = id }));
        }

        [HttpGet]
        public async Task<IActionResult> ListAspirantes()
        {
            return HandleResult(await Mediator.Send(new GetEvaluacionList.Query()));
        }

        [HttpGet("solicitud/{idsolicitud}")]
        public async Task<IActionResult> GetEvaluacionListBySolicitud(int idsolicitud)
        {
            return HandleResult(await Mediator.Send(new GetEvaluacionListBySolicitud.Query { idsolicitud = idsolicitud }));
        }

       
    }
}
