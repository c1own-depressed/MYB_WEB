﻿namespace Application.DTOs.SavingsDTOs
{
    public class EditSavingsDTO
    {
        public int Id { get; set; }

        public string SavingsName { get; set; } = string.Empty;

        public double Amount { get; set; }
    }
}