using Application.DTOs;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class HomeService : IHomeService
    {
        private readonly IExpenseCategoryService _expenseCategoryService;
        private readonly IIncomeService _incomeService;
        private readonly ISavingsService _savingsService;


        public HomeService(
            IExpenseCategoryService expenseCategoryService,
            IIncomeService incomeService,
            ISavingsService savingsService
            )
        {
            _expenseCategoryService = expenseCategoryService;
            _incomeService = incomeService;
            _savingsService = savingsService;
        }

        public async Task<HomeDTO> GetHomeDataAsync(string userId)
        {
            var categories = await _expenseCategoryService.GetExpenseCategoriesByUserIdAsync(userId);
            var incomes = await _incomeService.GetIncomesByUserIdAsync(userId);
            var savings = await _savingsService.GetSavingsByUserIdAsync(userId);

            return new HomeDTO
            {
                Categories = categories,
                Incomes = incomes,
                Savings = savings,
            };
        }
    }

}
