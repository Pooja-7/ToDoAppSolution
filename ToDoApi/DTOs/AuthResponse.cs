public class AuthResponse
{
    public int? ErrorNumber { get; set; } // Use nullable type
    public string ErrorDescription { get; set; }
    public bool Success { get; set; }
    public string UserId { get; set; }


    // Constructor for success
    public AuthResponse(bool success, string userId)
    {
        Success = success;
        ErrorNumber = 0;
        ErrorDescription = "Registered Successfully";
        UserId = userId;
    }

    // Constructor for failure
    // Error numbers added as the responses on failure, will help better in troubleshooting.
    public AuthResponse(int errorNumber, string errorDescription)
    {
        Success = false;
        ErrorNumber = errorNumber;
        ErrorDescription = errorDescription;
    }
}
