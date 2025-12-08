using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Mapper.Application.CommandsAndQueries.GeoMap.Commands.UpdateGeoMapCommand;
using Mapper.Application.Common.Mappings;

namespace Mapper.WebApi.Models
{
    public class UpdateGeoMapDto : IMapWith<UpdateGeoMapCommand>
    {
        [Required]
        public Guid Id { get; set; }
        public required string MapName { get; set; }
        public required string MapDescription { get; set; }
        public bool IsArchived { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UpdateGeoMapDto, UpdateGeoMapCommand>()
                .ForMember(mapCommand => mapCommand.Id,
                    opt => opt.MapFrom(mapDto => mapDto.Id))
                .ForMember(mapCommand => mapCommand.MapName,
                    opt => opt.MapFrom(mapDto => mapDto.MapName))
                .ForMember(mapCommand => mapCommand.MapDescription,
                    opt => opt.MapFrom(mapDto => mapDto.MapDescription))
                .ForMember(mapCommand => mapCommand.IsArchived,
                    opt => opt.MapFrom(mapDto => mapDto.IsArchived));
        }
    }
}
