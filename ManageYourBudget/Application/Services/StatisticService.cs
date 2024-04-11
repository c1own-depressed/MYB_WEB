using Application.DTOs.StatisticDTO;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.IncomeDTOs;
using Domain.Interfaces;
using Application.Utils;
using Domain.Entities;

namespace Application.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IUnitOfWork _unitOfWork;

        public async Task<IEnumerable<IncomeStatisticDTO>> getIncomesByDate (DateTime startDate, DateTime endDate, int UserId)
        {
            var user = await this._unitOfWork.Users.GetByIdAsync(UserId);
            var incomes = await this._unitOfWork.Incomes.GetIncomesByUserIdAsync(UserId);

            string currencyRepresentation = CurrencyUtils.FormatCurrencyDisplay(user.Currency);

            var incomeDTOs = incomes
                .Where(income => (income.Date >= startDate && income.Date <= endDate) || income.IsRegular)
                .Select(income => new IncomeDTO
                {
                    Id = income.Id,
                    IncomeName = income.IncomeName,
                    Amount = income.Amount,
                    CurrencyEmblem = currencyRepresentation,
                    Date = income.Date,
                    IsRegular = income.IsRegular,
                });
            incomeDTOs.GroupBy(x => x.Date.Month);
            var incomeByMonth = incomeDTOs
                .GroupBy(income => new { Year = income.Date.Year, Month = income.Date.Month })
                .Select(group => new IncomeStatisticDTO
                {
                    Month = new DateTime(group.Key.Year, group.Key.Month, 1),
                    TotalAmount = group.Sum(income => income.Amount),
                })
                .OrderBy(dto => dto.Month);
            return incomeByMonth;
        }
    }
}
