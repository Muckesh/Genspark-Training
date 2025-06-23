using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationController(IAuthenticationService authenticationService,
                                    ILogger<AuthenticationService> logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<UserLoginResponse>> UserLogin(UserLoginRequest loginRequest)
    {
        try
        {
            var result = await _authenticationService.Login(loginRequest);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Unauthorized(e.Message);
        }
    }
}