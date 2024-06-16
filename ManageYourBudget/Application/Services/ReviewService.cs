using Application.DTOs.ExpenseDTOs;
using Application.DTOs.IncomeDTOs;
using Application.DTOs.ReviewDTOs;
using Application.Interfaces;
using Application.Utils;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(IUnitOfWork unitOfWork, ILogger<ReviewService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<ReviewDTO>> GetAllReviews()
        {
            var reviews = await _unitOfWork.Reviews.GetAllReviewsAsync();
            var reviewDTOs = reviews.Select(review => new ReviewDTO
            {
                UserName = review.UserName,
                Rating = review.Rating,
                Text = review.Text,
                CreatedAt = review.CreatedAt,
            });
            return reviewDTOs;
        }

        public async Task<ServiceResult> AddReview(ReviewDTO model)
        {
            var review = new Review
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Rating = model.Rating,
                Text = model.Text,
                CreatedAt = DateTime.Now,
            };
            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.CompleteAsync();
            return new ServiceResult(success: true);
        }
    }
}
