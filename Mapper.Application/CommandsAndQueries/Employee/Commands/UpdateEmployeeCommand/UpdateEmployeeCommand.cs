using MediatR;

namespace Mapper.Application.CommandsAndQueries.Employee.Commands.UpdateEmployeeCommand
{
    public class UpdateEmployeeCommand : IRequest
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public string? Patronymic { get; set; }
        public required string Surname { get; set; }
        public string FullName => string.IsNullOrWhiteSpace(Patronymic) ? $"{FirstName} {Surname}" : $"{FirstName} {Surname} {Patronymic}";
        public string? Phone { get; set; }
        public string? Cabinet { get; set; }
        public string? Comment { get; set; }
        public Guid GeoMarkId { get; set; }
        public virtual Domain.GeoMark? GeoMark { get; set; }
        public Guid? EmployeePhotoId { get; set; }
        public virtual Domain.EmployeePhoto? EmployeePhoto { get; set; }
        public string? Email { get; set; }
        public bool IsArchived { get; set; }
    }
}
