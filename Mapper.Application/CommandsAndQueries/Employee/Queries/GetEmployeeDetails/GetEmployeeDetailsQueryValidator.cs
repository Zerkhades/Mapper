using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.Employee.Queries.GetEmployeeDetails
{
    public class GetEmployeeDetailsQueryValidator : AbstractValidator<GetEmployeeDetailsQuery>
    {
        public GetEmployeeDetailsQueryValidator()
        {
            RuleFor(employee => employee.Id).NotEqual(Guid.Empty);
        }
    }
}
