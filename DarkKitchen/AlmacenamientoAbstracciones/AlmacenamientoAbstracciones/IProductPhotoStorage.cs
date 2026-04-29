namespace AlmacenamientoAbstracciones;

public interface IProductPhotoStorage
{
    string Save(string base64, string extension);
    public void Delete(string path);
}
