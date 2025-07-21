using Microsoft.EntityFrameworkCore;
using VideoPortalAPi.Contexts;
using VideoPortalAPi.Interfaces;
using VideoPortalAPi.Models;
using VideoPortalAPi.Repositories;
using VideoPortalAPi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


#region DBContext
builder.Services.AddDbContext<VideoPortalDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
#endregion

#region Repositories
builder.Services.AddTransient<IRepository<Guid,TrainingVideo>,TrainingVideoRepository>();
#endregion

#region Services
builder.Services.AddTransient<IVideoService,VideoService>();
#endregion

#region Cors
builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",  // Angular dev server
            "http://127.0.0.1:4200" // Alternative Angular URL
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

app.UseCors();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
