using AutoMapper;
using BKStore_MVC.Models;
using BKStore_MVC.Repository;
using BKStore_MVC.Repository.Interfaces;
using BKStore_MVC.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BKStore_MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    /// <summary>
    /// Controller for managing categories.
    /// </summary>
    public class CategoryController : Controller
    {
        ICategoryRepository categoryRepository;
        IBookRepository bookRepository;
        IMapper _mapper;
        private readonly ILogger<CategoryController> _logger;
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController"/> class.
        /// </summary>
        public CategoryController(ICategoryRepository _categoryRepository, IMapper mapper, IBookRepository _bookRepository, ILogger<CategoryController> logger)
        {
            categoryRepository = _categoryRepository;
            bookRepository = _bookRepository;
            _mapper = mapper;
            _logger = logger;
        }
        /// <summary>
        /// Displays all categories.
        /// </summary>
        public IActionResult Index()
        {
            _logger.LogInformation("Index action called: Displaying all categories.");
            try
            {
                var categories = categoryRepository.GetAll();
                if (categories == null)
                {
                    _logger.LogWarning("No categories found in the repository.");
                    return View("Index", new List<Category>());
                }
                return View("Index", categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching categories.");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Shows details for a specific category.
        /// </summary>
        public IActionResult Details(int id)
        {
            _logger.LogInformation($"Details action called for category ID: {id}");
            try
            {
                var categoryFromDB = categoryRepository.GetByID(id);
                if (categoryFromDB == null)
                {
                    _logger.LogWarning($"Category with ID {id} not found.");
                    return NotFound("Category Not Found");
                }
                var books = bookRepository.GetBooksByCatgyId(categoryFromDB.CategoryID);
                if (books == null || !books.Any())
                {
                    _logger.LogWarning($"No books found for category ID {id}.");
                    return NotFound("There are no books in this category");
                }
                var bookVM = _mapper.Map<BookWithCategoryVM>(categoryFromDB);
                bookVM.books = books;
                return View("Details", bookVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching details for category ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Returns the view for creating a new category.
        /// </summary>
        public IActionResult New()
        {
            _logger.LogInformation("New action called: Displaying new category form.");
            return View("New");
        }
        /// <summary>
        /// Saves a new category to the repository.
        /// </summary>
        public IActionResult SaveNew(int id, Category categoryFromRequest)
        {
            _logger.LogInformation("SaveNew action called.");
            try
            {
                if (ModelState.IsValid)
                {
                    categoryRepository.Add(categoryFromRequest);
                    categoryRepository.Save();
                    _logger.LogInformation("New category saved successfully.");
                    return RedirectToAction("Index");
                }
                _logger.LogWarning("Model state invalid in SaveNew.");
                return View("Edit", categoryFromRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving new category.");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Returns the view for editing a category.
        /// </summary>
        public IActionResult Edit(int id)
        {
            _logger.LogInformation($"Edit action called for category ID: {id}");
            try
            {
                Category categoryFromDB = categoryRepository.GetByID(id);
                if (categoryFromDB == null)
                {
                    _logger.LogWarning($"Category with ID {id} not found for editing.");
                    return NotFound("Category Not Found");
                }
                return View("Edit", categoryFromDB);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while editing category ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Saves the edited category to the repository.
        /// </summary>
        public IActionResult SaveEdit(int id, Category categoryFromRequest)
        {
            _logger.LogInformation($"SaveEdit action called for category ID: {id}");
            try
            {
                if (ModelState.IsValid)
                {
                    var categoryFromDB = categoryRepository.GetByID(id);
                    if (categoryFromDB == null)
                    {
                        _logger.LogWarning($"Category with ID {id} not found for saving edit.");
                        return NotFound("Not Found");
                    }
                    _mapper.Map(categoryFromRequest, categoryFromDB);
                    categoryRepository.Update(categoryFromDB);
                    categoryRepository.Save();
                    _logger.LogInformation($"Category with ID {id} updated successfully.");
                    return RedirectToAction("Index");
                }
                _logger.LogWarning("Model state invalid in SaveEdit.");
                return View("Edit", categoryFromRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while saving edit for category ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Returns the view for deleting a category.
        /// </summary>
        public IActionResult Delete(int id)
        {
            _logger.LogInformation($"Delete action called for category ID: {id}");
            try
            {
                Category categoryFromDB = categoryRepository.GetByID(id);
                if (categoryFromDB == null)
                {
                    _logger.LogWarning($"Category with ID {id} not found for deletion.");
                    return NotFound("Not Found");
                }
                return View("Delete", categoryFromDB);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting category ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Confirms and deletes a category from the repository.
        /// </summary>
        public IActionResult ConfirmDelete(int id)
        {
            _logger.LogInformation($"ConfirmDelete action called for category ID: {id}");
            try
            {
                Category categoryFromDB = categoryRepository.GetByID(id);
                if (categoryFromDB == null)
                {
                    _logger.LogWarning($"Category with ID {id} not found for confirm delete.");
                    return NotFound("Not Found");
                }
                categoryRepository.Delete(id);
                categoryRepository.Save();
                _logger.LogInformation($"Category with ID {id} deleted successfully.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while confirming delete for category ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Checks if a category exists by ID.
        /// </summary>
        private bool CategoryExists(int id)
        {
            _logger.LogInformation($"Checking existence for category ID: {id}");
            var exists = categoryRepository.GetByID(id) != null;
            _logger.LogInformation($"Category existence for ID {id}: {exists}");
            return exists;
        }
    }
}
#region Test
//public IActionResult Details(int id)
//{
//   Category categoryFromDB = categoryRepository.GetByID(id);
//    if (categoryFromDB == null)
//    {
//        return NotFound("Category Not Found");
//    }
//    List<Book> Books = bookRepository.GetBooksByCatgyId(categoryFromDB.CategoryID);

//    if (Books == null)
//    {
//        return NotFound("There is no Books in this Category");
//    }

//    BookWithCategoryVM bookVM = new BookWithCategoryVM();

//    bookVM.CategoryId = categoryFromDB.CategoryID;
//    bookVM.CategoryName = categoryFromDB.Name;
//    bookVM.books = Books;

//    return View("Details", bookVM);
//}

// DONE !

#endregion