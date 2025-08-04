using System.Text;
using Ecommerce.Contexts;
using Ecommerce.Interfaces;
using Ecommerce.Middlewares;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Ecommerce.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecommerce Api", Version = "v1" });
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

#region DbContext
builder.Services.AddDbContext<EcommerceDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
#endregion

#region Repositories
builder.Services.AddScoped<IRepository<int, Category>, CategoryRepository>();
builder.Services.AddScoped<IRepository<int, Color>, ColorRepository>();
builder.Services.AddScoped<IRepository<int, ContactUs>, ContactUsRepository>();
builder.Services.AddScoped<IRepository<int, Model>, ModelRepository>();
builder.Services.AddScoped<IRepository<int, News>, NewsRepository>();
builder.Services.AddScoped<IRepository<int, Order>, OrderRepository>();
builder.Services.AddScoped<IRepository<int, OrderDetail>, OrderDetailRepository>();
builder.Services.AddScoped<IRepository<int, Product>, ProductRepository>();
builder.Services.AddScoped<IRepository<int, User>, UserRepository>();

#endregion

#region Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IColorService,ColorService>();
builder.Services.AddScoped<IModelService, ModelService>();
builder.Services.AddScoped<IContactUsService, ContactUsService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IPasswordService,PasswordService>();
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddSingleton<ITokenBlacklistService,TokenBlacklistService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
// builder.Services.AddScoped<IPaypalService, PaypalService>();
// builder.Services.AddSingleton<PaypalConfig>();


#endregion

builder.Services.Configure<PaypalSettings>(builder.Configuration.GetSection("PayPal"));
// builder.Services.AddSingleton(resolver =>
//     resolver.GetRequiredService<IOptions<PaypalSettings>>().Value);
builder.Services.AddHttpClient<PaypalService>();
// builder.Services.AddHttpClient();

#region Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


// builder.Services.AddHttpClient();

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;


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

#region Controllers
builder.Services.AddControllers();
                // .AddJsonOptions(options =>
                // {
                //     options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                //     options.JsonSerializerOptions.WriteIndented = true;
                // });
#endregion

// Http context
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors();
app.UseAuthentication();
app.UseMiddleware<TokenBlacklistMiddleware>();
app.UseAuthorization();


app.MapControllers();


app.Run();

