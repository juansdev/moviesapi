using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesApi.Controllers;
using MoviesApi.Dto;
using MoviesApi.Entities;

namespace MoviesApi.Tests.UnitTests;

[TestClass]
public class MoviesControllerTests : BaseTests
{
    private async Task<string> CreateDataTest()
    {
        var dataBaseName = Guid.NewGuid().ToString();
        var context = BuildContext(dataBaseName);
        var gender = new Gender { Name = "genre 1" };

        var movies = new List<Movie>
        {
            new() { Title = "Pelicula 1", ReleaseDate = new DateTime(2010, 1, 1), InTheaters = false },
            new() { Title = "No estrenada", ReleaseDate = DateTime.Today.AddDays(1), InTheaters = false },
            new() { Title = "Pelicula en Cines", ReleaseDate = DateTime.Today.AddDays(-1), InTheaters = true }
        };

        var movieWithGender = new Movie
        {
            Title = "Pelicula con Genero",
            ReleaseDate = new DateTime(2010, 1, 1),
            InTheaters = false
        };
        movies.Add(movieWithGender);

        context.Add(gender);
        context.AddRange(movies);
        await context.SaveChangesAsync();

        var movieGender = new MoviesGenders { GenderId = gender.Id, MovieId = movieWithGender.Id };
        context.Add(movieGender);
        await context.SaveChangesAsync();
        return dataBaseName;
    }

    [TestMethod]
    public async Task FilterByTitle()
    {
        var nameDb = await CreateDataTest();
        var mapper = ConfigAutoMapper();
        var context = BuildContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        var titleMovie = "Pelicula 1";
        var filterDto = new FilterMovieDto
        {
            Title = titleMovie,
            AmountRegisterByPage = 10
        };
        var response = await controller.Filter(filterDto);
        var movies = response.Value;
        Assert.AreEqual(1, movies.Count);
        Assert.AreEqual(titleMovie, movies[0].Title);
    }

    [TestMethod]
    public async Task FilterInTheaters()
    {
        var nameDb = await CreateDataTest();
        var mapper = ConfigAutoMapper();
        var context = BuildContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var filterDto = new FilterMovieDto
        {
            InTheaters = true
        };

        var response = await controller.Filter(filterDto);
        var movies = response.Value;
        Assert.AreEqual(1, movies.Count);
        Assert.AreEqual("Pelicula en Cines", movies[0].Title);
    }

    [TestMethod]
    public async Task FilterNextsReleases()
    {
        var nameDb = await CreateDataTest();
        var mapper = ConfigAutoMapper();
        var context = BuildContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var filterDto = new FilterMovieDto
        {
            NextReleases = true
        };

        var response = await controller.Filter(filterDto);
        var movies = response.Value;
        Assert.AreEqual(1, movies.Count);
        Assert.AreEqual("No estrenada", movies[0].Title);
    }

    [TestMethod]
    public async Task FilterByGender()
    {
        var nameDb = await CreateDataTest();
        var mapper = ConfigAutoMapper();
        var context = BuildContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var genderId = context.Genders.Select(gender => gender.Id).First();
        var filterDto = new FilterMovieDto
        {
            GenderId = genderId
        };
        var response = await controller.Filter(filterDto);
        var movies = response.Value;
        Assert.AreEqual(1, movies.Count);
        Assert.AreEqual("Pelicula con Genero", movies[0].Title);
    }

    [TestMethod]
    public async Task FilterOrderTitleAscending()
    {
        var nameDb = await CreateDataTest();
        var mapper = ConfigAutoMapper();
        var context = BuildContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var filterDto = new FilterMovieDto
        {
            FieldOrder = "title",
            OrderAsc = true
        };
        var response = await controller.Filter(filterDto);
        var movies = response.Value;

        var context2 = BuildContext(nameDb);
        var moviesDb = await context2.Movies.OrderBy(movie => movie.Title).ToListAsync();
        Assert.AreEqual(moviesDb.Count, movies.Count);

        for (var i = 0; i < moviesDb.Count; i++)
        {
            var movieController = movies[i];
            var movieDb = moviesDb[i];

            Assert.AreEqual(movieDb.Id, movieController.Id);
        }
    }

    [TestMethod]
    public async Task FilterTitleDesc()
    {
        var nameDb = await CreateDataTest();
        var mapper = ConfigAutoMapper();
        var context = BuildContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var filterDto = new FilterMovieDto
        {
            FieldOrder = "title",
            OrderAsc = false
        };
        var response = await controller.Filter(filterDto);
        var movies = response.Value;

        var context2 = BuildContext(nameDb);
        var moviesDb = await context2.Movies.OrderByDescending(movie => movie.Title).ToListAsync();
        Assert.AreEqual(moviesDb.Count, movies.Count);

        for (var i = 0; i < moviesDb.Count; i++)
        {
            var movieController = movies[i];
            var movieDb = moviesDb[i];

            Assert.AreEqual(movieDb.Id, movieController.Id);
        }
    }

    [TestMethod]
    public async Task FilterByFieldIncorrectReturnMovies()
    {
        var nameDb = await CreateDataTest();
        var mapper = ConfigAutoMapper();
        var context = BuildContext(nameDb);

        var mock = new Mock<ILogger<MoviesController>>();

        var controller = new MoviesController(context, mapper, null, mock.Object);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var filterDto = new FilterMovieDto
        {
            FieldOrder = "abc",
            OrderAsc = true
        };

        var response = await controller.Filter(filterDto);
        var movies = response.Value;

        var context2 = BuildContext(nameDb);
        var moviesDb = context2.Movies.ToList();
        Assert.AreEqual(moviesDb.Count, movies.Count);
        Assert.AreEqual(1, mock.Invocations.Count);
    }
}