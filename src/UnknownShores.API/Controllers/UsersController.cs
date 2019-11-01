using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnknownShores.Entities;
using UnknownShores.Services;

namespace UnknownShores.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            var token = _userService.Authenticate(model.Username, model.Password);

            if (token == null)
                return BadRequest(new { message = "Unknown user" });

            return Ok(token);
        }


        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var identifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            
            if(identifier == null)
                return BadRequest(new { message = "Invalid user claim" });
            
            var id = 0;
            if(!int.TryParse(identifier.Value, out id))
                return BadRequest(new { message = "Invalid user identifier" });

            var user = _userService.Profile(id);
            if (user == null)
                return BadRequest(new { message = "Unknown user" });

            return Ok(user);
        }
    }
}
