using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
    public class Doctor
    {
        public int Id { get; set; }
        public string education { get; set; }

        public int experience { get; set; }

        public string position { get; set; }

        public string WorkAddress { get; set; }

        public int Recommendtaions { get; set; } = 0;

        public float Price { get; set; }

        //One to Many RelationShip (Doctor --> AppUser) (AppUser --> Doctors)

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual AppUser User { get; set; }

        //Navigation Property for comments
        public virtual List<Comment> Comments { get; set; }
    }
}
