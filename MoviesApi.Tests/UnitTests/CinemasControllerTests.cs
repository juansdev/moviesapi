using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesApi.Controllers;
using MoviesApi.Dto;
using MoviesApi.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MoviesApi.Tests.UnitTests;

[TestClass]
public class CinemasControllerTests : BaseTests
{
    [TestMethod]
    public async Task GetCinemaTo5KmsOrLess()
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
        {
            var cinemas = new List<Cinema>
            {
                new()
                {
                    Name = "Agora",
                    Location = geometryFactory.CreatePoint(new Coordinate(-69.9388777, 18.4839233))
                }
            };
            context.AddRange(cinemas);
            await context.SaveChangesAsync();
        }

        var filter = new CinemaNearFilterDto
        {
            distanceByKms = 5,
            Latitude = 18.481139,
            Longitude = -69.938950
        };
        using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
        {
            var mapper = ConfigAutoMapper();
            var controller = new CinemasController(context, mapper, geometryFactory);
            var response = await controller.Nears(filter);
            var value = response.Value;
            Assert.AreEqual(2, value.Count);
        }
    }
}