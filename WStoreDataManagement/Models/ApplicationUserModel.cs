﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WStoreDataManagement.Models
{
    public class ApplicationUserModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public Dictionary<string, string> Roles { get; set; }
    }
}