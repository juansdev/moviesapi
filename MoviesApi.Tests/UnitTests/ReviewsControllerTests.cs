using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesApi.Controllers;
using MoviesApi.Dto;
using MoviesApi.Entities;

namespace MoviesApi.Tests.UnitTests;

[TestClass]
public class ReviewsControllerTests : BaseTests
{
    [TestMethod]
    public async Task UserCanNotCreateTwoReviewsForSameMovie()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        await CreateMovies(nameDb);

        var movieId = context.Movies.Select(movie => movie.Id).First();
        var review1 = new Review
        {
            MovieId = movieId,
            UserId = userDefaultId,
            Score = 5
        };
        context.Add(review1);
        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);
        var mapper = ConfigAutoMapper();

        var controller = new ReviewController(context, mapper);
        controller.ControllerContext = BuildControllerContext();

        var createReviewDto = new CreateReviewDto { Score = 5 };
        var response = await controller.Post(movieId, createReviewDto);

        var value = response as IStatusCodeActionResult;
        Assert.AreEqual(400, value.StatusCode.Value);
    }

    [TestMethod]
    public async Task CreateReview()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        await CreateMovies(nameDb);

        var movieId = context.Movies.Select(movie => movie.Id).First();
        var context2 = BuildContext(nameDb);

        var mapper = ConfigAutoMapper();
        var controller = new ReviewController(context2, mapper);
        controller.ControllerContext = BuildControllerContext();

        var createReviewDto = new CreateReviewDto { Score = 5 };
        var response = await controller.Post(movieId, createReviewDto);

        var value = response as NoContentResult;
        Assert.IsNotNull(value);

        var context3 = BuildContext(nameDb);
        var reviewDb = context3.Reviews.First();
        Assert.AreEqual(userDefaultId, reviewDb.UserId);
    }

    private async Task CreateMovies(string nameDb)
    {
        var context = BuildContext(nameDb);
        context.Movies.Add(new Movie
        {
            Title = "pelicula 1"
        });
        await context.SaveChangesAsync();
    }
}