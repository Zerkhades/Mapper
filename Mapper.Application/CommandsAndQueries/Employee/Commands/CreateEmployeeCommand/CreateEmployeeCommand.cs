using MediatR;

namespace Mapper.Application.CommandsAndQueries.Employee.Commands.CreateEmployeeCommand
{
    public class CreateEmployeeCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public string? Patronymic { get; set; }
        public required string Surname { get; set; }
        public string? Phone { get; set; }
        public string? Cabinet { get; set; }
        public string? Comment { get; set; }
        public string? Email { get; set; }
        public required Guid GeoMarkId { get; set; }
        public virtual Domain.GeoMark? GeoMark { get; set; }
        public Guid? EmployeePhotoId { get; set; }
        public virtual  Domain.EmployeePhoto? EmployeePhoto { get; set; }
        public bool IsArchived { get; set; }
    }
}
