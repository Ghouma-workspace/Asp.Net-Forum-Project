using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForum.BL.Entities
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [MaxLength(100)]
        [Display(Name = "A Text")]
        public string? Text { get; set; }

        [NotMapped]
        [DisplayName("Image")]//name of the property displayed to the user  
        public IFormFile? Commentpic { get; set; }

        public string? Picname { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post? Post { get; set; }

        [ForeignKey("User")]
        public string? Id;
        public User? User { get; set; }
        public virtual List<Repl>? Repls { get; set; }
    }
}
