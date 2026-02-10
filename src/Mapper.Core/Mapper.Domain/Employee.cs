namespace Mapper.Domain;

public class Employee
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public string? Patronymic { get; set; }
    public required string Surname { get; set; }
    public string FullName => string.IsNullOrWhiteSpace(Patronymic) ? $"{FirstName} {Surname}" : $"{FirstName} {Surname} {Patronymic}";
    public string? Phone { get; set; }
    public string? Cabinet { get; set; }
    public string? Comment { get; set; }
    public string? Email { get; set; }
    public required Guid GeoMarkId { get; set; }
    public WorkplaceMark? GeoMark { get; set; }
    public Guid? EmployeePhotoId { get; set; }
    public virtual EmployeePhoto? EmployeePhoto { get; set; }
    public bool IsArchived { get; set; }

    public Employee()
    {
        IsArchived = false;
    }
}
