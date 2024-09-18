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
    [NotifyDataErrorInfo]
    public partial class EmployeePhotoViewModel : ObservableValidator
    {
        private EmployeePhoto _employeePhoto;

        [Required]
        [ObservableProperty]
        private int _id;

        [Required]
        [ObservableProperty]
        private byte[] _photo;

        [Required]
        [ObservableProperty]
        private bool _isArchived;

        public EmployeePhotoViewModel(EmployeePhoto employeePhoto)
        {
            _employeePhoto = employeePhoto;
            Photo = Properties.Resources.userpic;
        }

        public void SaveChanges()
        {
            ValidateAllProperties();
            if(!HasErrors) return;
            _employeePhoto.Id = Id;
            _employeePhoto.Photo = Photo;
            _employeePhoto.IsArchived = IsArchived;
        }
    }
}
