﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ExpenseDTOs
{
    public class EditExpenseDTO
    {
        required public string Id { get; set; }

        public string ExpenseName { get; set; } = string.Empty;

        public double Amount { get; set; }
    }
}
