using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class BaseController : ControllerBase
{
    protected User CurrentUser => HttpContext.Items["User"] as User;

}