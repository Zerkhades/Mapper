using CommunityToolkit.Mvvm.Messaging.Messages;
using Mapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapper.Models.Models;

namespace Mapper.Messages
{
    public class EmployeeMessage : ValueChangedMessage<Employee>
    {
        public EmployeeMessage(Employee value) : base(value)
        {
        }
    }

    public class TransferCoordinatesMessage : ValueChangedMessage<Mark>
    {
        public TransferCoordinatesMessage(Mark value) : base(value)
        {
        }
    }
    public class GeoMarkMessage : ValueChangedMessage<GeoMark>
    {
        public GeoMarkMessage(GeoMark value) : base(value)
        {
        }
    }
    public class GeoMapMessage : ValueChangedMessage<GeoMap>
    {
        public GeoMapMessage(GeoMap value) : base(value)
        {
        }
    }
}
