using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Mapper.Domain
{
    public class GeoMark : Mark
    {

        public string MarkName { get; set; }
        public string? MarkDescription { get; set; }
        public string Color { get; set; }
        public string? Emoji { get; set; }
        public int? Size { get; set; }
        public bool IsEmoji { get; set; }
        public bool IsArchived { get; set; }
        public int GeoMapId { get; set; }
        public ObservableCollection<Employee>? Employees { get; set; }
        public ObservableCollection<GeoPhoto>? GeoPhotos { get; set; }


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
