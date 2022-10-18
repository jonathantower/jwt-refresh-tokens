using AngularClient.Models;
using AngularClient.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AngularClient.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MyContext _myContext;
        private readonly ITokenService _tokenService;

        public AuthController(MyContext myContext, ITokenService tokenService)
        {
            _myContext = myContext ?? throw new ArgumentNullException(nameof(myContext));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] User loginModel)
        {
            if (loginModel is null)
            {
                return BadRequest("Invalid client request");
            }

            var user = _myContext.Users.FirstOrDefault(u => 
                (u.UserName == loginModel.UserName) && (u.Password == loginModel.Password));
            if (user is null)
                return Unauthorized();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginModel.UserName),
                new Claim(ClaimTypes.Role, "Manager")
            };
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            _myContext.SaveChanges();

            HttpContext.Response.Cookies.Append(
                "refresh-token",
                refreshToken,
                new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict }); 

            return Ok(new AuthenticatedResponse
            {
                Token = accessToken
            });
        }
    }
}
