namespace GamerCore.Core.Models
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of items (even with filtering) without pagination.<br/><br/>
        /// This is needed by pagination to calculate the correct number of pages.
        /// </summary>
        public int TotalItems { get; set; }
    }
}