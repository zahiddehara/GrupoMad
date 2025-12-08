using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GrupoMad.Data;
using GrupoMad.Models;

namespace GrupoMad.Controllers
{
    [Authorize(Roles = "Administrator,SalesManager,StoreManager")]
    public class ProductTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductType
        public async Task<IActionResult> Index()
        {
            var productTypes = await _context.ProductTypes
                .Include(pt => pt.Products)
                .OrderBy(pt => pt.Name)
                .ToListAsync();
            return View(productTypes);
        }

        // GET: ProductType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _context.ProductTypes
                .Include(pt => pt.Products)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }

        // GET: ProductType/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypeCreateViewModel model)
        {
            if (model.HasVariants && (model.Variants == null || !model.Variants.Any(v => !v.IsDeleted)))
            {
                ModelState.AddModelError("", "Debe agregar al menos una variante activa cuando 'Tiene Variantes' está marcado.");
            }

            if (ModelState.IsValid)
            {
                var productType = new ProductType
                {
                    Name = model.Name,
                    Description = model.Description,
                    PricingType = model.PricingType,
                    IsActive = model.IsActive,
                    HasVariants = model.HasVariants,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Add(productType);
                await _context.SaveChangesAsync();

                if (model.HasVariants && model.Variants != null)
                {
                    int displayOrder = 1;
                    foreach (var variantVm in model.Variants.Where(v => !v.IsDeleted))
                    {
                        var variant = new ProductTypeVariant
                        {
                            ProductTypeId = productType.Id,
                            Name = variantVm.Name,
                            DisplayOrder = displayOrder++,
                            IsActive = variantVm.IsActive,
                            CreatedAt = DateTime.UtcNow
                        };
                        _context.ProductTypeVariants.Add(variant);
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Tipo de producto creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _context.ProductTypes
                .Include(pt => pt.ProductTypeVariants)
                .FirstOrDefaultAsync(pt => pt.Id == id);

            if (productType == null)
            {
                return NotFound();
            }

            var viewModel = new ProductTypeEditViewModel
            {
                Id = productType.Id,
                Name = productType.Name,
                Description = productType.Description,
                PricingType = productType.PricingType,
                IsActive = productType.IsActive,
                HasVariants = productType.HasVariants,
                Variants = productType.ProductTypeVariants.Select(v => new ProductTypeVariantViewModel
                {
                    Id = v.Id,
                    Name = v.Name,
                    DisplayOrder = v.DisplayOrder,
                    IsActive = v.IsActive
                }).OrderBy(v => v.DisplayOrder).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductTypeEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (model.HasVariants && (model.Variants == null || !model.Variants.Any(v => !v.IsDeleted)))
            {
                ModelState.AddModelError("", "Debe tener al menos una variante activa cuando 'Tiene Variantes' está marcado.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var productType = await _context.ProductTypes
                        .Include(pt => pt.ProductTypeVariants)
                        .FirstOrDefaultAsync(pt => pt.Id == id);

                    if (productType == null)
                    {
                        return NotFound();
                    }

                    productType.Name = model.Name;
                    productType.Description = model.Description;
                    productType.PricingType = model.PricingType;
                    productType.IsActive = model.IsActive;
                    productType.HasVariants = model.HasVariants;
                    productType.UpdatedAt = DateTime.UtcNow;

                    if (model.HasVariants && model.Variants != null)
                    {
                        var existingVariantIds = productType.ProductTypeVariants.Select(v => v.Id).ToList();
                        var submittedVariantIds = model.Variants.Where(v => v.Id > 0 && !v.IsDeleted).Select(v => v.Id).ToList();

                        var variantsToDelete = productType.ProductTypeVariants.Where(v => !submittedVariantIds.Contains(v.Id)).ToList();
                        _context.ProductTypeVariants.RemoveRange(variantsToDelete);

                        int displayOrder = 1;
                        foreach (var variantVm in model.Variants.Where(v => !v.IsDeleted))
                        {
                            if (variantVm.Id > 0)
                            {
                                var existingVariant = productType.ProductTypeVariants.FirstOrDefault(v => v.Id == variantVm.Id);
                                if (existingVariant != null)
                                {
                                    existingVariant.Name = variantVm.Name;
                                    existingVariant.DisplayOrder = displayOrder++;
                                    existingVariant.IsActive = variantVm.IsActive;
                                    existingVariant.UpdatedAt = DateTime.UtcNow;
                                }
                            }
                            else
                            {
                                var newVariant = new ProductTypeVariant
                                {
                                    ProductTypeId = productType.Id,
                                    Name = variantVm.Name,
                                    DisplayOrder = displayOrder++,
                                    IsActive = variantVm.IsActive,
                                    CreatedAt = DateTime.UtcNow
                                };
                                _context.ProductTypeVariants.Add(newVariant);
                            }
                        }
                    }
                    else
                    {
                        var allVariants = productType.ProductTypeVariants.ToList();
                        _context.ProductTypeVariants.RemoveRange(allVariants);
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Tipo de producto actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductTypeExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: ProductType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _context.ProductTypes
                .Include(pt => pt.Products)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productType == null)
            {
                return NotFound();
            }

            // Obtener el conteo de PriceLists asociadas
            var priceListCount = await _context.PriceLists
                .Where(pl => pl.ProductTypeId == id)
                .CountAsync();

            ViewBag.PriceListCount = priceListCount;

            return View(productType);
        }

        // POST: ProductType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productType = await _context.ProductTypes
                .Include(pt => pt.Products)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productType == null)
            {
                return NotFound();
            }

            // Verificar si tiene productos asociados
            if (productType.Products.Any())
            {
                TempData["Error"] = "No se puede eliminar el tipo de producto porque tiene productos asociados.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            // Obtener las PriceLists asociadas para eliminarlas en cascada
            var associatedPriceLists = await _context.PriceLists
                .Include(pl => pl.PriceListItems)
                    .ThenInclude(pli => pli.Discounts)
                .Where(pl => pl.ProductTypeId == id)
                .ToListAsync();

            // Eliminar en cascada: PriceListItemDiscounts -> PriceListItems -> PriceLists
            foreach (var priceList in associatedPriceLists)
            {
                if (priceList.PriceListItems != null)
                {
                    foreach (var item in priceList.PriceListItems)
                    {
                        if (item.Discounts != null)
                        {
                            _context.PriceListItemDiscounts.RemoveRange(item.Discounts);
                        }
                    }
                    _context.PriceListItems.RemoveRange(priceList.PriceListItems);
                }
                _context.PriceLists.Remove(priceList);
            }

            // Eliminar el ProductType
            _context.ProductTypes.Remove(productType);
            await _context.SaveChangesAsync();

            var message = "Tipo de producto eliminado exitosamente.";
            if (associatedPriceLists.Any())
            {
                message += $" También se eliminaron {associatedPriceLists.Count} lista(s) de precios asociada(s).";
            }

            TempData["Success"] = message;
            return RedirectToAction(nameof(Index));
        }

        private bool ProductTypeExists(int id)
        {
            return _context.ProductTypes.Any(e => e.Id == id);
        }
    }

    public class ProductTypeCreateViewModel
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public PricingType PricingType { get; set; }
        public bool IsActive { get; set; } = true;
        public bool HasVariants { get; set; } = false;
        public List<ProductTypeVariantViewModel> Variants { get; set; } = new List<ProductTypeVariantViewModel>();
    }

    public class ProductTypeEditViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public PricingType PricingType { get; set; }
        public bool IsActive { get; set; }
        public bool HasVariants { get; set; }
        public List<ProductTypeVariantViewModel> Variants { get; set; } = new List<ProductTypeVariantViewModel>();
    }

    public class ProductTypeVariantViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
