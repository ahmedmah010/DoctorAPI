using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class EditDTO
    {
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Email { get; set; }
        public string? img { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? Education { get; set; }
        public int? Experience { get; set; }
        public string? Position { get; set; }
        public string? WorkAdress { get; set; }
        public float? Price { get; set; }
    }
}
