namespace Mapper.WebApi.Models.Employees;

public class CreateEmployeeRequest
{
    public string FirstName { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public string? Patronymic { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Cabinet { get; set; }
    public string? Comment { get; set; }
    public Guid GeoMarkId { get; set; }
}

public class UpdateEmployeeRequest
{
    public string FirstName { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public string? Patronymic { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Cabinet { get; set; }
    public string? Comment { get; set; }
    public Guid GeoMarkId { get; set; }
}
