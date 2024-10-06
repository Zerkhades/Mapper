using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapper.Domain;
using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.UpdateGeoMapCommand
{
    public class UpdateGeoMapCommand : IRequest
    {
        public Guid Id { get; set; }
        public string MapName { get; set; }
        public string MapDescription { get; set; }
        public string IsArchived { get; set; }
        //public byte[] Map { get; set; }
        public ObservableCollection<Domain.GeoMark> GeoMarks { get; set; }
    }
}
