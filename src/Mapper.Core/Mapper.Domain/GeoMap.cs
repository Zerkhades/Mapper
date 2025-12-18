using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;


namespace Mapper.Domain
{
    public class GeoMap
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name { get; private set; } = default!;
        public string? Description { get; private set; }

        // Ссылка на файл изображения (не тащим blob в БД)
        public string ImagePath { get; private set; } = default!;
        public int ImageWidth { get; private set; }
        public int ImageHeight { get; private set; }

        public ICollection<GeoMark> Marks { get; private set; } = new List<GeoMark>();

        private GeoMap() { } // EF

        public GeoMap(string name, string imagePath, int width, int height, string? description = null)
        {
            Name = name;
            ImagePath = imagePath;
            ImageWidth = width;
            ImageHeight = height;
            Description = description;
        }
    }
}
