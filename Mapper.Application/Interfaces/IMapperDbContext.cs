using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapper.Domain;

namespace Mapper.Application.Interfaces
{
    public interface IMapperDbContext
    {
        public DbSet<GeoMap> GeoMaps { get; set; }
        public DbSet<GeoMark> GeoMarks { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeePhoto> EmployeesPhotos { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
