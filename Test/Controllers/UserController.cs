using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Test.Models;
using Test.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Test.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{

    UserService service;
    private string key;
    IMapper mapper;
    public UserController (UserService service, IConfiguration configuration, IMapper _mapper)
    {
        this.service = service;
        this.key = configuration.GetSection("JwtKey").ToString();
        this.mapper = _mapper;
    }



    // GET: api/<UserController>
    [HttpGet("{pageNumber}/{itemsPerPage}")]
    public async Task<ActionResult<List<User>>> Get(int pageNumber = 1, int itemsPerPage = 15)
    {
        var users = await service.Fetch(itemsPerPage, pageNumber);
        return Ok(users);
    }



    // GET api/<UserController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserGet>> Get(string id)
    {
        UserGet? user = await service.FetchById(id);
        if (user == null) 
        { 
            return BadRequest("User Does not exist");
        }
        else
        {
            return Ok(user);
        }
    }



    // POST api/<UserController>
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserRegister user)
    {
        User user1 = mapper.Map<User>(user);
        bool res = await service.Register(user1);

        if (res == false)
        {
            return BadRequest("Unknown error occured.");
        }

        return CreatedAtAction(nameof(Get), new { id = user1.Id }, user1);
    }



    // PUT api/<UserController>/
    [HttpPut("[action]")]
    public async Task<IActionResult> Update([FromBody] UserUpdate userUpdate)
    {
        bool isSuccessful = await service.Update(userUpdate);
        if (isSuccessful == false)
        {
            return BadRequest("Update operation failed.");
        }
        else
        {
            return Ok("Successfully updated.");
        }
    }




    // DELETE api/<UserController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var res = await service.Remove(id);
        if (res.DeletedCount == 0)
        {
            return BadRequest("The deletion was not performed.");
        }
        else
        {
            return Ok(res);
        }
    }




    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(UserLogin user)
    {
        var registered = await service.VerifyLogin(user);
        if (registered == true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(this.key);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Name)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(tokenHandler.WriteToken(token));
        }
        else
        {
            return Unauthorized("Not registered");
        }
    }

}
