using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Mapper.WebApi.Services;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context) => true;
}
