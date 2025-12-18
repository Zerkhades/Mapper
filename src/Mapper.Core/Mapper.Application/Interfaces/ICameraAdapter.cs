using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.Interfaces
{
    public interface ICameraAdapter
    {
        Task<bool> IsOnlineAsync(string? streamUrl, CancellationToken ct);
    }
}
