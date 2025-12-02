using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.Employee.Queries.GetEmployeeList
{
    public class GetEmployeeListQueryValidator : AbstractValidator<GetEmployeeListQuery>
    {
        public GetEmployeeListQueryValidator()
        {
            RuleFor(x => x.UserId).NotEqual(Guid.Empty);
        }
    }
}
