using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrailerTrack.Application;
using TrailerTrack.Application.Interfaces;
using TrailerTrack.Infrastructure;
using TrailerTrack.Infrastructure.Identity;
using TrailerTrack.Infrastructure.Persistence;
using TrailerTrack.Web.Components;
using TrailerTrack.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddHttpContextAccessor();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(userManager, roleManager, db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorPages(); 
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapFallback(async context =>
{
    context.Response.Redirect("/not-found");
    await Task.CompletedTask;
});

app.Run();
