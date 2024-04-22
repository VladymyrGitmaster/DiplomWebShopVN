using DiplomWebShopVN.Models.Domain;
using DiplomWebShopVN.Repositories.Abstract;
using DiplomWebShopVN.Repositories.Implementation;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Connect to DataBase add Service
builder.Services.AddDbContext<DatabaseContext>(options => options
.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlDbConnStr")));
// For Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(option =>
        option.LoginPath = "/UserAuthentication/Login");
//[Google Authentication
var googleAuthNSection = builder.Configuration.GetSection("GoogleAuthentication");
builder.Services.Configure<GoogleOptions>(googleAuthNSection);
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = googleAuthNSection["ClientId"];
    options.ClientSecret = googleAuthNSection["ClientSecret"];
});
//Google Authentication]
//==============//Transient, Scoped, Singleton???
//AddTransient Transient - передбачає, що сервіс створюється щоразу, коли його запитують. Цей життєвий цикл найкраще підходить для легковажних сервісів, що не фіксують стан.
//AddScoped Scoped - сервіс створюються один раз для кожного запиту.
//AddSingleton Singleton - сервіс створюється при першому запиті (або при запуску ConfigureServices, якщо ви вказуєте інстанс там), а потім кожен наступний запит використовуватиме той же інстанс.
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddTransient<IAllProducts, ProductRepository>();
builder.Services.AddTransient<IProductCategory, CategoryRepository>();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseSession();
app.UseRouting();
app.UseStatusCodePages();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapRazorPages();
app.Run();
