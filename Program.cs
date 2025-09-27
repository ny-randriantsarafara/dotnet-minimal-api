using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoRepository>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();

var app = builder.Build();

var todosGroup = app.MapGroup("/todos");

todosGroup.MapGet("/", async ([FromServices] ITodoRepository todoRepository) => await todoRepository.GetTodos());

todosGroup.MapGet("/{id}", async (int id, [FromServices] ITodoRepository todoRepository) =>
{
    var todo = await todoRepository.GetTodoById(id);
    return todo is null ? Results.NotFound() : Results.Ok(todo);
});

todosGroup.MapPost("/", async (Todo todo, [FromServices] ITodoRepository todoRepository) =>
{
    var createdTodo = await todoRepository.CreateTodo(todo);
    return Results.Created($"/todos/{createdTodo?.Id}", createdTodo);
});

todosGroup.MapPut("/{id}", async (int id, Todo todo, [FromServices] ITodoRepository todoRepository) =>
{
    var updatedTodo = await todoRepository.UpdateTodo(id, todo);
    return updatedTodo is null ? Results.NotFound() : Results.Ok(updatedTodo);
});

todosGroup.MapDelete("/{id}", async (int id, [FromServices] ITodoRepository todoRepository) =>
{
    var deleted = await todoRepository.DeleteTodo(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.Run();
