using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapper.Models;
using Mapper.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Mapper.DBService.DbContexts
{
    public class MapperDbContext : DbContext
    {
        public MapperDbContext(DbContextOptions options) : base(options) { }
        public DbSet<GeoMap> GeoMaps { get; set; }
        public DbSet<GeoMark> GeoMarks { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeePhoto> EmployeesPhotos { get; set; }
        //public DbSet<AccessToService> AccessToServices { get; set; }
    }
}
