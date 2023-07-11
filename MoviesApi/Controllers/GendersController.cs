using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Dto;
using MoviesApi.Entities;

namespace MoviesApi.Controllers;

[ApiController]
[Route("api/genders")]
public class GendersController : CustomBaseController
{
    public GendersController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    [HttpGet]
    public async Task<ActionResult<List<GenderDto>>> Get()
    {
        return await Get<Gender, GenderDto>();
    }

    [HttpGet("{id:int}", Name = "getGender")]
    public async Task<ActionResult<GenderDto>> Get(int id)
    {
        return await Get<Gender, GenderDto>(id);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreateGenderDto createGenderDto)
    {
        return await Post<CreateGenderDto, Gender, GenderDto>(createGenderDto, "getGender");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] CreateGenderDto createGenderDto)
    {
        return await Put<CreateGenderDto, Gender>(id, createGenderDto);
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Delete(int id)
    {
        return await Delete<Gender>(id);
    }
}