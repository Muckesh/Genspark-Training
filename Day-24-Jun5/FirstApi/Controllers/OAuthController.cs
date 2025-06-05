using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

public class OAuthController : ControllerBase
{
    private readonly IRepository<string, User> _userRepository;
    private readonly ITokenService _tokenService;

    public OAuthController(IRepository<string, User> userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    [HttpGet("login-google")]
    public IActionResult LoginWithGoogle()
    {
        var properties = new AuthenticationProperties { RedirectUri = "/google-login-callback" };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-login-callback")]
    public async Task<IActionResult> GoogleResponse()
    {
        var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authResult.Succeeded) return Unauthorized("Google Authentication Failed.");

        var claimsPrincipal = authResult.Principal;
        var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        var name = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;

        var user = await _userRepository.Get(email);
            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Role = "Patient", 
                };

                await _userRepository.Add(user);
            }

            var jwtToken = await _tokenService.GenerateToken(user);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok(new { Name = name,Email = email,token = jwtToken });

    }
    
}