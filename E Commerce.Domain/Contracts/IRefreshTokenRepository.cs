using E_Commerce.Domain.Entities.IdentityModule;

namespace E_Commerce.Domain.Contracts
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(string userId);
        Task<RefreshToken?> GetLatestActiveByUserIdAsync(string userId);
        void Update(RefreshToken refreshToken);
        Task RevokeAllActiveForUserAsync(string userId);
        Task<int> DeleteExpiredAsync();
        Task<int> SaveChangesAsync();
    }
}
