using CommunityToolkit.Mvvm.ComponentModel;
using Mapper.Models.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Mapper.ViewModels
{
    [NotifyDataErrorInfo]
    public partial class GeoMapViewModel : ObservableValidator
    {
        private GeoMap _geoMap;

        [Required(ErrorMessage = "ID обязателен")]
        [Range(1, int.MaxValue, ErrorMessage = "ID должен быть больше нуля")]
        [ObservableProperty]
        private int _id;

        [Required(ErrorMessage = "Название карты обязательно")]
        [MinLength(3, ErrorMessage = "Название карты должно содержать минимум 3 символа")]
        [MaxLength(100, ErrorMessage = "Название карты не может превышать 100 символов")]
        [ObservableProperty]
        private string _mapName;

        [MaxLength(500, ErrorMessage = "Описание карты не может превышать 500 символов")]
        [ObservableProperty]
        private string _mapDescription;

        [Required(ErrorMessage = "Файл карты обязателен")]
        [ObservableProperty]
        private byte[] _map;


        [Required(ErrorMessage = "Наличие метки удаления обязательно")]
        [ObservableProperty]
        private bool _isArchived;

        [Required(ErrorMessage = "Test")]
        [ObservableProperty]
        private ObservableCollection<GeoMark> _geoMarks;

        public GeoMapViewModel(GeoMap geoMap)
        {
            _geoMap = geoMap;
            _geoMarks = new ObservableCollection<GeoMark>(_geoMap.GeoMarks ?? new ObservableCollection<GeoMark>());
        }


        // Метод для сохранения изменений обратно в модель
        public bool SaveChanges()
        {
            ValidateAllProperties();
            if (HasErrors) return false;
            _geoMap.Id = Id;
            _geoMap.MapName = MapName;
            _geoMap.MapDescription = MapDescription;
            _geoMap.Map = Map;
            _geoMap.IsArchived = IsArchived;
            _geoMap.GeoMarks = GeoMarks;
            return true;
        }
    }
}
