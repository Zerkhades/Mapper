using MediatR;

namespace Mapper.Application.CommandsAndQueries.Employee.Commands.CreateEmployeeCommand
{
    public class CreateEmployeeCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string? Patronymic { get; set; }
        public string Surname { get; set; }
        public string? Phone { get; set; }
        public string? Cabinet { get; set; }
        public string? Comment { get; set; }
        public string? Email { get; set; }
        public Guid GeoMarkId { get; set; }
        public Guid PhotoId { get; set; }
        public bool IsArchived { get; set; }
    }
}
