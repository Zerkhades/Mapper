using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.Interfaces
{
    public interface IS3ObjectStorage
    {
        Task<string> PutAsync(string key, Stream content, string contentType, CancellationToken ct);
    }
}
