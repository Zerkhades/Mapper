using AutoMapper;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Features.DTOs;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.CameraArchive.Queries;

public class GetCameraMotionAlertsHandler 
    : IRequestHandler<GetCameraMotionAlertsQuery, List<CameraMotionAlertListItemDto>>
{
    private readonly IMapperDbContext _db;
    private readonly IMapper _mapper;

    public GetCameraMotionAlertsHandler(IMapperDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<CameraMotionAlertListItemDto>> Handle(
        GetCameraMotionAlertsQuery request, CancellationToken ct)
    {
        var cameraExists = await _db.GeoMarks
            .OfType<CameraMark>()
            .AnyAsync(x => x.Id == request.CameraMarkId, ct);

        if (!cameraExists)
            throw new NotFoundException($"Camera {request.CameraMarkId} not found", request.CameraMarkId);

        var alerts = await _db.CameraMotionAlerts
            .AsNoTracking()
            .Where(x => x.CameraMarkId == request.CameraMarkId)
            .OrderByDescending(x => x.DetectedAt)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(x => new CameraMotionAlertListItemDto
            {
                Id = x.Id,
                DetectedAt = x.DetectedAt,
                Severity = x.Severity.ToString(),
                MotionPercentage = x.MotionPercentage,
                IsResolved = x.IsResolved
            })
            .ToListAsync(ct);

        return alerts;
    }
}

public class GetCameraMotionAlertByIdHandler 
    : IRequestHandler<GetCameraMotionAlertByIdQuery, CameraMotionAlertDto>
{
    private readonly IMapperDbContext _db;
    private readonly IMapper _mapper;

    public GetCameraMotionAlertByIdHandler(IMapperDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CameraMotionAlertDto> Handle(
        GetCameraMotionAlertByIdQuery request, CancellationToken ct)
    {
        var cameraExists = await _db.GeoMarks
            .OfType<CameraMark>()
            .AnyAsync(x => x.Id == request.CameraMarkId, ct);

        if (!cameraExists)
            throw new NotFoundException($"Camera {request.CameraMarkId} not found", request.CameraMarkId);

        var alert = await _db.CameraMotionAlerts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.AlertId && x.CameraMarkId == request.CameraMarkId, ct);

        if (alert is null)
            throw new NotFoundException($"Motion alert {request.AlertId} not found", request.AlertId);

        return new CameraMotionAlertDto
        {
            Id = alert.Id,
            CameraMarkId = alert.CameraMarkId,
            DetectedAt = alert.DetectedAt,
            ConfirmedAt = alert.ConfirmedAt,
            Severity = alert.Severity.ToString(),
            MotionPercentage = alert.MotionPercentage,
            SnapshotUrl = alert.SnapshotPath != null ? $"/api/files/{alert.SnapshotPath}" : null,
            IsResolved = alert.IsResolved,
            ResolutionNotes = alert.ResolutionNotes,
            RelatedVideoArchiveId = alert.RelatedVideoArchiveId
        };
    }
}

public class GetUnresolvedCameraMotionAlertsHandler 
    : IRequestHandler<GetUnresolvedCameraMotionAlertsQuery, List<CameraMotionAlertListItemDto>>
{
    private readonly IMapperDbContext _db;
    private readonly IMapper _mapper;

    public GetUnresolvedCameraMotionAlertsHandler(IMapperDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<CameraMotionAlertListItemDto>> Handle(
        GetUnresolvedCameraMotionAlertsQuery request, CancellationToken ct)
    {
        var cameraExists = await _db.GeoMarks
            .OfType<CameraMark>()
            .AnyAsync(x => x.Id == request.CameraMarkId, ct);

        if (!cameraExists)
            throw new NotFoundException($"Camera {request.CameraMarkId} not found", request.CameraMarkId);

        var alerts = await _db.CameraMotionAlerts
            .AsNoTracking()
            .Where(x => x.CameraMarkId == request.CameraMarkId && !x.IsResolved)
            .OrderByDescending(x => x.DetectedAt)
            .Select(x => new CameraMotionAlertListItemDto
            {
                Id = x.Id,
                DetectedAt = x.DetectedAt,
                Severity = x.Severity.ToString(),
                MotionPercentage = x.MotionPercentage,
                IsResolved = x.IsResolved
            })
            .ToListAsync(ct);

        return alerts;
    }
}

public class GetCameraMotionAlertsInDateRangeHandler 
    : IRequestHandler<GetCameraMotionAlertsInDateRangeQuery, List<CameraMotionAlertListItemDto>>
{
    private readonly IMapperDbContext _db;
    private readonly IMapper _mapper;

    public GetCameraMotionAlertsInDateRangeHandler(IMapperDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<CameraMotionAlertListItemDto>> Handle(
        GetCameraMotionAlertsInDateRangeQuery request, CancellationToken ct)
    {
        var cameraExists = await _db.GeoMarks
            .OfType<CameraMark>()
            .AnyAsync(x => x.Id == request.CameraMarkId, ct);

        if (!cameraExists)
            throw new NotFoundException($"Camera {request.CameraMarkId} not found", request.CameraMarkId);

        var startDate = request.StartDate.ToUniversalTime();
        var endDate = request.EndDate.ToUniversalTime();

        var alerts = await _db.CameraMotionAlerts
            .AsNoTracking()
            .Where(x => x.CameraMarkId == request.CameraMarkId
                && x.DetectedAt >= startDate
                && x.DetectedAt <= endDate)
            .OrderByDescending(x => x.DetectedAt)
            .Select(x => new CameraMotionAlertListItemDto
            {
                Id = x.Id,
                DetectedAt = x.DetectedAt,
                Severity = x.Severity.ToString(),
                MotionPercentage = x.MotionPercentage,
                IsResolved = x.IsResolved
            })
            .ToListAsync(ct);

        return alerts;
    }
}
