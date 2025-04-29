using System.ComponentModel.DataAnnotations;

namespace GamerCore.Api.Models
{
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}