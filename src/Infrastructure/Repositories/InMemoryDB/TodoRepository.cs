using Microsoft.EntityFrameworkCore;

public class TodoRepository : DbContext, ITodoRepository
{
    private DbSet<Todo> Todos => Set<Todo>();

    public TodoRepository(DbContextOptions<TodoRepository> options) : base(options)
    {
    }

    public async Task<IEnumerable<Todo>> GetTodos()
    {
        return await Todos.ToListAsync();
    }

    public async Task<Todo?> GetTodoById(int id)
    {
        return await Todos.FindAsync(id);
    }

    public async Task<Todo?> CreateTodo(Todo todo)
    {
        var entity = await Todos.AddAsync(todo);
        await SaveChangesAsync();
        return entity.Entity;
    }

    public async Task<Todo?> UpdateTodo(int id, Todo todo)
    {
        var existingTodo = await Todos.FindAsync(id);
        if (existingTodo == null)
        {
            return null;
        }

        existingTodo.Title = todo.Title;
        existingTodo.IsCompleted = todo.IsCompleted;

        await SaveChangesAsync();
        return existingTodo;
    }

    public async Task<bool> DeleteTodo(int id)
    {
        var todo = await Todos.FindAsync(id);
        if (todo == null)
        {
            return false;
        }

        Todos.Remove(todo);
        await SaveChangesAsync();
        return true;
    }
}