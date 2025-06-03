using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


public class MinimumExperienceHandler : AuthorizationHandler<MinimumExperienceRequirement>
{
    private readonly IRepository<int, Doctor> _doctorRepository;
    public MinimumExperienceHandler(IRepository<int,Doctor> doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumExperienceRequirement requirement)
    {
        // var email = context.User.Identity?.Name;
        var email = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(email))
        {
            context.Fail();
            return;
        }
        var doctors = await _doctorRepository.GetAll();
        // var doctor = doctors.FirstOrDefault(d => d.Email == email);
        var doctor = doctors.FirstOrDefault(d => d.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (doctor != null && doctor.YearsOfExperience >= requirement.MinYears)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}