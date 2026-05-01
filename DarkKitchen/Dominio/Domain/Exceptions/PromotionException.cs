namespace Domain.Exceptions;

public class PromotionException : Exception
{
    public PromotionException(string menssage)
        : base(menssage)
    {
    }
}
