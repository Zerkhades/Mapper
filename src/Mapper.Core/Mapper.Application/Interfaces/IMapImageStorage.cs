using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.Interfaces
{
    public interface IMapImageStorage
    {
        Task<string> SaveAsync(Stream content, string fileName, string contentType, CancellationToken ct);
    }
}
