namespace Domain;

public sealed class Phone
{
    public string Value { get; }

    private Phone(string value)
    {
        Value = value;
    }

    private Phone()
    {
        Value = null!;
    }

    public static Result<Phone> TryMakePhone(string number)
    {
        if(string.IsNullOrWhiteSpace(number))
        {
            return Result<Phone>.Fail("Telefono no puede estar vacío");
        }

        var phoneNormalized = number.Trim();

        if(!phoneNormalized.All(char.IsDigit))
        {
            return Result<Phone>.Fail("Telefono solo puede contener números");
        }

        return Result<Phone>.Ok(new Phone(phoneNormalized));
    }

    public override string ToString()
    {
        return Value;
    }
}
