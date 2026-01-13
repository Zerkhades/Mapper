using AutoMapper;
using Mapper.Application.Features.DTOs;
using Mapper.Domain;

namespace Mapper.Application.Common.Mappings
{
    public class GeoMapProfile : Profile
    {
        public GeoMapProfile()
        {
            CreateMap<GeoMap, GeoMapDetailsDto>()
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.ImagePath))
                .ForMember(d => d.Marks, o => o.MapFrom(s => s.Marks));

            CreateMap<GeoMark, GeoMarkDto>()
                .ForMember(d => d.TargetGeoMapId, o => o.MapFrom(s => (s as TransitionMark) != null ? ((TransitionMark)s).TargetGeoMapId : (Guid?)null))
                .ForMember(d => d.WorkplaceCode, o => o.MapFrom(s => (s as WorkplaceMark) != null ? ((WorkplaceMark)s).WorkplaceCode : null))
                .ForMember(d => d.EmployeeIds, o => o.MapFrom(s => (s as WorkplaceMark) != null ? ((WorkplaceMark)s).Employees.Select(e => e.EmployeeId).ToList() : null))
                .ForMember(d => d.CameraName, o => o.MapFrom(s => (s as CameraMark) != null ? ((CameraMark)s).CameraName : null))
                .ForMember(d => d.StreamUrl, o => o.MapFrom(s => (s as CameraMark) != null ? ((CameraMark)s).StreamUrl : null));
        }
    }
}
