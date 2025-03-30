using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.CommandsAndQueries.Employee.Commands.UpdateEmployeeCommand
{
    public class UpdateEmployeeCommandHandler
            : IRequestHandler<UpdateEmployeeCommand>
    {
        private readonly IMapperDbContext _dbContext;

        public UpdateEmployeeCommandHandler(IMapperDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var entity =
                await _dbContext.Employees.FirstOrDefaultAsync(employee =>
                    employee.Id == request.Id, cancellationToken);

            if (entity == null || entity.Id != request.Id)
            {
                throw new NotFoundException(nameof(Employee), request.Id);
            }
            entity.Id = request.Id;
            entity.FirstName = request.FirstName;
            entity.Patronymic = request.Patronymic;
            entity.Surname = request.Surname;
            entity.Phone = request.Phone;
            entity.Cabinet = request.Cabinet;
            entity.Comment = request.Comment;
            entity.Email = request.Email;
            entity.GeoMarkId = request.GeoMarkId;
            entity.PhotoId = request.PhotoId;
            entity.IsArchived = request.IsArchived;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
