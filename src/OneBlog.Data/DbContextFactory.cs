using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OneBlog.Configuration;
using OneBlog.Data.Providers;
using OneBlog.Helpers;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OneBlog.Data
{
    public class DbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>, IDbContextFactory
    {

        private DataSettings _dataSetting { get; set; }

        public DbContextFactory()
        {

        }

        public DbContextFactory(IOptions<DataSettings> dataOptions)
        {
            _dataSetting = dataOptions.Value;
        }


        public void Configuring(DbContextOptionsBuilder optionsBuilder)
        {
            var currentAssembly = typeof(DbContextFactory).GetTypeInfo().Assembly;
            var allDataProviders = currentAssembly.GetTypes<IDataProvider>();//获取所有数据提供类型
            var selectedDataProvider = allDataProviders.SingleOrDefault(x => x.Provider == _dataSetting.Provider);
            selectedDataProvider.Configuring(optionsBuilder, _dataSetting.ConnectionString);
        }

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var dataSetting = JsonConfigurationHelper.GetAppSettings<DataSettings>(nameof(Configuration.DataSettings), configuration);
            _dataSetting = dataSetting;
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            return new ApplicationDbContext(this, builder.Options);
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

    public interface IDbContextFactory
    {
        void Configuring(DbContextOptionsBuilder optionsBuilder);
    }
}
