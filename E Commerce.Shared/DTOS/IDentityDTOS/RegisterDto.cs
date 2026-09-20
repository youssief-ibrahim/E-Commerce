using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Shared.DTOS.IDentityDTOS
{
    public record RegisterDto(
    [EmailAddress] string email,
    string Name,
    string UserName,
    string password,
    [Phone] string PhoneNumber
    );
}
