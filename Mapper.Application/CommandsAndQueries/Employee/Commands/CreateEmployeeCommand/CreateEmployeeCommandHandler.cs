using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;

namespace Mapper.Application.CommandsAndQueries.Employee.Commands.CreateEmployeeCommand
{
    public class CreateEmployeeCommandHandler
            : IRequestHandler<CreateEmployeeCommand, Guid>
    {
        private readonly IMapperDbContext _dbContext;
        public CreateEmployeeCommandHandler(IMapperDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task<Guid> Handle(CreateEmployeeCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Employees
                .FindAsync(new object[] { request.Id }, cancellationToken);
            if (entity != null)
            {
                throw new AlreadyExistsException(nameof(Domain.Employee), request.Id);
            }

            var employee = new Domain.Employee
            {
                Id = new Guid(),
                FirstName = request.FirstName,
                Patronymic = request.Patronymic,
                Surname = request.Surname,
                Phone = request.Phone,
                Cabinet = request.Cabinet,
                Comment = request.Comment,
                Email = request.Email,
                GeoMarkId = request.GeoMarkId,
                PhotoId = request.PhotoId,
                IsArchived = request.IsArchived,
            };
            await _dbContext.Employees.AddAsync(employee, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return employee.Id;
        }
    }
}
