# Custom DataTable Component

Componente de tabla reutilizable completamente personalizado, sin dependencias de Bootstrap.

## Ubicación de archivos

```
/ViewComponents/
  - DataTableViewComponent.cs           # View Component principal
  - README.md                            # Esta documentación

/Models/ViewComponents/
  - DataTableConfig.cs                   # Modelos de configuración

/Views/Shared/Components/DataTable/
  - Default.cshtml                       # Vista del componente

/wwwroot/css/
  - custom-datatable.css                 # Estilos personalizados
```

## Implementación en producción

El componente está actualmente implementado en:

- **Product/Index** - Vista de listado de productos

## Uso básico

### 1. Incluir el CSS en tu layout o vista

```html
<link rel="stylesheet" href="~/css/custom-datatable.css" />
```

### 2. Configurar el componente en tu controlador

```csharp
var config = new DataTableConfig<Product>
{
    TableId = "myTable",
    Data = products,
    MakeRowsClickable = true,
    RowClickUrl = "/Product/Details/{id}",
    GetRowId = p => p.Id,
    Columns = new List<DataTableColumn<Product>>
    {
        new DataTableColumn<Product>
        {
            Header = "Nombre",
            ValueSelector = p => p.Name
        },
        // ... más columnas
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
        // ... más acciones
    }
};

return View(config);
```

### 3. Invocar el componente en tu vista

```razor
@model DataTableConfig<Product>

@await Component.InvokeAsync("DataTable", Model)
```

## Configuración

### DataTableConfig<T>

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `TableId` | string | ID único de la tabla |
| `Data` | List<T> | Lista de datos a mostrar |
| `Columns` | List<DataTableColumn<T>> | Configuración de columnas |
| `ShowActions` | bool | Mostrar columna de acciones |
| `Actions` | List<DataTableAction<T>> | Lista de acciones disponibles |
| `MakeRowsClickable` | bool | Hacer filas clickeables |
| `RowClickUrl` | string | URL template para clicks (usa {id}) |
| `GetRowId` | Func<T, int> | Función para obtener ID de la fila |

### DataTableColumn<T>

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Header` | string | Texto del encabezado |
| `ValueSelector` | Func<T, object?> | Función que obtiene el valor a mostrar |
| `CssClassSelector` | Func<T, string>? | Clase CSS condicional |

### DataTableAction<T>

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Label` | string | Texto de la acción |
| `Icon` | string | Icono de Bootstrap Icons (sin prefijo bi-) |
| `UrlSelector` | Func<T, string> | Función que genera la URL |
| `CssClass` | string | Clases CSS adicionales |
| `IsDivider` | bool | Si es un divisor en el dropdown |

## Ejemplos de columnas con badges

```csharp
new DataTableColumn<Product>
{
    Header = "Estado",
    ValueSelector = p => p.IsActive
        ? new HtmlString("<span class=\"custom-badge custom-badge-success\">Activo</span>")
        : new HtmlString("<span class=\"custom-badge custom-badge-danger\">Inactivo</span>")
}
```

## Clases de badges disponibles

- `custom-badge-primary` - Azul
- `custom-badge-success` - Verde
- `custom-badge-danger` - Rojo
- `custom-badge-warning` - Amarillo
- `custom-badge-info` - Celeste
- `custom-badge-secondary` - Gris

## Características

- ✅ Diseño moderno y limpio
- ✅ Sin dependencias de Bootstrap
- ✅ Usa la fuente Geist del sistema
- ✅ Filas clickeables opcionales
- ✅ Dropdown de acciones con tres puntos
- ✅ Estilos hover suaves
- ✅ Completamente personalizable
- ✅ Responsive
- ✅ Color de texto: #171717

## Personalización de estilos

Puedes sobrescribir los estilos modificando `/wwwroot/css/custom-datatable.css` o agregando tus propios estilos CSS.

## Ejemplo de implementación

Puedes ver un ejemplo completo de implementación en:
- **Controllers/ProductController.cs** - Método `Index()`
- **Views/Product/Index.cshtml** - Vista que usa el componente
