using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyForum.BL.Entities
{
    public class Repl
    {
        [Key]
        public int IdRepl { get; set; }

        [MaxLength(100)]
        [Display(Name = "Text")]
        public string? Text { get; set; }

        [NotMapped]
        [DisplayName("Image")]//name of the property displayed to the user  
        public IFormFile? Replpic { get; set; }

        public string? Picname { get; set; }

        [ForeignKey("Comment")]
        public int CommentId { get; set; }
        public Comment? Comment { get; set; }

        [ForeignKey("User")]
        public string? Id;
        public User? User { get; set; }
    }
}
