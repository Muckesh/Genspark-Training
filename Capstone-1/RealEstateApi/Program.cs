using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RealEstateApi.Contexts;
using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Repositories;
using RealEstateApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Real Estate Api", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a token.",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id ="Bearer"
                }
            },
            new string[]{}
        }
    });
});

#region Controllers
builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    options.JsonSerializerOptions.WriteIndented = true;
                });
#endregion

#region DBContext
builder.Services.AddDbContext<RealEstateDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
#endregion

#region Repositories
builder.Services.AddTransient<IRepository<Guid,User>,UserRepository>();
builder.Services.AddTransient<IRepository<Guid,Agent>,AgentRepository>();
builder.Services.AddTransient<IRepository<Guid,Buyer>,BuyerRepository>();
builder.Services.AddTransient<IRepository<Guid,Inquiry>,InquiryRepository>();
builder.Services.AddTransient<IRepository<Guid,PropertyImage>,PropertyImageRepository>();
builder.Services.AddTransient<IRepository<Guid,PropertyListing>,PropertyListingRepository>();
#endregion

#region Services
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IPasswordService, PasswordService>();
builder.Services.AddTransient<IAgentService, AgentService>();
builder.Services.AddTransient<IBuyerService,BuyerService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
