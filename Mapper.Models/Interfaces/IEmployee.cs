using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Models.Interfaces
{
    public interface IEmployee
    {
        public int Id { get;}

        public string FirstName { get; }


        public string? Patronymic { get; }

        public string Surname { get; }

        public string FullName => $"{FirstName} {Surname} {Patronymic}";

        public string? Phone { get; }

        public string? Cabinet { get; }

        public string? Comment { get; }

        public string? Email { get; }

        public Models.GeoMark GeoMark { get; }

        public int GeoMarkId { get; }

        public Models.EmployeePhoto? Photo { get; }

        public int? PhotoId { get; }

        public bool IsArchived { get; }
    }
}
