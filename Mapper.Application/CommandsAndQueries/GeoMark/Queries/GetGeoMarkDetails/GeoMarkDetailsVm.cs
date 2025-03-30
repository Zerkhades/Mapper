using AutoMapper;
using Mapper.Application.Common.Mappings;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Queries.GetGeoMarkDetails
{
    public class GeoMarkDetailsVm : IMapWith<Domain.GeoMark>
    {
        public Guid Id { get; set; }
        public virtual Domain.GeoMap GeoMap {get;set;}
        public virtual Guid GeoMapId { get; set; }
        public string MarkName { get; set; } = string.Empty;
        public string? MarkDescription { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Emoji { get; set; } = string.Empty;
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
            profile.CreateMap<Domain.GeoMark, GeoMarkDetailsVm>()
                .ForMember(markCommand => markCommand.Id,
                    opt => opt.MapFrom(markDto => markDto.Id))
                .ForMember(markCommand => markCommand.GeoMap,
                    opt => opt.MapFrom(markDto => markDto.GeoMap))
                .ForMember(markCommand => markCommand.GeoMapId,
                    opt => opt.MapFrom(markDto => markDto.GeoMapId))
                .ForMember(markCommand => markCommand.MarkName,
                    opt => opt.MapFrom(markDto => markDto.MarkName))
                .ForMember(markCommand => markCommand.MarkDescription,
                    opt => opt.MapFrom(markDto => markDto.MarkDescription))
                .ForMember(markCommand => markCommand.Color,
                    opt => opt.MapFrom(markDto => markDto.Color))
                .ForMember(markCommand => markCommand.Emoji,
                    opt => opt.MapFrom(markDto => markDto.Emoji))
                .ForMember(markCommand => markCommand.Size,
                    opt => opt.MapFrom(markDto => markDto.Size))
                .ForMember(markCommand => markCommand.IsEmoji,
                    opt => opt.MapFrom(markDto => markDto.IsEmoji))
                .ForMember(markCommand => markCommand.IsArchived,
                    opt => opt.MapFrom(markDto => markDto.IsArchived))
                .ForMember(markCommand => markCommand.IsEditable,
                    opt => opt.MapFrom(markDto => markDto.IsEditable))
                .ForMember(markCommand => markCommand.CreationDate,
                    opt => opt.MapFrom(markDto => markDto.CreationDate))
                .ForMember(markCommand => markCommand.EditDate,
                    opt => opt.MapFrom(markDto => markDto.EditDate))
                .ForMember(markCommand => markCommand.EditedBy,
                    opt => opt.MapFrom(markDto => markDto.EditedBy))
                .ForMember(markCommand => markCommand.Employees,
                    opt => opt.MapFrom(markDto => markDto.Employees))
                .ForMember(markCommand => markCommand.GeoPhotos,
                    opt => opt.MapFrom(markDto => markDto.GeoPhotos));
        }
    }
}
