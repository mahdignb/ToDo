using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoList.Data
{
    public class DatabaseContext:IdentityDbContext<User>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
           : base(options)
        {
        }
        public DbSet<ToDoItems> toDoItems { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
