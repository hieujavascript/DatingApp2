using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // tra ve cac loi cho cho response (400 , 500 , 401 , 404)
    public class BuggyController: BaseApiController
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context) {
            _context = context;            
        }
        [Authorize]
        [HttpGet("Auth")]
        public ActionResult<string> GetSecret() {
            return "Secrec text";
        }
        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound() {
            var thing = _context.Users.Find(-1);
            if(thing == null)   
            return NotFound();
            
            return Ok(thing);
        }
        [HttpGet("server-error")]
         public ActionResult<string> GetServerError() {
           var thing = _context.Users.Find(-1);
           return thing.ToString();
        }
        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest() {
            return BadRequest("this is bad request");
        }
    }
}