using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;


var builder = WebApplication.CreateBuilder();




// условная бд с пользователями
var people = new List<Person>
{
    new Person("tom@gmail.com", "12345"),
    new Person("bob@gmail.com", "55555")
};

// аутентификация с помощью куки
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseStaticFiles();
app.UseAuthentication();   // добавление middleware аутентификации 
app.UseAuthorization();   // добавление middleware авторизации 

app.MapGet("/login", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    var response = context.Response;
    var request = context.Request;
    await response.SendFileAsync("Authorization.html");
});
app.MapGet("/index", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    var response = context.Response;
    var request = context.Request;
    await response.SendFileAsync("index.html");
});

app.MapPost("/login", async (string? returnUrl, HttpContext context) =>
{
    // получаем из формы email и пароль
    var form = context.Request.Form;
    // если email и/или пароль не установлены, посылаем статусный код ошибки 400
    if (!form.ContainsKey("email") || !form.ContainsKey("password"))
        return Results.BadRequest("Email и/или пароль не установлены");

    string email = form["email"];
    string password = form["password"];
    //string con = @"Data Source=PC-HUMKA\\SQLEXPRESS;Initial Catalog=contest;Integrated Security=True";
    //SqlDataAdapter sda1 = new SqlDataAdapter("SELECT logg, pas FROM Users", con);
    //DataTable dt1 = new DataTable();
    //sda1.Fill(dt1);
    //var v = dt1.Rows[0][0];
    //var v1 = dt1.Rows[0][1];
    //people = new List<Person>
    //{
    //    new Person(v.ToString(), v1.ToString())
    //};
    // находим пользователя 
    Person? person = people.FirstOrDefault(p => p.Email == email && p.Password == password);
    // если пользователь не найден, отправляем статусный код 401
    if (person is null) return Results.Unauthorized();

    var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Email) };
    // создаем объект ClaimsIdentity
    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    // установка аутентификационных куки
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
    return Results.Redirect(returnUrl ?? "/");
});

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.Map("/", [Authorize] () =>
{
	return Results.Redirect("/index");
});

app.Run();

record class Person(string Email, string Password);