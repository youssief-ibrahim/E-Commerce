using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Shared.DTOS.IDentityDTOS
{
    public record UserDto(
       string email,
       string DisplayName,
       string token);
}
