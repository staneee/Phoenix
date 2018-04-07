using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OneBlog.Configuration;
using System.IO;

namespace OneBlog.Data
{
    public class DbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {

        private DataSettings _dataSetting { get; set; }

        public DbContextFactory()
        {

        }

        public DbContextFactory(IOptions<DataSettings> dataOptions)
        {
            _dataSetting = dataOptions.Value;
        }

        public AppDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var dataSetting = JsonConfigurationHelper.GetAppSettings<DataSettings>(nameof(Configuration.DataSettings), configuration);
            _dataSetting = dataSetting;
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            return new AppDbContext(builder.Options, _dataSetting);
        }
    }

    public class JsonConfigurationHelper
    {
        public static T GetAppSettings<T>(string key, IConfigurationRoot config) where T : class, new()
        {
            //IConfiguration config = new ConfigurationBuilder()
            //    .SetBasePath(currentClassDir)
            //    .Add(new JsonConfigurationSource { Path = "appsettings.json", Optional = false, ReloadOnChange = true })
            //    .Build();

            //IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<T>(config.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<T>>()
                .Value;
            return appconfig;
        }
    }

}
