using GamerCore.Core.Entities;

namespace GamerCore.Api.Services
{
    public interface IJwtService
    {
        Task<string> GenerateJwtTokenAsync(AppUser user);
    }
}