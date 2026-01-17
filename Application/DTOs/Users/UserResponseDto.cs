using Domain.Enums;

namespace Application.DTOs.Users;
public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public UserRole Role { get; set; } = UserRole.Employee;
}
