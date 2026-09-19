using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Shared.DTOS.IDentityDTOS
{
    public record RefreshTokenRequestDto(
        [Required] string RefreshToken
    );
}
