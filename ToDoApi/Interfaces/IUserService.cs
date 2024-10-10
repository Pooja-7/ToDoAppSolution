public interface IUserService
{
    Task<ServiceResponse<List<User>>> GetAllUsersAsync();
}
