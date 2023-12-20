using dotnet_kp.Database;
using dotnet_kp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace dotnet_kp.Pages.Admin.Products;

public class ProductInputModel
{
    public string Name { get; set; }

    public string? Description { get; set; }

    public float? Price { get; set; }


    public IFormFile ImageFile { get; set; } = new FormFile(Stream.Null, 0, 0, null, null);

    public int CategoryId { get; set; }
}

[Authorize(policy: "AdminOnly")]
public class ProductModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ProductModel> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductModel(ApplicationDbContext dbContext, ILogger<ProductModel> logger,
        IWebHostEnvironment webHostEnvironment)
    {
        _dbContext = dbContext;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }

    [Inject] public IWebHostEnvironment WebHostEnvironment { get; set; }


    [BindProperty] 
    public ProductInputModel ProductInput { get; set; }


    [BindProperty(SupportsGet = true)] public int? ProductId { get; set; }
    public List<Product> AllProducts { get; set; }
    public List<Category> AllCategories { get; set; }

   public string UploadWay { get; set; } 

    public void OnGet()
    {
        AllProducts = _dbContext.Products.ToList();
        AllCategories = _dbContext.Categories.ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            try
            {
                var file = Request.Form.Files["file"];
                var filePath = "";
                var uniqueFileName = "";
                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
                    filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                
                var newProduct = new Product
                {
                    Name = ProductInput.Name,
                    Description = ProductInput.Description,
                    Price = ProductInput.Price,
                    ImageUrl = "/images/" + uniqueFileName,
                    CategoryId = ProductInput.CategoryId
                };

                _dbContext.Products.Add(newProduct);
                await _dbContext.SaveChangesAsync();

                return RedirectToPage("/Admin/Products/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostRemoveProductAsync(int productId)
    {
        _logger.LogInformation("i am at remove product");
        try
        {
            var foundedProductToDelete = _dbContext.Products.FirstOrDefault(c => c.ProductId == productId);
            if (foundedProductToDelete == null)
            {
                return NotFound();
            }

            _dbContext.Products.Remove(foundedProductToDelete);
            await _dbContext.SaveChangesAsync();
            return RedirectToPage("/Admin/Products/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }

        return Page();
    }

    public async Task<IActionResult> OnPostEditProductAsync(int productId)
    {
        _logger.LogInformation("i am at edit product");
        try
        {
            var productToChange = _dbContext.Products.FirstOrDefault(c => c.ProductId == productId);
            var file = Request.Form.Files["file_edit"];
            var uniqueFileName = "";
            if (productToChange != null)
            {
                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                productToChange.Name =
                    string.IsNullOrEmpty(ProductInput.Name) ? productToChange.Name : ProductInput.Name;
                productToChange.Description = string.IsNullOrEmpty(ProductInput.Description)
                    ? productToChange.Description
                    : ProductInput.Description;
                productToChange.Price =
                    string.IsNullOrEmpty(ProductInput.Price.ToString()) ? productToChange.Price : ProductInput.Price;
                productToChange.ImageUrl = "/images/" + uniqueFileName;
                productToChange.CategoryId = string.IsNullOrEmpty(ProductInput.CategoryId.ToString())
                    ? productToChange.CategoryId
                    : ProductInput.CategoryId;
                await _dbContext.SaveChangesAsync();
                return RedirectToPage("/Admin/Products/Index");
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
}