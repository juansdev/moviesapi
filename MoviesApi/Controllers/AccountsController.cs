using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MoviesApi;
using MoviesApi.Controllers;
using MoviesApi.Dto;

namespace PeliculasAPI.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : CustomBaseController
{
    private readonly IConfiguration _configuration;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext context;

    public AccountsController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration,
        ApplicationDbContext context,
        IMapper mapper)
        : base(context, mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        this.context = context;
    }

    [HttpPost("Create")]
    public async Task<ActionResult<UserTokenDto>> CreateUser([FromBody] UserInfoDto model)
    {
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
            return await BuildToken(model);
        return BadRequest(result.Errors);
    }

    [HttpPost("Login")]
    public async Task<ActionResult<UserTokenDto>> Login([FromBody] UserInfoDto model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Email,
            model.Password, false, false);

        if (result.Succeeded)
            return await BuildToken(model);
        return BadRequest("Invalid login attempt");
    }

    [HttpPost("RenovateToken")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<UserTokenDto>> Renovate()
    {
        var userInfo = new UserInfoDto
        {
            Email = HttpContext.User.Identity.Name
        };

        return await BuildToken(userInfo);
    }

    private async Task<UserTokenDto> BuildToken(UserInfoDto userInfo)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userInfo.Email),
            new(ClaimTypes.Email, userInfo.Email)
        };

        var identityUser = await _userManager.FindByEmailAsync(userInfo.Email);

        claims.Add(new Claim(ClaimTypes.NameIdentifier, identityUser.Id));

        var claimsDB = await _userManager.GetClaimsAsync(identityUser);

        claims.AddRange(claimsDB);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiracion = DateTime.UtcNow.AddYears(1);

        var token = new JwtSecurityToken(
            null,
            null,
            claims,
            expires: expiracion,
            signingCredentials: creds);

        return new UserTokenDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiracion
        };
    }

    [HttpGet("Users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<List<UserDto>>> Get([FromQuery] PaginationDto paginationDTO)
    {
        var queryable = context.Users.AsQueryable();
        queryable = queryable.OrderBy(x => x.Email);
        return await Get<IdentityUser, UserDto>(paginationDTO);
    }

    [HttpGet("Rols")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<List<string>>> GetRoles()
    {
        return await context.Roles.Select(x => x.Name).ToListAsync();
    }

    [HttpPost("AssignRol")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult> AssignRol(EditRolDto editarRolDTO)
    {
        var user = await _userManager.FindByIdAsync(editarRolDTO.UserId);
        if (user == null) return NotFound();

        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, editarRolDTO.NameRol));
        return NoContent();
    }

    [HttpPost("RemoveRol")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult> DeleteRol(EditRolDto editarRolDTO)
    {
        var user = await _userManager.FindByIdAsync(editarRolDTO.UserId);
        if (user == null) return NotFound();

        await _userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editarRolDTO.NameRol));
        return NoContent();
    }
}