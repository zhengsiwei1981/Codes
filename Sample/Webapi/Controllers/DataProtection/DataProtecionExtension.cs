using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Webapi.MyExtension
{
    public static class DataProtecionExtension
    {
        public static void SampleDataProtection(this IServiceCollection services)
        {
            services.AddDbContext<DataProtectionDbContext>(options =>
            {
                options.UseInMemoryDatabase("DBContext");
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.EnableSensitiveDataLogging();
            });

            //services.AddOptions<KeyManagementOptions>()
            //    .Configure<IServiceScopeFactory>((options, factory) =>
            //    {
                    
            //        options.XmlRepository = new CustomXmlRepository(factory);
            //    });
            services.AddDataProtection().SetApplicationName("myapplication").PersistKeysToDbContext<DataProtectionDbContext>();
        }
    }
    public class DataProtectionDbContext : DbContext, IDataProtectionKeyContext
    {
        public DataProtectionDbContext(DbContextOptions<DataProtectionDbContext> options)
        : base(options)
        {
        }
        public DbSet<XmlKey> XmlKeys { get; set; }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
    public class XmlKey
    {
        public Guid Id { get; set; }
        public string Xml { get; set; }

        public XmlKey()
        {
            this.Id = Guid.NewGuid();
        }
    }
    public class CustomXmlRepository : IXmlRepository
    {
        private readonly IServiceScopeFactory factory;

        public CustomXmlRepository(IServiceScopeFactory factory)
        {
            this.factory = factory;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            using (var scope = factory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataProtectionDbContext>();
                var keys = context.XmlKeys.ToList()
                    .Select(x => XElement.Parse(x.Xml))
                    .ToList();
                return keys;
            }
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            var key = new XmlKey
            {
                Xml = element.ToString(SaveOptions.DisableFormatting)
            };

            using (var scope = factory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataProtectionDbContext>();
                context.XmlKeys.Add(key);
                context.SaveChanges();
            }
        }
    }
}
