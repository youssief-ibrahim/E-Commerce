namespace E_Commerce.Shared.DTOS.IDentityDTOS
{
    public record UserDto(
       string email,
       string DisplayName,
       string token,
       string? refreshToken = null,
       DateTime? refreshTokenExpiration = null);
}
