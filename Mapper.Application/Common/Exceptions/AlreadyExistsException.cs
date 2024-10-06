using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.Common.Exceptions
{
    public class AlreadyExistsException(string name, object key) : Exception($"Entity \"{name}\" ({key}) is already exists in DB.");
}
