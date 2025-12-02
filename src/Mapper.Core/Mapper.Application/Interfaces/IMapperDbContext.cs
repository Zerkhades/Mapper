using Microsoft.EntityFrameworkCore;
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
