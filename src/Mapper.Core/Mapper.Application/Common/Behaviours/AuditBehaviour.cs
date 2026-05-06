using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using System.Reflection;
using System.Text.Json;

namespace Mapper.Application.Common.Behaviours;

public class AuditBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };

    private readonly IMapperDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AuditBehaviour(IMapperDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();
        var requestName = typeof(TRequest).Name;

        if (requestName.Contains("Query", StringComparison.OrdinalIgnoreCase))
        {
            return response;
        }

        var action = ResolveAction(requestName);
        var entityType = ResolveEntityType(requestName);
        var entityId = ResolveEntityId(request, response);
        var metadata = BuildMetadata(requestName, request);

        _context.AuditEvents.Add(new AuditEvent(
            action,
            entityType,
            entityId,
            _currentUserService.UserId,
            metadata));

        await _context.SaveChangesAsync(cancellationToken);

        return response;
    }

    private static string BuildMetadata(string requestName, TRequest request)
    {
        var parameters = typeof(TRequest)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .ToDictionary(property => property.Name, property => FormatValue(property.GetValue(request)));

        var metadata = JsonSerializer.Serialize(new
        {
            request = requestName,
            parameters
        }, JsonOptions);

        return metadata.Length <= 4000 ? metadata : metadata[..4000];
    }

    private static object? FormatValue(object? value)
    {
        return value switch
        {
            null => null,
            Stream stream => new { type = "Stream", canRead = stream.CanRead, canSeek = stream.CanSeek },
            byte[] bytes => new { type = "ByteArray", length = bytes.Length },
            _ => value
        };
    }

    private static string ResolveAction(string requestName)
    {
        return requestName switch
        {
            var name when name.StartsWith("Create", StringComparison.OrdinalIgnoreCase) => "Create",
            var name when name.StartsWith("Add", StringComparison.OrdinalIgnoreCase) => "Create",
            var name when name.StartsWith("Update", StringComparison.OrdinalIgnoreCase) => "Update",
            var name when name.StartsWith("Set", StringComparison.OrdinalIgnoreCase) => "Update",
            var name when name.StartsWith("Mark", StringComparison.OrdinalIgnoreCase) => "Update",
            var name when name.StartsWith("Confirm", StringComparison.OrdinalIgnoreCase) => "Update",
            var name when name.StartsWith("Resolve", StringComparison.OrdinalIgnoreCase) => "Update",
            var name when name.StartsWith("Link", StringComparison.OrdinalIgnoreCase) => "Update",
            var name when name.StartsWith("Delete", StringComparison.OrdinalIgnoreCase) => "Delete",
            _ => "Execute"
        };
    }

    private static string ResolveEntityType(string requestName)
    {
        var entityType = requestName;
        foreach (var prefix in new[] { "Create", "Add", "Update", "Set", "Mark", "Confirm", "Resolve", "Link", "Delete" })
        {
            if (entityType.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                entityType = entityType[prefix.Length..];
                break;
            }
        }

        return entityType
            .Replace("Command", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("AsArchived", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("ToVideo", string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private static Guid? ResolveEntityId(TRequest request, TResponse response)
    {
        if (response is Guid responseId && responseId != Guid.Empty)
        {
            return responseId;
        }

        var idProperty = typeof(TRequest)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(property =>
                property.PropertyType == typeof(Guid)
                && (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)
                    || property.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase)));

        if (idProperty?.GetValue(request) is Guid requestId && requestId != Guid.Empty)
        {
            return requestId;
        }

        return null;
    }
}
