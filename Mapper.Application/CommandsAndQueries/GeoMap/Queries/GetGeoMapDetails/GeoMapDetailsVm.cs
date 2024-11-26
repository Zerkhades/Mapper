using System.Collections.ObjectModel;
using AutoMapper;
using Mapper.Application.Common.Mappings;
using Mapper.Domain;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapDetails
{
    public class GeoMapDetailsVm : IMapWith<Domain.GeoMap>
    {
        public Guid Id { get; set; }
        public string MapName { get; set; }
        public string MapDescription { get; set; }
        //public byte[] Map { get; set; }
        public bool IsArchived { get; set; }
        public virtual IList<Domain.GeoMark>? GeoMarks { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.GeoMap, GeoMapDetailsVm>()
                .ForMember(mapCommand => mapCommand.Id,
                    opt => opt.MapFrom(mapDto => mapDto.Id))
                .ForMember(mapCommand => mapCommand.MapName,
                    opt => opt.MapFrom(mapDto => mapDto.MapName))
                .ForMember(mapCommand => mapCommand.MapDescription,
                    opt => opt.MapFrom(mapDto => mapDto.MapDescription))
                //.ForMember(mapCommand => mapCommand.Map,
                //    opt => opt.MapFrom(mapDto => mapDto.Map))
                .ForMember(mapCommand => mapCommand.IsArchived,
                    opt => opt.MapFrom(mapDto => mapDto.IsArchived))
            .ForMember(mapCommand => mapCommand.GeoMarks,
                opt => opt.MapFrom(mapDto => mapDto.GeoMarks));
        }
    }
}
