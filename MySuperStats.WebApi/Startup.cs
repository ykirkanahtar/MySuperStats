using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySuperStats.WebApi.ApplicationSettings;
using MySuperStats.WebApi.Business;
using MySuperStats.WebApi.Data;
using MySuperStats.WebApi.Data.Repositories;
using MySuperStats.WebApi.Models;
using Newtonsoft.Json;
using CustomFramework.BaseWebApi.Data.Extensions;
using MySuperStats.Contracts.Resources;
using CustomFramework.BaseWebApi.Utils.Extensions;
using CustomFramework.BaseWebApi.Data.Enums;
using CustomFramework.BaseWebApi.Identity.Extensions;
using CustomFramework.BaseWebApi.Authorization.Utils;
using CustomFramework.EmailProvider;
using CustomFramework.BaseWebApi.Identity.Data;
using CustomFramework.BaseWebApi.Resources;

namespace MySuperStats.WebApi
{
    public class Startup
    {
        public static DatabaseProvider DbProvider = DatabaseProvider.MsSql;
        public static AppSettings AppSettings { get; private set; }
        public static string ConnectionString { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            ConnectionString = Configuration.GetValue<string>("ConnectionStrings:ApplicationContext");

            AppSettings = new AppSettings();
            Configuration.GetSection("AppSettings").Bind(AppSettings);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseContext<ApplicationContext>(ConnectionString, DbProvider);

            IdentityModelExtension<User, Role, ApplicationContext>.AddIdentityModel(services, new IdentityModel
            {
                AppName = AppSettings.AppName,
                EmailConfirmationViaUrl = true,
                SenderEmailAddress = AppSettings.SenderEmailAddress,
                Token = AppSettings.Token,
                EmailConfig = AppSettings.EmailConfig,
                GeneratedPasswordLength = 0
            }, AppSettings.Token, true);

            services.AddLogging(logging =>
            {
                logging.AddDebug();

            });

            services.AddSwaggerDocumentation(AppSettings.AppName, 1);

            services.AddWebApiUtilServices();

            services.AddAutoMapper();

            services.AddSingleton<IToken, Token>(p => AppSettings.Token);
            services.AddSingleton<IEmailConfig, EmailConfig>(p => AppSettings.EmailConfig);
            services.AddScoped<IAppSettings, AppSettings>(p => AppSettings);
            services.AddTransient<ILocalizationService, LocalizationService>();

            var cultureInfos = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("tr-TR"),
            };

            services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = cultureInfos;
                options.SupportedUICultures = cultureInfos;
                options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
            });

            services.AddTransient<IUnitOfWorkIdentity, UnitOfWorkWebApi>();
            services.AddTransient<IUnitOfWorkWebApi, UnitOfWorkWebApi>();

            /*********Repositories*********/
            services.AddTransient<IMatchRepository, MatchRepository>();
            services.AddTransient<ITeamRepository, TeamRepository>();
            services.AddTransient<IBasketballStatRepository, BasketballStatRepository>();
            services.AddTransient<IMatchGroupRepository, MatchGroupRepository>();
            services.AddTransient<IMatchGroupUserRepository, MatchGroupUserRepository>();
            services.AddTransient<IMatchGroupTeamRepository, MatchGroupTeamRepository>();
            services.AddTransient<IFootballStatRepository, FootballStatRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IPlayerRepository, PlayerRepository>();
            /*********Repositories*********/


            /*********Managers*********/
            services.AddTransient<IMatchManager, MatchManager>();
            services.AddTransient<IBasketballStatManager, BasketballStatManager>();
            services.AddTransient<ITeamManager, TeamManager>();
            services.AddTransient<IMatchGroupManager, MatchGroupManager>();
            services.AddTransient<IMatchGroupUserManager, MatchGroupUserManager>();
            services.AddTransient<IMatchGroupTeamManager, MatchGroupTeamManager>();
            services.AddTransient<IFootballStatManager, FootballStatManager>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<IPermissionChecker, PermissionChecker>();
            services.AddTransient<IPlayerManager, PlayerManager>();
            /*********Managers*********/

            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            services.AddMvc(options =>
                {
                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Culture = CultureInfo.CurrentUICulture;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseSwaggerDocumentation(AppSettings.AppName, 1);

            app.UseErrorWrappingMiddleware();

            app.UseMvc();

            app.UseHttpsRedirection(); //Bu satır UseMvc'in üstüne yazılırsa localhost için SSL hatası veriyor

        }

    }
}