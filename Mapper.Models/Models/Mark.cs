using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Mapper.Models.Interfaces;

namespace Mapper.Models.Models
{
    public class Mark: IMark
    {
        public double XPos { get; set; }
        public double YPos { get; set; }
    }
}
