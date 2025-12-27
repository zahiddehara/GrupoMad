namespace GrupoMad.Models.ViewComponents
{
    public class DataTableConfig<T>
    {
        public string TableId { get; set; } = "dataTable";
        public List<T> Data { get; set; } = new();
        public List<DataTableColumn<T>> Columns { get; set; } = new();
        public bool ShowActions { get; set; } = true;
        public List<DataTableAction<T>> Actions { get; set; } = new();
        public bool EnableFilters { get; set; } = false;
        public DataTableFilters? Filters { get; set; }
        public bool MakeRowsClickable { get; set; } = false;
        public string? RowClickUrl { get; set; }
        public Func<T, int>? GetRowId { get; set; }
    }

    public class DataTableFilters
    {
        public bool ShowSearchFilter { get; set; } = true;
        public string SearchPlaceholder { get; set; } = "Escriba para buscar...";
        public List<string> SearchAttributes { get; set; } = new(); // data-attributes para buscar
        public List<DataTableDropdownFilter> DropdownFilters { get; set; } = new();
    }

    public class DataTableDropdownFilter
    {
        public string Id { get; set; } = "";
        public string Label { get; set; } = "";
        public string DataAttribute { get; set; } = ""; // data-attribute que contiene el valor a filtrar
        public string Placeholder { get; set; } = "Seleccione...";
    }

    public class DataTableColumn<T>
    {
        public string Header { get; set; } = "";
        public Func<T, object?> ValueSelector { get; set; } = _ => null;
        public Func<T, string>? CssClassSelector { get; set; }
        public bool IsSortable { get; set; } = false;
        public bool IsFilterable { get; set; } = false;
        public string? FilterKey { get; set; }
        public string? DataAttribute { get; set; } // Nombre del data-attribute (ej: "name", "sku")
        public Func<T, string>? DataAttributeValueSelector { get; set; } // Función para obtener el valor del data-attribute
    }

    public class DataTableAction<T>
    {
        public string Label { get; set; } = "";
        public string Icon { get; set; } = "";
        public Func<T, string> UrlSelector { get; set; } = _ => "#";
        public string CssClass { get; set; } = "dropdown-item";
        public bool IsDivider { get; set; } = false;
    }
}
