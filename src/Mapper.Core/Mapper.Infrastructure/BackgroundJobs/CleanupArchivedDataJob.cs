using Hangfire;
using Mapper.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mapper.Infrastructure.BackgroundJobs;

public class CleanupArchivedDataJob
{
    private readonly IMapperDbContext _context;
    private readonly ILogger<CleanupArchivedDataJob> _logger;

    public CleanupArchivedDataJob(IMapperDbContext context, ILogger<CleanupArchivedDataJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task Execute(CancellationToken ct)
    {
        _logger.LogInformation("Starting cleanup of archived data older than 90 days");

        var cutoffDate = DateTime.UtcNow.AddDays(-90);

        // Удаляем старые архивные записи
        var deletedMaps = await _context.GeoMaps
            .IgnoreQueryFilters()
            .Where(m => m.IsDeleted && m.DeletedAt < cutoffDate)
            .ToListAsync();

        _context.GeoMaps.RemoveRange(deletedMaps);

        var deletedMarks = await _context.GeoMarks
            .IgnoreQueryFilters()
            .Where(m => m.IsDeleted && m.DeletedAt < cutoffDate)
            .ToListAsync();

        _context.GeoMarks.RemoveRange(deletedMarks);

        var deletedEmployees = await _context.Employees
            .IgnoreQueryFilters()
            .Where(e => e.IsArchived)
            .ToListAsync();

        // Для сотрудников оставляем только флаг архивации

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Cleanup completed. Removed {MapsCount} maps and {MarksCount} marks",
            deletedMaps.Count,
            deletedMarks.Count);
    }
}