using Mapper.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.Features
{
    public record GeoMapDetailsDto(
        Guid Id,
        string Name,
        string ImageUrl,
        int ImageWidth,
        int ImageHeight,
        IReadOnlyList<GeoMarkDto> Marks);

    public record GeoMarkDto(
        Guid Id,
        GeoMarkType Type,
        double X,
        double Y,
        string Title,
        string? Description,
        Guid? TargetGeoMapId,
        string? WorkplaceCode,
        IReadOnlyList<Guid>? EmployeeIds,
        string? CameraName,
        string? StreamUrl);
}
