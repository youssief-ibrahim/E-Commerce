using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Shared.DTOS.IDentityDTOS
{
    public class ForgetPasswordDto
    {
        [Required(ErrorMessage = "Email is required"),EmailAddress]
        public string Email { get; set; } = null!;
        public string WebLink { get; set; } = null!;
    }
}
