using MCServerManager.Service.HostedService;
using MCServerManager.Service;
using Microsoft.AspNetCore.HttpOverrides;
using MCServerManager.Library.Data.Data;
using MCServerManager.Library.Data.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Settings.json");
builder.Configuration.AddJsonFile("StyleSettings.json");

builder.Services.AddSingleton<IGameServerDataContext, GameServerDataFileRepository>();
builder.Services.AddSingleton<GameServerService>();
builder.Services.AddHostedService<RunGameServersHostedService>();

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
