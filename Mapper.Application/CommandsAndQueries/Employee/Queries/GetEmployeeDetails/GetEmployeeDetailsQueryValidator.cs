using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.Employee.Queries.GetEmployeeDetails
{
    public class GetEmployeeDetailsQueryValidator : AbstractValidator<GetEmployeeDetailsQuery>
    {
        public GetEmployeeDetailsQueryValidator()
        {
            RuleFor(employee => employee.Id).NotEqual(Guid.Empty);
            //RuleFor(getEmployeeDetailsQueryValidator => getEmployeeDetailsQueryValidator.FirstName)
            //    .NotEmpty().MaximumLength(100);
            //RuleFor(getEmployeeDetailsQueryValidator => getEmployeeDetailsQueryValidator.Surname)
            //    .NotEmpty().MaximumLength(100);
            //RuleFor(getEmployeeDetailsQueryValidator => getEmployeeDetailsQueryValidator.FullName)
            //    .NotEmpty().MaximumLength(200);
        }
    }
}
