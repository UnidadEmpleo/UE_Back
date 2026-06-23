using API.Seguridad.Application.Anexos.Commands;
using API.Seguridad.Application.Anexos.Queries;
using API.Seguridad.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace API.Seguridad.Controllers.Anexos
{
    public class AnexoController : BaseApiController
    {
        [HttpPost("Asociacion/{linkId}")]
        public async Task<IActionResult> CreateAnexo(int linkId, [FromForm] CreateAnexo.Command command)
        {
            command.LinkId = linkId;
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnexo(int id, [FromBody] UpdateAnexo.Command command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnexo(int id)
        {
            var result = await Mediator.Send(new DeleteAnexo.Command { Id = id });
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetAnexoById.Query { Id = id });
            return HandleResult(result);
        }

        [HttpGet("Paciente/{pacienteId}")]
        public async Task<IActionResult> GetByPacienteId(int pacienteId)
        {
            var result = await Mediator.Send(new GetAnexosByPacienteId.Query { PacienteId = pacienteId });
            return HandleResult(result);
        }

        [HttpGet("{id}/blob")]
        public async Task<IActionResult> GetBlobById(int id)
        {
            var result = await Mediator.Send(new GetAnexoBlobById.Query { Id = id });
            return HandleResult(result);
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var result = await Mediator.Send(new GetAnexoFileById.Query { Id = id });
            
            if (!result.IsSuccess || result.Value.Blob == null)
                return NotFound(result.Error);

            return File(result.Value.Blob, result.Value.MimeType, result.Value.FileName);
        }
    }
}