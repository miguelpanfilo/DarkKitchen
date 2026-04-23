using AlmacenamientoAbstracciones;
using Dominio;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Users;

namespace LogicaDeNegocio.Initialization;

public class InitializationService : IInitializationService
{
    private const string AdminEmail = "admin@gmail.com";
    private const string AdminName = "Admin";
    private const string AdminLastName = "System";
    private const string AdminPhone = "099123456";
    private const string AdminPassword = "PasswordSeguraee3!";

    private readonly IRepositoryUser _repositoryUser;
    private readonly IProductRepository _productRepository;

    public InitializationService(
        IRepositoryUser repositoryUser,
        IProductRepository productRepository)
    {
        _repositoryUser = repositoryUser;
        _productRepository = productRepository;
    }

    public void EnsureInitialized()
    {
        EnsureAdminCreated();
        EnsureProductsCreated();
    }

    private void EnsureAdminCreated()
    {
        if(_repositoryUser.EmailExist(AdminEmail))
        {
            return;
        }

        var phoneResult = Phone.TryMakePhone(AdminPhone);
        if(!phoneResult.Success)
        {
            throw new Exception(phoneResult.ErrorMessage);
        }

        var adminResult = User.MakeIntern(
            AdminName,
            AdminLastName,
            AdminEmail,
            phoneResult.Value!,
            AdminPassword,
            UserRoles.Administrative);

        if(!adminResult.Success)
        {
            throw new Exception(adminResult.ErrorMessage);
        }

        _repositoryUser.Add(adminResult.Value!);
    }

    private void EnsureProductsCreated()
    {
        var existingProducts = _productRepository.GetAll();

        if(existingProducts.Any())
        {
            return;
        }

        var products = new List<Product>
        {
            Product.MakeProduct(
                "BURG01",
                "Hamburguesa completa",
                "Hamburguesa con cheddar, lechuga, tomate y salsa especial.",
                "Burgers",
                "Comida",
                350,
                new List<ProductPhoto>
                {
                    new ProductPhoto("seed/burguesa.jpg")
                }),

            Product.MakeProduct(
                "PIZZ01",
                "Pizza muzzarella grande",
                "Pizza grande con salsa, muzzarella y oregano fresco.",
                "Pizzas",
                "Comida",
                520,
                new List<ProductPhoto>
                {
                    new ProductPhoto("seed/pizza.jpg")
                }),

            Product.MakeProduct(
                "REFR01",
                "Refresco cola 1.5L",
                "Bebida cola de un litro y medio para acompanar pedidos.",
                "Bebidas",
                "Bebida",
                140,
                new List<ProductPhoto>
                {
                    new ProductPhoto("seed/refresco.jpg")
                })
        };

        foreach(var product in products)
        {
            _productRepository.Create(product);
        }
    }
}
