using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesApi.Controllers;
using MoviesApi.Dto;
using MoviesApi.Entities;

namespace MoviesApi.Tests.UnitTests;

[TestClass]
public class GendersControllerTests : BaseTests
{
    [TestMethod]
    public async Task GetAllGenders()
    {
        // Preparacion
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigAutoMapper();

        context.Genders.Add(new Gender { Name = "Genero 1" });
        context.Genders.Add(new Gender { Name = "Genero 2" });
        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);

        // Prueba
        var controller = new GendersController(context2, mapper);
        var response = await controller.Get();

        // Verificacion
        var genders = response.Value;
        Assert.AreEqual(2, genders?.Count);
    }

    [TestMethod]
    public async Task GetGenderByIdNotExist()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigAutoMapper();

        var controller = new GendersController(context, mapper);
        var response = await controller.Get(1);
        var result = response.Result as StatusCodeResult;
        Assert.AreEqual(404, result?.StatusCode);
    }

    [TestMethod]
    public async Task GetGenderByIdExist()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigAutoMapper();

        context.Genders.Add(new Gender { Name = "Genero 1" });
        context.Genders.Add(new Gender { Name = "Genero 2" });
        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);
        var controller = new GendersController(context2, mapper);

        var id = 1;
        var response = await controller.Get(id);
        var result = response.Value;
        Assert.AreEqual(id, result?.Id);
    }

    [TestMethod]
    public async Task CreateGender()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigAutoMapper();

        var newGender = new CreateGenderDto { Name = "Nuevo Genero" };
        var controller = new GendersController(context, mapper);
        var response = await controller.Post(newGender);
        var result = response as CreatedAtRouteResult;
        Assert.IsNotNull(result);

        var context2 = BuildContext(nameDb);
        var amount = await context2.Genders.CountAsync();
        Assert.AreEqual(1, amount);
    }

    [TestMethod]
    public async Task PutGender()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigAutoMapper();

        context.Genders.Add(new Gender { Name = "Genero 1" });
        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);
        var controller = new GendersController(context2, mapper);

        var createGenderDto = new CreateGenderDto { Name = "Nuevo nombre" };

        var id = 1;
        var response = await controller.Put(id, createGenderDto);

        var result = response as StatusCodeResult;
        Assert.AreEqual(204, result.StatusCode);

        var context3 = BuildContext(nameDb);
        var exist = await context3.Genders.AnyAsync(gender => gender.Name == "Nuevo nombre");
        Assert.IsTrue(exist);
    }

    [TestMethod]
    public async Task TryDeleteGenderNotExist()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigAutoMapper();

        var controller = new GendersController(context, mapper);

        var response = await controller.Delete(1);
        var result = response as StatusCodeResult;
        Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public async Task DeleteGender()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigAutoMapper();

        context.Genders.Add(new Gender { Name = "Genero 1" });
        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);
        var controller = new GendersController(context2, mapper);

        var response = await controller.Delete(1);
        var result = response as StatusCodeResult;
        Assert.AreEqual(204, result.StatusCode);

        var context3 = BuildContext(nameDb);
        var exist = await context3.Genders.AnyAsync();
        Assert.IsFalse(exist);
    }
}