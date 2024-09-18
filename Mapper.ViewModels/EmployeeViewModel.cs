using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mapper.Models.Models;

namespace Mapper.ViewModels
{
    public partial class EmployeeViewModel(Employee employee) : ObservableValidator
    {
        private Employee _employee = employee;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле {0} не может быть пустым")]
        [ObservableProperty]
        [MinLength(2)]
        [MaxLength(100)]
        [NotifyDataErrorInfo]
        [NotifyPropertyChangedFor(nameof(FullName))]
        private string _firstName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName))]
        private string? _patronymic;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле {0} не может быть пустым")]
        [NotifyPropertyChangedFor(nameof(FullName))]
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [MinLength(2)]
        [MaxLength(100)]
        private string _surname;

        public string FullName => $"{FirstName} {Patronymic} {Surname}";
        [ObservableProperty]
        private string? _phone;
        [ObservableProperty]
        private string? _cabinet;
        [ObservableProperty]
        private string? _comment;
        [ObservableProperty]
        [EmailAddress]
        private string? _email;
        [ObservableProperty]
        private GeoMark _geoMark;
        [ObservableProperty]
        private int _geoMarkId;
        [ObservableProperty]
        private EmployeePhoto? _photo;
        [ObservableProperty]
        private int? _photoId;
        [ObservableProperty]
        private bool _isArchived;

        //public Employee Clone()
        //{
        //    return (Employee)this.MemberwiseClone();
        //}

        public bool SaveChanges()
        {
            ValidateAllProperties();
            if (HasErrors) return false;
            _employee.FirstName = FirstName;
            _employee.Patronymic = Patronymic;
            _employee.Surname = Surname;
            _employee.Phone = _phone;
            _employee.Cabinet = Cabinet;
            _employee.Comment = Comment;
            _employee.Email = Email;
            _employee.GeoMark = GeoMark;
            _employee.GeoMarkId = GeoMarkId;
            _employee.Photo = Photo;
            _employee.PhotoId = PhotoId;
            _employee.IsArchived = IsArchived;
            return true;
        }
    }
}
