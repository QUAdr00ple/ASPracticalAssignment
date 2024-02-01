using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using WebApplication3.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
	// Lockout settings
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); 
	options.Lockout.MaxFailedAccessAttempts = 3; 
	options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache(); //save session in memory
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(60);
});

builder.Services.ConfigureApplicationCookie(Config =>
{
    Config.LoginPath = "/Login";
});

builder.Services.AddDataProtection();
builder.Services.AddTransient<ImageController>();

builder.Services.AddLogging(logging =>
{
	logging.AddConsole();  
});

builder.Services.AddScoped<AuditLogService>();
builder.Services.AddScoped<PasswordHistoryService>();
builder.Services.AddTransient<EmailService>();

builder.Services.AddDataProtection()
    .SetApplicationName("BookWorm");

builder.Services.AddScoped<IDataProtector>(provider =>
{
    var dataProtectionProvider = provider.GetRequiredService<IDataProtectionProvider>();
    return dataProtectionProvider.CreateProtector("B00kw0rm");
});

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

app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
