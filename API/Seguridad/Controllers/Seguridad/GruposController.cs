using API.Seguridad.Application.Seguridad.Grupos.Commands;
using API.Seguridad.Application.Seguridad.Grupos.Queries;
using API.Seguridad.DTOs.Seguridad;
using Microsoft.AspNetCore.Mvc;

namespace API.Seguridad.Controllers.Seguridad
{
    
    /// <summary>
    /// Controlador para gestionar los grupos de Azure AD.
    /// </summary>
    public class GruposController(ILogger<UsersController> logger) : BaseApiController
    {
        /// <summary>
        /// Obtiene la lista de grupos de Azure AD.
        /// </summary>
        /// <returns>Lista de grupos de Azure AD.</returns>
        /// <response code="200">Lista de grupos obtenida correctamente.</response>
        /// <response code="404">No se encontraron grupos.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        public async Task<ActionResult<List<GroupDto>>> GetGrupos()
        {
            logger.LogInformation("Fetching all groups at {Time}", DateTime.UtcNow);
            logger.LogError("An error occurred while fetching groups at {Time}", DateTime.UtcNow);
            return HandleResult(await Mediator.Send(new GetGrupoList.Query()));
        }

        /// <summary>
        /// Obtiene un grupo de Azure AD por su ID.
        /// </summary>
        /// <param name="id" >El ID del grupo de Azure AD.</param>
        /// <returns>El grupo de Azure AD</returns>
        /// <response code="200">Grupo obtenido correctamente.</response>
        /// <response code="404">No se encontro el grupo</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> GetGrupo(string id)
        {
            return HandleResult(await Mediator.Send(new GetGrupoById.Query { Id = id }));
        }

        /// <summary>
        /// Crea un nuevo grupo de Azure AD.
        /// </summary>
        /// <param name="command">El comando para crear el grupo.</param>
        /// <returns>El ID del grupo creado.</returns>
        /// <response code="200">Grupo creado correctamente.</response>
        /// <response code="400">Error al crear el grupo.</response>
        /// <response code="500">Error interno del servidor.</response> 
        [HttpPost]
        public async Task<ActionResult<string>> CreateGrupo([FromBody] CreateGrupo.Command command)
        {
            return HandleResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Actualiza la informacion de un grupo de Azure AD.
        /// </summary>
        /// <param name="id">El ID del grupo de Azure AD.</param>
        /// <returns>El grupo de Azure AD</returns>
        /// <response code="200">Grupo actualizado correctamente.</response>
        /// <response code="400">Error al actualizar el grupo.</response>
        /// <response code="404">No se encontro el grupo</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateGrupo(string id, [FromBody] UpdateGrupo.Command command)
        {
            command.Id = id;
            return HandleResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Inactiva un grupo de Azure AD.
        /// </summary>
        /// <param name="id">El ID del grupo de Azure AD.</param>
        /// <returns></returns>
        /// <response code="200">Grupo inactivado correctamente.</response>
        /// <response code="400">Error al inactivar el grupo.</response>
        /// <response code="404">No se encontro el grupo</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGrupo(string id)
        {
            return HandleResult(await Mediator.Send(new DeleteGrupo.Command { Id = id }));
        }
    }
    
}