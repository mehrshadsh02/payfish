﻿using payfish.Models;
using System;
using System.Collections.Generic;

namespace payfish.ViewModels.Admin
{
    public class EditEmployeeViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Code { get; set; }
        public string Position { get; set; }
        public DateTime HireDate { get; set; }
        public List<LeaveDate> LeaveDays { get; set; } = new();

        public int RoleId { get; set; }
        public List<Role> Roles { get; set; } = new();
    }
}
