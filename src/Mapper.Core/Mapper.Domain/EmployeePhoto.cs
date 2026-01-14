namespace Mapper.Domain;

public class EmployeePhoto
{
    public Guid Id { get; set; }
    //public byte[] Photo { get; set; }
    public bool IsArchived { get; set; }

    public EmployeePhoto()
    {
        IsArchived = false;
    }
}
