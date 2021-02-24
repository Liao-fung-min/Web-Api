using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApi_jwt.Model;

namespace WebApi_jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogginController : ControllerBase
    {
        private IConfiguration _config;
        public LogginController(IConfiguration config)
        {

            _config = config;

        }
        [HttpGet]
        public IActionResult Login(string username, string password)
        
        {

            UserModel login = new UserModel();
            login.Username = username;
            login.Password = password;
            IActionResult responese = Unauthorized();
            var user = AuthenticateUser(login);

            if (user != null)
            {

                var tokenstr = GenerateJSONWebToken(user);
                responese = Ok(new { token = tokenstr });

            }
            return responese;

        }
        private UserModel AuthenticateUser(UserModel login)
        {
            UserModel user = null;
            if (login.Username == "ALEX" && login.Password == "123")
            {
                user = new UserModel { Username = "liaofungmin", EmailAddress = "abcdefghijklmnopgrstu@gmail.com", Password = "123" };

            }
            return user;
        }

        private string GenerateJSONWebToken(UserModel userinfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var clasmis = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userinfo.Username),
            new Claim(JwtRegisteredClaimNames.Email, userinfo.EmailAddress),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


            var token = new JwtSecurityToken
                (
                    issuer: _config["Jwt:Issur"],
                    audience: _config["Jwt:Issur"],
                    clasmis,
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: credentials
                );
            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }
        [Authorize]
        [HttpPost("~/scuess")]
        public string Post() {
            //var identity = HttpContext.User.Identity as ClaimsIdentity;
            //IList<Claim> claim = identity.Claims.ToList();
            //var username = claim[1].Value;
            //var username = "ALEX";
            return "成功";
          
        }
        //[Authorize]
        [HttpGet("GetValue")]
        public ActionResult<IEnumerable<string>> Get() {

            return new string[] { "Value1", "Value2", "Value3" };
        
        }
    }

}



