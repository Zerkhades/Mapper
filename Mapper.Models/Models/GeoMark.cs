using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Mapper.Models.Interfaces;

namespace Mapper.Models.Models
{
    public class GeoMark : Mark, IGeoMark
    {
        public Guid Id { get; set; }
        public string MarkName { get; set; }
        public string? MarkDescription { get; set; }
        public string Color { get; set; }
        public string? Emoji { get; set; }
        public int? Size { get; set; }
        public bool IsEmoji { get; set; }
        public bool IsArchived { get; set; }
        public int GeoMapId { get; set; }
        public virtual ObservableCollection<Employee>? Employees { get; set; }
        public virtual ObservableCollection<GeoPhoto>? GeoPhotos { get; set; }


        public GeoMark()
        {
            Emoji = "\uD83D\uDE00";  //😀 , hehe
            Size = 16;
            XPos = 0;
            YPos = 0;
            Color = "#FF0000";
            Employees = new();
            GeoPhotos = new();
            IsEmoji = false;
            IsArchived = false;
        }


    }




}
