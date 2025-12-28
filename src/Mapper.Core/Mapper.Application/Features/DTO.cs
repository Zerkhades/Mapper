using Mapper.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.Features
{
    public record GeoMapDetailsDto(
        Guid Id = default,
        string Name = default!,
        string ImageUrl = default!,
        int ImageWidth = default,
        int ImageHeight = default,
        IReadOnlyList<GeoMarkDto> Marks = null!);

    public record GeoMarkDto(
        Guid Id = default,
        GeoMarkType Type = default,
        double X = default,
        double Y = default,
        string Title = default!,
        string? Description = null,
        Guid? TargetGeoMapId = null,
        string? WorkplaceCode = null,
        IReadOnlyList<Guid>? EmployeeIds = null,
        string? CameraName = null,
        string? StreamUrl = null);
}
