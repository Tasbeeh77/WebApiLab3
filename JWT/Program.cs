using JWT.Data.Context;
using JWT.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region Default config
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region DB Config
builder.Services.AddDbContext<CompanyContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Company")));
#endregion

#region Identity config

builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<CompanyContext>();

#endregion

#region Authentication config

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "cool";
    options.DefaultChallengeScheme = "cool";
}).AddJwtBearer("cool", options =>
{
    var stringKey = builder.Configuration.GetValue<string>("secretkey") ?? string.Empty;
    var bytesKey = Encoding.ASCII.GetBytes(stringKey);
    var key = new SymmetricSecurityKey(bytesKey);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = key,
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

#endregion

#region Authorization

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy
        .RequireClaim(ClaimTypes.Role, "Manager", "CEO")
        .RequireClaim(ClaimTypes.NameIdentifier));

    options.AddPolicy("users", policy => policy
        .RequireClaim(ClaimTypes.Role, "User","Employee", "Manager", "CEO")
        .RequireClaim(ClaimTypes.NameIdentifier));
});

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
