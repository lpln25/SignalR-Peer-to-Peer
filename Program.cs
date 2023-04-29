using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebAppSaba.Contexts;
using WebAppSaba.Hubs;
using WebAppSaba.Models.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var mvcBuilder = builder.Services.AddControllersWithViews();
#if DEBUG
mvcBuilder.AddRazorRuntimeCompilation();
#endif
builder.Services.AddSignalR();
var conntecionstring = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<DatabaseContext>(options =>
options.UseSqlServer(conntecionstring));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatMessageService, ChatMessageService>();
builder.Services.AddScoped<IChatFileService, ChatFileService>();
builder.Services.AddScoped<IChatHistoryService, ChatHistoryService>();
builder.Services.AddSingleton<IOnlineUserService, OnlineUserService>();

builder.Services.AddAuthentication(options =>
options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{ options.LoginPath = "/Home/Login"; });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.MapHub<ChatHub>("/chatroomhub");

app.Run();
