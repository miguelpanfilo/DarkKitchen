namespace Contratos;

public class ModificarClaveRequestDTO
{
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
