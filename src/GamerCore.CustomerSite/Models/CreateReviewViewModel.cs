using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GamerCore.CustomerSite.Models
{
    public class CreateReviewViewModel
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(100)]
        public string? ReviewTitle { get; set; }

        [StringLength(1000)]
        public string? ReviewText { get; set; }

        [Required]
        [HiddenInput]
        public string ProductId { get; set; } = string.Empty;
    }
}