using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.CommandsAndQueries.Employee.Commands.DeleteEmployeeCommand
{
    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand>
    {
        private readonly IMapperDbContext _dbContext;

        public DeleteEmployeeCommandHandler(IMapperDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Employees.FirstOrDefaultAsync(employee =>
                employee.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Employee), request.Id);
            }

            _dbContext.Employees.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
