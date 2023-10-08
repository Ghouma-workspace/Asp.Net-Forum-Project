using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForum.BL.Entities
{
    public class Blog
    {
        [Key]
        public int BlogId { get; set; }

        [Display(Name = "Blog URL")]
        [Required, MaxLength(80)]
        public string? Url { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 100 characters")]
        public string? Description { set; get; }

        [ForeignKey("Theme")]
        public int ThemeId { get; set; }
        public Theme? Theme { get; set; }
        
        [ForeignKey("User")]
        public string? Id { get; set; }
        public User? User { get; set; }
        public virtual List<Post>? Posts { get; set; }
    }
}
