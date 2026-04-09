namespace Dominio;

public sealed class Phone
{
    public string Value { get; }

    private Phone(string value)
    {
        Value = value;
    }

    public static Result<Phone> TryMakePhone(string number)
    {
        if(string.IsNullOrWhiteSpace(number))
        {
            return Result<Phone>.Fail("Phone cannot be empty");
        }

        var phoneNormalized = number.Trim();

        if(!phoneNormalized.All(char.IsDigit))
        {
            return Result<Phone>.Fail("Phone must contain only digits");
        }

        return Result<Phone>.Ok(new Phone(phoneNormalized));
    }

    public override string ToString()
    {
        return Value;
    }
}
