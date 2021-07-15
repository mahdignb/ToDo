using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Data;

namespace ToDoList.Data
{
    public class ToDoItems
    {
        public long Id { get; set; }

        public string TaskName { get; set; }

        public TaskStatus Status { get; set; }

        //[ForeignKey(nameof(User.Name))]
        public string Author { get; set; }

        public string Description { get; set; }

        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        public User Users { get; set; }
    }
}
