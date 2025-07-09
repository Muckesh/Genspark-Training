using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RealEstateApi.Contexts;
using RealEstateApi.Hubs;
using RealEstateApi.Interfaces;
using RealEstateApi.Middlewares;
using RealEstateApi.Models;
using RealEstateApi.Repositories;
using RealEstateApi.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
            .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
            .Enrich.FromLogContext()
            .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5011); 
});

builder.Host.UseSerilog();
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

builder.Services.AddSignalR();


#region Controllers
builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    options.JsonSerializerOptions.WriteIndented = true;
                });
#endregion

builder.Services.AddHttpContextAccessor();


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
builder.Services.AddTransient<IRepository<Guid,InquiryReply>,InquiryReplyRepository>();
builder.Services.AddTransient<IRepository<Guid,PropertyImage>,PropertyImageRepository>();
builder.Services.AddTransient<IRepository<Guid,PropertyListing>,PropertyListingRepository>();
#endregion

#region Services
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IPasswordService, PasswordService>();
builder.Services.AddTransient<IUserService,UserService>();
builder.Services.AddTransient<IAgentService, AgentService>();
builder.Services.AddTransient<IBuyerService, BuyerService>();
builder.Services.AddTransient<IInquiryService, InquiryService>();
builder.Services.AddTransient<IPropertyImageService, PropertyImageService>();
builder.Services.AddTransient<IPropertyListingService, PropertyListingService>();
builder.Services.AddTransient<IImageCleanupService,ImageCleanUpService>();
builder.Services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();
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

                    // Add this for SignalR JWT support
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            
                            if (!string.IsNullOrEmpty(accessToken) && 
                                path.StartsWithSegments("/hubs/inquiries"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
#endregion

#region RateLimiting
builder.Services.AddRateLimiter(options=>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("PerUserPolicy",context=>
    {
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? context.Connection.RemoteIpAddress?.ToString();

        return RateLimitPartition.GetTokenBucketLimiter(userId ?? "anonymous", _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 1000,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0,
            ReplenishmentPeriod = TimeSpan.FromHours(1),
            TokensPerPeriod = 1000,
            AutoReplenishment=true
        });
    });
});
#endregion

#region Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version")
        );
});

builder.Services.AddVersionedApiExplorer(options=>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
#endregion

#region Cors
builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",  // Angular dev server
            "http://127.0.0.1:4200", // Alternative Angular URL
            "http://127.0.0.1:5500" 
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
#endregion



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRateLimiter();

app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\": \"Too many requests. Try again later.\"}");
    }
});


app.UseAuthentication();
app.UseMiddleware<TokenBlacklistMiddleware>();
app.UseAuthorization();
app.UseCors();
app.UseWebSockets();
app.MapHub<NotificationHub>("/hubs/notifications");

app.MapHub<InquiryHub>("/hubs/inquiries");

app.MapControllers().RequireRateLimiting("PerUserPolicy");

app.Run();
