namespace BusinessLogicsAbstractions.Imports;

public interface IProductImporterProvider
{
    List<IProductImporter> GetImporters();
}
