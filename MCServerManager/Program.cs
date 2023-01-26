using MCServerManager.Data;
using MCServerManager.Service.HostedService;
using MCServerManager.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using MCServerManager.Library.Data.Data;
using MCServerManager.Library.Data.Interface;
using MCServerManager.Data.FilterAttributes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Configuration.AddJsonFile("Settings.json");
builder.Configuration.AddJsonFile("StyleSettings.json");

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserServerAccessFilter>();
builder.Services.AddScoped<UserServiceAccessFilter>();

builder.Services.AddSingleton<IGameServerDataContext, GameServerDataFileRepository>();
builder.Services.AddSingleton<IGameServerDataContext, ServerDataRepository>();
builder.Services.AddSingleton<GameServerService>();
builder.Services.AddHostedService<RunGameServersHostedService>();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddControllers();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapRazorPages();
	endpoints.MapControllers();
	endpoints.MapControllerRoute(
		name: "console",
		pattern: "{controller}/{id:guid}/{action}");
});

app.Run();
