using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Data;

namespace ToDoList.Configuration
{
    public class MapperInitializer :Profile
    {
        public MapperInitializer()
        {
        CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
