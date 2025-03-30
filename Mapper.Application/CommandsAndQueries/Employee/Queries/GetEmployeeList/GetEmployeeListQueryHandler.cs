using AutoMapper;
using AutoMapper.QueryableExtensions;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.CommandsAndQueries.Employee.Queries.GetEmployeeList
{
    public class GetEmployeeListQueryHandler
                : IRequestHandler<GetEmployeeListQuery, EmployeeListVm>
    {
        private readonly IMapperDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetEmployeeListQueryHandler(IMapperDbContext dbContext,
            IMapper mapper) =>
            (_dbContext, _mapper) = (dbContext, mapper);

        public async Task<EmployeeListVm> Handle(GetEmployeeListQuery request, CancellationToken cancellationToken)
        {
            var employeesQuery = await _dbContext.Employees
                .ProjectTo<EmployeeLookupDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new EmployeeListVm { Employees = employeesQuery };
        }
    }
}
