using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using System.Text;

namespace Webapi.MyExtension
{
    public static class ObjectPoolExtension
    {
        public static void SampleObjectPool(this IServiceCollection services)
        {
            services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton<ObjectPool<StringBuilder>>(serviceProvider =>
            {
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                var policy = new Microsoft.Extensions.ObjectPool.StringBuilderPooledObjectPolicy();
                return provider.Create(policy);
            });
        }
    }
}
