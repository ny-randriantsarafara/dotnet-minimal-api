using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("ApiKeyScheme")
    .AddScheme<ApiKeyAuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKeyScheme", "API Key Authentication", options => { options.ApiKeyHeaderName = "X-API-KEY"; });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<TodoRepository>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<ITodoRepository, TodoRepository>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

var todosGroup = app.MapGroup("/todos");

todosGroup.MapGet("/", async ([FromServices] ITodoRepository todoRepository) => await todoRepository.GetTodos()).RequireAuthorization();

todosGroup.MapGet("/{id}", async (int id, [FromServices] ITodoRepository todoRepository) =>
{
    var todo = await todoRepository.GetTodoById(id);
    return todo is null ? Results.NotFound() : Results.Ok(todo);
}).RequireAuthorization();

todosGroup.MapPost("/", async (Todo todo, [FromServices] ITodoRepository todoRepository) =>
{
    var createdTodo = await todoRepository.CreateTodo(todo);
    return Results.Created($"/todos/{createdTodo?.Id}", createdTodo);
}).RequireAuthorization();

todosGroup.MapPut("/{id}", async (int id, Todo todo, [FromServices] ITodoRepository todoRepository) =>
{
    var updatedTodo = await todoRepository.UpdateTodo(id, todo);
    return updatedTodo is null ? Results.NotFound() : Results.Ok(updatedTodo);
}).RequireAuthorization();

todosGroup.MapDelete("/{id}", async (int id, [FromServices] ITodoRepository todoRepository) =>
{
    var deleted = await todoRepository.DeleteTodo(id);
    return deleted ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

app.Run();
