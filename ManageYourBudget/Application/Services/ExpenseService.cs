using Domain.Entities;
using Domain.Interfaces;


namespace Application.Services
{
    public class ExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExpenseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddExpense(ExpenseCategory expense)
        {
            _unitOfWork.ExpenseCategories.AddAsync(expense);
            //await _unitOfWork.SaveAsync();
        }
    }
}
