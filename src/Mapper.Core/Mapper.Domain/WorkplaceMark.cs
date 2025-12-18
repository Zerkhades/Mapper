using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Domain
{
    public sealed class WorkplaceMark : GeoMark
    {
        public string WorkplaceCode { get; private set; } = default!;

        public ICollection<WorkplaceEmployee> Employees { get; private set; } = new List<WorkplaceEmployee>();

        private WorkplaceMark() { }

        public WorkplaceMark(Guid geoMapId, double x, double y, string title, string workplaceCode, string? description = null)
            : base(geoMapId, GeoMarkType.Workplace, x, y, title, description)
        {
            WorkplaceCode = workplaceCode;
        }

        public void SetWorkplaceCode(string code) => WorkplaceCode = code;
    }

    public class WorkplaceEmployee
    {
        public Guid WorkplaceMarkId { get; private set; }
        public Guid EmployeeId { get; private set; }

        private WorkplaceEmployee() { }

        public WorkplaceEmployee(Guid workplaceMarkId, Guid employeeId)
        {
            WorkplaceMarkId = workplaceMarkId;
            EmployeeId = employeeId;
        }
    }

}
