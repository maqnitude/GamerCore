using System.Text.Json;
using GamerCore.Core.Models;
using GamerCore.CustomerSite.Models;

namespace GamerCore.CustomerSite.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductService> _logger;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ProductService(IHttpClientFactory httpClientFactory, ILogger<ProductService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<PagedResult<ProductViewModel>> GetProductsAsync(int page = 1, int[]? categoryIds = null)
        {
            string apiUrl = "/api/products";

            apiUrl += $"?page={page}";

            if (categoryIds != null && categoryIds.Length > 0)
            {
                foreach (int id in categoryIds)
                {
                    // NOTE: Be careful double check to make sure the parameter name matches the API
                    apiUrl += $"&categoryIds={id}";
                }
            }

            var client = _httpClientFactory.CreateClient("GamerCoreDev");

            try
            {
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    using var contentStream = await response.Content.ReadAsStreamAsync();
                    var pagedResult = await JsonSerializer
                        .DeserializeAsync<PagedResult<ProductViewModel>>(contentStream, _jsonSerializerOptions);

                    if (pagedResult == null)
                    {
                        _logger.LogWarning($"API call to {apiUrl} succeeded but return null data.");
                        return new PagedResult<ProductViewModel>();
                    }

                    _logger.LogInformation("Successfully retrieved {Count}.", pagedResult.Items.Count);
                    return pagedResult;
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