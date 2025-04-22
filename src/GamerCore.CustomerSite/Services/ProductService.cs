using System.Text.Json;
using GamerCore.Core.Models;
using GamerCore.CustomerSite.Models;

namespace GamerCore.CustomerSite.Services
{
    public class ProductService : IProductService
    {
        private readonly string _apiBaseEndpoint = "/api/Products";

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
            string apiEndpoint = _apiBaseEndpoint;
            apiEndpoint += $"?page={page}";

            if (categoryIds != null && categoryIds.Length > 0)
            {
                foreach (int id in categoryIds)
                {
                    // NOTE: Be careful double check to make sure the parameter name matches the API
                    apiEndpoint += $"&categoryIds={id}";
                }
            }

            var client = _httpClientFactory.CreateClient("GamerCoreDev");

            try
            {
                var response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    using var contentStream = await response.Content.ReadAsStreamAsync();
                    var pagedResult = await JsonSerializer
                        .DeserializeAsync<PagedResult<ProductViewModel>>(contentStream, _jsonSerializerOptions);

                    if (pagedResult == null)
                    {
                        _logger.LogWarning("API call to {ApiEndpoint} succeeded but return null data.", apiEndpoint);
                        return new PagedResult<ProductViewModel>();
                    }

                    _logger.LogInformation("Successfully retrieved {Count}.", pagedResult.Items.Count);
                    return pagedResult;
                }
                else
                {
                    // _logger.LogError("API call to {ApiEndpoint} failed with status code {StatusCode}", apiEndpoint, response.StatusCode);
                    throw new HttpRequestException($"API request failed with status code {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed when calling {ApiEndpoint}.", apiEndpoint);
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize response from {ApiEndpoint}.", apiEndpoint);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching products from {ApiEndpoint}.", apiEndpoint);
                throw;
            }
        }

        public async Task<ProductDetailsViewModel> GetProductDetailsAsync(int id)
        {
            string apiEndpoint = _apiBaseEndpoint + $"/Details/{id}";

            var client = _httpClientFactory.CreateClient("GamerCoreDev");

            try
            {
                var response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    using var contentStream = await response.Content.ReadAsStreamAsync();
                    var productDetails = await JsonSerializer
                        .DeserializeAsync<ProductDetailsViewModel>(contentStream, _jsonSerializerOptions);

                    // It's either found or not found so this is just to be extra safe
                    if (productDetails == null)
                    {
                        return new ProductDetailsViewModel();
                    }

                    _logger.LogInformation("Successfully retrieved product details (id: {Id})", productDetails.ProductId);
                    return productDetails;
                }
                else
                {
                    throw new HttpRequestException($"API request failed with status code {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed when calling {ApiEndpoint}.", apiEndpoint);
                throw;
            }
            // JsonException won't be reached because the API will return NotFound first
            // catch (JsonException ex)
            // {
            //     _logger.LogError(ex, "Failed to deserialize response from {ApiEndpoint}.", apiEndpoint);
            //     throw;
            // }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching products from {ApiEndpoint}.", apiEndpoint);
                throw;
            }
        }
    }
}