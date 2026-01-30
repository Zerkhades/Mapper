using AutoMapper;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Features.DTOs;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.CameraArchive.Queries;

public class GetCameraVideoArchiveHandler 
    : IRequestHandler<GetCameraVideoArchiveQuery, List<CameraVideoArchiveListItemDto>>
{
    private readonly IMapperDbContext _db;
    private readonly IMapper _mapper;

    public GetCameraVideoArchiveHandler(IMapperDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<CameraVideoArchiveListItemDto>> Handle(
        GetCameraVideoArchiveQuery request, CancellationToken ct)
    {
        var cameraExists = await _db.GeoMarks
            .OfType<CameraMark>()
            .AnyAsync(x => x.Id == request.CameraMarkId, ct);

        if (!cameraExists)
            throw new NotFoundException($"Camera {request.CameraMarkId} not found", request.CameraMarkId);

        var videos = await _db.CameraVideoArchives
            .AsNoTracking()
            .Where(x => x.CameraMarkId == request.CameraMarkId)
            .OrderByDescending(x => x.RecordedAt)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(x => new CameraVideoArchiveListItemDto
            {
                Id = x.Id,
                RecordedAt = x.RecordedAt,
                Duration = x.Duration,
                ThumbnailUrl = x.ThumbnailPath != null ? $"/api/files/{x.ThumbnailPath}" : null,
                HasMotionDetected = x.HasMotionDetected,
                Resolution = x.Resolution
            })
            .ToListAsync(ct);

        return videos;
    }
}

public class GetCameraVideoArchiveByIdHandler 
    : IRequestHandler<GetCameraVideoArchiveByIdQuery, CameraVideoArchiveDto>
{
    private readonly IMapperDbContext _db;
    private readonly IMapper _mapper;

    public GetCameraVideoArchiveByIdHandler(IMapperDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CameraVideoArchiveDto> Handle(
        GetCameraVideoArchiveByIdQuery request, CancellationToken ct)
    {
        var cameraExists = await _db.GeoMarks
            .OfType<CameraMark>()
            .AnyAsync(x => x.Id == request.CameraMarkId, ct);

        if (!cameraExists)
            throw new NotFoundException($"Camera {request.CameraMarkId} not found", request.CameraMarkId);

        var video = await _db.CameraVideoArchives
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.VideoArchiveId && x.CameraMarkId == request.CameraMarkId, ct);

        if (video is null)
            throw new NotFoundException($"Video archive {request.VideoArchiveId} not found", request.VideoArchiveId);

        return _mapper.Map<CameraVideoArchiveDto>(video);
    }
}

public class GetCameraVideoArchiveTimelineHandler 
    : IRequestHandler<GetCameraVideoArchiveTimelineQuery, List<CameraVideoArchiveListItemDto>>
{
    private readonly IMapperDbContext _db;
    private readonly IMapper _mapper;

    public GetCameraVideoArchiveTimelineHandler(IMapperDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<CameraVideoArchiveListItemDto>> Handle(
        GetCameraVideoArchiveTimelineQuery request, CancellationToken ct)
    {
        var cameraExists = await _db.GeoMarks
            .OfType<CameraMark>()
            .AnyAsync(x => x.Id == request.CameraMarkId, ct);

        if (!cameraExists)
            throw new NotFoundException($"Camera {request.CameraMarkId} not found", request.CameraMarkId);

        var startDate = request.StartDate.ToUniversalTime();
        var endDate = request.EndDate.ToUniversalTime();

        var videos = await _db.CameraVideoArchives
            .AsNoTracking()
            .Where(x => x.CameraMarkId == request.CameraMarkId
                && x.RecordedAt >= startDate
                && x.RecordedAt <= endDate)
            .OrderByDescending(x => x.RecordedAt)
            .Select(x => new CameraVideoArchiveListItemDto
            {
                Id = x.Id,
                RecordedAt = x.RecordedAt,
                Duration = x.Duration,
                ThumbnailUrl = x.ThumbnailPath != null ? $"/api/files/{x.ThumbnailPath}" : null,
                HasMotionDetected = x.HasMotionDetected,
                Resolution = x.Resolution
            })
            .ToListAsync(ct);

        return videos;
    }
}
