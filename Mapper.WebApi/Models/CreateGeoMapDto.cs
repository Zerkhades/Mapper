using System.Collections.ObjectModel;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Mapper.Application.CommandAndQueries.GeoMap.Commands.CreateGeoMapCommand;
using Mapper.Application.Common.Mappings;
using Mapper.Domain;

namespace Mapper.WebApi.Models
{
    public class CreateGeoMapDto : IMapWith<CreateGeoMapCommand>
    {
        [Required]
        public Guid Id { get; set; }
        public string MapName { get; set; }
        public string MapDescription { get; set; }
        public byte[] Map { get; set; }
        public bool IsArchived { get; set; }
        public virtual ObservableCollection<GeoMark>? GeoMarks { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateGeoMapDto, CreateGeoMapCommand>()
                .ForMember(mapCommand => mapCommand.MapName,
                    opt => opt.MapFrom(mapDto => mapDto.MapName))
                .ForMember(mapCommand => mapCommand.MapDescription,
                    opt => opt.MapFrom(mapDto => mapDto.MapDescription))
                .ForMember(mapCommand => mapCommand.Map,
                    opt => opt.MapFrom(mapDto => mapDto.Map))
                .ForMember(mapCommand => mapCommand.IsArchived,
                    opt => opt.MapFrom(mapDto => mapDto.IsArchived));
            //.ForMember(mapCommand => mapCommand.GeoMarks,
            //    opt => opt.MapFrom(mapDto => mapDto.GeoMarks));
        }
    }
}
