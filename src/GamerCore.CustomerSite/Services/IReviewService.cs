using GamerCore.Core.Models;

namespace GamerCore.CustomerSite.Services
{
    public interface IReviewService
    {
        Task<bool> CreateReviewAsync(CreateReviewDto createReviewDto);
    }
}