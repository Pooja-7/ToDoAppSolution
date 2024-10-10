using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ToDoController : BaseController // Inherit from BaseController
{
    private readonly IToDoService _toDoService;

    public ToDoController(IToDoService toDoService)
    {
        _toDoService = toDoService;
    }

    // GET: api/ToDo/getAllByUserId/{userId}
    // Retrieve all ToDo items for a specific user
    [HttpGet("getAllByUserId/{userId}")]
    public async Task<ActionResult<ServiceResponse<IEnumerable<ToDoItem>>>> GetAllToDosByUserId(string userId)
    {
        // Call the service method to get ToDo items by UserId
        var response = await _toDoService.GetAllToDoItemsByUserIdAsync(userId);

        if (!response.Success)
        {
            return StatusCode(500, response); // Return 500 Internal Server Error with the error response
        }

        return Ok(response); // Return 200 OK with the list of ToDo items
    }


    // GET: api/ToDo/getById/{id}
    // Retrieve a specific ToDo item by ID
    [HttpGet("getById/{id}")]
    public async Task<ActionResult<ServiceResponse<ToDoItem>>> GetToDoById(int id)
    {
        var response = await _toDoService.GetToDoItemByIdAsync(id); // Fetch specific item
        if (!response.Success)
        {
            if (response.ErrorNumber == 301)
                return NotFound(response); // Return 404 Not Found if item doesn't exist
            return StatusCode(500, response); // Return 500 for other errors
        }
        return Ok(response); // Return 200 OK with the specific ToDo item
    }

    // POST: api/ToDo/create
    // Create a new ToDo item
    [HttpPost("create")]
    public async Task<ActionResult<ServiceResponse<ToDoItem>>> CreateToDoItem([FromBody] ToDoItem toDoItem)
    {
        var response = await _toDoService.CreateToDoItemAsync(toDoItem); // Create item
        if (!response.Success)
        {
            return StatusCode(500, response); // Return 500 Internal Server Error for any failure
        }
        return CreatedAtAction(nameof(GetToDoById), new { id = response.Data.Id }, response); // Return 201 Created
    }

    // PUT: api/ToDo/update/{id}
    // Update an existing ToDo item
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateToDoItem(string userId, int id, [FromBody] ToDoItem toDoItem)
    {
        var response = await _toDoService.UpdateToDoItemAsync(userId, id, toDoItem);
        if (!response.Success)
        {
            if (response.ErrorNumber == 301)
                return NotFound(response); // Return 404 Not Found if item doesn't exist
            return StatusCode(500, response); // Return 500 Internal Server Error for other errors
        }
        return NoContent(); // Return 204 No Content on successful update
    }

    // DELETE: api/ToDo/delete/{userId}/{id}
    // Delete a ToDo item by UserId
    [HttpDelete("delete/{userId}/{id}")]
    public async Task<IActionResult> DeleteToDoItem(string userId, int id)
    {
        var response = await _toDoService.DeleteToDoItemByUserIdAsync(userId, id);
        if (!response.Success)
        {
            if (response.ErrorNumber == 307)
                return NotFound(response); // Return 404 Not Found if item doesn't exist
            return StatusCode(500, response); // Return 500 Internal Server Error for other errors
        }
        return NoContent(); // Return 204 No Content on successful deletion
    }
}
