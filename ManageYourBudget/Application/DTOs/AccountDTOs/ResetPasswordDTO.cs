﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AccountDTOs
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; }

        public string Token { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}

