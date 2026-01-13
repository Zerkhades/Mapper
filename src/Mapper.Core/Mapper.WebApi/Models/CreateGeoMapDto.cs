using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Mapper.Application.Common.Mappings;

namespace Mapper.WebApi.Models
{
    public class CreateGeoMapDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
    }
}
