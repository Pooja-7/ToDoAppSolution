using System.Collections.Generic;
using System.Threading.Tasks;

public interface IToDoService
{
    Task<ServiceResponse<ToDoItem>> GetToDoItemByIdAsync(int id);
    Task<ServiceResponse<ToDoItem>> CreateToDoItemAsync(ToDoItem toDoItem); 
    Task<ServiceResponse<ToDoItem>> UpdateToDoItemAsync(string userId, int id, ToDoItem toDoItem);
    Task<ServiceResponse<ToDoItem>> DeleteToDoItemByUserIdAsync( string userId, int id); 
    Task<ServiceResponse<IEnumerable<ToDoItem>>> GetAllToDoItemsByUserIdAsync(string userId);
}
