using AutoMapper;
using Mapper.Application.Common.Mappings;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapList
{
    //public class GeoMapLookupDto : IMapWith<Domain.GeoMap>
    //{
    //    public Guid Id { get; set; }
    //    public required string MapName { get; set; }
    //    public required string MapDescription { get; set; }
    //    public virtual IList<Domain.GeoMark>? GeoMarks { get; set; }
    //    public bool IsArchived { get; set; }

    //    public void Mapping(Profile profile)
    //    {
    //        profile.CreateMap<Domain.GeoMap, GeoMapLookupDto>()
    //            .ForMember(geoMapDto => geoMapDto.Id, opt => opt.MapFrom(geoMap => geoMap.Id))
    //            .ForMember(geoMapDto => geoMapDto.MapName, opt => opt.MapFrom(geoMap => geoMap.MapName))
    //            .ForMember(geoMapDto => geoMapDto.MapDescription, opt => opt.MapFrom(geoMap => geoMap.MapDescription))
    //            .ForMember(geoMapDto => geoMapDto.GeoMarks, opt => opt.MapFrom(geoMap => geoMap.GeoMarks))
    //            .ForMember(geoMapDto => geoMapDto.IsArchived, opt => opt.MapFrom(geoMap => geoMap.IsArchived));

    //    }
    //}
}
