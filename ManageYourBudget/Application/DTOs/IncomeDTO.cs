﻿namespace Application.DTOs
{
    public class IncomeDTO
    {
        public int Id { get; set; }

        public string IncomeName { get; set; } = string.Empty;

        public double Amount { get; set; }
    }
}