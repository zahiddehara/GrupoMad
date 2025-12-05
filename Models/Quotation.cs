using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrupoMad.Models
{
    public enum QuotationStatus
    {
        Draft = 1,      // Borrador
        Sent = 2,       // Enviada
        Accepted = 3,   // Aceptada
        Rejected = 4,   // Rechazada
        Expired = 5     // Expirada
    }

    public class Quotation
    {
        public int Id { get; set; }

        /// <summary>
        /// Número de cotización (ej: COT-2025-0001)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string QuotationNumber { get; set; }

        /// <summary>
        /// Fecha de creación de la cotización
        /// </summary>
        [Required]
        public DateTime QuotationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha hasta la cual la cotización es válida
        /// </summary>
        [Required]
        public DateTime ValidUntil { get; set; }

        /// <summary>
        /// Estado de la cotización
        /// </summary>
        [Required]
        public QuotationStatus Status { get; set; } = QuotationStatus.Draft;

        // Relación con Contact (Cliente)
        [Required]
        public int ContactId { get; set; }
        public Contact Contact { get; set; }

        // Dirección de entrega (datos copiados, no relacional)
        // Se copian los datos al crear la cotización para mantener histórico inmutable

        [Required]
        [StringLength(100)]
        public string DeliveryFirstName { get; set; }

        [StringLength(100)]
        public string? DeliveryLastName { get; set; }

        [StringLength(200)]
        public string? DeliveryStreet { get; set; }

        [StringLength(20)]
        public string? DeliveryExteriorNumber { get; set; }

        [StringLength(20)]
        public string? DeliveryInteriorNumber { get; set; }

        [StringLength(100)]
        public string? DeliveryNeighborhood { get; set; }

        [StringLength(100)]
        public string? DeliveryCity { get; set; }

        public MexicanState? DeliveryStateID { get; set; }

        [StringLength(10)]
        public string? DeliveryPostalCode { get; set; }

        [StringLength(13)]
        public string? DeliveryRFC { get; set; }

        [StringLength(500)]
        public string? DeliveryReference { get; set; }

        // Relación con Store (Tienda que genera la cotización)
        [Required]
        public int StoreId { get; set; }
        public Store Store { get; set; }

        // Relación con User (Usuario que creó la cotización)
        [Required]
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        /// <summary>
        /// Notas u observaciones generales de la cotización
        /// </summary>
        [StringLength(2000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Términos y condiciones específicos de esta cotización
        /// </summary>
        [StringLength(2000)]
        public string? TermsAndConditions { get; set; }

        /// <summary>
        /// Descuento global aplicado al total (porcentaje)
        /// </summary>
        [Column(TypeName = "decimal(5,2)")]
        public decimal GlobalDiscountPercentage { get; set; } = 0;

        /// <summary>
        /// Costo de envío/instalación
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingCost { get; set; } = 0;

        // Items de la cotización
        public List<QuotationItem> Items { get; set; } = new List<QuotationItem>();

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Fecha en que la cotización fue enviada al cliente
        /// </summary>
        public DateTime? SentAt { get; set; }

        /// <summary>
        /// Fecha en que la cotización fue aceptada/rechazada
        /// </summary>
        public DateTime? RespondedAt { get; set; }

        // Métodos de cálculo

        /// <summary>
        /// Calcula el subtotal de todos los items (sin descuento global ni envío)
        /// </summary>
        public decimal GetSubtotal()
        {
            return Items?.Sum(item => item.GetLineTotal()) ?? 0;
        }

        /// <summary>
        /// Calcula el monto del descuento global
        /// </summary>
        public decimal GetGlobalDiscountAmount()
        {
            var subtotal = GetSubtotal();
            return subtotal * (GlobalDiscountPercentage / 100);
        }

        /// <summary>
        /// Calcula el total final (subtotal - descuento global + envío)
        /// Nota: Los precios ya incluyen impuestos
        /// </summary>
        public decimal GetTotal()
        {
            var subtotal = GetSubtotal();
            var globalDiscount = GetGlobalDiscountAmount();
            return subtotal - globalDiscount + ShippingCost;
        }

        /// <summary>
        /// Verifica si la cotización está expirada
        /// </summary>
        public bool IsExpired()
        {
            return DateTime.UtcNow > ValidUntil && Status == QuotationStatus.Sent;
        }

        /// <summary>
        /// Verifica si la cotización puede ser editada
        /// </summary>
        public bool CanBeEdited()
        {
            return Status == QuotationStatus.Draft;
        }
    }

    public class QuotationItem
    {
        public int Id { get; set; }

        [Required]
        public int QuotationId { get; set; }
        public Quotation Quotation { get; set; }

        /// <summary>
        /// Producto de la cotización (del catálogo)
        /// </summary>
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        /// <summary>
        /// Color del producto (si aplica)
        /// </summary>
        public int? ProductColorId { get; set; }
        public ProductColor? ProductColor { get; set; }

        /// <summary>
        /// Variante del producto (si aplica)
        /// Por ejemplo: "3 vías", "4 vías", etc.
        /// </summary>
        [StringLength(100)]
        public string? Variant { get; set; }

        /// <summary>
        /// Cantidad de productos
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Precio unitario del producto
        /// Se guarda aquí para mantener histórico (precio al momento de cotizar)
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Precio con descuento aplicado (si aplica)
        /// Se guarda para mantener histórico
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountedPrice { get; set; }

        // Campos para productos que requieren medidas/dimensiones

        /// <summary>
        /// Ancho/Largo (en metros)
        /// - Para PerSquareMeter: representa el ancho
        /// - Para PerLinearMeter: representa el ancho
        /// - Para PerRangeLength: representa el largo del producto
        /// - Para PerRangeDimensions: representa el ancho
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Width { get; set; }

        /// <summary>
        /// Alto (en metros)
        /// - Para PerSquareMeter: representa el alto
        /// - Para PerRangeDimensions: representa el alto
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Height { get; set; }

        /// <summary>
        /// Descripción o notas específicas de este item
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Orden de visualización en la cotización
        /// </summary>
        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Métodos de cálculo

        /// <summary>
        /// Calcula la cantidad efectiva según el PricingType del producto
        /// </summary>
        public decimal GetEffectiveQuantity()
        {
            if (Product == null || Product.ProductType == null)
                return Quantity;

            switch (Product.ProductType.PricingType)
            {
                case PricingType.PerSquareMeter:
                    // Cantidad = Ancho * Alto * Cantidad
                    if (Width.HasValue && Height.HasValue)
                        return Width.Value * Height.Value * Quantity;
                    break;

                case PricingType.PerLinearMeter:
                    // Cantidad = Ancho * Cantidad
                    if (Width.HasValue)
                        return Width.Value * Quantity;
                    break;

                case PricingType.PerRangeLength:
                case PricingType.PerRangeDimensions:
                    // Para rangos, la cantidad es directa (el precio ya considera las dimensiones)
                    return Quantity;

                case PricingType.PerUnit:
                default:
                    // Cantidad = Cantidad directa
                    return Quantity;
            }

            return Quantity;
        }

        /// <summary>
        /// Calcula el total de la línea (cantidad efectiva * precio con descuento)
        /// </summary>
        public decimal GetLineTotal()
        {
            return GetEffectiveQuantity() * DiscountedPrice;
        }

        /// <summary>
        /// Calcula el ahorro por descuento en esta línea
        /// </summary>
        public decimal GetDiscountAmount()
        {
            var effectiveQty = GetEffectiveQuantity();
            return effectiveQty * (UnitPrice - DiscountedPrice);
        }

        /// <summary>
        /// Verifica si este item tiene descuento aplicado
        /// </summary>
        public bool HasDiscount()
        {
            return DiscountedPrice < UnitPrice;
        }
    }
}
