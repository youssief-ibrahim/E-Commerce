using E_Commerce.Domain.Contracts;

namespace E_Commerce.Web.Jobs
{
    public class RefreshTokenCleanupJob
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<RefreshTokenCleanupJob> _logger;

        public RefreshTokenCleanupJob(
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<RefreshTokenCleanupJob> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("[RefreshTokenCleanup] Starting cleanup of expired refresh tokens at {Time}", DateTime.Now);

            var deletedCount = await _refreshTokenRepository.DeleteExpiredAsync();
            await _refreshTokenRepository.SaveChangesAsync();

            _logger.LogInformation("[RefreshTokenCleanup] Successfully deleted {Count} expired refresh token(s) at {Time}", deletedCount, DateTime.Now);
        }
    }
}
