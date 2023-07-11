using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoviesApi.Tests.IntegrationTests;

[TestClass]
public class ReviewsControllerTests : BaseTests
{
    private static readonly string url = "api/movies/1/reviews";

    [TestMethod]
    public async Task GetReviewsReturn404MovieNotExist()
    {
        var nameDb = Guid.NewGuid().ToString();
        var factory = BuildWebApplicationFactory(nameDb);

        var client = factory.CreateClient();
        var response = await client.GetAsync(url);
        Assert.AreEqual(404, (int)response.StatusCode);
    }

    // [TestMethod]
    // public async Task GetReviewsReturnListVoid()
    // {
    //     var nameDb = Guid.NewGuid().ToString();
    //     var factory = BuildWebApplicationFactory(nameDb);
    //
    //     var context = BuildContext(nameDb);
    //     context.Movies.Add(new Movie() { Id=1, Title = "Pelicula 1" });
    //     await context.SaveChangesAsync();
    //     var client = factory.CreateClient();
    //     var response = await client.GetAsync(url);
    //
    //     response.EnsureSuccessStatusCode();
    //
    //     var reviews = JsonConvert.DeserializeObject<List<ReviewDto>>(await response.Content.ReadAsStringAsync());
    //     Assert.AreEqual(0, reviews.Count);
    // }
}