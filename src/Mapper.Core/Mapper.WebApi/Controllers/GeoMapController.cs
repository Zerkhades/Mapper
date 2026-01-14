using Asp.Versioning;
using AutoMapper;
using Mapper.Application.Features.DTOs;
using Mapper.Application.Features.GeoMaps.Commands.CreateGeoMap;
using Mapper.Application.Features.GeoMaps.Commands.DeleteGeoMap;
using Mapper.Application.Features.GeoMaps.Queries.GetGeoMapById;
using Mapper.Application.Features.GeoMaps.Queries.GetGeoMapList;
using Mapper.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Mapper.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/geomaps")]
    public class GeomapController : BaseController
    {
        private readonly IMapper _mapper;

        public GeomapController(IMapper mapper) => _mapper = mapper;

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Guid>> Create([FromForm] CreateGeoMapDto dto, IFormFile file, CancellationToken ct)
        {
            await using var stream = file.OpenReadStream();

            var id = await Mediator.Send(new CreateGeoMapCommand(
                dto.Name,
                dto.Description,
                stream,
                file.FileName,
                file.ContentType,
                dto.ImageWidth,
                dto.ImageHeight
            ), ct);

            return Ok(id);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GeoMapDetailsDto>> Get(Guid id, CancellationToken ct)
            => Ok(await Mediator.Send(new GetGeoMapByIdQuery(id), ct));

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<GeoMapListItemDto>>> GetList(CancellationToken ct)
            => Ok(await Mediator.Send(new GetGeoMapListQuery(), ct));

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await Mediator.Send(new DeleteGeoMapCommand(id), ct);
            return NoContent();
        }

    }
}
