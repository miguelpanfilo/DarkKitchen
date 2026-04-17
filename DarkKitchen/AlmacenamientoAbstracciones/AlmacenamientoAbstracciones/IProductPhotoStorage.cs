using Contratos;

namespace AlmacenamientoAbstracciones;

public interface IProductPhotoStorage
{
    string Save(PhotoDTO photo);
    public void Delete(string path);
}
