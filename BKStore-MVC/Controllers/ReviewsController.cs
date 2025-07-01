using Microsoft.AspNetCore.Mvc;
using BKStore_MVC.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using BKStore_MVC.Models.Context;

namespace BKStore_MVC.Controllers
{
    /// <summary>
    /// Controller for managing book reviews.
    /// </summary>
    public class ReviewsController : Controller
    {
        private readonly BKstore_System _context;
        private readonly ILogger<ReviewsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewsController"/> class.
        /// </summary>
        public ReviewsController(BKstore_System context, ILogger<ReviewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Displays all reviews.
        /// </summary>
        public IActionResult Index()
        {
            try
            {
                var reviews = _context.BookRatings.ToList();
                return View(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all reviews.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Displays reviews for a specific book.
        /// </summary>
        public IActionResult BookReviews(int bookId)
        {
            try
            {
                var reviews = _context.BookRatings.Where(r => r.BookID == bookId).ToList();
                ViewBag.BookId = bookId;
                return View("BookReviews", reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching reviews for book ID: {bookId}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Returns the view for adding a new review.
        /// </summary>
        public IActionResult Add(int bookId)
        {
            ViewBag.BookId = bookId;
            return View();
        }

        /// <summary>
        /// Handles the POST request to add a new review.
        /// </summary>
        [HttpPost]
        public IActionResult Add(BookRating review)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when adding review.");
                return View(review);
            }
            try
            {
                _context.BookRatings.Add(review);
                _context.SaveChanges();
                _logger.LogInformation("Review added successfully.");
                return RedirectToAction("BookReviews", new { bookId = review.BookID });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a review by its ID.
        /// </summary>
        public IActionResult Delete(int id)
        {
            try
            {
                var review = _context.BookRatings.FirstOrDefault(r => r.BookRatingID == id);
                if (review == null)
                {
                    _logger.LogWarning($"Review with ID {id} not found for deletion.");
                    return NotFound("Review not found");
                }
                int bookId = review.BookID;
                _context.BookRatings.Remove(review);
                _context.SaveChanges();
                _logger.LogInformation($"Review with ID {id} deleted successfully.");
                return RedirectToAction("BookReviews", new { bookId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting review with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
