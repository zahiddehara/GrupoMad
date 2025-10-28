using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GrupoMad.Services;
using GrupoMad.Models;
using GrupoMad.Data;

namespace GrupoMad.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly ApplicationDbContext _context;

        public ProductController(ProductService productService, ApplicationDbContext context)
        {
            _productService = productService;
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name");
            ViewData["ProductTypes"] = new SelectList(Enum.GetValues(typeof(ProductType)));
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SKU,Name,Description,ProductType,StoreId,IsActive")] ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar que el SKU sea único
                if (!await _productService.IsSKUUniqueAsync(model.SKU))
                {
                    ModelState.AddModelError("SKU", "Este SKU ya existe.");
                    ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", model.StoreId);
                    ViewData["ProductTypes"] = new SelectList(Enum.GetValues(typeof(ProductType)));
                    return View(model);
                }

                // Crear instancia concreta según el tipo
                Product product = model.ProductType switch
                {
                    ProductType.Accessory => new AccessoryProduct
                    {
                        SKU = model.SKU,
                        Name = model.Name,
                        Description = model.Description,
                        StoreId = model.StoreId,
                        IsActive = model.IsActive
                    },
                    ProductType.Blind => new BlindProduct
                    {
                        SKU = model.SKU,
                        Name = model.Name,
                        Description = model.Description,
                        StoreId = model.StoreId,
                        IsActive = model.IsActive
                    },
                    _ => throw new ArgumentException("Tipo de producto no válido")
                };

                await _productService.CreateProductAsync(product);
                return RedirectToAction(nameof(Index));
            }

            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", model.StoreId);
            ViewData["ProductTypes"] = new SelectList(Enum.GetValues(typeof(ProductType)));
            return View(model);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
            ViewData["PricingTypes"] = new SelectList(Enum.GetValues(typeof(PricingType)), product.PricingType);
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SKU,Name,Description,ProductType,PricingType,StoreId,IsActive")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Verificar que el SKU sea único (excluyendo el producto actual)
                if (!await _productService.IsSKUUniqueAsync(product.SKU, product.Id))
                {
                    ModelState.AddModelError("SKU", "Este SKU ya existe.");
                    ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
                    ViewData["PricingTypes"] = new SelectList(Enum.GetValues(typeof(PricingType)), product.PricingType);
                    return View(product);
                }

                var result = await _productService.UpdateProductAsync(id, product);
                if (result == null)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
            ViewData["PricingTypes"] = new SelectList(Enum.GetValues(typeof(PricingType)), product.PricingType);
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Product/ManageColors/5
        public async Task<IActionResult> ManageColors(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            var allColors = _context.Colors.Where(c => c.IsActive).ToList();
            ViewBag.AvailableColors = allColors;

            return View(product);
        }

        // POST: Product/AddColor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddColor(int productId, int colorId, string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                TempData["Error"] = "El SKU es requerido.";
                return RedirectToAction(nameof(ManageColors), new { id = productId });
            }

            var result = await _productService.AddColorToProductAsync(productId, colorId, sku);

            if (!result)
            {
                TempData["Error"] = "No se pudo agregar el color. Verifica que el SKU sea único y que el color no esté ya asignado.";
            }
            else
            {
                TempData["Success"] = "Color agregado exitosamente.";
            }

            return RedirectToAction(nameof(ManageColors), new { id = productId });
        }

        // POST: Product/UpdateColorSKU
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateColorSKU(int productId, int colorId, string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                TempData["Error"] = "El SKU es requerido.";
                return RedirectToAction(nameof(ManageColors), new { id = productId });
            }

            var result = await _productService.UpdateProductColorSKUAsync(productId, colorId, sku);

            if (!result)
            {
                TempData["Error"] = "No se pudo actualizar el SKU. Verifica que sea único.";
            }
            else
            {
                TempData["Success"] = "SKU actualizado exitosamente.";
            }

            return RedirectToAction(nameof(ManageColors), new { id = productId });
        }

        // POST: Product/RemoveColor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveColor(int productId, int colorId)
        {
            await _productService.RemoveColorFromProductAsync(productId, colorId);
            return RedirectToAction(nameof(ManageColors), new { id = productId });
        }
    }

    // ViewModel para crear productos
    public class ProductCreateViewModel
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public ProductType ProductType { get; set; }
        public PricingType PricingType { get; set; }
        public int? StoreId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
