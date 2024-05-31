using Chefster;
using Auth0.AspNetCore.Authentication;

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

builder.Services.AddAuth0WebAppAuthentication(options => 
{
    options.Domain = Environment.GetEnvironmentVariable("AUTH_DOMAIN");
    options.ClientId = Environment.GetEnvironmentVariable("AUTH_CLIENT_ID");
    options.CallbackPath = "/account/callback";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Index}/{action=Index}/{id?}");

app.Run();
