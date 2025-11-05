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


# Una vez eliminando los colores, revisar:

⏺ Basándome en la búsqueda, estos son los archivos críticos que debes revisar:

  1. Base de datos y configuración:

  - Data/ApplicationDbContext.cs - Remover configuración de la tabla Color
  - Nueva migración necesaria - Para agregar campos nuevos a ProductColor y remover
  tabla Color

  2. Modelo obsoleto:

  - Models/Color.cs - Eliminar este archivo (ya no lo necesitas)

  3. Controlador obsoleto:

  - Controllers/ColorController.cs - Eliminar o revisar (ya no necesitas gestión
  separada de colores)

  4. Vistas obsoletas de Color:

  - Views/Color/Index.cshtml
  - Views/Color/Create.cshtml
  - Views/Color/Edit.cshtml
  - Views/Color/Details.cshtml
  - Views/Color/Delete.cshtml
  Acción: Eliminar toda la carpeta Views/Color/

  5. Archivos que usan Color:

  - Controllers/ProductController.cs - Revisar método ManageColors
  - Services/ProductService.cs - Revisar lógica de colores
  - Services/PriceListService.cs - Verificar si usa Color
  - Views/Product/ManageColors.cshtml - Actualizar para trabajar sin tabla Color
  separada


 dotnet ef migrations add RefactorProductColorEmbedColorName

