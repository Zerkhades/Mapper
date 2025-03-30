using System.ComponentModel.DataAnnotations;

namespace Mapper.Domain
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string? Patronymic { get; set; }
        public string FullName => $"{FirstName} {Surname} {Patronymic}";
        public string Surname { get; set; }
        public string? Phone { get; set; }
        public string? Cabinet { get; set; }
        public string? Comment { get; set; }
        public string? Email { get; set; }
        public GeoMark GeoMark { get; set; }
        public Guid GeoMarkId { get; set; }
        public EmployeePhoto? EmployeePhoto { get; set; }
        public Guid PhotoId { get; set; }
        public bool IsArchived { get; set; }

        public Employee()
        {
            IsArchived = false;
        }
    }

}
