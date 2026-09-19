using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.IdentityModule;
using E_Commerce.Persistence.Data.DbContext;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Persistence.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly EcomerceDbContext dbContext;

        public RefreshTokenRepository(EcomerceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await dbContext.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await dbContext.RefreshTokens
                .FirstOrDefaultAsync(token => token.TokenHash == tokenHash);
        }

        public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(string userId)
        {
            var now = DateTime.Now;
            return await dbContext.RefreshTokens
                .Where(token => token.UserId == userId && token.RevokedOn == null && token.ExpiresOn > now)
                .ToListAsync();
        }

        public void Update(RefreshToken refreshToken)
        {
            dbContext.RefreshTokens.Update(refreshToken);
        }

        public async Task RevokeAllActiveForUserAsync(string userId)
        {
            var now = DateTime.Now;
            var tokens = await dbContext.RefreshTokens
                .Where(token => token.UserId == userId && token.RevokedOn == null && token.ExpiresOn > now)
                .ToListAsync();

            foreach (var token in tokens)
                token.RevokedOn = now;
        }

        public async Task<int> DeleteExpiredAsync()
        {
            return await dbContext.RefreshTokens
                .Where(token => token.ExpiresOn <= DateTime.Now)
                .ExecuteDeleteAsync();
        }

        public async Task<int> SaveChangesAsync() =>await dbContext.SaveChangesAsync();
    }
}
