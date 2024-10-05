using AutoMapper;
using Mapper.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapList
{
    public class GeoMapLookupDto : IMapWith<Domain.GeoMap>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.GeoMap, GeoMapLookupDto>()
                .ForMember(geoMapDto => geoMapDto.Id,
                    opt => opt.MapFrom(note => note.Id));
            //.ForMember(geoMapDto => geoMapDto.Title,
            //    opt => opt.MapFrom(geoMapDto => geoMapDto.Title));
        }
    }
}
