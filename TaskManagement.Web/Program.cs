using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddSession(options =>
{
    //options.IdleTimeout = TimeSpan.FromMinutes(1); // ⏱ 1 minute
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddHttpClient("TaskApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7202/api/");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();

    if (!path.Contains("/account") &&
        context.Session.GetString("token") == null)
    {
        context.Response.Redirect("/Account/Login");
        return;
    }
    await next();
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
