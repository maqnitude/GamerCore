using System.Text.Json;
using GamerCore.CustomerSite.Models;

namespace GamerCore.CustomerSite.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CategoryService> _logger;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public CategoryService(IHttpClientFactory httpClientFactory, ILogger<CategoryService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<List<CategoryViewModel>> GetCategoriesAsync()
        {
            string apiUrl = "/api/categories";
            var client = _httpClientFactory.CreateClient("GamerCoreDev");

            try
            {
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    using var contentStream = await response.Content.ReadAsStreamAsync();
                    var categories = await JsonSerializer
                        .DeserializeAsync<List<CategoryViewModel>>(contentStream, _jsonSerializerOptions);

                    if (categories == null)
                    {
                        _logger.LogWarning("API call to {ApiUrl} succeeded but return null data.", apiUrl);
                        return [];
                    }

                    _logger.LogInformation("Successfully retrieved {Count} categories.", categories.Count);
                    return categories;
                }
                else
                {
                    _logger.LogError("API call to {ApiUrl} failed with status code {StatusCode}", apiUrl, response.StatusCode);
                    throw new HttpRequestException($"API request failed with status code {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed when calling {ApiUrl}.", apiUrl);
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize response from {ApiUrl}.", apiUrl);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching products from {ApiUrl}.", apiUrl);
                throw;
            }
        }
    }
}