using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Clinic Api", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a token.",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{ }
        }
    });
});

builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

builder.Services.AddDbContext<ClinicContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

#region AuthorizationPolicy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ExperiencedDocOnly", policy =>
    policy.Requirements.Add(new MinimumExperienceRequirement(3)));
});

builder.Services.AddTransient<IAuthorizationHandler, MinimumExperienceHandler>();
#endregion

#region Repositories
builder.Services.AddTransient<IRepository<int, Doctor>, DoctorRepository>();
builder.Services.AddTransient<IRepository<int,Patient>,PatientRepository>();
builder.Services.AddTransient<IRepository<int, Speciality>, SpecialityRepository>();
builder.Services.AddTransient<IRepository<int, Appointment>, AppointmentRepository>();
builder.Services.AddTransient<IRepository<int, DoctorSpeciality>, DoctorSpecialityRepository>();
builder.Services.AddTransient < IRepository<string, User>,UserRepository>();
#endregion

#region Services
builder.Services.AddTransient<IDoctorService, DoctorService>();
builder.Services.AddTransient<IPatientService,PatientService>();
// builder.Services.AddTransient<IDoctorService,DoctorServiceWithTransaction>();
builder.Services.AddTransient<IOtherContextFunctionalities, OtherFunctionalityImplementation>();
builder.Services.AddTransient<IEncryptionService, EncryptionService>();
builder.Services.AddTransient<ITokenService,TokenService>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IAppointmentService,AppointmentService>();
#endregion

#region AuthenticationFilter
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Keys:JwtTokenKey"]))
                    };
                });
#endregion

#region Misc
builder.Services.AddAutoMapper(typeof(User));
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
