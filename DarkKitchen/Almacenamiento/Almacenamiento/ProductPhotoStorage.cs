using AlmacenamientoAbstracciones;
using Contratos;

namespace Almacenamiento;

public class ProductPhotoStorage : IProductPhotoStorage
{
    public string Save(PhotoDTO photo) => throw new NotImplementedException();

    public void Delete(string path) => throw new NotImplementedException();
}
