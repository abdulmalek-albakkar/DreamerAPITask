using System;
using System.Text;
using Dreamer.API.Configs;
using Dreamer.API.Filters;
using Dreamer.API.Middlewares;
using Dreamer.API.Tools;
using Dreamer.Domain.Repositories;
using Dreamer.EmailNotifier.Dto;
using Dreamer.EmailNotifier.IService;
using Dreamer.EmailNotifier.Service;
using Dreamer.IDomain.Repositories;
using Dreamer.InternalHangfire.IService;
using Dreamer.InternalHangfire.Service;
using Dreamer.ShippingCostCalculator.IService;
using Dreamer.ShippingCostCalculator.Service;
using Dreamer.SqlServer.Database;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace Dreamer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Main Services
            services.AddTransient<ISecurityRepository, SecurityRepository>();
            services.AddTransient<IProductsRepository, ProductsRepository>();
            services.AddTransient<ICartRepository, CartRepository>();
            services.AddTransient<ISeedingRepository, SeedingRepository>();
            services.AddTransient<IEmailNotifierService, EmailNotifierService>();
            services.AddTransient<IShippingCostCalculatorService, ShippingCostCalculatorService>();
            services.AddTransient<IInternalHangfireService, InternalHangfireService>();

            services.AddDbContext<DreamerDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")
                , sqlDbOptions => sqlDbOptions.MigrationsAssembly("Dreamer.SqlServer")));
            #endregion

            #region LastUserActivityDateFilter
            services.AddScoped<LastActivityFilter>();
            services.AddMvc().AddMvcOptions(options => {
                options.Filters.AddService(typeof(LastActivityFilter));
            });
            #endregion

            #region Token services
            var tokenConfig = Configuration
                    .GetSection(nameof(TokenConfig))
                    .Get<TokenConfig>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        RequireExpirationTime = true,
                        ValidIssuer = tokenConfig.Domain,
                        ValidAudience = tokenConfig.Domain,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfig.TokenKey))
                    };
                });
            #endregion

            #region Hangfire Service
            DatabaseHelper.CreateIfNotExists(Configuration.GetConnectionString("HangfireConnection"));
            var sqlServerStorageOptions = new SqlServerStorageOptions
            {
                QueuePollInterval = TimeSpan.FromMinutes(1)
            };
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), sqlServerStorageOptions));
            #endregion

            #region Configs
            services.Configure<TokenConfig>(Configuration.GetSection("TokenConfig"));
            services.Configure<MailConfig>(Configuration.GetSection("MailConfig"));
            #endregion

            #region Swagger
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Dreamer Web API",
                    Description = "Dreamer Test Project",
                    Contact = new Contact
                    {
                        Name = "Abdulmalek Albakkar",
                        Email = "abdulmalekalbakkar@gmail.com",
                        Url = "http://mallok.me"
                    }
                });
            });
            #endregion

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireServer();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dreamer Web API");
            });
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                   name: "default",
                   template: "{controller}/{action}");
            });
        }
    }
}
