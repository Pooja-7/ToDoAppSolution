using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly ToDoContext _context;

    public UserService(ToDoContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<List<User>>> GetAllUsersAsync()
    {
        var response = new ServiceResponse<List<User>>();
        try
        {
            response.Data = await _context.Users.ToListAsync();
            response.Success = true;
            response.ErrorDescription = "Success"; // Set the success message here
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorDescription = "An error occurred while retrieving users."; // Optionally set an error message
        }
        
        return response;
    }
}
