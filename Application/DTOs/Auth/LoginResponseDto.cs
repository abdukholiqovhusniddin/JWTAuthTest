using Domain.Enums;

namespace Application.DTOs.Auth;
public class LoginResponseDto
{
    public Guid UserId { get; set; }
    public UserRole Role { get; set; } = UserRole.Employee;
    public TokenResponseDto Tokens { get; set; } = null!;
}
