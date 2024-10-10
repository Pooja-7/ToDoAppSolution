using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ToDoApi.Services
{

    public class ToDoService : IToDoService
    {
        private readonly ToDoContext _context;

        public ToDoService(ToDoContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<IEnumerable<ToDoItem>>> GetAllToDoItemsAsync()
        {
            try
            {
                var items = await _context.ToDoItems.ToListAsync(); // Fetch all items without user filtering

                return new ServiceResponse<IEnumerable<ToDoItem>>
                {
                    Success = true,
                    Data = items,
                    ErrorDescription = "Success"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<ToDoItem>>
                {
                    Success = false,
                    ErrorNumber = 300,
                    ErrorDescription = "Failed to retrieve ToDo items. " + ex.Message
                };
            }
        }


        public async Task<ServiceResponse<IEnumerable<ToDoItem>>> GetAllToDoItemsByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new ServiceResponse<IEnumerable<ToDoItem>>
                {
                    Success = false,
                    ErrorNumber = 400,
                    ErrorDescription = "User ID cannot be null or empty."
                };
            }

            try
            {
                // Fetch items that belong to the specified user
                var items = await _context.ToDoItems
                                          .Where(item => item.UserId == userId)
                                          .ToListAsync();

                return new ServiceResponse<IEnumerable<ToDoItem>>
                {
                    Success = true,
                    Data = items,
                    ErrorDescription = "Success"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<ToDoItem>>
                {
                    Success = false,
                    ErrorNumber = 300,
                    ErrorDescription = "Failed to retrieve ToDo items. " + ex.Message
                };
            }
        }


        public async Task<ServiceResponse<ToDoItem>> GetToDoItemByIdAsync(int id)
        {
            try
            {
                var item = await _context.ToDoItems.FindAsync(id); // Fetch specific item without user filtering

                if (item == null)
                {
                    return new ServiceResponse<ToDoItem>
                    {
                        Success = false,
                        ErrorNumber = 301,
                        ErrorDescription = $"ToDo item with ID {id} not found."
                    };
                }

                return new ServiceResponse<ToDoItem>
                {
                    Success = true,
                    Data = item
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ToDoItem>
                {
                    Success = false,
                    ErrorNumber = 302,
                    ErrorDescription = "Failed to retrieve ToDo item. " + ex.Message
                };
            }
        }

        public async Task<ServiceResponse<ToDoItem>> CreateToDoItemAsync(ToDoItem toDoItem)
        {
            try
            {
                _context.ToDoItems.Add(toDoItem);
                await _context.SaveChangesAsync();

                return new ServiceResponse<ToDoItem>
                {
                    Success = true,
                    Data = toDoItem
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ToDoItem>
                {
                    Success = false,
                    ErrorNumber = 303,
                    ErrorDescription = "Failed to create ToDo item. " + ex.Message
                };
            }
        }

        public async Task<ServiceResponse<ToDoItem>> UpdateToDoItemAsync(string userId, int id, ToDoItem toDoItem)
        {
            // Validate ID and UserId before proceeding
            if (id != toDoItem.Id)
            {
                return new ServiceResponse<ToDoItem>
                {
                    Success = false,
                    ErrorNumber = 304,
                    ErrorDescription = "ID mismatch. Cannot update ToDo item."
                };
            }

            try
            {
                // Find the existing ToDoItem
                var existingItem = await _context.ToDoItems.FindAsync(id);
                if (existingItem == null)
                {
                    return new ServiceResponse<ToDoItem>
                    {
                        Success = false,
                        ErrorNumber = 301,
                        ErrorDescription = "ToDo item not found."
                    };
                }

                // Check if the UserId matches before updating
                if (existingItem.UserId != userId)
                {
                    return new ServiceResponse<ToDoItem>
                    {
                        Success = false,
                        ErrorNumber = 302,
                        ErrorDescription = "User ID does not match. Cannot update ToDo item."
                    };
                }

                // Update the properties of the existing item
                existingItem.Title = toDoItem.Title;
                existingItem.IsCompleted = toDoItem.IsCompleted;
                // Update other fields as necessary

                // Mark the entity as modified
                _context.Entry(existingItem).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return new ServiceResponse<ToDoItem>
                {
                    Success = true,
                    ErrorNumber = 0,
                    ErrorDescription = "Sucess"
                };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new ServiceResponse<ToDoItem>
                {
                    Success = false,
                    ErrorNumber = 305,
                    ErrorDescription = "Concurrency error during update. " + ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ToDoItem>
                {
                    Success = false,
                    ErrorNumber = 306,
                    ErrorDescription = "Failed to update ToDo item. " + ex.Message
                };
            }
        }


        public async Task<ServiceResponse<ToDoItem>> DeleteToDoItemByUserIdAsync(string userId, int id)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new ServiceResponse<ToDoItem>
                {
                    Success = false,
                    ErrorNumber = 400,
                    ErrorDescription = "User ID cannot be null or empty."
                };
            }

            try
            {
                var toDoItem = await _context.ToDoItems.FindAsync(id);

                if (toDoItem == null)
                {
                    return new ServiceResponse<ToDoItem>
                    {
                        Success = false,
                        ErrorNumber = 307,
                        ErrorDescription = "ToDo item not found."
                    };
                }

                // Check if the ToDoItem belongs to the specified user
                if (toDoItem.UserId != userId)
                {
                    return new ServiceResponse<ToDoItem>
                    {
                        Success = false,
                        ErrorNumber = 308,
                        ErrorDescription = "User ID does not match. Cannot delete ToDo item."
                    };
                }

                // Remove the ToDoItem
                _context.ToDoItems.Remove(toDoItem);
                await _context.SaveChangesAsync();

                return new ServiceResponse<ToDoItem>
                {
                    Success = true,
                    ErrorNumber = 0,
                    ErrorDescription = "Deleted Successfully"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ToDoItem>
                {
                    Success = false,
                    ErrorNumber = 309,
                    ErrorDescription = "Failed to delete ToDo item. " + ex.Message
                };
            }
        }


    }
}





