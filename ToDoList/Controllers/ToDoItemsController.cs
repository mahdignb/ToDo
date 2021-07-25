using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Data;
using Serilog;
using Google.Apis.Drive.v3;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoItemsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ToDoItemsController(DatabaseContext context)
        {
            _context = context;
        }

        #region ShowTasks(admin)
        // GET: api/<ToDoItemsController>
        [Authorize(Roles = "Administrator")]
        [SwaggerOperation(Tags = new[] { "ShowTasks" })]
        [HttpGet(Name = "ShowTasks")]
        public async Task<ActionResult<IEnumerable<ToDoItems>>> Get()
        {
            return await _context.toDoItems.ToListAsync();
            //if (username == Data.User.UserName && password==Data.User.Password)
            //{
            //return await _context.toDoItems.ToListAsync();
            //}
            //else
            //{
            //    return NotFound();
            //}
        }
        #endregion

        #region ShowTask(User)
        // GET api/<ToDoItemsController>/5
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "ShowTasks" })]
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoItems>> Get(long id)
        {
            var todoItem = await _context.toDoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }
        #endregion

        #region CreateTasks(User)
        // POST api/<ToDoItemsController>
        [Authorize(Roles = "User")]
        [SwaggerOperation(Tags = new[] { "CreateTasks" })]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ActionName(nameof(ToDoItems))]
        [HttpPost]
        public async Task<ActionResult<ToDoItems>> CreateTodoItem(ToDoItems toDo)
        {
            _context.toDoItems.Add(toDo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ToDoItems), new { id = toDo.Id }, toDo);
        }
        #endregion

        #region UpdateTasks(User)
        // PUT api/<ToDoItemsController>/5
        [Authorize(Roles = "User")]
        [SwaggerOperation(Tags = new[] { "UpdateTasks" })]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, ToDoItems todoItems)
        {
            if (id != todoItems.Id)
            {
                return BadRequest();
            }
            _context.Entry(todoItems).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        #endregion

        #region DeleteTasks
        // DELETE api/<ToDoItemsController>/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Tags = new[] { "DeleteTasks" })]
        public async Task<IActionResult> DeleteToDoItem(long id)
        {
            var todoItem = await _context.toDoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            _context.toDoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        private bool TodoItemExists(long id) =>
             _context.toDoItems.Any(e => e.Id == id);
    }
}
