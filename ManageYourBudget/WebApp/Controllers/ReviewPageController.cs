using Application.DTOs.ReviewDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApp.Controllers
{
    public class ReviewPageController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewPageController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index()
        {
            var reviews = await _reviewService.GetAllReviews();
            return View(reviews);
        }
        // Display the form to create a new review
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewDTO reviewCreateDTO)
        {
            if (ModelState.IsValid)
            {
                var userName = User.FindFirstValue(ClaimTypes.Name);
                reviewCreateDTO.UserName = userName;
                await _reviewService.AddReview(reviewCreateDTO);
                return RedirectToAction(nameof(Index));
            }
            return View(reviewCreateDTO);
        }
    }
}
