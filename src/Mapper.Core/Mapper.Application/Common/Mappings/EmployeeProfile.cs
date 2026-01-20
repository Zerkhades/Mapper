using AutoMapper;
using Mapper.Application.Features.Employees.Queries.GetEmployeeDetails;
using Mapper.Application.Features.Employees.Queries.GetEmployeeList;
using Mapper.Domain;

namespace Mapper.Application.Common.Mappings;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, EmployeeListItemDto>();
        CreateMap<Employee, EmployeeDetailsDto>();
    }
}
