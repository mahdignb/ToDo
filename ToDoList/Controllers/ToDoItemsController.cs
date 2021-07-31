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
using System;
using System.Net.Http;
using System.Net;
using ToDoList.Services;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.ServiceModel.Channels;

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
        [Authorize(Roles ="User,Admin")]
        [SwaggerOperation(Tags = new[] { "ShowTasks" })]
        [HttpGet(Name = "ShowTasks")]
        public async Task<ActionResult<IEnumerable<ToDoItems>>> Get()
        {
            return await _context.toDoItems
                .Where(option => option.User.UserName == User.Identity.Name)
                .ToListAsync();
        }
        #endregion

        #region ShowTaskById(User)
        // GET api/<ToDoItemsController>/5
        [Authorize()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "ShowTasks" })]
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoItems>> Get(long id)
        {
            var todo = _context.toDoItems;
            bool currentUserTask = todo.AsQueryable().Any(e => e.Id == id && e.User.UserName == User.Identity.Name);

            if (currentUserTask ==false)
            {
                return NotFound();
            }
            var todoItem = await _context.toDoItems.FindAsync(id);
            return Ok(todoItem);
        }
        #endregion

        #region CreateTasks(User)
        // POST api/<ToDoItemsController>
        [Authorize(Roles ="User")]
        [SwaggerOperation(Tags = new[] { "CreateTasks" })]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ActionName(nameof(ToDoItems))]
        [HttpPost]
        public async Task<ActionResult<ToDoItems>> CreateTodoItem(ToDoItems toDo)
        {
            //toDo.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            toDo.Author = User.Identity.Name;
            toDo.UserId = _context.User
                .Where(m => m.UserName == toDo.Author)
                .Select(m => m.Id)
                .SingleOrDefault();
            //var y = User.Identity.GetUserName();
            //var userResult = _context.User.FirstOrDefault(user => user.Id == toDo.UserId);
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
        public async Task<ActionResult<string>> UpdateTodoItem(long id, ToDoItems todoItems)
        {
            if (id != todoItems.Id && todoItems.Author != User.Identity.Name)
            {
                return BadRequest();
            }
            _context.Entry(todoItems).State = EntityState.Modified;
            todoItems.Author = User.Identity.Name;
            todoItems.UserId = _context.User
                .Where(m => m.UserName == todoItems.Author)
                .Select(m => m.Id)
                .SingleOrDefault();
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

            return "Value changed Successfully";
        }
        #endregion

        #region DeleteTasks
        // DELETE api/<ToDoItemsController>/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Tags = new[] { "DeleteTasks" })]
        public async Task<ActionResult<string>> DeleteToDoItem(long id)
        {
            var todo = _context.toDoItems;
            bool currentUserTask = todo.AsQueryable().Any(e => e.Id == id && e.User.UserName == User.Identity.Name);

            if (currentUserTask == false)
            {
                return NotFound();
            }
            var todoItem = await _context.toDoItems.FindAsync(id);
            _context.toDoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
            return $"Task {id} deleted succesfully";
        }
        #endregion


        //[ActionName(nameof(ToDoItems))]
        //[SwaggerOperation(Tags = new[] { "CreateTasks" })]
        //[Route("{id}")]
        //[HttpPut]
        //public async Task<ActionResult<string>> CreateTodoItem(long id, ToDoItems todo)
        //{
        //    if (id != todo.Id)
        //    {
        //        return BadRequest();
        //    }
        //    _context.Entry(todo).State = EntityState.Modified;
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    //return Accepted();
        //    return "Value changed successfully";
        //}
        private bool TodoItemExists(long id) =>
             _context.toDoItems.Any(e => e.Id == id);
    }
}
