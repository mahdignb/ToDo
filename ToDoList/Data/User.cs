using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoList.Data
{
    public class User
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public  string UserName { get; set; }

        public  string Password { get; set; }

        public string Email { get; set; }

        //public bool IsAuthenticated { get; set; }
    }
}
