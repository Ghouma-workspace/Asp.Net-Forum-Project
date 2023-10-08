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
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Display(Name = "Title")]
        [MaxLength(50)]
        [Required(ErrorMessage = "Title is required")]
        public string? Title { get; set; }

        [Display(Name = "Description")]
        [MaxLength(100)]
        [Required(ErrorMessage = "Description is required")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 100 characters")]
        public string? Description { get; set; }

        [Display(Name = "Content")]
        [Required(ErrorMessage = "Content is required")]
        [StringLength(10000, MinimumLength = 100, ErrorMessage = "Content must be more than 100 characters")]
        public string? Content { get; set; }

        [NotMapped]
        [DisplayName("Image")]//name of the property  displayed to the user  
        public IFormFile? Coverpic { get; set; }

        public string? Picname { get; set; }

        [Display(Name = "Date of Publication")]
        [Required]
        public DateTime PublishedDateTime { get; set; }

        [ForeignKey("Blog")]
        public int BlogId { get; set; }
        public Blog? Blog { get; set; }
        public virtual List<Comment>? Comments { get; set; }
    }
}
