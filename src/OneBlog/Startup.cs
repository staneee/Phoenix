using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.WebEncoders;
using OneBlog.Configuration;
using OneBlog.Data;
using OneBlog.Data.Common;
using OneBlog.Data.Contracts;
using OneBlog.Data.Repository;
using OneBlog.Helpers;
using OneBlog.Logger;
using OneBlog.MetaWeblog;
using OneBlog.Mvc;
using OneBlog.Services;
using SS.MetaWeblog;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using Autofac.Extensions.DependencyInjection;
using System.Text.Unicode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace OneBlog
{
    public class Startup
    {

        private IHostingEnvironment _hostingEnvironment { get; }
        private IConfiguration _configuration { get; }

        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            //中文支持
            //EncodingProvider provider = CodePagesEncodingProvider.Instance;
            //Encoding.RegisterProvider(provider);
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }


        public IServiceProvider ConfigureServices(IServiceCollection svcs)
        {

            svcs.Configure<AppSettings>(_configuration.GetSection(nameof(AppSettings)));
            svcs.Configure<DataSettings>(_configuration.GetSection(nameof(DataSettings)));
            svcs.Configure<QiniuSettings>(_configuration.GetSection(nameof(QiniuSettings)));
            svcs.Configure<EditorSettings>(_configuration.GetSection(nameof(EditorSettings)));

            svcs.AddSession();
            svcs.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            svcs.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                         {
                               "image/svg+xml",
                               "application/atom+xml"
                            }); ;
                options.Providers.Add<GzipCompressionProvider>();
            });


            if (_hostingEnvironment.IsDevelopment())
            {
                svcs.AddTransient<IMailService, LoggingMailService>();
            }
            else
            {
                svcs.AddTransient<IMailService, MailService>();
            }

            svcs.AddDbContext<ApplicationDbContext>(ServiceLifetime.Scoped);

            svcs.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


            svcs.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });

            svcs.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ThemeViewLocationExpander());
            });

            var builder = new ContainerBuilder();
            builder.RegisterType<PostsRepository>().As<IPostsRepository>();
            builder.RegisterType<DashboardRepository>().As<IDashboardRepository>();
            builder.RegisterType<ViewRenderService>().As<IViewRenderService>();
            builder.RegisterType<CommentsRepository>().As<ICommentsRepository>();
            builder.RegisterType<TagsRepository>().As<ITagsRepository>();
            builder.RegisterType<RolesRepository>().As<IRolesRepository>();
            builder.RegisterType<LookupsRepository>().As<ILookupsRepository>();
            builder.RegisterType<CategoriesRepository>().As<ICategoriesRepository>();
            builder.RegisterType<UsersRepository>().As<IUsersRepository>();
            builder.RegisterType<DbContextFactory>().As<IDbContextFactory>();

            builder.RegisterType<UrlHelperFactory>().As<IUrlHelperFactory>();
            builder.RegisterType<ActionContextAccessor>().As<IActionContextAccessor>();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>();

            builder.RegisterType<JsonService>();
            builder.RegisterType<ApplicationInitializer>();
            builder.RegisterType<ApplicationEnvironment>();
            builder.RegisterType<QiniuService>();
            builder.RegisterType<NavigationHelper>();

            builder.Populate(svcs);

            // Supporting Live Writer (MetaWeblogAPI)
            svcs.AddMetaWeblog<WeblogProvider>();

            // Add Caching Support
            svcs.AddMemoryCache(opt => opt.ExpirationScanFrequency = TimeSpan.FromMinutes(5));
            //// Add MVC to the container
            var mvcBuilder = svcs.AddMvc();
            //mvcBuilder.AddJsonOptions(opts => opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());
            mvcBuilder.AddJsonOptions(r =>
            {
                r.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                r.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });

            //var mvcCore = svcs.AddMvcCore();
            //mvcCore.AddJsonFormatters(options => options.ContractResolver = new CamelCasePropertyNamesContractResolver());
            // Add Https - renable once Azure Certs work
            if (_hostingEnvironment.IsProduction())
            {
                mvcBuilder.AddMvcOptions(options => options.Filters.Add(new RequireHttpsAttribute()));
            }

            return IocContainer.RegisterAutofac(builder);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              ILoggerFactory loggerFactory,
                              IMailService mailService,
                              IServiceScopeFactory scopeFactory)
        {

            app.UseResponseCompression();
            app.UseSession();
            // Add the following to the request pipeline only in development environment.
            if (_hostingEnvironment.IsDevelopment())
            {
                loggerFactory.AddDebug(LogLevel.Information);
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // Support logging to email
                loggerFactory.AddEmail(mailService, LogLevel.Critical);
                loggerFactory.AddConsole(LogLevel.Error);

                // Early so we can catch the StatusCode error
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseExceptionHandler("/Exception");
            }


            app.UseStaticFiles();
            // Support MetaWeblog API
            app.UseMetaWeblog("/livewriter");
            app.UseMiddleware<OldSysMiddleware>();
            // Keep track of Active # of users for Vanity Project
            app.UseMiddleware<ActiveUsersMiddleware>();

            app.UseAuthentication();

            app.UseMvc();

            if (_configuration["OneDb:TestData"] != "True")
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var initializer = scope.ServiceProvider.GetService<ApplicationInitializer>();
                    initializer.SeedAsync().Wait();
                }
            }
        }
    }
}
