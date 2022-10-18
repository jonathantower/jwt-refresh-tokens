using AngularClient.Models;
using AngularClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AngularClient.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly MyContext _userContext;
        private readonly ITokenService _tokenService;

        public TokenController(MyContext userContext, ITokenService tokenService)
        {
            this._userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            this._tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(TokenApiModel tokenApiModel)
        {
            if (tokenApiModel is null)
                return BadRequest("Invalid client request");

            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = HttpContext.Request.Cookies["refresh-token"];

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default

            var user = _userContext.Users.SingleOrDefault(u => u.UserName == username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid client request");

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            _userContext.SaveChanges();

            HttpContext.Response.Cookies.Append(
                "refresh-token",
                newRefreshToken,
                new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });

            return Ok(new AuthenticatedResponse()
            {
                Token = newAccessToken
            });
        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;

            var user = _userContext.Users.SingleOrDefault(u => u.UserName == username);
            if (user == null) return BadRequest();

            user.RefreshToken = null;

            _userContext.SaveChanges();

            return NoContent();
        }
    }
}
