namespace Cool.Application.Dtos.Users;

public class CreateUserModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string TenantName { get; set; }
    public Guid TenantId { get; set; }
}
