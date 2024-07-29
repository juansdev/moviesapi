using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesApi.Dto;
using Newtonsoft.Json;

namespace MoviesApi.Tests.IntegrationTests;

[TestClass]
public class GendersControllerTests : BaseTests
{
    private static readonly string url = "api/genders";

    [TestMethod]
    public async Task GetAllGenders()
    {
        var nameDb = Guid.NewGuid().ToString();
        var factory = BuildWebApplicationFactory(nameDb);

        var client = factory.CreateClient();
        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var genders = JsonConvert.DeserializeObject<List<GenderDto>>(await response.Content.ReadAsStringAsync());

        Assert.AreEqual(7, genders.Count);
    }

    // [TestMethod]
    // public async Task DeleteGender()
    // {
    //     var nameDb = Guid.NewGuid().ToString();
    //     var factory = BuildWebApplicationFactory(nameDb);
    //
    //     var context = BuildContext(nameDb);
    //     context.Genders.Add(new Gender { Name = "Gender 1" });
    //     await context.SaveChangesAsync();
    //
    //     var client = factory.CreateClient();
    //     var response = await client.DeleteAsync($"{url}/1");
    //     response.EnsureSuccessStatusCode();
    //
    //     var context2 = BuildContext(nameDb);
    //     var exist = await context2.Genders.AnyAsync();
    //     Assert.IsFalse(exist);
    // }

    [TestMethod]
    public async Task DeleteGenderReturn401()
    {
        var nameDb = Guid.NewGuid().ToString();
        var factory = BuildWebApplicationFactory(nameDb, false);

        var client = factory.CreateClient();
        var response = await client.DeleteAsync($"{url}/1");

        Assert.AreEqual("Unauthorized", response.ReasonPhrase);
    }
}