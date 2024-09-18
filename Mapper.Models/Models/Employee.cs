using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Mapper.Models.Interfaces;

namespace Mapper.Models.Models
{
    public  class Employee : IEmployee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string? Patronymic { get; set; }
        public string FullName => $"{FirstName} {Surname} {Patronymic}";
        public string Surname { get; set; }
        public string? Phone { get; set; }
        public string? Cabinet { get; set; }
        public string? Comment { get; set; }
        public string? Email { get; set; }
        public GeoMark GeoMark { get; set; }
        public int GeoMarkId { get; set; }
        public EmployeePhoto? Photo { get; set; }
        public int? PhotoId { get; set; }
        public bool IsArchived { get; set; }

        public Employee()
        {
            Photo = new EmployeePhoto();
            IsArchived = false;
        }
    }

}
