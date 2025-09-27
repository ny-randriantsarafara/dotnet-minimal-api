public interface ITodoRepository
{
    Task<IEnumerable<Todo>> GetTodos();
    Task<Todo?> GetTodoById(int id);
    Task<Todo?> CreateTodo(Todo todo);
    Task<Todo?> UpdateTodo(int id, Todo todo);
    Task<bool> DeleteTodo(int id);
}
