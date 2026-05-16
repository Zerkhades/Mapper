using FluentValidation;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Features.Routes.DTOs;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.Routes.Commands;

public record CreateRouteNodeCommand(
    Guid GeoMapId,
    double X,
    double Y,
    string? Title,
    Guid? GeoMarkId) : IRequest<Guid>;

public record UpdateRouteNodeCommand(
    Guid GeoMapId,
    Guid RouteNodeId,
    double X,
    double Y,
    string? Title,
    Guid? GeoMarkId) : IRequest;

public record DeleteRouteNodeCommand(Guid GeoMapId, Guid RouteNodeId) : IRequest;

public class CreateRouteNodeCommandValidator : AbstractValidator<CreateRouteNodeCommand>
{
    public CreateRouteNodeCommandValidator()
    {
        RuleFor(x => x.Title).MaximumLength(200);
    }
}

public class UpdateRouteNodeCommandValidator : AbstractValidator<UpdateRouteNodeCommand>
{
    public UpdateRouteNodeCommandValidator()
    {
        RuleFor(x => x.Title).MaximumLength(200);
    }
}

public sealed class CreateRouteNodeCommandHandler : IRequestHandler<CreateRouteNodeCommand, Guid>
{
    private readonly IMapperDbContext _db;

    public CreateRouteNodeCommandHandler(IMapperDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateRouteNodeCommand request, CancellationToken ct)
    {
        await EnsureMapExists(request.GeoMapId, ct);
        await EnsureMarkBelongsToMap(request.GeoMapId, request.GeoMarkId, ct);

        var node = new RouteNode(request.GeoMapId, request.X, request.Y, request.Title, request.GeoMarkId);
        _db.RouteNodes.Add(node);
        await _db.SaveChangesAsync(ct);

        return node.Id;
    }

    private async Task EnsureMapExists(Guid geoMapId, CancellationToken ct)
    {
        if (!await _db.GeoMaps.AnyAsync(x => x.Id == geoMapId, ct))
            throw new NotFoundException(nameof(GeoMap), geoMapId);
    }

    private async Task EnsureMarkBelongsToMap(Guid geoMapId, Guid? geoMarkId, CancellationToken ct)
    {
        if (geoMarkId is null)
            return;

        var exists = await _db.GeoMarks.AnyAsync(x => x.Id == geoMarkId && x.GeoMapId == geoMapId, ct);
        if (!exists)
            throw new NotFoundException(nameof(GeoMark), geoMarkId);
    }
}

public sealed class UpdateRouteNodeCommandHandler : IRequestHandler<UpdateRouteNodeCommand>
{
    private readonly IMapperDbContext _db;

    public UpdateRouteNodeCommandHandler(IMapperDbContext db) => _db = db;

    public async Task Handle(UpdateRouteNodeCommand request, CancellationToken ct)
    {
        var node = await _db.RouteNodes
            .FirstOrDefaultAsync(x => x.Id == request.RouteNodeId && x.GeoMapId == request.GeoMapId, ct);

        if (node is null)
            throw new NotFoundException(nameof(RouteNode), request.RouteNodeId);

        if (request.GeoMarkId is not null)
        {
            var markExists = await _db.GeoMarks
                .AnyAsync(x => x.Id == request.GeoMarkId && x.GeoMapId == request.GeoMapId, ct);
            if (!markExists)
                throw new NotFoundException(nameof(GeoMark), request.GeoMarkId);
        }

        node.Update(request.X, request.Y, request.Title, request.GeoMarkId);
        await _db.SaveChangesAsync(ct);
    }
}

public sealed class DeleteRouteNodeCommandHandler : IRequestHandler<DeleteRouteNodeCommand>
{
    private readonly IMapperDbContext _db;

    public DeleteRouteNodeCommandHandler(IMapperDbContext db) => _db = db;

    public async Task Handle(DeleteRouteNodeCommand request, CancellationToken ct)
    {
        var node = await _db.RouteNodes
            .FirstOrDefaultAsync(x => x.Id == request.RouteNodeId && x.GeoMapId == request.GeoMapId, ct);

        if (node is null)
            throw new NotFoundException(nameof(RouteNode), request.RouteNodeId);

        var connectedEdges = await _db.RouteEdges
            .Where(x => x.GeoMapId == request.GeoMapId &&
                (x.FromNodeId == request.RouteNodeId || x.ToNodeId == request.RouteNodeId))
            .ToListAsync(ct);

        _db.RouteEdges.RemoveRange(connectedEdges);
        node.SoftDelete();
        await _db.SaveChangesAsync(ct);
    }
}
