using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
    public class NormalUser //This model in case you want to add additional fields in the future for the normal user
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual AppUser User { get; set; }
    }
}
