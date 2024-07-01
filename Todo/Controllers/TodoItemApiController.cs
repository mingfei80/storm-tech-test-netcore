using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Todo.Data;
using Todo.Data.Entities;

namespace Todo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TodoItemApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TodoItemApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Create(TodoItemRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Title))
                {
                    return BadRequest("Invalid data.");
                }

                var todoItem = new TodoItem(request.TodoListId, request.ResponsiblePartyId, request.Title, request.Importance, request.Rank)
                {
                    Rank = request.Rank,
                    IsDone = false
                };

                _context.TodoItems.Add(todoItem);
                await _context.SaveChangesAsync();

                return Ok(todoItem);
            }
            catch (Exception ex)
            {

                return NotFound();
                throw ex;
            }
        }
    }
}

public class TodoItemRequest
{
    public int TodoListId { get; set; }
    public string Title { get; set; }
    public string ResponsiblePartyId { get; set; }
    public Importance Importance { get; set; }
    public int Rank { get; set; }
}