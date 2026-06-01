namespace Contract;

public class ModifyPasswordRequestDTO
{
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
