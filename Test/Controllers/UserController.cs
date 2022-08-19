using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Test.Models;
using Test.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Test.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{

    UserService _userService;
    ITokenService _tokenService;
    ICacheService _cacheService;
    private readonly string _key;
    IMapper _mapper;
    public UserController (UserService userService,
                           IConfiguration configuration,
                           IMapper mapper,
                           TokenService tokenService,
                           CacheService cacheService)
    {
        _key = configuration.GetSection("JwtKey").ToString();
        _mapper = mapper;
        _userService = userService;
        _tokenService = tokenService;
        _cacheService = cacheService;
    }



    // GET: api/<UserController>
    [HttpGet("list/{pageNumber}/{itemsPerPage}")]
    public async Task<ActionResult<List<User>>> Get(int pageNumber = 1, int itemsPerPage = 15)
    {
        var users = await _userService.Fetch(itemsPerPage, pageNumber);
        return Ok(users);
    }

    // GET: api/<UserController>
    [HttpGet("[action]")]
    public async Task<ActionResult<Object>> GetCollectionSize()
    {
        var totalRecords = await _userService.getCollectionSize();
        return Ok(new { count = totalRecords });
    }

    // GET api/<UserController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserGet>> Get(string id)
    {
        UserGet? user = await _userService.FetchById(id);
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
    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromBody] UserRegister user)
    {
        User user1 = _mapper.Map<User>(user);
        bool res = await _userService.Register(user1);

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
        bool isSuccessful = await _userService.Update(userUpdate);
        if (isSuccessful == false)
        {
            return BadRequest(new { message = "Update operation failed." });
        }
        else
        {
            return Ok(new { message = "Successfully updated." });
        }
    }




    // DELETE api/<UserController>/5
    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var res = await _userService.Remove(id);
        if (res.DeletedCount == 0)
        {
            return BadRequest("The deletion was not performed.");
        }
        else
        {
            return Ok(res);
        }
    }

    // Get all active users
    [HttpGet("[action]")]
    public async Task<List<string>> GetAllActiveUsers()
    {
        return await _cacheService.getAll();
    }
}
