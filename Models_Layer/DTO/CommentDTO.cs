using Models.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class CommentDTO
    {
        public int? id { get; set; }
        public string? AuthorUserName { get; set; } = string.Empty;
        public DateTime? Created { get; set; } = DateTime.Now;

        public string? fname { get; set; }
        public string? lname { get; set; }

        [Required]
        public string Content { get; set; }
        [Required]
        public bool IsRecommended { get; set; }
    }
}
