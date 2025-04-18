using System.Web;

namespace GamerCore.CustomerSite.Extensions
{
    public static class UrlExtensions
    {
        public static string UpdateQueryParameter(this string url, string param, string value)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            // Split the URL into path and query
            var parts = url.Split('?', 2);
            string path = parts[0];
            string query = parts.Length > 1 ? parts[1] : string.Empty;

            // Parse the query string
            var queryParams = HttpUtility.ParseQueryString(query);
            queryParams[param] = value;

            // Reconstruct the URL
            var updatedQuery = queryParams.ToString();

            return string.IsNullOrEmpty(updatedQuery) ? path : $"{path}?{updatedQuery}";
        }
    }
}