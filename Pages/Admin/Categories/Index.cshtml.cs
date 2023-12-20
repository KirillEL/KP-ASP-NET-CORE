using System.ComponentModel.DataAnnotations;
using dotnet_kp.Database;
using dotnet_kp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace dotnet_kp.Pages.Admin.Categories;

public class CategoryInputModel
{
    [Required] [StringLength(50)] public string Name { get; set; }

    [MaxLength(200)] public string Description { get; set; }
}

[Authorize(policy: "AdminOnly")]
public class CategoryModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CategoryModel> _logger;

    public CategoryModel(ApplicationDbContext dbContext, ILogger<CategoryModel> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public List<Category> AllCategories { get; set; }
    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }
    [BindProperty] public CategoryInputModel CategoryInput { get; set; }

    public void OnGet()
    {
        AllCategories = _dbContext.Categories.ToList();
    }

    public IActionResult OnPost()
    {
        _logger.LogInformation("HERREREEEEE");
        if (ModelState.IsValid)
        {
            try
            {
                var newCategory = new Category
                {
                    Name = CategoryInput.Name,
                    Description = CategoryInput.Description
                };
                _dbContext.Categories.Add(newCategory);
                _dbContext.SaveChanges();
                return RedirectToPage("/Admin/Categories/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while adding category");
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostEditCategoryAsync(int categoryId)
    {
        _logger.LogInformation("i am in Edit");
        try
        {
            var categoryToChange = _dbContext.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (categoryToChange != null)
            {
                categoryToChange.Name = CategoryInput.Name;
                categoryToChange.Description = CategoryInput.Description;
                await _dbContext.SaveChangesAsync();
                return RedirectToPage("/Admin/Categories/Index");
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }

        return Page();
    }

    public async Task<IActionResult> OnPostRemoveCategoryAsync(int categoryId)
    {
        _logger.LogInformation("XXXXXXX");
        try
        {
            var categoryToDelete = _dbContext.Categories.FirstOrDefault(c => c.CategoryId == categoryId);

            if (categoryToDelete == null)
            {
                return NotFound();
            }

            _dbContext.Categories.Remove(categoryToDelete);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Admin/Categories/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return Page();
        }

        return Page();
    }
}