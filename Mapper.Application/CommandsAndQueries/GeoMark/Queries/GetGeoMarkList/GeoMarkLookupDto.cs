using AutoMapper;
using Mapper.Application.Common.Mappings;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Queries.GetGeoMarkList
{
    public class GeoMarkLookupDto : IMapWith<Domain.GeoMark>
    {
        public Guid Id { get; set; }
        public virtual Domain.GeoMap? GeoMap { get; set; }
        public Guid GeoMapId { get; set; }
        public string MarkName { get; set; }
        public string? MarkDescription { get; set; }
        public string Color { get; set; }
        public string Emoji { get; set; }
        public int? Size { get; set; }
        public bool IsEmoji { get; set; }
        public bool IsArchived { get; set; }
        public bool IsEditable { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? EditDate { get; set; }
        public Guid? EditedBy { get; set; }
        public virtual IList<Domain.Employee>? Employees { get; set; }
        public virtual IList<Domain.GeoPhoto>? GeoPhotos { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.GeoMark, GeoMarkLookupDto>()
                .ForMember(geoMarkDto => geoMarkDto.Id, opt => opt.MapFrom(geoMark => geoMark.Id))
                .ForMember(geoMarkDto => geoMarkDto.GeoMap, opt => opt.MapFrom(geoMark => geoMark.GeoMap))
                .ForMember(geoMarkDto => geoMarkDto.GeoMapId, opt => opt.MapFrom(geoMark => geoMark.GeoMapId))
                .ForMember(geoMarkDto => geoMarkDto.MarkName, opt => opt.MapFrom(geoMark => geoMark.MarkName))
                .ForMember(geoMarkDto => geoMarkDto.MarkDescription, opt => opt.MapFrom(geoMark => geoMark.MarkDescription))
                .ForMember(geoMarkDto => geoMarkDto.Color, opt => opt.MapFrom(geoMark => geoMark.Color))
                .ForMember(geoMarkDto => geoMarkDto.Emoji, opt => opt.MapFrom(geoMark => geoMark.Emoji))
                .ForMember(geoMarkDto => geoMarkDto.Size, opt => opt.MapFrom(geoMark => geoMark.Size))
                .ForMember(geoMarkDto => geoMarkDto.IsEmoji, opt => opt.MapFrom(geoMark => geoMark.IsEmoji))
                .ForMember(geoMarkDto => geoMarkDto.IsArchived, opt => opt.MapFrom(geoMark => geoMark.IsArchived))
                .ForMember(geoMarkDto => geoMarkDto.IsEditable, opt => opt.MapFrom(geoMark => geoMark.IsEditable))
                .ForMember(geoMarkDto => geoMarkDto.CreationDate, opt => opt.MapFrom(geoMark => geoMark.CreationDate))
                .ForMember(geoMarkDto => geoMarkDto.EditDate, opt => opt.MapFrom(geoMark => geoMark.EditDate))
                .ForMember(geoMarkDto => geoMarkDto.EditedBy, opt => opt.MapFrom(geoMark => geoMark.EditedBy))
                .ForMember(geoMarkDto => geoMarkDto.Employees, opt => opt.MapFrom(geoMark => geoMark.Employees))
                .ForMember(geoMarkDto => geoMarkDto.GeoPhotos, opt => opt.MapFrom(geoMark => geoMark.GeoPhotos));
        }
    }
}
