using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
    public class AppUser : IdentityUser //common fields between all users
    {
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string img { get; set; }
        public string city { get; set; }
    }
}
