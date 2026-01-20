using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Mapper.Application.Common.Behaviours;

namespace Mapper.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services
                .AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() });
            services.AddTransient(typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>),
                typeof(LoggingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>),
                typeof(PerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>),
                typeof(TransactionBehaviour<,>));
            return services;
        }
    }
}
