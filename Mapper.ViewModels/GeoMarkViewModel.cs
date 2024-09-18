using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mapper.Models.Models;

namespace Mapper.ViewModels
{
    [NotifyDataErrorInfo]
    public partial class GeoMarkViewModel(GeoMark geoMark) : ObservableValidator
    {
        private GeoMark _geoMark = geoMark;

        [Required(ErrorMessage = "Название точки не должно быть пустым!")]

        [ObservableProperty]
        [MinLength(2, ErrorMessage = "Название точки не должны быть меньше 2 символов")]
        [MaxLength(100)]
        private string? _markName;

        [Required(ErrorMessage = "Описание точки не должно быть пустым!")]
        [ObservableProperty]
        [MinLength(2, ErrorMessage = "Описание точки не должны быть меньше 2 символов")]
        private string? _markDescription;

        [Required]
        [ObservableProperty]
        private string _color;

        [Required]
        [ObservableProperty]
        private string? _emoji;

        [Required]
        [ObservableProperty]
        private int? _size;

        [Required]
        [ObservableProperty]
        private bool _isEmoji;

        [Required]
        [ObservableProperty]
        private bool _isArchived;

        [Required] 
        [ObservableProperty] 
        private int _geoMapId;

        [Required]
        [ObservableProperty]
        private ObservableCollection<Employee>? _employees;

        [Required]
        [ObservableProperty] 
        private ObservableCollection<GeoPhoto>? _geoPhotos;

        public GeoMark Clone()
        {
            return (GeoMark)this.MemberwiseClone();
        }

        public bool SaveChanges()
        {
            ValidateAllProperties();
            if (HasErrors) return false;
            _geoMark.MarkName = MarkName;
            _geoMark.MarkDescription = MarkDescription;
            _geoMark.Color = Color;
            _geoMark.Emoji = Emoji;
            _geoMark.Size = Size;
            _geoMark.IsEmoji = IsEmoji;
            _geoMark.IsArchived = IsArchived;
            _geoMark.GeoMapId = GeoMapId;
            return true;
        }
    }
}
