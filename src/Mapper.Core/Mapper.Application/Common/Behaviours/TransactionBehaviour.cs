using MediatR;
using Mapper.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Common.Behaviours;

public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMapperDbContext _context;
    private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
    private readonly DbContext _efContext;

    public TransactionBehaviour(IMapperDbContext context, ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
    {
        _context = context;
        _logger = logger;
        _efContext = context as DbContext ?? throw new ArgumentException("IMapperDbContext должен быть DbContext");
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Для команд используем транзакции
        if (!requestName.Contains("Query"))
        {
            try
            {
                await using var transaction = await _efContext.Database.BeginTransactionAsync(cancellationToken);

                _logger.LogInformation("Begin transaction for {RequestName}", requestName);

                var response = await next();

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Committed transaction for {RequestName}", requestName);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction failed for {RequestName}", requestName);
                throw;
            }
        }

        return await next();
    }
}