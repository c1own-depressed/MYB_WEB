using Application.DTOs.ReviewDTOs;
using Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetAllReviews();

        Task<ServiceResult> AddReview(ReviewDTO model);
    }
}
