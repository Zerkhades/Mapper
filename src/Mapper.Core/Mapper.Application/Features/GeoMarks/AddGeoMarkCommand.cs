using Mapper.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.Features.GeoMarks
{
    public record AddGeoMarkCommand(
        Guid GeoMapId,
        GeoMarkType Type,
        double X,
        double Y,
        string Title,
        string? Description,
        Guid? TargetGeoMapId,
        string? WorkplaceCode,
        IReadOnlyList<Guid>? EmployeeIds,
        string? CameraName,
        string? StreamUrl
    ) : IRequest<Guid>;

}
