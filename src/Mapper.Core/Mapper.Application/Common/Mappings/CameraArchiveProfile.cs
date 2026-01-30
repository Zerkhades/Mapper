using AutoMapper;
using Mapper.Application.Features.DTOs;
using Mapper.Domain;

namespace Mapper.Application.Common.Mappings;

public class CameraArchiveProfile : Profile
{
    public CameraArchiveProfile()
    {
        CreateMap<CameraVideoArchive, CameraVideoArchiveDto>();
        CreateMap<CameraMotionAlert, CameraMotionAlertDto>()
            .ForMember(d => d.Severity, o => o.MapFrom(s => s.Severity.ToString()))
            .ForMember(d => d.SnapshotUrl, o => o.MapFrom(s => s.SnapshotPath));
        CreateMap<CameraStatusHistory, CameraStatusHistoryDto>()
            .ForMember(d => d.Reason, o => o.MapFrom(s => s.Reason.ToString()));
    }
}
