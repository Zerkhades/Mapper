using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Mapper.Application.CommandsAndQueries.GeoMap.Commands.CreateGeoMapCommand;
using Mapper.Domain;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapDetails
{
    internal class GeoMapDetailsVm
    {
        public Guid Id { get; set; }
        public string MapName { get; set; }
        public string MapDescription { get; set; }
        public byte[] Map { get; set; }
        public bool IsArchived { get; set; }
        public virtual ObservableCollection<GeoMark>? GeoMarks { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.GeoMap, CreateGeoMapCommand>()
                .ForMember(mapCommand => mapCommand.MapName,
                    opt => opt.MapFrom(mapDto => mapDto.MapName))
                .ForMember(mapCommand => mapCommand.MapDescription,
                    opt => opt.MapFrom(mapDto => mapDto.MapDescription))
                .ForMember(mapCommand => mapCommand.Map,
                    opt => opt.MapFrom(mapDto => mapDto.Map))
                .ForMember(mapCommand => mapCommand.IsArchived,
                    opt => opt.MapFrom(mapDto => mapDto.IsArchived));
        }
}
