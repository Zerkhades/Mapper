using AutoMapper;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Features.DTOs;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.CameraArchive.Queries;

public class GetCameraStatusHistoryHandler 
    : IRequestHandler<GetCameraStatusHistoryQuery, List<CameraStatusHistoryListItemDto>>
{
    private readonly IMapperDbContext _db;
    private readonly IMapper _mapper;

    public GetCameraStatusHistoryHandler(IMapperDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<CameraStatusHistoryListItemDto>> Handle(
        GetCameraStatusHistoryQuery request, CancellationToken ct)
    {
        var cameraExists = await _db.GeoMarks
            .OfType<CameraMark>()
            .AnyAsync(x => x.Id == request.CameraMarkId, ct);

        if (!cameraExists)
            throw new NotFoundException($"Camera {request.CameraMarkId} not found", request.CameraMarkId);

        var statuses = await _db.CameraStatusHistories
            .AsNoTracking()
            .Where(x => x.CameraMarkId == request.CameraMarkId)
            .OrderByDescending(x => x.ChangedAt)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(x => new CameraStatusHistoryListItemDto
            {
                Id = x.Id,
                ChangedAt = x.ChangedAt,
                IsOnline = x.IsOnline,
                Reason = x.Reason.ToString(),
                DurationSinceLastChange = x.DurationSinceLastChange
            })
            .ToListAsync(ct);

        return statuses;
    }
}

public class GetCameraStatusHistoryInDateRangeHandler 
    : IRequestHandler<GetCameraStatusHistoryInDateRangeQuery, List<CameraStatusHistoryListItemDto>>
{
    private readonly IMapperDbContext _db;
    private readonly IMapper _mapper;

    public GetCameraStatusHistoryInDateRangeHandler(IMapperDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<CameraStatusHistoryListItemDto>> Handle(
        GetCameraStatusHistoryInDateRangeQuery request, CancellationToken ct)
    {
        var cameraExists = await _db.GeoMarks
            .OfType<CameraMark>()
            .AnyAsync(x => x.Id == request.CameraMarkId, ct);

        if (!cameraExists)
            throw new NotFoundException($"Camera {request.CameraMarkId} not found", request.CameraMarkId);

        var startDate = request.StartDate.ToUniversalTime();
        var endDate = request.EndDate.ToUniversalTime();

        var statuses = await _db.CameraStatusHistories
            .AsNoTracking()
            .Where(x => x.CameraMarkId == request.CameraMarkId
                && x.ChangedAt >= startDate
                && x.ChangedAt <= endDate)
            .OrderByDescending(x => x.ChangedAt)
            .Select(x => new CameraStatusHistoryListItemDto
            {
                Id = x.Id,
                ChangedAt = x.ChangedAt,
                IsOnline = x.IsOnline,
                Reason = x.Reason.ToString(),
                DurationSinceLastChange = x.DurationSinceLastChange
            })
            .ToListAsync(ct);

        return statuses;
    }
}

public class GetCameraCurrentStatusHandler 
    : IRequestHandler<GetCameraCurrentStatusQuery, CameraStatusHistoryDto?>
{
    private readonly IMapperDbContext _db;
    private readonly IMapper _mapper;

    public GetCameraCurrentStatusHandler(IMapperDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CameraStatusHistoryDto?> Handle(
        GetCameraCurrentStatusQuery request, CancellationToken ct)
    {
        var cameraExists = await _db.GeoMarks
            .OfType<CameraMark>()
            .AnyAsync(x => x.Id == request.CameraMarkId, ct);

        if (!cameraExists)
            throw new NotFoundException($"Camera {request.CameraMarkId} not found", request.CameraMarkId);

        var status = await _db.CameraStatusHistories
            .AsNoTracking()
            .Where(x => x.CameraMarkId == request.CameraMarkId)
            .OrderByDescending(x => x.ChangedAt)
            .FirstOrDefaultAsync(ct);

        if (status is null)
            return null;

        return new CameraStatusHistoryDto
        {
            Id = status.Id,
            CameraMarkId = status.CameraMarkId,
            IsOnline = status.IsOnline,
            Reason = status.Reason.ToString(),
            ChangedAt = status.ChangedAt,
            DurationSinceLastChange = status.DurationSinceLastChange,
            Details = status.Details,
            ResponseTimeMs = status.ResponseTimeMs
        };
    }
}
