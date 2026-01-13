using Asp.Versioning;
using AutoMapper;
using Mapper.Application.Features.DTOs;
using Mapper.Application.Features.GeoMaps.Commands.CreateGeoMap;
using Mapper.Application.Features.GeoMaps.Queries.GetGeoMapById;
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

        /// <summary>
        /// Gets the list of geomaps
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /geomap
        /// </remarks>
        /// <returns>Returns GeoMapListVm</returns>
        /// <response code="200">Success</response>
        /// <response code="401">If the user is unauthorized</response>
        //[HttpGet]
        ////[Authorize]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult<GeoMapListVm>> GetAll()
        //{
        //    var query = new GetGeoMapListQuery();
        //    var vm = await Mediator.Send(query);
        //    return Ok(vm);
        //}

        /// <summary>
        /// Gets the geomap by id
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /geomap/D34D349E-43B8-429E-BCA4-793C932FD580
        /// </remarks>
        /// <param name="id">Geomap id (guid)</param>
        /// <returns>Returns GeomapDetailsVm</returns>
        /// <response code="200">Success</response>
        /// <response code="401">If the user in unauthorized</response>
        //[HttpGet("{id}")]
        ////[Authorize]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult<GeoMapListVm>> Get(Guid id)
        //{
        //    var query = new GetGeoMapDetailsQuery()
        //    {
        //        Id = id
        //    };
        //    var vm = await Mediator.Send(query);
        //    return Ok(vm);
        //}

        /// <summary>
        /// Creates geomap
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// POST /geomap
        /// {
        ///     title: "geomap title",
        ///     details: "geomap details"
        /// }
        /// </remarks>
        /// <param name="creategeomapDto">CreategeomapDto object</param>
        /// <returns>Returns id (guid)</returns>
        /// <response code="201">Success</response>
        /// <response code="401">If the user is unauthorized</response>
        //[HttpPost]
        ////[Authorize]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult<Guid>> Create([FromBody] CreateGeoMapDto createGeoMapDto)
        //{
        //    var command = _mapper.Map<Application.CommandsAndQueries.GeoMap.Commands.CreateGeoMapCommand.CreateGeoMapCommand>(createGeoMapDto);
        //    //command.Id = UserId;
        //    var geomapId = await Mediator.Send(command);
        //    return CreatedAtAction(nameof(Get), new { id = geomapId }, geomapId);
        //}

        /// <summary>
        /// Updates the geomap
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// PUT /geomap
        /// {
        ///     title: "updated geomap title"
        /// }
        /// </remarks>
        /// <param name="updategeomapDto">UpdategeomapDto object</param>
        /// <returns>Returns NoContent</returns>
        /// <response code="204">Success</response>
        /// <response code="401">If the user is unauthorized</response>
        //[HttpPut]
        ////[Authorize]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<IActionResult> Update([FromBody] UpdateGeoMapDto updateGeoMapDto)
        //{
        //    var command = _mapper.Map<UpdateGeoMapCommand>(updateGeoMapDto);
        //    command.Id = UserId;
        //    await Mediator.Send(command);
        //    return NoContent();
        //}

        /// <summary>
        /// Archives the geomap by id
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// PUT /geomap/88DEB432-062F-43DE-8DCD-8B6EF79073D3
        /// </remarks>
        /// <param name="id">Id of the geomap (guid)</param>
        /// <returns>Returns NoContent</returns>
        /// <response code="204">Success</response>
        /// <response code="401">If the user is unauthorized</response>
        //[HttpPut("{id}")]
        ////[Authorize]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<IActionResult> Archive(Guid id)
        //{
        //    var command = new ArchiveGeoMapCommand
        //    {
        //        Id = id
        //    };
        //    await Mediator.Send(command);
        //    return NoContent();
        //}

        /// <summary>
        /// Deletes the geomap by id
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// DELETE /geomap/88DEB432-062F-43DE-8DCD-8B6EF79073D3
        /// </remarks>
        /// <param name="id">Id of the geomap (guid)</param>
        /// <returns>Returns NoContent</returns>
        /// <response code="204">Success</response>
        /// <response code="401">If the user is unauthorized</response>
        //[HttpDelete("{id}")]
        ////[Authorize]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var command = new DeleteGeoMapCommand
        //    {
        //        Id = id
        //    };
        //    await Mediator.Send(command);
        //    return NoContent();
        //}
    }
}
