using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AccountDTOs
{
    public class UserLoginDTO
    {
        required public string Email { get; set; }

        required public string Password { get; set; }
    }
}
