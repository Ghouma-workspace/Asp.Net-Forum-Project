using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForum.BL.Entities
{
    public class Theme
    {
        [Key]
        public int ThemeId { get; set; }

        [Display(Name = "A Title for the theme")]
        [Required(ErrorMessage = "The title is required")]
        [MaxLength(20)]
        public string? ThemeTitle { get; set; }

        [Display(Name = "Theme Color in Hex")]
        [MaxLength(20)]
        public string? ThemebgColor { get; set; }

        public virtual List<Blog>? Blogs { get; set; }
    }
}
