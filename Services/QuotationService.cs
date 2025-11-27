using GrupoMad.Data;
using GrupoMad.Models;
using Microsoft.EntityFrameworkCore;

namespace GrupoMad.Services
{
    public class QuotationService
    {
        private readonly ApplicationDbContext _context;

        public QuotationService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Genera el siguiente número de cotización para una tienda específica
        /// Formato: STORE{StoreId}-COT-{Year}-{Sequence}
        /// Ejemplo: STORE1-COT-2025-0001
        /// </summary>
        public async Task<string> GenerateQuotationNumberAsync(int storeId)
        {
            var year = DateTime.UtcNow.Year;
            var prefix = $"STORE{storeId}-COT-{year}-";

            // Buscar la última cotización de esta tienda en este año
            var lastQuotation = await _context.Quotations
                .Where(q => q.StoreId == storeId && q.QuotationNumber.StartsWith(prefix))
                .OrderByDescending(q => q.QuotationNumber)
                .Select(q => q.QuotationNumber)
                .FirstOrDefaultAsync();

            int nextSequence = 1;

            if (lastQuotation != null)
            {
                // Extraer el número de secuencia del último número
                var parts = lastQuotation.Split('-');
                if (parts.Length == 4 && int.TryParse(parts[3], out int lastSequence))
                {
                    nextSequence = lastSequence + 1;
                }
            }

            return $"{prefix}{nextSequence:D4}";
        }

        /// <summary>
        /// Obtiene el precio de un producto desde las listas de precios de una tienda
        /// Retorna el precio con descuento si está disponible
        /// </summary>
        public async Task<(decimal unitPrice, decimal discountedPrice, string? variant)?> GetProductPriceAsync(
            int productId,
            int storeId,
            string? variant = null)
        {
            // Buscar el producto en las listas de precios de la tienda
            var priceListItem = await _context.PriceListItems
                .Include(pli => pli.Discounts)
                .Include(pli => pli.PriceList)
                .Where(pli =>
                    pli.ProductId == productId &&
                    pli.PriceList.StoreId == storeId &&
                    pli.PriceList.IsActive &&
                    (variant == null || pli.Variant == variant))
                .OrderByDescending(pli => pli.PriceList.UpdatedAt ?? pli.PriceList.CreatedAt)
                .FirstOrDefaultAsync();

            if (priceListItem == null)
                return null;

            var unitPrice = priceListItem.GetBasePrice();
            var discountedPrice = priceListItem.GetFinalPrice();

            return (unitPrice, discountedPrice, priceListItem.Variant);
        }

        /// <summary>
        /// Verifica si una cotización puede cambiar a un estado específico
        /// </summary>
        public bool CanChangeStatus(Quotation quotation, QuotationStatus newStatus)
        {
            return newStatus switch
            {
                QuotationStatus.Draft => false, // No se puede regresar a borrador
                QuotationStatus.Sent => quotation.Status == QuotationStatus.Draft,
                QuotationStatus.Accepted => quotation.Status == QuotationStatus.Sent && !quotation.IsExpired(),
                QuotationStatus.Rejected => quotation.Status == QuotationStatus.Sent,
                QuotationStatus.Expired => quotation.Status == QuotationStatus.Sent && quotation.IsExpired(),
                _ => false
            };
        }

        /// <summary>
        /// Cambia el estado de una cotización
        /// </summary>
        public async Task<bool> ChangeStatusAsync(int quotationId, QuotationStatus newStatus, int userId)
        {
            var quotation = await _context.Quotations.FindAsync(quotationId);

            if (quotation == null || !CanChangeStatus(quotation, newStatus))
                return false;

            quotation.Status = newStatus;
            quotation.UpdatedAt = DateTime.UtcNow;

            switch (newStatus)
            {
                case QuotationStatus.Sent:
                    quotation.SentAt = DateTime.UtcNow;
                    break;
                case QuotationStatus.Accepted:
                case QuotationStatus.Rejected:
                    quotation.RespondedAt = DateTime.UtcNow;
                    break;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Duplica una cotización existente creando una nueva en estado Draft
        /// </summary>
        public async Task<Quotation> DuplicateQuotationAsync(int quotationId, int userId)
        {
            var original = await _context.Quotations
                .Include(q => q.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.ProductType)
                .Include(q => q.Contact)
                .Include(q => q.Store)
                .FirstOrDefaultAsync(q => q.Id == quotationId);

            if (original == null)
                throw new InvalidOperationException("Cotización no encontrada");

            var newQuotation = new Quotation
            {
                QuotationNumber = await GenerateQuotationNumberAsync(original.StoreId),
                QuotationDate = DateTime.UtcNow,
                ValidUntil = DateTime.UtcNow.AddDays(15), // 15 días por defecto
                Status = QuotationStatus.Draft,
                ContactId = original.ContactId,
                StoreId = original.StoreId,
                CreatedByUserId = userId,

                // Copiar dirección de entrega
                DeliveryFirstName = original.DeliveryFirstName,
                DeliveryLastName = original.DeliveryLastName,
                DeliveryStreet = original.DeliveryStreet,
                DeliveryExteriorNumber = original.DeliveryExteriorNumber,
                DeliveryInteriorNumber = original.DeliveryInteriorNumber,
                DeliveryNeighborhood = original.DeliveryNeighborhood,
                DeliveryCity = original.DeliveryCity,
                DeliveryStateID = original.DeliveryStateID,
                DeliveryPostalCode = original.DeliveryPostalCode,
                DeliveryRFC = original.DeliveryRFC,
                DeliveryReference = original.DeliveryReference,

                Notes = original.Notes,
                TermsAndConditions = original.TermsAndConditions,
                GlobalDiscountPercentage = original.GlobalDiscountPercentage,
                ShippingCost = original.ShippingCost,

                Items = original.Items.Select((item, index) => new QuotationItem
                {
                    ProductId = item.ProductId,
                    Variant = item.Variant,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountedPrice = item.DiscountedPrice,
                    Width = item.Width,
                    Height = item.Height,
                    Description = item.Description,
                    DisplayOrder = index
                }).ToList()
            };

            _context.Quotations.Add(newQuotation);
            await _context.SaveChangesAsync();

            return newQuotation;
        }

        /// <summary>
        /// Calcula la fecha de validez por defecto (15 días desde hoy)
        /// </summary>
        public DateTime GetDefaultValidUntil()
        {
            return DateTime.UtcNow.AddDays(15);
        }
    }
}
