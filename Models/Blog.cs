using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogsConsole.Models
{
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogId { get; set; }
        [Required(ErrorMessage = "Blog Name is Required")]
        public string Name { get; set; }

        public List<Post> Posts { get; set; }
    }
}