using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TimesheetPro.Core.Domain.IdentityEntities;
using TimesheetPro.Core.Serivces;
using TimesheetPro.Infrastructure.Data;
using TimesheetPro.UI.Seed;

var builder = WebApplication.CreateBuilder(args);



//Database connection
builder.Services.AddDbContext<TimesheetProDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        //password rule(should be true in the production env)
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredUniqueChars = 3;
    })
    .AddEntityFrameworkStores<TimesheetProDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();

//Service
builder.Services.AddScoped<TimesheetService>();

var app = builder.Build();

//Identity Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeedData.SeedAsync(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//enable authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Project}/{action=Index}/{id?}");

app.Run();
