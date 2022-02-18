using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Entities;
using API.Interface;
using API.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController:BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context , ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) {

            using var hmac = new HMACSHA512();
            if(await CheckExistUser(registerDto.Username)) return BadRequest("user is taken");

            var user = new AppUser 
            {
                UserName = registerDto.Username.ToLower(), 
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
             _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return  new UserDto {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }
        public Task<bool> CheckExistUser(string username) {
           return  _context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto logindto) {

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == logindto.Username);
            if(user == null) return Unauthorized("Invalid User");
            
            //get key
            using var hmac = new HMACSHA512(user.PasswordSalt);
            // get PasswordHash of user current login thought PasswordSalt Key
            var computerhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(logindto.Password));
            for(int i = 0 ; i < computerhash.Length ; i ++) {
                if(computerhash[i] != user.PasswordHash[i]) return Unauthorized ("Password Invalid");
            }
            return new UserDto {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}
