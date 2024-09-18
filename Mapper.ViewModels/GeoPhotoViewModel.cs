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
    public partial class GeoPhotoViewModel(GeoPhoto geoPhoto) : ObservableValidator
    {
        private GeoPhoto _geoPhoto = geoPhoto;

        [Required]
        [ObservableProperty]
        private int _id;

        [Required]
        [ObservableProperty]
        private string _photoName;

        [Required]
        [ObservableProperty]
        private byte[] _file;

        [Required]
        [ObservableProperty]
        private bool _isArchived;

        private bool SaveChanges()
        {
            ValidateAllProperties();
            if(HasErrors) return false;
            _geoPhoto.Id = Id;
            _geoPhoto.PhotoName = PhotoName;
            _geoPhoto.File = File;
            _geoPhoto.IsArchived = IsArchived;
            return true;
        }
    }
}
