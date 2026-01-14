namespace Mapper.Domain;

public sealed class WorkplaceMark : GeoMark
{
    public string WorkplaceCode { get; private set; } = default!;

    public ICollection<Employee> Employees { get; private set; } = new List<Employee>();

    private WorkplaceMark() { } // EF

    public WorkplaceMark(Guid geoMapId, double x, double y, string title, string workplaceCode, string? description = null)
        : base(geoMapId, GeoMarkType.Workplace, x, y, title, description)
    {
        WorkplaceCode = workplaceCode;
    }

    public void SetWorkplaceCode(string code) => WorkplaceCode = code;
}
