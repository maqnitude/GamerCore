using System.ComponentModel.DataAnnotations;

namespace GamerCore.Api.Models
{
    public class UpdateCategoryDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}