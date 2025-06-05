public interface IAuthenticationService
{
    Task<UserLoginResponseDto> Login(UserLoginRequestDto user);
}