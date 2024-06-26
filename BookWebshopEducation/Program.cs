using Microsoft.EntityFrameworkCore;

using BookWebshopEducation.DataAccess.Data;
using BookWebshopEducation.DataAccess.Repository.IRepository;
using BookWebshopEducation.DataAccess.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using BookWebshopEducation.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Add DbContext as a service with connection string from appsettings.json
//builder.Services.AddDbContext<ApplicationDbContext>(options => 
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
//        sqlServerOptionsAction: sqlOptions =>
//        {
//            sqlOptions.EnableRetryOnFailure();
//        }));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.Configure<StripeSetting>(builder.Configuration.GetSection("Stripe"));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Use https instead http
app.UseHttpsRedirection();

// Adds all wwwroot files
app.UseStaticFiles();

// Adds routing
// https://localhost:55555/Category/Index 
// https://localhost:55555/Category
// https://localhost:55555/Category/Edit/3
// https://localhost:55555/Product/Details/3
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

// This is something we will work on later


// Default route - if nothing is defined go to Home/Index/..
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

// Runs the project
app.Run();

// update-database -> create a database without any migration at beginning 
// add-migration -> create a migration 
// update-database -> after add-migration needed to actually update the db