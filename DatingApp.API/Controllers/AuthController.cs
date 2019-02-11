using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Models;
using DatingApp.API.Dtos;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config=config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //validate request
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if(await _repo.UserExists(userForRegisterDto.Username))
            return BadRequest("Username already exists.");

            var userToCrete= new User
            {
                Username=userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCrete,userForRegisterDto.Password);

            return StatusCode(210);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFormRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if(userFormRepo == null)
            return Unauthorized();

            var claims =new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFormRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFormRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds= new SigningCredentials(Key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject= new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials= creds
            };

            var tokenHandler= new JwtSecurityTokenHandler();

            var token= tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new{
                token = tokenHandler.WriteToken(token);
            });
        }

    }
}