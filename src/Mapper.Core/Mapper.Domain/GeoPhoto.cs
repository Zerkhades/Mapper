
namespace Mapper.Domain
{
    public class GeoPhoto
    {
        public Guid Id { get; set; }
        public string PhotoName { get; set; }
        //public byte[] File { get; set; }
        public bool IsArchived { get; set; }

        public GeoPhoto()
        {
            IsArchived = false;
        }

    }
}
