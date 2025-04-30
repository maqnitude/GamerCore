using System.ComponentModel.DataAnnotations;

namespace GamerCore.Api.Models
{
    public class UpdateCategoryDto
    {
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}