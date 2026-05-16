using FluentValidation;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.Routes.Commands;

public record CreateRouteEdgeCommand(
    Guid GeoMapId,
    Guid FromNodeId,
    Guid ToNodeId,
    bool IsBidirectional,
    double? CostOverride,
    string? Description) : IRequest<Guid>;

public record UpdateRouteEdgeCommand(
    Guid GeoMapId,
    Guid RouteEdgeId,
    bool IsBidirectional,
    double? CostOverride,
    bool IsDisabled,
    string? Description) : IRequest;

public record DeleteRouteEdgeCommand(Guid GeoMapId, Guid RouteEdgeId) : IRequest;

public class CreateRouteEdgeCommandValidator : AbstractValidator<CreateRouteEdgeCommand>
{
    public CreateRouteEdgeCommandValidator()
    {
        RuleFor(x => x.ToNodeId).NotEqual(x => x.FromNodeId);
        RuleFor(x => x.CostOverride).GreaterThan(0).When(x => x.CostOverride is not null);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class UpdateRouteEdgeCommandValidator : AbstractValidator<UpdateRouteEdgeCommand>
{
    public UpdateRouteEdgeCommandValidator()
    {
        RuleFor(x => x.CostOverride).GreaterThan(0).When(x => x.CostOverride is not null);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public sealed class CreateRouteEdgeCommandHandler : IRequestHandler<CreateRouteEdgeCommand, Guid>
{
    private readonly IMapperDbContext _db;

    public CreateRouteEdgeCommandHandler(IMapperDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateRouteEdgeCommand request, CancellationToken ct)
    {
        var nodes = await _db.RouteNodes
            .Where(x => x.GeoMapId == request.GeoMapId &&
                (x.Id == request.FromNodeId || x.Id == request.ToNodeId))
            .Select(x => x.Id)
            .ToListAsync(ct);

        if (!nodes.Contains(request.FromNodeId))
            throw new NotFoundException(nameof(RouteNode), request.FromNodeId);

        if (!nodes.Contains(request.ToNodeId))
            throw new NotFoundException(nameof(RouteNode), request.ToNodeId);

        var edge = new RouteEdge(
            request.GeoMapId,
            request.FromNodeId,
            request.ToNodeId,
            request.IsBidirectional,
            request.CostOverride,
            request.Description);

        _db.RouteEdges.Add(edge);
        await _db.SaveChangesAsync(ct);

        return edge.Id;
    }
}

public sealed class UpdateRouteEdgeCommandHandler : IRequestHandler<UpdateRouteEdgeCommand>
{
    private readonly IMapperDbContext _db;

    public UpdateRouteEdgeCommandHandler(IMapperDbContext db) => _db = db;

    public async Task Handle(UpdateRouteEdgeCommand request, CancellationToken ct)
    {
        var edge = await _db.RouteEdges
            .FirstOrDefaultAsync(x => x.Id == request.RouteEdgeId && x.GeoMapId == request.GeoMapId, ct);

        if (edge is null)
            throw new NotFoundException(nameof(RouteEdge), request.RouteEdgeId);

        edge.Update(request.IsBidirectional, request.CostOverride, request.IsDisabled, request.Description);
        await _db.SaveChangesAsync(ct);
    }
}

public sealed class DeleteRouteEdgeCommandHandler : IRequestHandler<DeleteRouteEdgeCommand>
{
    private readonly IMapperDbContext _db;

    public DeleteRouteEdgeCommandHandler(IMapperDbContext db) => _db = db;

    public async Task Handle(DeleteRouteEdgeCommand request, CancellationToken ct)
    {
        var edge = await _db.RouteEdges
            .FirstOrDefaultAsync(x => x.Id == request.RouteEdgeId && x.GeoMapId == request.GeoMapId, ct);

        if (edge is null)
            throw new NotFoundException(nameof(RouteEdge), request.RouteEdgeId);

        _db.RouteEdges.Remove(edge);
        await _db.SaveChangesAsync(ct);
    }
}
