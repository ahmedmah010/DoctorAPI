using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class DoctorRegisterDTO : UserRegisterDTO
    {
        [Required, MaxLength(100), MinLength(5)]
        public string education { get; set; }
        [Required]
        public int experience { get; set; }

        [Required, MaxLength(100), MinLength(3)]
        public string position { get; set; }

        [Required, MaxLength(100), MinLength(10)]
        public string WorkAddress { get; set; }

        [Required,Range(0,10000)]
        public float Price { get; set; }
    }
}
