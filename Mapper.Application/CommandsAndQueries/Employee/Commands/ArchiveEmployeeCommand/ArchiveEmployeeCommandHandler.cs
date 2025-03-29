using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.CommandsAndQueries.Employee.Commands.ArchiveEmployeeCommand
{
    public class ArchiveEmployeeCommandHandler
            : IRequestHandler<ArchiveEmployeeCommand>
    {
        private readonly IMapperDbContext _dbContext;

        public ArchiveEmployeeCommandHandler(IMapperDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task Handle(ArchiveEmployeeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Employees
                .FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Domain.Employee), request.Id);
            }

            entity.IsArchived = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
