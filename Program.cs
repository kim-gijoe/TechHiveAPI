using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// --- Swagger setup ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// ----------------------

var app = builder.Build();

// --- Enable Swagger UI ---
app.UseSwagger();
app.UseSwaggerUI();
// --------------------------

// 1. Error handling
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] {ex.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = "Internal server error." });
    }
});

// 2. Authentication
var validToken = "mysecrettoken";

app.Use(async (context, next) =>
{
    if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Missing Authorization header");
        return;
    }

    var token = authHeader.ToString().Replace("Bearer ", "");

    if (token != validToken)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Invalid token");
        return;
    }

    await next();
});

// 3. Logging
app.Use(async (context, next) =>
{
    var method = context.Request.Method;
    var path = context.Request.Path;

    await next();

    var status = context.Response.StatusCode;

    Console.WriteLine($"[LOG] {method} {path} → {status}");
});


// In-memory user list
var users = new List<User>();

app.MapGet("/users", (int? skip, int? take) =>
{
    var query = users.AsQueryable();

    int skipValue = skip ?? 0;
    int takeValue = take ?? 50; // default page size

    var result = query.Skip(skipValue).Take(takeValue).ToList();

    return Results.Ok(result);
});

// GET: Retrieve a specific user by ID
app.MapGet("/users/{id}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

// POST: Add a new user
app.MapPost("/users", (User newUser) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(newUser.UserName))
        {
            return Results.BadRequest("UserName is required.");
        }

        if (newUser.Age <= 0)
        {
            return Results.BadRequest("Age must be greater than 0.");
        }

        newUser.Id = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;
        users.Add(newUser);

        return Results.Created($"/users/{newUser.Id}", newUser);
    }
    catch (Exception ex)
    {
        // In a real app, you'd log this
        Console.WriteLine($"Error in POST /users: {ex.Message}");
        return Results.StatusCode(500);
    }
});


// PUT: Update an existing user
app.MapPut("/users/{id}", (int id, User updatedUser) =>
{
    if (string.IsNullOrWhiteSpace(updatedUser.UserName))
    {
        return Results.BadRequest("UserName is required.");
    }

    if (updatedUser.Age <= 0)
    {
        return Results.BadRequest("Age must be greater than 0.");
    }

    var existing = users.FirstOrDefault(u => u.Id == id);
    if (existing is null)
        return Results.NotFound();

    existing.UserName = updatedUser.UserName;
    existing.Age = updatedUser.Age;

    return Results.Ok(existing);
});

// DELETE: Remove a user by ID
app.MapDelete("/users/{id}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null)
        return Results.NotFound();

    users.Remove(user);
    return Results.NoContent();
});

app.Run();

// User model
public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Age { get; set; }
}

