using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Helpers;
using NetTopologySuite;

namespace MoviesApi.Tests;

public class BaseTests
{
    protected string userDefaultEmail = "ejemplo@hotmail.com";
    protected string userDefaultId = "9722b56a-77ea-4e41-941d-e319b6eb3712";

    protected ApplicationDbContext BuildContext(string nameDb)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(nameDb).Options;
        var dbContext = new ApplicationDbContext(options);
        return dbContext;
    }

    protected IMapper ConfigAutoMapper()
    {
        var config = new MapperConfiguration(options =>
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
            options.AddProfile(new AutoMapperProfiles(geometryFactory));
        });
        return config.CreateMapper();
    }

    protected ControllerContext BuildControllerContext()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.Name, userDefaultEmail),
            new(ClaimTypes.Email, userDefaultEmail),
            new(ClaimTypes.NameIdentifier, userDefaultId)
        }));
        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };
    }

    protected WebApplicationFactory<Startup> BuildWebApplicationFactory(string nameDb, bool ignoreSegurity = true)
    {
        var factory = new WebApplicationFactory<Startup>();
        factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptorDbContext =
                    services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptorDbContext == null) services.Remove(descriptorDbContext);

                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(nameDb));
                if (ignoreSegurity)
                {
                    services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();
                    services.AddControllers(options => { options.Filters.Add(new UserFalseFilter()); });
                }
            });
        });
        return factory;
    }
}