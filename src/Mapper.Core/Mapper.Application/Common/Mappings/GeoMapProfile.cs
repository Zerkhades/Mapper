using AutoMapper;
using Mapper.Application.Features.DTOs;
using Mapper.Domain;

namespace Mapper.Application.Common.Mappings
{
    public class GeoMapProfile : Profile
    {
        public GeoMapProfile()
        {
            CreateMap<GeoMap, GeoMapListItemDto>()
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.ImagePath));

            CreateMap<GeoMap, GeoMapDetailsDto>()
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.ImagePath))
                .ForMember(d => d.Marks, o => o.Ignore());

            CreateMap<GeoMark, GeoMarkDto>()
                .Include<CameraMark, GeoMarkDto>()
                .Include<WorkplaceMark, GeoMarkDto>()
                .Include<TransitionMark, GeoMarkDto>()
                .ForMember(d => d.TargetGeoMapId, o => o.Ignore())
                .ForMember(d => d.WorkplaceCode, o => o.Ignore())
                .ForMember(d => d.EmployeeIds, o => o.Ignore())
                .ForMember(d => d.CameraName, o => o.Ignore())
                .ForMember(d => d.StreamUrl, o => o.Ignore());

            CreateMap<CameraMark, GeoMarkDto>()
                .ForMember(d => d.TargetGeoMapId, o => o.Ignore())
                .ForMember(d => d.WorkplaceCode, o => o.Ignore())
                .ForMember(d => d.EmployeeIds, o => o.Ignore());

            CreateMap<WorkplaceMark, GeoMarkDto>()
                .ForMember(d => d.TargetGeoMapId, o => o.Ignore())
                .ForMember(d => d.CameraName, o => o.Ignore())
                .ForMember(d => d.StreamUrl, o => o.Ignore())
                .ForMember(d => d.EmployeeIds, o => o.Ignore());

            CreateMap<TransitionMark, GeoMarkDto>()
                .ForMember(d => d.WorkplaceCode, o => o.Ignore())
                .ForMember(d => d.EmployeeIds, o => o.Ignore())
                .ForMember(d => d.CameraName, o => o.Ignore())
                .ForMember(d => d.StreamUrl, o => o.Ignore());
        }
    }
}
