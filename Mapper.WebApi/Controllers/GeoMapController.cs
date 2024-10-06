using AutoMapper;
using System;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Mapper.Application;
using Mapper.Application.CommandsAndQueries.GeoMap.Commands.CreateGeoMapCommand;
using Mapper.Application.CommandsAndQueries.GeoMap.Commands.DeleteGeoMapCommand;
using Mapper.Application.CommandsAndQueries.GeoMap.Commands.UpdateGeoMapCommand;
using Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapDetails;
using Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapList;
using Mapper.WebApi.Models;


namespace Mapper.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [Route("api/{version:apiVersion}/[controller]")]
    public class GeomapController : BaseController
    {
        private readonly IMapper _mapper;

        public GeomapController(IMapper mapper) => _mapper = mapper;

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
        [HttpGet]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<GeoMapListVm>> GetAll()
        {
            var query = new GetGeoMapDetailsQuery()
            {
                //Id = 
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

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
        [HttpGet("{id}")]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<GeoMapListVm>> Get(Guid id)
        {
            var query = new GetGeoMapDetailsQuery()
            {
                Id = id
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

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
        [HttpPost]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateGeoMapDto createGeoMapDto)
        {
            var command = _mapper.Map<CreateGeoMapCommand>(createGeoMapDto);
            command.Id = UserId;
            var geomapId = await Mediator.Send(command);
            return Ok(geomapId);
        }

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
        [HttpPut]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update([FromBody] UpdateGeoMapCommand updateGeoMapDto)
        {
            var command = _mapper.Map<UpdateGeoMapCommand>(updateGeoMapDto);
            command.Id = UserId;
            await Mediator.Send(command);
            return NoContent();
        }
        
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
        [HttpDelete("{id}")]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Archive(Guid id)
        {
            var command = new ArchiveGeoMapCommand
            {
                Id = id
            };
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
