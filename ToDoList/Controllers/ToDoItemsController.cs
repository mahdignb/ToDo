using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Data;
using Serilog;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        // GET: api/<ToDoItemsController>
        [SwaggerOperation(Tags = new[] { "ShowTasks" })]
        [HttpGet(Name ="ShowTasks")]
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

        // POST api/<ToDoItemsController>
        [SwaggerOperation(Tags = new[] { "CreateTasks" })]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ActionName(nameof(ToDoItems))]
        [HttpPost]
        public async Task<ActionResult<ToDoItems>> CreateTodoItem(ToDoItems toDo)
        {
            _context.toDoItems.Add(toDo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ToDoItems), new { id = toDo.Id },toDo);
        }

        // PUT api/<ToDoItemsController>/5
        [SwaggerOperation(Tags = new[] { "UpdateTasks" })]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id,ToDoItems todoItems)
        {
            if (id !=todoItems.Id)
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
        private bool TodoItemExists(long id) =>
             _context.toDoItems.Any(e => e.Id == id);
    }
}
