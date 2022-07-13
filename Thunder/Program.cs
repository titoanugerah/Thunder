using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Thunder.DataAccess;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
services
    .AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddDbContext<ThunderDB>();

services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.LoginPath = configuration["Authentication:LoginPath"];
}).AddGoogle(googleOptions =>
{
    googleOptions.ClientId = configuration["Authentication:ClientId"];
    googleOptions.ClientSecret = configuration["Authentication:ClientSecret"];
    googleOptions.Scope.Add("profile");
    googleOptions.Events.OnCreatingTicket = (context) =>
    {
        string picture = context.User.GetProperty("picture").GetString();
        context.Identity.AddClaim(new Claim("picture", picture));
        return Task.CompletedTask;
    };
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseStatusCodePagesWithRedirects("/Error/{0}"); 
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
