using Microsoft.AspNetCore.Authorization;

public class MinimumExperienceRequirement : IAuthorizationRequirement
{
    public float MinYears { get; set; }

    public MinimumExperienceRequirement(float years)
    {
        MinYears = years;
    }
}
