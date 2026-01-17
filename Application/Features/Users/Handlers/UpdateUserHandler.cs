using Application.Commons;
using Application.Exceptions;
using Application.Features.Users.Commands;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Users.Handlers;

public class UpdateUserHandler(IUserRepository repository) : IRequestHandler<UpdateUserCommand, ApiResponse<bool>>
{
    private readonly IUserRepository _repository = repository;
    public async Task<ApiResponse<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var newUser = request.UpdateUser;
        var user = await _repository.GetByIdAsync(newUser.Id, true) ??
            throw new ApiException("User not found");

        user.Username = newUser.Username ?? user.Username;
        user.Email = newUser.Email ?? user.Email;
        user.Role = newUser.Role;

        if (!string.IsNullOrEmpty(newUser.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
        else
            throw new ApiException("Password cannot be empty");

        await _repository.UpdateAsync(user);

        return new ApiResponse<bool> { Data = true };
    }
}
