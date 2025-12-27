using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using GrupoMad.Services;
using GrupoMad.Models;
using GrupoMad.Models.ViewComponents;
using GrupoMad.Data;

namespace GrupoMad.Controllers
{
    [Authorize(Roles = "Administrator,SalesManager,StoreManager")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly PriceListService _priceListService;
        private readonly ApplicationDbContext _context;

        public ProductController(ProductService productService, PriceListService priceListService, ApplicationDbContext context)
        {
            _productService = productService;
            _priceListService = priceListService;
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();

            // Configurar la tabla personalizada
            var config = new DataTableConfig<Product>
            {
                TableId = "productsTable",
                Data = products,
                MakeRowsClickable = true,
                RowClickUrl = "/Product/Details/{id}",
                GetRowId = p => p.Id,
                EnableFilters = true,
                Filters = new DataTableFilters
                {
                    ShowSearchFilter = true,
                    SearchPlaceholder = "Buscar por nombre o SKU...",
                    SearchAttributes = new List<string> { "name", "sku" },
                    DropdownFilters = new List<DataTableDropdownFilter>
                    {
                        new DataTableDropdownFilter
                        {
                            Id = "productTypeFilter",
                            Label = "Tipo de Producto",
                            DataAttribute = "producttype",
                            Placeholder = "Todos los tipos"
                        },
                        new DataTableDropdownFilter
                        {
                            Id = "storeFilter",
                            Label = "Tienda",
                            DataAttribute = "store",
                            Placeholder = "Todas las tiendas"
                        },
                        new DataTableDropdownFilter
                        {
                            Id = "statusFilter",
                            Label = "Estado",
                            DataAttribute = "status",
                            Placeholder = "Todos"
                        }
                    }
                },
                Columns = new List<DataTableColumn<Product>>
                {
                    new DataTableColumn<Product>
                    {
                        Header = "SKU",
                        ValueSelector = p => p.SKU ?? "",
                        DataAttribute = "sku",
                        DataAttributeValueSelector = p => (p.SKU ?? "").ToLower()
                    },
                    new DataTableColumn<Product>
                    {
                        Header = "Nombre",
                        ValueSelector = p => p.Name ?? "",
                        DataAttribute = "name",
                        DataAttributeValueSelector = p => (p.Name ?? "").ToLower()
                    },
                    new DataTableColumn<Product>
                    {
                        Header = "Tipo",
                        ValueSelector = p => p.ProductType != null
                            ? new Microsoft.AspNetCore.Html.HtmlString($"<span class=\"custom-badge custom-badge-info\">{p.ProductType.Name}</span>")
                            : new Microsoft.AspNetCore.Html.HtmlString("<span class=\"custom-badge custom-badge-secondary\">Sin tipo</span>"),
                        DataAttribute = "producttype",
                        DataAttributeValueSelector = p => p.ProductType?.Name ?? "Sin tipo"
                    },
                    new DataTableColumn<Product>
                    {
                        Header = "Tipo de Precio",
                        ValueSelector = p => p.ProductType != null
                            ? p.ProductType.PricingType switch
                            {
                                PricingType.PerSquareMeter => "Por m²",
                                PricingType.PerUnit => "Por Unidad",
                                PricingType.PerLinearMeter => "Por Metro Lineal",
                                PricingType.PerRangeLength => "Por Rango de Largo",
                                PricingType.PerRangeDimensions => "Por Rango de Dimensiones",
                                _ => ""
                            }
                            : ""
                    },
                    new DataTableColumn<Product>
                    {
                        Header = "Tienda",
                        ValueSelector = p => p.Store != null
                            ? new Microsoft.AspNetCore.Html.HtmlString($"<span class=\"custom-badge custom-badge-warning\">{p.Store.Name}</span>")
                            : new Microsoft.AspNetCore.Html.HtmlString("<span class=\"custom-badge custom-badge-primary\">Global</span>"),
                        DataAttribute = "store",
                        DataAttributeValueSelector = p => p.Store?.Name ?? "Global"
                    },
                    new DataTableColumn<Product>
                    {
                        Header = "Colores",
                        ValueSelector = p => p.ProductColors != null && p.ProductColors.Any()
                            ? new Microsoft.AspNetCore.Html.HtmlString($"<span class=\"custom-badge custom-badge-secondary\">{p.ProductColors.Count} colores</span>")
                            : new Microsoft.AspNetCore.Html.HtmlString("<span style=\"color: #9ca3af;\">Sin colores</span>")
                    },
                    new DataTableColumn<Product>
                    {
                        Header = "Estado",
                        ValueSelector = p => p.IsActive
                            ? new Microsoft.AspNetCore.Html.HtmlString("<span class=\"custom-badge custom-badge-success\">Activo</span>")
                            : new Microsoft.AspNetCore.Html.HtmlString("<span class=\"custom-badge custom-badge-danger\">Inactivo</span>"),
                        DataAttribute = "status",
                        DataAttributeValueSelector = p => p.IsActive ? "active" : "inactive"
                    }
                },
                ShowActions = true,
                Actions = new List<DataTableAction<Product>>
                {
                    new DataTableAction<Product>
                    {
                        Label = "Editar",
                        Icon = "pencil",
                        UrlSelector = p => $"/Product/Edit/{p.Id}"
                    },
                    new DataTableAction<Product>
                    {
                        Label = "Colores",
                        Icon = "palette",
                        UrlSelector = p => $"/Product/ManageColors/{p.Id}"
                    },
                    new DataTableAction<Product>
                    {
                        Label = "Precios",
                        Icon = "currency-dollar",
                        UrlSelector = p => $"/Product/PricesByStore/{p.Id}"
                    },
                    new DataTableAction<Product>
                    {
                        IsDivider = true
                    },
                    new DataTableAction<Product>
                    {
                        Label = "Eliminar",
                        Icon = "trash",
                        UrlSelector = p => $"/Product/Delete/{p.Id}",
                        CssClass = "dropdown-item text-danger"
                    }
                }
            };

            return View(config);
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
            ViewData["ProductTypes"] = new SelectList(_context.ProductTypes.Where(pt => pt.IsActive), "Id", "Name");
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SKU,Name,Description,ProductTypeId,StoreId,IsActive")] ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar que el SKU sea único
                if (!await _productService.IsSKUUniqueAsync(model.SKU))
                {
                    ModelState.AddModelError("SKU", "Este SKU ya existe.");
                    ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", model.StoreId);
                    ViewData["ProductTypes"] = new SelectList(_context.ProductTypes.Where(pt => pt.IsActive), "Id", "Name", model.ProductTypeId);
                    return View(model);
                }

                var product = new Product
                {
                    SKU = model.SKU,
                    Name = model.Name,
                    Description = model.Description,
                    ProductTypeId = model.ProductTypeId,
                    StoreId = model.StoreId,
                    IsActive = model.IsActive
                };

                await _productService.CreateProductAsync(product);
                return RedirectToAction(nameof(Index));
            }

            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", model.StoreId);
            ViewData["ProductTypes"] = new SelectList(_context.ProductTypes.Where(pt => pt.IsActive), "Id", "Name", model.ProductTypeId);
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

            var viewModel = new ProductEditViewModel
            {
                Id = product.Id,
                SKU = product.SKU,
                Name = product.Name,
                Description = product.Description,
                ProductTypeId = product.ProductTypeId,
                StoreId = product.StoreId,
                IsActive = product.IsActive
            };

            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", product.StoreId);
            ViewData["ProductTypes"] = new SelectList(_context.ProductTypes.Where(pt => pt.IsActive), "Id", "Name", product.ProductTypeId);
            return View(viewModel);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SKU,Name,Description,ProductTypeId,StoreId,IsActive")] ProductEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Verificar que el SKU sea único (excluyendo el producto actual)
                if (!await _productService.IsSKUUniqueAsync(model.SKU, model.Id))
                {
                    ModelState.AddModelError("SKU", "Este SKU ya existe.");
                    ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", model.StoreId);
                    ViewData["ProductTypes"] = new SelectList(_context.ProductTypes.Where(pt => pt.IsActive), "Id", "Name", model.ProductTypeId);
                    return View(model);
                }

                // Obtener el producto existente
                var existingProduct = await _productService.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Actualizar las propiedades del producto existente
                existingProduct.SKU = model.SKU;
                existingProduct.Name = model.Name;
                existingProduct.Description = model.Description;
                existingProduct.ProductTypeId = model.ProductTypeId;
                existingProduct.StoreId = model.StoreId;
                existingProduct.IsActive = model.IsActive;

                var result = await _productService.UpdateProductAsync(id, existingProduct);
                if (result == null)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", model.StoreId);
            ViewData["ProductTypes"] = new SelectList(_context.ProductTypes.Where(pt => pt.IsActive), "Id", "Name", model.ProductTypeId);
            return View(model);
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

            return View(product);
        }

        // POST: Product/AddColor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddColor(int productId, string colorName, string sku)
        {
            if (string.IsNullOrWhiteSpace(colorName))
            {
                TempData["Error"] = "El nombre del color es requerido.";
                return RedirectToAction(nameof(ManageColors), new { id = productId });
            }

            if (string.IsNullOrWhiteSpace(sku))
            {
                TempData["Error"] = "El SKU es requerido.";
                return RedirectToAction(nameof(ManageColors), new { id = productId });
            }

            var result = await _productService.AddColorToProductAsync(productId, colorName, sku);

            if (!result)
            {
                TempData["Error"] = "No se pudo agregar el color. Verifica que el SKU sea único.";
            }
            else
            {
                TempData["Success"] = "Color agregado exitosamente.";
            }

            return RedirectToAction(nameof(ManageColors), new { id = productId });
        }

        // POST: Product/UpdateColor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateColor(int productId, int productColorId, string? colorName, string? sku)
        {
            if (string.IsNullOrWhiteSpace(colorName) && string.IsNullOrWhiteSpace(sku))
            {
                TempData["Error"] = "Debes proporcionar al menos el nombre del color o el SKU.";
                return RedirectToAction(nameof(ManageColors), new { id = productId });
            }

            var result = await _productService.UpdateProductColorAsync(productColorId, colorName, sku);

            if (!result)
            {
                TempData["Error"] = "No se pudo actualizar el color. Verifica que el SKU sea único.";
            }
            else
            {
                TempData["Success"] = "Color actualizado exitosamente.";
            }

            return RedirectToAction(nameof(ManageColors), new { id = productId });
        }

        // POST: Product/RemoveColor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveColor(int productId, int productColorId)
        {
            await _productService.RemoveColorFromProductAsync(productColorId);
            TempData["Success"] = "Color eliminado exitosamente.";
            return RedirectToAction(nameof(ManageColors), new { id = productId });
        }

        // GET: Product/GlobalPrice/5
        public async Task<IActionResult> GlobalPrice(int? id)
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

            var priceItem = await _priceListService.GetProductPriceFromGlobalListAsync(id.Value);
            var priceValue = await _priceListService.GetProductPriceValueFromGlobalListAsync(id.Value);

            ViewBag.Product = product;
            ViewBag.PriceItem = priceItem;
            ViewBag.PriceValue = priceValue;

            return View();
        }

        // GET: Product/PricesByStore/5
        public async Task<IActionResult> PricesByStore(int? id)
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

            var priceItems = await _priceListService.GetProductPricesByStoreAsync(id.Value);

            ViewBag.Product = product;
            ViewBag.PriceItems = priceItems;

            return View();
        }
    }

    // ViewModel para crear productos
    public class ProductCreateViewModel
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int ProductTypeId { get; set; }
        public int? StoreId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ViewModel para editar productos
    public class ProductEditViewModel
    {
        public int Id { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int ProductTypeId { get; set; }
        public int? StoreId { get; set; }
        public bool IsActive { get; set; }
    }
}
