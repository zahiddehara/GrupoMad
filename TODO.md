# Crear controller y views para Color

# Tipos de productos con sus campos (todos los nombres seran en idioma ingles):

Cada color tendra su propio SKU, como se manejaria esto?
El campo "ControlSide" no iria en la tabla Product

## Accesorios

- Color

## Curtains

- Color
- Finish (Este campo va en la tabla de QuotationItem? , porque las curtains se personalizan al cotizar. Hay como 10 finishes para las curtains)
- Cortinero? (Este campo es booleano, en la cotizacion se podra elegir si la curtain lleva cortinero o no, entoncces va en la tabla QuotationItem?)
- Cordon or Baston (En la tabla QuotationItem? Son 2 opciones, cordon o baston)
- Lado de control (izquierdo o derecho)
- Una pieza o dos piezas (En la tabla QuotationItem? Son 2 opciones, una pieza o dos piezas)

## Toldo

- Color
- Manual / Motorizado (En la tabla QuotationItem?)
- Retractil o Vertical (En la tabla QuotationItem?)

## Cortinero

- Cordon or Baston (En la tabla QuotationItem?)
- Lado de control (izquierdo o derecho)
- Techo / Pared (En la tabla QuotationItem?)

## Dia y noche

- Color 1
- Color 2
- Exterior / Interior (En la tabla QuotationItem?)
- Lado de control (izquierdo o derecho) 

## Panel

- Color
- Lado de control 
- Numero de vias (En la tabla QuotationItem?, son 3 opciones "3 vias, 4 vias y 5 vias")
- Sin galeria / con fascia (En la tabla QuotationItem?, 2 opciones: "Sin galeria" o "Con fascia")

## Persianas

- Color
- Exterior / Interior (En la tabla QuotationItem?)
- Lado de control (izquierdo o derecho)

## Wave

- Color
- Manual / Motorizado (En la tabla QuotationItem?, son 2 opciones: "Manual" y "Motorizado")
- Lado de control (izquierdo o derecho)


# Logica para aplicar descuento

  Para la lógica de aplicar el descuento según el tipo de precio activo, podrías crear un
  método helper en tu PriceListItem o en tu lógica de negocio:

  // Ejemplo de método helper
  public decimal GetFinalPrice()
  {
      decimal basePrice = 0;

      if (PricePerUnit.HasValue)
          basePrice = PricePerUnit.Value;
      else if (PricePerSquareMeter.HasValue)
          basePrice = PricePerSquareMeter.Value;
      else if (PricePerLinearMeter.HasValue)
          basePrice = PricePerLinearMeter.Value;

      // Aplicar descuento si existe
      if (DiscountAmount.HasValue)
          basePrice -= DiscountAmount.Value;

      return basePrice > 0 ? basePrice : 0; // Evitar precios negativos
  }


## Vistas administrador y no administrador

  Opciones disponibles:

  1. Condicionales en vistas compartidas (Approach actual)

  @if (User.IsInRole("Administrator"))
  {
      // Contenido para admin
  }
  else
  {
      // Contenido para otros usuarios
  }
  Pros: Simple, menos archivos, código DRY
  Contras: Vistas complejas con muchos @if, difícil mantener cuando crece

  ---
  2. Partial Views por rol

  // En la vista principal
  @if (ViewBag.IsAdmin)
  {
      <partial name="_AdminFilters" />
  }
  else
  {
      <partial name="_UserFilters" />
  }
  Pros: Modular, código separado y limpio
  Contras: Más archivos, pero mejor organizado

  ---
  3. View Components (Recomendado para lógica compleja)

  // ContactFiltersViewComponent.cs
  public class ContactFiltersViewComponent : ViewComponent
  {
      public async Task<IViewComponentResult> InvokeAsync()
      {
          var isAdmin = User.IsInRole("Administrator");
          // Lógica específica
          return View(isAdmin ? "Admin" : "User", model);
      }
  }

  // En la vista
  @await Component.InvokeAsync("ContactFilters")
  Pros: Lógica separada, testeable, reutilizable
  Contras: Más código inicial

  ---
  4. Areas para administración (Recomendado para separación total)

  /Areas
    /Admin
      /Controllers
        /ContactController.cs
      /Views
        /Contact
          /Index.cshtml
    /User (o sin área para usuarios normales)
      /Controllers
      /Views
  Pros: Separación completa, escalable, URLs distintas (/Admin/Contact)
  Contras: Duplicación de código, más estructura

  ---
  Mi recomendación para tu proyecto:

  Usa un enfoque híbrido basado en la complejidad de cada vista:

  Para diferencias pequeñas (como filtros):

  ✅ Usar Partial Views

  // Views/Contact/Index.cshtml
  <div class="row mb-4">
      @if (ViewBag.IsAdmin)
      {
          <partial name="_AdminSearchFilters" model="Model" />
      }
      else
      {
          <partial name="_UserSearchFilters" model="Model" />
      }
  </div>

  // Views/Contact/_AdminSearchFilters.cshtml
  // Filtros completos con select de empresa

  // Views/Contact/_UserSearchFilters.cshtml
  // Solo campo de búsqueda

  Para vistas completamente diferentes:

  ✅ Considerar un Area de Admin

  Esto aplica para:
  - User management (solo admin)
  - Store management (solo admin)
  - Company management (solo admin)
  - Reportes avanzados (solo admin)

  /Areas/Admin/Controllers/UserController.cs
  /Areas/Admin/Views/User/Index.cshtml

  Para las vistas compartidas con pequeñas diferencias:

  ✅ Mantener condicionales simples o Partial Views

  Esto aplica para:
  - Contact (todos los roles, pero filtros diferentes)
  - Quotation (todos los roles, pero permisos diferentes)
  - Home (todos los roles, widgets diferentes)

  ---
  Estructura recomendada para tu proyecto:

  /Areas
    /Admin                          ← Para funcionalidad exclusiva de admin
      /Controllers
        UserController.cs
        StoreController.cs
        CompanyController.cs
        ReportsController.cs
      /Views
        /User/Index.cshtml
        /Store/Index.cshtml
        /Company/Index.cshtml

  /Controllers                      ← Para funcionalidad compartida
    ContactController.cs
    QuotationController.cs
    HomeController.cs

  /Views
    /Contact
      Index.cshtml                  ← Vista principal
      _AdminSearchFilters.cshtml    ← Partial para admin
      _UserSearchFilters.cshtml     ← Partial para usuarios
    /Quotation
      Index.cshtml
      _AdminActions.cshtml
    /Shared
      _Layout.cshtml
      _AdminSidebar.cshtml          ← Si necesitas sidebar diferente

  ---
  Implementación rápida:

  ¿Quieres que te ayude a:
  1. Crear un Area de Admin y mover UserController, StoreController, CompanyController ahí?
  2. Refactorizar Contact/Index.cshtml usando Partial Views para los filtros?
  3. Ambas cosas?

  La opción más limpia y escalable sería hacer ambas, pero podemos empezar con lo que consideres más
  prioritario.

