## CSV LINQPad Driver

A lightweight dynamic driver for querying any CSV files for LINQPad 
built with SolarWinds in mind.

### Terminology

* Schema model – a simple representation of the files – mainly their paths and CSV headers
* Schema – nested structure of the Schema model used by LINQPad and visible to the user
* Syntax tree – abstract representation of multiple classes that represent the schema
  * Data classes – per each CSV file a data class is generated containing all headers as public fields 
  * DataContext class – named "TypedDataContext" by default, contains all generated data classes as 
queryable `IEnumerable<DataType>` properties
  * DataExtensions class – contains dynamically appended extension methods usable when querying 
specific data classes

### Flow

Right after the user decides to use driver when opening a connection, a basic
window appears for a selection of a folder containing the desired CSV files – 
in SolarWinds this is usually the DB folder in diagnostics.

This is implemented in `DynamicDriver.ShowConnectionDialog` along with saving
the `SelectedPath`.

After that, the `DynamicDriver.GetSchemaAndBuildAssembly` is automatically
called as mentioned in the LINQPad driver documentation.
It uses `DriverResultBuilder` to fluently:
1. Use `ModelReader` to read a schema model from the CSV files.
2. Then use the model to build "syntax tree" using `SyntaxTreeBuilder`
3. Then use the model to build "schema" using `SchemaBuilder`
4. Emit the built "syntax tree" into provided `assemblyToBuild` using `CodeEmitter`
5. Return the built "schema" to LINQPad

### How do they work?

#### `ModelReader` – How does schema model reading work?
1. Reads all files in a selected file path
2. Returns `FileModel` for each CSV file containing its path and headers

#### `SyntaxTreeBuilder` – How does syntax tree building work?
1. On construction, creates the DataContext class
2. On each call of `AddModel`:
   * creates a new Data class for the specific `FileModel`
   * adds an `IEnumerable` property of the Data class to the DataContext class
3. When built, returns `SyntaxTree` containing:
   * One DataContext class with many `IEnumerable` properties, one for each Data class
   * Many Data classes
   * One DataExtensions class containing quality-of-life extensions for querying specific Data classes,
if they are available

#### `SchemaBuilder` – How does LINQPad schema building work?
Groups the File models into categories based on their prefixes – parts before underscore or before
a SolarWinds-specific prefix (e.g. VoIP). If there is only one File model with the prefix, it is 
not grouped.

1. On each call of `AddModel`, assigns the specific `FileModel` to an existing category or 
creates a new one
2. When built, returns both types sorted alphabetically:
   * multiple "category" `ExplorerItem` grouped by prefixes, each containing "data" `ExplorerItem`, 
   each containing "column" `ExplorerItem`
   * multiple "data" `ExplorerItem`, each containing "column" `ExplorerItem`

#### `CodeEmitter` – How does syntax tree emitting work?
Compiles the provided `SyntaxTree` and emits the resulting stream into the provided assembly.

#### How does querying data work?
Each data property in DataContext class automatically calls `CsvLinqPadDriver.Csv.CsvReader` with
the path to its CSV file which in turn calls `CsvHelper.CsvReader` (a NuGet package dependency).

#### How do UserExtensions work?
**Static**

Using the `DynamicDriver.GetNamespacesToAdd`, already accessible to the user so that 
they can call for example:
  * `Nodes.GetStatistics(n => n.ObjectSubType)` to see the used polling types
  * `Engines.GetRatio(e => e.Elements.ToInt32() > 12000)` to see the ratio of over-utilized engines

**Dynamic**

Not available to the user, only via extensions methods in `DataExtensions` class, so that the
namespace is not polluted and the user can't call `Nodes.ParseCortex()` or 
`Engines.WhereDelayed<Nodes>("1999-11-30".ToDateTime())`