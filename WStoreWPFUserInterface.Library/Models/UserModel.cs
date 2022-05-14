﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WStoreWPFUserInterface.Library.Models
{
    public class UserModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public Dictionary<string, string> Roles { get; set; } = new Dictionary<string, string>();

        public string RolesFlatten
        {
            get 
            {
                return string.Join(", ", Roles.Values);
            }
        }

    }
}
