using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Data.Constants;

namespace ToDoList.Data
{
    public class ToDoItems
    {
        public long Id { get; set; }

        public string TaskName { get; set; }

        public Status Status { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public User User { get; set; }
    }
}
