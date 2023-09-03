using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class UserRegisterDTO
    {
        [Required, MaxLength(50), MinLength(3)]
        public string fname { get; set; }

        [Required, MaxLength(50), MinLength(3)]
        public string lname { get; set; }

        [Required, MaxLength(50), MinLength(3)]
        public string username { get; set; }

        [DataType(DataType.Password), Required]
        public string password { get; set; }

        [Compare("password"), Required, DataType(DataType.Password)]
        public string confirmpassword { get; set; }

        [DataType(DataType.EmailAddress), Required]
        public string email { get; set; }

        [DataType(DataType.PhoneNumber), Required]
        public string phone { get; set; }
        [Required]
        public string city { get; set; }

        public string imgurl { get; set; }


    }
}
