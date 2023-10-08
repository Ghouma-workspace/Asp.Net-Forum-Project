using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForum.BL.Entities
{
    public class User : IdentityUser
    {
        [Display(Name = "Enter Your First Name")]
        [Required]
        public string? FirstName { get; set; }

        [Display(Name = "Enter Your Last Name")]
        [Required]
        public string? LastName { get; set; }
        public int? UsernameChangeLimit { get; set; } = 10;

        [Display(Name = "Profil Picture")]
        public byte[]? ProfilePicture { get; set; }

        public virtual List<Blog>? Blogs { get; set; }
        public virtual List<Comment>? Comments { get; set; }
    }
}
