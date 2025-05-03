namespace GamerCore.Core.Models
{
    // Standardized api response wrapper class
    // Only returned by AuthController for now
    public class ApiResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public List<string> Errors { get; set; } = [];
    }
}