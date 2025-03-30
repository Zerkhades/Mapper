using AutoMapper;
using Mapper.Application.Common.Mappings;

namespace Mapper.Application.CommandsAndQueries.Employee.Queries.GetEmployeeDetails
{
    public class EmployeeDetailsVm : IMapWith<Domain.Employee>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? Patronymic { get; set; }
        public string Surname { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {Surname} {Patronymic}";
        public string? Phone { get; set; }
        public string? Cabinet { get; set; }
        public string? Comment { get; set; }
        public string? Email { get; set; }
        public virtual Domain.GeoMark GeoMark { get; set; }
        public Guid GeoMarkId { get; set; }
        public virtual Domain.EmployeePhoto? EmployeePhoto { get; set; }
        public Guid EmployeePhotoId { get; set; }
        public bool IsArchived { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Employee, EmployeeDetailsVm>()
                .ForMember(empVm => empVm.Id,
                    opt => opt.MapFrom(emp => emp.Id))
                .ForMember(empVm => empVm.FirstName,
                    opt => opt.MapFrom(emp => emp.FirstName))
                .ForMember(empVm => empVm.Patronymic,
                    opt => opt.MapFrom(emp => emp.Patronymic))
                .ForMember(empVm => empVm.Surname,
                    opt => opt.MapFrom(emp => emp.Surname))
                .ForMember(empVm => empVm.FullName,
                    opt => opt.MapFrom(emp => emp.FullName))
                .ForMember(empVm => empVm.Phone,
                    opt => opt.MapFrom(emp => emp.Phone))
                .ForMember(empVm => empVm.Cabinet,
                    opt => opt.MapFrom(emp => emp.Cabinet))
                .ForMember(empVm => empVm.Comment,
                    opt => opt.MapFrom(emp => emp.Comment))
                .ForMember(empVm => empVm.Email,
                    opt => opt.MapFrom(emp => emp.Email))
                .ForMember(empVm => empVm.GeoMark,
                    opt => opt.MapFrom(emp => emp.GeoMark))
                .ForMember(empVm => empVm.GeoMarkId,
                    opt => opt.MapFrom(emp => emp.GeoMarkId))
                .ForMember(empVm => empVm.EmployeePhoto,
                    opt => opt.MapFrom(emp => emp.EmployeePhoto))
                .ForMember(empVm => empVm.EmployeePhotoId,
                    opt => opt.MapFrom(emp => emp.PhotoId))
                .ForMember(empVm => empVm.IsArchived,
                    opt => opt.MapFrom(emp => emp.IsArchived));
        }
    }
}
