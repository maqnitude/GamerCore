using System.ComponentModel.DataAnnotations;

namespace GamerCore.Core.Models
{
    public class CreateReviewDto
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(100)]
        public string? ReviewTitle { get; set; }

        [StringLength(1000)]
        public string? ReviewText { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string ProductId { get; set; } = string.Empty;
    }
}