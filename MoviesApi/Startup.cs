using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Helpers;
using MoviesApi.Services;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MoviesApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Startup));
        services.AddTransient<IFileStorage, FileStorageAzure>();
        // services.AddTransient<IFileStorage, FileStorageLocal>();
        services.AddHttpContextAccessor();
        services.AddSingleton(NtsGeometryServices.Instance.CreateGeometryFactory(4326));
        services.AddSingleton(provider =>
            new MapperConfiguration(config =>
            {
                var geometryFactory = provider.GetRequiredService<GeometryFactory>();
                config.AddProfile(new AutoMapperProfiles(geometryFactory));
            }).CreateMapper());
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptions => sqlServerOptions.UseNetTopologySuite()));
        // Add services to the container.
        services.AddControllers().AddNewtonsoftJson();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}