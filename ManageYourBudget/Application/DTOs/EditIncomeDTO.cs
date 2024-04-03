﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class EditIncomeDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double Amount { get; set; }
    }
}