using bookShoop.Application_Data;
using bookShoop.Data.DataSeeder;
using BookShopping.Dto.Pyment;
using BookShopping.Infrustructure;
using BookShopping.Infrustructure.Abstruct;
using BookShopping.Infrustructure.Repositoreis;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
// Configure authentication with Cookie and Google
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//})
//.AddCookie()
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbcontext")));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // 👈 إضافة دعم الأدوار
    .AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddIdentity<IdentityUser, IdentityRole>(option => option.SignIn.RequireConfirmedAccount = false)
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//   .AddDefaultTokenProviders();

#region Add Servues Injaction
builder.Services.moduleInfrustructureDependencies();


builder.Services.AddSingleton<IEmailSender, FakeEmailSender>();

#endregion
//Stripe
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
Stripe.StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var userManger = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManger = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedAsync(roleManger);
    await UserSeeder.SeedAsync(userManger);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapRazorPages();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
