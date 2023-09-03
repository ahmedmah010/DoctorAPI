using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
    public class Comment
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime Created { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }

        [ForeignKey("Author")]
        public string AuthortId { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual AppUser Author { get; set; }

        public bool IsRecommended { get; set; }

        

        // Doctor ---> Comments 
        // Comment --> Doctor

        // User --> Comments
        // Comment --> User
    }
}
