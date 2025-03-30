using AutoMapper;
using Mapper.Application.Common.Mappings;

namespace Mapper.Application.CommandsAndQueries.Employee.Queries.GetEmployeeList
{
    public class EmployeeLookupDto : IMapWith<Domain.Employee>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string? Patronymic { get; set; }
        public string Surname { get; set; }
        public string? Phone { get; set; }
        public string? Cabinet { get; set; }
        public string? Comment { get; set; }
        public string? Email { get; set; }
        public virtual Domain.GeoMark Geomark { get; set; }
        public Guid GeoMarkId { get; set; }
        public virtual Domain.EmployeePhoto EmployeePhoto { get; set; }
        public Guid PhotoId { get; set; }
        public bool IsArchived { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Employee, EmployeeLookupDto>()
                .ForMember(employeeDto => employeeDto.Id, opt => opt.MapFrom(employee => employee.Id))
                .ForMember(employeeDto => employeeDto.FirstName, opt => opt.MapFrom(employee => employee.FirstName))
                .ForMember(employeeDto => employeeDto.Patronymic, opt => opt.MapFrom(employee => employee.Patronymic))
                .ForMember(employeeDto => employeeDto.Surname, opt => opt.MapFrom(employee => employee.Surname))
                .ForMember(employeeDto => employeeDto.Phone, opt => opt.MapFrom(employee => employee.Phone))
                .ForMember(employeeDto => employeeDto.Cabinet, opt => opt.MapFrom(employee => employee.Cabinet))
                .ForMember(employeeDto => employeeDto.Comment, opt => opt.MapFrom(employee => employee.Comment))
                .ForMember(employeeDto => employeeDto.Email, opt => opt.MapFrom(employee => employee.Email))
                .ForMember(employeeDto => employeeDto.GeoMarkId, opt => opt.MapFrom(employee => employee.GeoMarkId))
                .ForMember(employeeDto => employeeDto.Geomark, opt => opt.MapFrom(employee => employee.GeoMark))
                .ForMember(employeeDto => employeeDto.PhotoId, opt => opt.MapFrom(employee => employee.PhotoId))
                .ForMember(employeeDto => employeeDto.EmployeePhoto, opt => opt.MapFrom(employee => employee.EmployeePhoto))
                .ForMember(employeeDto => employeeDto.IsArchived, opt => opt.MapFrom(employee => employee.IsArchived));
        }
    }
}
