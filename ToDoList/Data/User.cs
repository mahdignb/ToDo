using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoList.Data
{
    public class User:IdentityUser
    {
        //public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [ForeignKey(nameof(ToDoItems))]
        public int TodoItemsId { get; set; }
        public ToDoItems todoItem { get; set; }
    }
}
