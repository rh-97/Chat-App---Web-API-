using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Test.Helpers;
using Test.Models;
using Test.Services;

namespace Test.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{

    TokenService _tokenService;
    CacheService _cacheService;
    private readonly string _key;
    public AuthController(IConfiguration configuration, TokenService tokenService, CacheService cacheService)
    {
        _key = configuration.GetSection("JwtKey").ToString();
        _tokenService = tokenService;
        _cacheService = cacheService;
    }


    [HttpPost("login")]
    public async Task<ActionResult<Object>> Login(UserLogin user)
    {
        var registered = await _tokenService.VerifyLogin(user);
        if (registered == true)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name)
            };

            var accessToken = JwtHelpers.GenerateAccessToken(claims);
            var refreshToken = JwtHelpers.GenerateRefreshToken();

            //_service.loginModel.RefreshToken = refreshToken;
            //_service.loginModel.Expires = DateTime.Now.AddMinutes(10);

            await _tokenService.SaveRefreshToken(new Token
            {
                Name = user.Name,
                RefreshToken = refreshToken,
                ExpireDateTime = DateTime.UtcNow.AddMinutes(10)
            });

            await _cacheService.Set<int>("dr-" + user.Name, 1);

            return Ok(new TokenPair
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
        else
        {
            return Unauthorized("Not registered");
        }
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenPair tokenPair)
    {
        if (tokenPair == null) return BadRequest("Invalid Token Request.");

        string accessToken = tokenPair.AccessToken;
        string refreshToken = tokenPair.RefreshToken;

        var principal = JwtHelpers.GetPrincipalFromExpiredToken(accessToken);
        var username = principal.Identity.Name;

        Token token = await _tokenService.GetToken(username, refreshToken);

        if (token == null || token.ExpireDateTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid Token Request.");
        }

        var newAccessToken = JwtHelpers.GenerateAccessToken(principal.Claims);
        var newRefreshToken = JwtHelpers.GenerateRefreshToken();

        bool save = await _tokenService.SaveRefreshToken(new Token
        {
            Name = username,
            RefreshToken = newRefreshToken,
            ExpireDateTime = token.ExpireDateTime
        });

        if (save == false)
        {
            return StatusCode(StatusCodes.Status304NotModified,"Internal server Error: New token could not be saved.");
        }

        return Ok(new TokenPair
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });


    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] TokenPair tokenPair)
    {
        if (tokenPair == null) return BadRequest("Invalid Token Request.");

        string accessToken = tokenPair.AccessToken;
        string refreshToken = tokenPair.RefreshToken;

        var principal = JwtHelpers.GetPrincipalFromExpiredToken(accessToken);
        var username = principal.Identity.Name;


        bool delete = await _tokenService.DaleteRefreshToken(username, refreshToken);

        if (delete == false)
        {
            return NotFound("Invalid token.");
        }

        _cacheService.Delete("dr-" + username);

        return Ok("Successfully logged out.");

    }
}
