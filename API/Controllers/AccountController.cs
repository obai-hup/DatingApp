using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using API.DTOS;
using Microsoft.EntityFrameworkCore;
using API.Intrfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _db;
        private readonly ITokkenService _tokkenService;

        public AccountController(DataContext db, ITokkenService tokkenService)
        {
            _db = db;
            _tokkenService = tokkenService;
        }

        /*
        ***
        * api/Account/Register
        ***
        */
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto Regdto)
        {

            if (await UserExists(Regdto.UserName)) return BadRequest("Username is taken");
            using var hmac = new HMACSHA512();
            var user = new AppUser()
            {
                UserName = Regdto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Regdto.Password)),
                PasswordSalt = hmac.Key
            };

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            return new UserDto
            {
                Username = user.UserName,
                Tokken = _tokkenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto LogDto)
        {
            //Find The User In DataBase 
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserName == LogDto.Username);

            if (user == null) return Unauthorized("Invalid username");

            // Unhash PasswordSalt
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(LogDto.Password));

            //Because It Byt[] Loop Over Each Elements

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDto
            {
                Username = user.UserName,
                Tokken = _tokkenService.CreateToken(user)
            };

        }

        private async Task<bool> UserExists(string username)
        {
            return await _db.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}