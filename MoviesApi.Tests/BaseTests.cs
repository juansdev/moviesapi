using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Helpers;
using NetTopologySuite;

namespace MoviesApi.Tests;

public class BaseTests
{
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
}