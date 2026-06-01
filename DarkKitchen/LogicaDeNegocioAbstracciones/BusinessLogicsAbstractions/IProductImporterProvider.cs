namespace BusinessLogicsAbstractions;

public interface IProductImporterProvider
{
    List<IProductImporter> GetImporters();
}
