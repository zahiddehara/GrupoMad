using GrupoMad.Data;
using GrupoMad.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GrupoMad.Services
{
    public class QuotationPdfService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public QuotationPdfService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<byte[]> GenerateQuotationPdfAsync(int quotationId)
        {
            var quotation = await _context.Quotations
                .Include(q => q.Contact)
                .Include(q => q.Store)
                    .ThenInclude(s => s.Company)
                .Include(q => q.CreatedByUser)
                .Include(q => q.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.ProductType)
                .Include(q => q.Items)
                    .ThenInclude(i => i.ProductColor)
                .Include(q => q.Items)
                    .ThenInclude(i => i.ProductTypeVariant)
                .Include(q => q.Items)
                    .ThenInclude(i => i.ProductTypeHeadingStyle)
                .FirstOrDefaultAsync(q => q.Id == quotationId);

            if (quotation == null)
            {
                throw new Exception("Cotización no encontrada");
            }

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(c => ComposeHeader(c, quotation));
                    page.Content().Element(c => ComposeContent(c, quotation));
                    page.Footer().Element(c => ComposeFooter(c, quotation));
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container, Quotation quotation)
        {
            container.Column(column =>
            {
                column.Spacing(10);

                // Logo y datos de la empresa
                column.Item().Row(row =>
                {
                    // Logo (si existe en la Company)
                    var company = quotation.Store?.Company;
                    var logoPath = !string.IsNullOrEmpty(company?.LogoPath)
                        ? Path.Combine(_webHostEnvironment.WebRootPath, company.LogoPath.TrimStart('/', '\\'))
                        : null;

                    if (logoPath != null && File.Exists(logoPath))
                    {
                        row.RelativeItem(1).Height(60).Image(logoPath);
                    }
                    else
                    {
                        row.RelativeItem(1).Height(60).AlignMiddle().Text(company?.Name ?? "")
                            .FontSize(24).Bold().FontColor(Colors.Blue.Darken3);
                    }

                    // Información de la empresa
                    row.RelativeItem(2).Column(col =>
                    {
                        col.Item().AlignRight().Text(company?.Name ?? "")
                            .FontSize(14).Bold().FontColor(Colors.Blue.Darken3);
                        col.Item().AlignRight().Text($"Sucursal: {quotation.Store?.Name ?? ""}")
                            .FontSize(10).FontColor(Colors.Grey.Darken2);

                        // Datos fiscales de la empresa
                        if (!string.IsNullOrEmpty(company?.RFC))
                        {
                            col.Item().AlignRight().Text($"RFC: {company.RFC}")
                                .FontSize(10).FontColor(Colors.Grey.Darken2);
                        }

                        // Dirección de la empresa
                        if (!string.IsNullOrEmpty(company?.Street))
                        {
                            var address = company.Street;
                            if (!string.IsNullOrEmpty(company.ExteriorNumber))
                                address += $" {company.ExteriorNumber}";
                            if (!string.IsNullOrEmpty(company.InteriorNumber))
                                address += $" Int. {company.InteriorNumber}";

                            col.Item().AlignRight().Text(address)
                                .FontSize(10).FontColor(Colors.Grey.Darken2);
                        }

                        if (!string.IsNullOrEmpty(company?.Neighborhood) || !string.IsNullOrEmpty(company?.City))
                        {
                            var cityLine = "";
                            if (!string.IsNullOrEmpty(company.Neighborhood))
                                cityLine += company.Neighborhood;
                            if (!string.IsNullOrEmpty(company.City))
                            {
                                if (!string.IsNullOrEmpty(cityLine))
                                    cityLine += ", ";
                                cityLine += company.City;
                            }

                            col.Item().AlignRight().Text(cityLine)
                                .FontSize(10).FontColor(Colors.Grey.Darken2);
                        }

                        if (company?.StateID != null || !string.IsNullOrEmpty(company?.PostalCode))
                        {
                            var stateLine = "";
                            if (company.StateID != null)
                                stateLine += company.StateID.ToString();
                            if (!string.IsNullOrEmpty(company.PostalCode))
                            {
                                if (!string.IsNullOrEmpty(stateLine))
                                    stateLine += ", ";
                                stateLine += $"C.P. {company.PostalCode}";
                            }

                            col.Item().AlignRight().Text(stateLine)
                                .FontSize(10).FontColor(Colors.Grey.Darken2);
                        }

                        // Email de la empresa
                        if (!string.IsNullOrEmpty(company?.Email))
                        {
                            col.Item().AlignRight().Text(company.Email)
                                .FontSize(10).FontColor(Colors.Grey.Darken2);
                        }
                    });
                });

                // Línea divisoria
                column.Item().LineHorizontal(2).LineColor(Colors.Blue.Darken2);

                // Información de la cotización
                column.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("COTIZACIÓN").FontSize(18).Bold().FontColor(Colors.Blue.Darken3);
                        col.Item().Text($"No. {quotation.QuotationNumber}").FontSize(12).FontColor(Colors.Grey.Darken2);
                    });

                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignRight().Text($"Fecha: {quotation.QuotationDate.ToLocalTime():dd/MM/yyyy}").FontSize(10);
                        col.Item().AlignRight().Text($"Válida hasta: {quotation.ValidUntil.ToLocalTime():dd/MM/yyyy}").FontSize(10);
                        col.Item().AlignRight().Text(GetStatusText(quotation.Status))
                            .FontSize(10).Bold().FontColor(GetStatusColor(quotation.Status));
                    });
                });
            });
        }

        private void ComposeContent(IContainer container, Quotation quotation)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Spacing(15);

                // Información del cliente y dirección de entrega
                column.Item().Row(row =>
                {
                    // Cliente
                    row.RelativeItem().BorderLeft(3).BorderColor(Colors.Blue.Darken2).PaddingLeft(10).Column(col =>
                    {
                        col.Item().Text("CLIENTE").FontSize(11).Bold().FontColor(Colors.Blue.Darken3);
                        col.Item().PaddingTop(5).Text($"{quotation.Contact?.FirstName} {quotation.Contact?.LastName}").FontSize(10).Bold();

                        if (!string.IsNullOrEmpty(quotation.Contact?.RFC))
                        {
                            col.Item().Text($"RFC: {quotation.Contact?.RFC}").FontSize(9);
                        }

                        if (!string.IsNullOrEmpty(quotation.Contact?.Email))
                        {
                            col.Item().Text($"Email: {quotation.Contact?.Email}").FontSize(9);
                        }

                        // Dirección del cliente
                        if (!string.IsNullOrEmpty(quotation.Contact?.Street))
                        {
                            var contactAddress = quotation.Contact.Street;
                            if (!string.IsNullOrEmpty(quotation.Contact.ExteriorNumber))
                                contactAddress += $" {quotation.Contact.ExteriorNumber}";
                            if (!string.IsNullOrEmpty(quotation.Contact.InteriorNumber))
                                contactAddress += $" Int. {quotation.Contact.InteriorNumber}";

                            col.Item().Text(contactAddress).FontSize(9);
                        }

                        if (!string.IsNullOrEmpty(quotation.Contact?.Neighborhood) || !string.IsNullOrEmpty(quotation.Contact?.City))
                        {
                            var contactCityLine = "";
                            if (!string.IsNullOrEmpty(quotation.Contact.Neighborhood))
                                contactCityLine += quotation.Contact.Neighborhood;
                            if (!string.IsNullOrEmpty(quotation.Contact.City))
                            {
                                if (!string.IsNullOrEmpty(contactCityLine))
                                    contactCityLine += ", ";
                                contactCityLine += quotation.Contact.City;
                            }

                            col.Item().Text(contactCityLine).FontSize(9);
                        }

                        if (quotation.Contact?.StateID != null || !string.IsNullOrEmpty(quotation.Contact?.PostalCode))
                        {
                            var contactStateLine = "";
                            if (quotation.Contact.StateID != null)
                                contactStateLine += quotation.Contact.StateID.ToString();
                            if (!string.IsNullOrEmpty(quotation.Contact.PostalCode))
                            {
                                if (!string.IsNullOrEmpty(contactStateLine))
                                    contactStateLine += ", ";
                                contactStateLine += $"C.P. {quotation.Contact.PostalCode}";
                            }

                            col.Item().Text(contactStateLine).FontSize(9);
                        }
                    });

                    // Dirección de entrega
                    row.RelativeItem().BorderLeft(3).BorderColor(Colors.Blue.Darken2).PaddingLeft(10).Column(col =>
                    {
                        col.Item().Text("DIRECCIÓN DE ENTREGA").FontSize(11).Bold().FontColor(Colors.Blue.Darken3);
                        col.Item().PaddingTop(5).Text($"{quotation.DeliveryFirstName} {quotation.DeliveryLastName}").FontSize(10).Bold();

                        var address = $"{quotation.DeliveryStreet} {quotation.DeliveryExteriorNumber}";
                        if (!string.IsNullOrEmpty(quotation.DeliveryInteriorNumber))
                            address += $" Int. {quotation.DeliveryInteriorNumber}";
                        col.Item().Text(address).FontSize(9);

                        col.Item().Text($"{quotation.DeliveryNeighborhood}, {quotation.DeliveryCity}").FontSize(9);
                        col.Item().Text($"{quotation.DeliveryStateID}, C.P. {quotation.DeliveryPostalCode}").FontSize(9);

                        if (!string.IsNullOrEmpty(quotation.DeliveryRFC))
                        {
                            col.Item().Text($"RFC: {quotation.DeliveryRFC}").FontSize(9).FontColor(Colors.Grey.Darken1);
                        }
                    });
                });

                // Tabla de items con columnas dinámicas
                column.Item().Element(c => ComposeItemsTable(c, quotation));

                // Notas y totales
                column.Item().Row(row =>
                {
                    // Notas y términos
                    if (!string.IsNullOrEmpty(quotation.Notes) || !string.IsNullOrEmpty(quotation.TermsAndConditions))
                    {
                        row.RelativeItem(2).PaddingRight(10).Column(col =>
                        {
                            if (!string.IsNullOrEmpty(quotation.Notes))
                            {
                                col.Item().Background(Colors.Grey.Lighten3).Padding(8).Column(noteCol =>
                                {
                                    noteCol.Item().Text("NOTAS").FontSize(10).Bold().FontColor(Colors.Blue.Darken3);
                                    noteCol.Item().PaddingTop(3).Text(quotation.Notes).FontSize(9);
                                });
                                col.Spacing(5);
                            }

                            if (!string.IsNullOrEmpty(quotation.TermsAndConditions))
                            {
                                col.Item().Background(Colors.Grey.Lighten3).Padding(8).Column(termCol =>
                                {
                                    termCol.Item().Text("TÉRMINOS Y CONDICIONES").FontSize(10).Bold().FontColor(Colors.Blue.Darken3);
                                    termCol.Item().PaddingTop(3).Text(quotation.TermsAndConditions).FontSize(9);
                                });
                            }
                        });
                    }

                    // Totales
                    row.RelativeItem(1).Column(col =>
                    {
                        col.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(totalCol =>
                        {
                            totalCol.Item().Row(r =>
                            {
                                r.RelativeItem().Text("Subtotal:").FontSize(10);
                                r.AutoItem().Text(quotation.GetSubtotal().ToString("C")).FontSize(10).Bold();
                            });

                            if (quotation.GlobalDiscountPercentage > 0)
                            {
                                totalCol.Item().Row(r =>
                                {
                                    r.RelativeItem().Text($"Descuento Global ({quotation.GlobalDiscountPercentage}%):").FontSize(10);
                                    r.AutoItem().Text($"-{quotation.GetGlobalDiscountAmount():C}").FontSize(10).FontColor(Colors.Green.Darken2);
                                });
                            }

                            if (quotation.ShippingCost > 0)
                            {
                                totalCol.Item().Row(r =>
                                {
                                    r.RelativeItem().Text("Envío/Instalación:").FontSize(10);
                                    r.AutoItem().Text(quotation.ShippingCost.ToString("C")).FontSize(10);
                                });
                            }

                            totalCol.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Darken1);

                            totalCol.Item().PaddingTop(5).Row(r =>
                            {
                                r.RelativeItem().Text("TOTAL:").FontSize(12).Bold().FontColor(Colors.Blue.Darken3);
                                r.AutoItem().Text(quotation.GetTotal().ToString("C")).FontSize(12).Bold().FontColor(Colors.Blue.Darken3);
                            });
                        });
                    });
                });
            });
        }

        private void ComposeItemsTable(IContainer container, Quotation quotation)
        {
            var items = quotation.Items.OrderBy(i => i.DisplayOrder).ToList();

            // Determinar qué columnas mostrar basándose en si se usan o no
            var columnConfig = DetermineVisibleColumns(items);

            container.Table(table =>
            {
                // Definir columnas dinámicamente
                int columnCount = columnConfig.Count(c => c.Value);

                // Columna #
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25); // #

                    if (columnConfig["Product"]) columns.RelativeColumn(3); // Producto
                    if (columnConfig["Variant"]) columns.RelativeColumn(1.5f); // Variante
                    if (columnConfig["HeadingStyle"]) columns.RelativeColumn(1.5f); // Estilo Cab.
                    if (columnConfig["Color"]) columns.RelativeColumn(1.5f); // Color
                    if (columnConfig["ControlSide"]) columns.RelativeColumn(1); // Lado Ctrl.
                    if (columnConfig["ValanceType"]) columns.RelativeColumn(1); // Galería
                    if (columnConfig["MountingType"]) columns.RelativeColumn(1); // Montaje
                    if (columnConfig["Width"]) columns.RelativeColumn(1); // Ancho
                    if (columnConfig["Height"]) columns.RelativeColumn(1); // Alto
                    if (columnConfig["Quantity"]) columns.RelativeColumn(1); // Cantidad
                    if (columnConfig["UnitPrice"]) columns.RelativeColumn(1.2f); // P. Unit.
                    if (columnConfig["DiscountedPrice"]) columns.RelativeColumn(1.2f); // P. Desc.
                    if (columnConfig["Total"]) columns.RelativeColumn(1.5f); // Total
                });

                // Encabezado de la tabla
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("#").FontSize(9).Bold().FontColor(Colors.White);

                    if (columnConfig["Product"]) header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Producto").FontSize(9).Bold().FontColor(Colors.White);
                    if (columnConfig["Variant"]) header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Variante").FontSize(9).Bold().FontColor(Colors.White);
                    if (columnConfig["HeadingStyle"]) header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Estilo Cab.").FontSize(9).Bold().FontColor(Colors.White);
                    if (columnConfig["Color"]) header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Color").FontSize(9).Bold().FontColor(Colors.White);
                    if (columnConfig["ControlSide"]) header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Lado Ctrl.").FontSize(9).Bold().FontColor(Colors.White);
                    if (columnConfig["ValanceType"]) header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Galería").FontSize(9).Bold().FontColor(Colors.White);
                    if (columnConfig["MountingType"]) header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Montaje").FontSize(9).Bold().FontColor(Colors.White);
                    if (columnConfig["Width"]) header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Ancho").FontSize(9).Bold().FontColor(Colors.White);
                    if (columnConfig["Height"]) header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Alto").FontSize(9).Bold().FontColor(Colors.White);
                    if (columnConfig["Quantity"]) header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Cant.").FontSize(9).Bold().FontColor(Colors.White);
                    if (columnConfig["UnitPrice"]) header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("P. Unit.").FontSize(9).Bold().FontColor(Colors.White);
                    if (columnConfig["DiscountedPrice"]) header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("P. Desc.").FontSize(9).Bold().FontColor(Colors.White);
                    if (columnConfig["Total"]) header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Total").FontSize(9).Bold().FontColor(Colors.White);
                });

                // Filas de items
                int itemNumber = 1;
                foreach (var item in items)
                {
                    var bgColor = itemNumber % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

                    table.Cell().Background(bgColor).Padding(5).Text(itemNumber.ToString()).FontSize(8);

                    if (columnConfig["Product"])
                    {
                        table.Cell().Background(bgColor).Padding(5).Column(col =>
                        {
                            col.Item().Text(item.Product?.Name ?? "").FontSize(8).Bold();
                            col.Item().Text(item.Product?.ProductType?.Name ?? "").FontSize(7).FontColor(Colors.Grey.Darken1);
                            if (!string.IsNullOrEmpty(item.Description))
                            {
                                col.Item().Text(item.Description).FontSize(7).Italic().FontColor(Colors.Grey.Darken1);
                            }
                            if (!string.IsNullOrEmpty(item.Notes))
                            {
                                col.Item().PaddingTop(2).Text($"Nota: {item.Notes}").FontSize(7).Italic().FontColor(Colors.Blue.Darken1);
                            }
                        });
                    }

                    if (columnConfig["Variant"])
                        table.Cell().Background(bgColor).Padding(5).Text(GetVariantText(item)).FontSize(8);

                    if (columnConfig["HeadingStyle"])
                        table.Cell().Background(bgColor).Padding(5).Text(GetHeadingStyleText(item)).FontSize(8);

                    if (columnConfig["Color"])
                        table.Cell().Background(bgColor).Padding(5).Text(GetColorText(item)).FontSize(8);

                    if (columnConfig["ControlSide"])
                        table.Cell().Background(bgColor).Padding(5).Text(GetControlSideText(item.ControlSide)).FontSize(8);

                    if (columnConfig["ValanceType"])
                        table.Cell().Background(bgColor).Padding(5).Text(GetValanceTypeText(item.ValanceType)).FontSize(8);

                    if (columnConfig["MountingType"])
                        table.Cell().Background(bgColor).Padding(5).Text(GetMountingTypeText(item.MountingType)).FontSize(8);

                    if (columnConfig["Width"])
                        table.Cell().Background(bgColor).Padding(5).AlignRight().Text(item.Width?.ToString("0.00") ?? "-").FontSize(8);

                    if (columnConfig["Height"])
                        table.Cell().Background(bgColor).Padding(5).AlignRight().Text(item.Height?.ToString("0.00") ?? "-").FontSize(8);

                    if (columnConfig["Quantity"])
                    {
                        table.Cell().Background(bgColor).Padding(5).Column(col =>
                        {
                            col.Item().AlignRight().Text(item.Quantity.ToString("0.00")).FontSize(8);
                            if (item.GetEffectiveQuantity() != item.Quantity)
                            {
                                col.Item().AlignRight().Text($"Ef: {item.GetEffectiveQuantity():0.00}").FontSize(7).FontColor(Colors.Grey.Darken1);
                            }
                        });
                    }

                    if (columnConfig["UnitPrice"])
                        table.Cell().Background(bgColor).Padding(5).AlignRight().Text(item.UnitPrice.ToString("C")).FontSize(8);

                    if (columnConfig["DiscountedPrice"])
                    {
                        table.Cell().Background(bgColor).Padding(5).Column(col =>
                        {
                            col.Item().AlignRight().Text(item.DiscountedPrice.ToString("C")).FontSize(8);
                            if (item.HasDiscount())
                            {
                                col.Item().AlignRight().Text($"Ahorro: {item.GetDiscountAmount():C}").FontSize(7).FontColor(Colors.Green.Darken2);
                            }
                        });
                    }

                    if (columnConfig["Total"])
                        table.Cell().Background(bgColor).Padding(5).AlignRight().Text(item.GetLineTotal().ToString("C")).FontSize(8).Bold();

                    itemNumber++;
                }
            });
        }

        private Dictionary<string, bool> DetermineVisibleColumns(List<QuotationItem> items)
        {
            return new Dictionary<string, bool>
            {
                ["Product"] = true, // Siempre mostrar
                ["Variant"] = items.Any(i => !string.IsNullOrEmpty(i.Variant)),
                ["HeadingStyle"] = items.Any(i => !string.IsNullOrEmpty(i.HeadingStyle)),
                ["Color"] = items.Any(i => i.ProductColorId.HasValue),
                ["ControlSide"] = items.Any(i => i.ControlSide.HasValue),
                ["ValanceType"] = items.Any(i => i.ValanceType.HasValue),
                ["MountingType"] = items.Any(i => i.MountingType.HasValue),
                ["Width"] = items.Any(i => i.Width.HasValue),
                ["Height"] = items.Any(i => i.Height.HasValue),
                ["Quantity"] = true, // Siempre mostrar
                ["UnitPrice"] = true, // Siempre mostrar
                ["DiscountedPrice"] = true, // Siempre mostrar
                ["Total"] = true // Siempre mostrar
            };
        }

        private string GetVariantText(QuotationItem item)
        {
            if (item.ProductTypeVariant != null)
                return item.ProductTypeVariant.Name;
            if (!string.IsNullOrEmpty(item.Variant))
                return item.Variant;
            return "-";
        }

        private string GetHeadingStyleText(QuotationItem item)
        {
            if (item.ProductTypeHeadingStyle != null)
                return item.ProductTypeHeadingStyle.Name;
            if (!string.IsNullOrEmpty(item.HeadingStyle))
                return item.HeadingStyle;
            return "-";
        }

        private string GetColorText(QuotationItem item)
        {
            if (item.ProductColor != null)
                return $"{item.ProductColor.Name}\n{item.ProductColor.SKU}";
            return "-";
        }

        private string GetControlSideText(ControlSide? controlSide)
        {
            return controlSide switch
            {
                ControlSide.Left => "Izquierda",
                ControlSide.Right => "Derecha",
                _ => "-"
            };
        }

        private string GetValanceTypeText(ValanceType? valanceType)
        {
            return valanceType switch
            {
                ValanceType.No => "No",
                ValanceType.Fascia => "Fascia",
                ValanceType.FC => "F/C",
                ValanceType.Cofre => "Cofre",
                ValanceType.CofreC => "Cofre/C",
                ValanceType.Lambrequin => "Lambrequín",
                ValanceType.GalPVC => "Gal de PVC",
                ValanceType.GalConRelleno => "Gal con relleno",
                ValanceType.GalLisa => "Gal lisa",
                _ => "-"
            };
        }

        private string GetMountingTypeText(MountingType? mountingType)
        {
            return mountingType switch
            {
                MountingType.Techo => "Techo",
                MountingType.Pared => "Pared",
                _ => "-"
            };
        }

        private string GetStatusText(QuotationStatus status)
        {
            return status switch
            {
                QuotationStatus.Draft => "BORRADOR",
                QuotationStatus.Sent => "ENVIADA",
                QuotationStatus.Accepted => "ACEPTADA",
                QuotationStatus.Rejected => "RECHAZADA",
                QuotationStatus.Expired => "EXPIRADA",
                _ => status.ToString()
            };
        }

        private string GetStatusColor(QuotationStatus status)
        {
            return status switch
            {
                QuotationStatus.Draft => Colors.Grey.Darken2,
                QuotationStatus.Sent => Colors.Blue.Medium,
                QuotationStatus.Accepted => Colors.Green.Darken2,
                QuotationStatus.Rejected => Colors.Orange.Darken2,
                QuotationStatus.Expired => Colors.Red.Darken2,
                _ => Colors.Grey.Medium
            };
        }

        private void ComposeFooter(IContainer container, Quotation quotation)
        {
            container.AlignCenter().Column(column =>
            {
                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                column.Item().PaddingTop(5).Text(text =>
                {
                    text.Span("Creado por: ").FontSize(8).FontColor(Colors.Grey.Darken1);
                    text.Span($"{quotation.CreatedByUser?.FirstName} {quotation.CreatedByUser?.LastName}").FontSize(8).Bold().FontColor(Colors.Grey.Darken2);
                    text.Span(" | ").FontSize(8).FontColor(Colors.Grey.Darken1);
                    text.Span($"Fecha de creación: {quotation.CreatedAt.ToLocalTime():dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });
        }
    }
}
