using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Shared.DTOS.IDentityDTOS
{
    public class ConfirmEmailDto
    {
        [Required(ErrorMessage = "Email is required"), EmailAddress]
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
