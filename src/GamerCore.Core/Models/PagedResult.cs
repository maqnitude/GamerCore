using System.Text.Json.Serialization;

namespace GamerCore.Core.Models
{
    public class PagedResult<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; } = [];

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of items (even with filtering) without pagination.<br/><br/>
        /// This is needed by pagination to calculate the correct number of pages.
        /// </summary>
        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }
    }
}