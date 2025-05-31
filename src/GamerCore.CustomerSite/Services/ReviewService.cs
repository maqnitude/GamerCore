using System.Text;
using System.Text.Json;
using GamerCore.Core.Models;

namespace GamerCore.CustomerSite.Services
{
    public class ReviewService : IReviewService
    {
        private readonly string _baseApiEndpoint = "/api/reviews";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IHttpClientFactory httpClientFactory,
            ILogger<ReviewService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<bool> CreateReviewAsync(CreateReviewDto createReviewDto)
        {
            string apiEndpoint = _baseApiEndpoint;

            var client = _httpClientFactory.CreateClient("GamerCoreDev");

            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(createReviewDto),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(apiEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed when calling {ApiEndpoint}.", apiEndpoint);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating review at {ApiEndpoint}.", apiEndpoint);
                throw;
            }
        }
    }
}