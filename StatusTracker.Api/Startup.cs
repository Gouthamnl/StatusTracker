using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Logger;
using Logger.Models;
using Logger.Serialization;
using TrackingService.DAL.Context;
using TrackingService.DAL.Models;
using TrackingService.DAL.Repository;

namespace StatusTracker.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string AllowTBISpecificOrigins = "AllowTBISpecificOrigins";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(AllowTBISpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("https://tracking.taxbackinternational.com")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddTransient<IDbContext, DbContext>();
            services.AddTransient<IStatusTrackerRepository, StatusTrackerRepository>();
            services.AddTransient<IConnectionManager, ConnectionManager>();
            services.AddScoped<ILog, Log>();
            services.AddScoped<ISerializationManager, JsonSerializationManager>();
            services.AddOptions<DatabaseModel>().Configure(options => Configuration.GetSection(nameof(DatabaseModel)).Bind(options));
            services.AddOptions<LogOptions>().Configure(options => Configuration.GetSection(nameof(LogOptions)).Bind(options));
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors(AllowTBISpecificOrigins);
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
