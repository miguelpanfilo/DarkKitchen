# Mecanismo de extensibilidad — Product Importers

El sistema descubre importadores de productos en runtime a partir de DLLs ubicadas en la carpeta `plugins/` junto al ejecutable. No requiere recompilar ni modificar el sistema base.

## Pasos para crear un importador externo

### 1. Referenciar los contratos

Crear un proyecto Class Library (.NET 8) y agregar referencia a `Contract.dll` y `BusinessLogicsAbstractions.dll` (ambos archivos se encuentran en la carpeta `plugins/` junto al ejecutable):

```xml
<ItemGroup>
  <Reference Include="Contract">
    <HintPath>Contract.dll</HintPath>
  </Reference>
  <Reference Include="BusinessLogicsAbstractions">
    <HintPath>BusinessLogicsAbstractions.dll</HintPath>
  </Reference>
</ItemGroup>
```

### 2. Implementar la interfaz

La interfaz `IProductImporter` define tres miembros obligatorios:

- `Id`: identificador único del formato (ej: `"excel"`, `"tsv"`)
- `Name`: nombre legible que se muestra en el frontend (ej: `"Excel"`)
- `Import(Stream)`: recibe el archivo como stream y retorna la lista de productos

```csharp
using Contract;
using BusinessLogicsAbstractions;

public class MiImportador : IProductImporter
{
    public string Id => "mi-formato";
    public string Name => "Mi Formato";

    public List<ImportedProductDto> Import(Stream fileStream)
    {
        try
        {
            // leer el stream usando la librería de tu formato
            // y retornar la lista de ImportedProductDto
        }
        catch (ExcepcionEspecificaDeTuLibreria)
        {
            // Capturar SOLO la excepción de parseo de tu librería,
            // no Exception genérico — así no se ocultan bugs propios.
            // Ejemplo: JsonException para JSON, XmlException para XML, etc.
            throw new ValidationException("El archivo tiene un formato inválido");
        }
    }
}
```

`ImportedProductDto` tiene los siguientes campos:

```csharp
public class ImportedProductDto
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CommercialLine { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public List<string> PhotoPaths { get; set; }
}
```

### 3. Compilar y desplegar

Compilar el proyecto como Class Library. Copiar el `.dll` resultante a la carpeta `plugins/` junto al ejecutable de la WebApi.

Si el plugin usa librerías NuGet externas (no incluidas en el runtime de .NET), también deben copiarse sus DLLs a la carpeta `plugins/`. Las librerías del runtime de .NET (`System.Text.Json`, `System.Xml.Serialization`, etc.) no necesitan copiarse porque ya están disponibles.

**Ejemplo:** un plugin que usa `CsvHelper` debe copiar tanto `CsvImporter.dll` como `CsvHelper.dll` a `plugins/`.

### 4. El sistema lo descubre automáticamente

Al detectar el nuevo DLL, el `FileSystemWatcher` notifica via SignalR al frontend, que actualiza la lista de importadores disponibles sin necesidad de reiniciar el servidor.
