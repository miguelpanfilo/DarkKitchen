using Contratos;

namespace AlmacenamientoAbstracciones;

public interface IProductPhotoStorage
{
    string Save(PhotoDTO photo);
}
