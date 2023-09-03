using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class ChangePassDTO
    {
        [Required]
        public string CurrentPass { get; set; }
        [Required]
        public string NewPass { get; set; }
    }
}
