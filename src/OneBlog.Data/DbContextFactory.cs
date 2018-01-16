using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
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
        /// <summary>
        /// 数据配置项
        /// </summary>
        private DataSettings _dataSetting { get; }

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

        /// <summary>
        /// 创建DB的方法
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            return new ApplicationDbContext(this, builder.Options);
        }
    }

    public interface IDbContextFactory
    {
        void Configuring(DbContextOptionsBuilder optionsBuilder);
    }
}
