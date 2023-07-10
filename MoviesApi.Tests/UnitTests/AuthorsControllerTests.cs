using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesApi.Controllers;
using MoviesApi.Dto;
using MoviesApi.Entities;
using MoviesApi.Services;

namespace MoviesApi.Tests.UnitTests;

[TestClass]
public class AuthorsControllerTests : BaseTests
{
    [TestMethod]
    public async Task GetPeoplePaginated()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigAutoMapper();

        context.Authors.Add(new Author { Name = "Actor1" });
        context.Authors.Add(new Author { Name = "Actor2" });
        context.Authors.Add(new Author { Name = "Actor3" });

        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);
        var controller = new AuthorsController(context2, mapper, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var page1 = await controller.Get(new PaginationDto { Page = 1, AmountRegisterByPage = 2 });
        var authorPage1 = page1.Value;
        Assert.AreEqual(2, authorPage1.Count);

        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var page2 = await controller.Get(new PaginationDto { Page = 2, AmountRegisterByPage = 2 });
        var authorPage2 = page2.Value;
        Assert.AreEqual(1, authorPage2.Count);

        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var page3 = await controller.Get(new PaginationDto { Page = 3, AmountRegisterByPage = 2 });
        var authorPage3 = page3.Value;
        Assert.AreEqual(0, authorPage3.Count);
    }

    [TestMethod]
    public async Task CreateAuthorWithoutPhoto()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigAutoMapper();

        var author = new CreateAuthorDto { Name = "Felipe", BirthdayDate = DateTime.Today };
        var mock = new Mock<IFileStorage>();
        mock.Setup(mock => mock.SaveFile(null, null, null, null)).Returns(Task.FromResult("url"));
        var controller = new AuthorsController(context, mapper, mock.Object);
        var response = await controller.Post(author);

        var result = response as CreatedAtRouteResult;
        Assert.AreEqual(201, result.StatusCode);

        var context2 = BuildContext(nameDb);
        var list = await context2.Authors.ToListAsync();
        Assert.AreEqual(1, list.Count);
        Assert.IsNull(list[0].Photo);
        Assert.AreEqual(0, mock.Invocations.Count);
    }

    [TestMethod]
    public async Task CreateAuthorWithPhoto()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigAutoMapper();

        var content = Encoding.UTF8.GetBytes("Imagen de prueba");
        var file = new FormFile(new MemoryStream(content), 0, content.Length, "Data", "image.jpg");
        file.Headers = new HeaderDictionary();
        file.ContentType = "image/jpg";
        var author = new CreateAuthorDto
        {
            Name = "Nuevo actor",
            BirthdayDate = DateTime.Now,
            Photo = file
        };

        var mock = new Mock<IFileStorage>();
        mock.Setup(x => x.SaveFile(content, ".jpg", "authors", file.ContentType)).Returns(Task.FromResult("url"));

        var controller = new AuthorsController(context, mapper, mock.Object);
        var response = await controller.Post(author);
        var result = response as CreatedAtRouteResult;
        Assert.AreEqual(201, result.StatusCode);

        var context2 = BuildContext(nameDb);
        var list = await context2.Authors.ToListAsync();
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("url", list[0].Photo);
        Assert.AreEqual(1, mock.Invocations.Count);
    }

    [TestMethod]
    public async Task PatchReturn404IfAuthorExist()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigAutoMapper();

        var controller = new AuthorsController(context, mapper, null);
        var patchDoc = new JsonPatchDocument<AuthorPatchDto>();
        var response = await controller.Patch(1, patchDoc);
        var result = response as StatusCodeResult;
        Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public async Task PatchUpdateAFiled()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigAutoMapper();

        var birthdayDate = DateTime.Now;
        var author = new Author
        {
            Name = "Felipe",
            BirthdayDate = birthdayDate
        };
        context.Add(author);
        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);
        var controller = new AuthorsController(context2, mapper, null);

        var objectValidator = new Mock<IObjectModelValidator>();
        objectValidator.Setup(mock => mock.Validate(
            It.IsAny<ActionContext>(),
            It.IsAny<ValidationStateDictionary>(),
            It.IsAny<string>(),
            It.IsAny<object>()));
        controller.ObjectValidator = objectValidator.Object;

        var patchDoc = new JsonPatchDocument<AuthorPatchDto>();
        patchDoc.Operations.Add(new Operation<AuthorPatchDto>(
            "replace",
            "/name",
            null,
            "Claudia"
        ));
        var response = await controller.Patch(1, patchDoc);
        var result = response as StatusCodeResult;
        Assert.AreEqual(204, result.StatusCode);

        var context3 = BuildContext(nameDb);
        var authorDb = await context3.Authors.FirstAsync();
        Assert.AreEqual("Claudia", authorDb.Name);
        Assert.AreEqual(birthdayDate, authorDb.BirthdayDate);
    }
}