using Auth0.AspNetCore.Authentication;
using Chefster;
using Chefster.Context;
using Chefster.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

try
{
    var path = Path.Combine(builder.Environment.ContentRootPath, ".env");
    DotEnv.Load(path);
}
catch (Exception ex)
{
    Console.WriteLine($"Error loading .env file: {ex.Message}");
}

var domain = Environment.GetEnvironmentVariable("AUTH_DOMAIN");
var clientId = Environment.GetEnvironmentVariable("AUTH_CLIENT_ID");
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    if (domain != null && clientId != null)
    {
        options.Domain = domain;
        options.ClientId = clientId;
    }
});

var connString = Environment.GetEnvironmentVariable("SQL_CONN_STR");
builder.Services.AddDbContext<ChefsterDbContext>(options =>
{
    options.UseSqlServer(connString);
});

builder.Services.AddScoped<FamilyService>();
builder.Services.AddScoped<MemberService>();
builder.Services.AddScoped<NoteService>();
builder.Services.AddControllers();

builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chefster Backend", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

var isProd = Environment.GetEnvironmentVariable("IS_PROD");
if (isProd == "false")
{
    Console.WriteLine("WE ARE IN DEVELOPMENT!");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chefster Backend");
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Index}/{action=Index}/{id?}");

app.Run();
