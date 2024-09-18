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
    public partial class MarkViewModel(Mark mark) : ObservableValidator
    {
        private Mark _mark = mark;

        [Required(ErrorMessage = "Координата обязательна")]
        [ObservableProperty]
        [Range(0, double.MaxValue, ErrorMessage = "Координаты не могут быть меньше нуля")]
        private double _xPos;

        [Required(ErrorMessage = "Координата обязательна")]
        [ObservableProperty]
        [Range(0, double.MaxValue, ErrorMessage = "Координаты не могут быть меньше нуля")]
        private double _yPos;

        public bool SaveChanges()
        {
            ValidateAllProperties();
            if (HasErrors) return false;
            _mark.XPos = XPos;
            _mark.YPos = YPos;
            return true;
        }
    }
}
