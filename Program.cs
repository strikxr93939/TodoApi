using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

using (var db = new AppDbContext())
{
    db.Database.EnsureCreated();
}
app.MapPost("/register", (RegisterRequest request) =>
{
   using var db = new AppDbContext();

   if (db.Users.Any(u => u.Username == request.Username))
        return Results.BadRequest("Пользователь уже существует."); 

    var user = new User
    {
      Username = request.Username,
      PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
    };

    db.Users.Add(user);
    db.SaveChanges();

    return Results.Ok("Регистрация успешна!");

});
app.MapPost("/login", (LoginRequest request) =>
{
    using var db = new AppDbContext();
    var user =db.Users.FirstOrDefault(u => u.Username == request.Username);
    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
    return Results.Unauthorized();

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkey12345supersecretkey123456"));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
      new Claim("userId", user.Id.ToString()),
      new Claim("username", user.Username)  
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddHours(24),
        signingCredentials: creds
    );

    string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new {token = tokenString});
});

app.MapPost("/todos", (TodoItem todo, HttpContext context) =>
{
   var userId = GetUserIdFromToken(context);
   if (userId == null) return Results.Unauthorized();

   using var db = new AppDbContext();
   todo.UserId = userId.Value;
   db.Todos.Add(todo);
   db.SaveChanges();

   return Results.Ok(todo); 
});

app.MapGet("/todos", (HttpContext context) =>
{
   var userId = GetUserIdFromToken(context);
   if (userId == null) return Results.Unauthorized();


   using var db = new AppDbContext();
   var todos = db.Todos.Where(t => t.UserId == userId.Value).ToList();

   return Results.Ok(todos); 
});

app.MapPut("/todos/{id}", (int id, TodoItem updated, HttpContext context) =>
{
    var userId = GetUserIdFromToken(context);
    if (userId == null) return Results.Unauthorized();

    using var db = new AppDbContext();
    var todo = db.Todos.FirstOrDefault(t => t.Id == id && t.UserId == userId.Value);

    if(todo == null) return Results.NotFound("Задача не найдена");

    todo.Title = updated.Title;
    todo.IsDone = updated.IsDone;
    db.SaveChanges();

    return Results.Ok(todo);
});

app.MapDelete("todos/{id}", (int id, HttpContext context) =>
{
    var userId = GetUserIdFromToken(context);
    if (userId == null) return Results.Unauthorized();

    using var db = new AppDbContext();
    var todo = db.Todos.FirstOrDefault(t => t.Id == id && t.UserId == userId.Value);

    if(todo == null) return Results.NotFound("Задача не найдена");

    db.Todos.Remove(todo);
    db.SaveChanges();

    return Results.Ok("Задача удалена");
});

app.Run();

int? GetUserIdFromToken(HttpContext context)
{
    var authHeader = context.Request.Headers["Authorization"].ToString();
    if(!authHeader.StartsWith("Bearer "))
        return null;

    var token = authHeader.Substring(7);
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkey12345supersecretkey123456"));

    var handler = new JwtSecurityTokenHandler();
    var principal = handler.ValidateToken(token, new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateIssuer = false,
        ValidateAudience = false
    }, out _);

    var userId = principal.FindFirst("userId")?.Value;
    return userId != null ? int.Parse(userId) : null;
}


public record LoginRequest(string Username, string Password);
public record RegisterRequest(string Username, string Password);