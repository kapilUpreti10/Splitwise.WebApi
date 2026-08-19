using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Splitwise.DataAccess;
using Splitwise.Models;
using Splitwise.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// aspnet core identity

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
     

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


// generating hash pas 


Console.WriteLine(PasswordHashGenerator.Generate("kapil@example.com"));
Console.WriteLine(PasswordHashGenerator.Generate("niraj@example.com"));
Console.WriteLine(PasswordHashGenerator.Generate("pratap@example.com"));
Console.WriteLine(PasswordHashGenerator.Generate("pariskar@example.com"));
Console.WriteLine(PasswordHashGenerator.Generate("parbat@example.com"));

app.Run();
