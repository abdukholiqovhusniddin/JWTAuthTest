using Application.Commons;
using Application.Exceptions;
using Application.Features.Users.Commands;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Users.Handlers;

public class DeleteUserHandler(IUserRepository repository, IUnitOfWork unitOfWork) 
    : IRequestHandler<DeleteUserCommand, ApiResponse<bool>>
{
    private readonly IUserRepository _repository = repository;

    public async Task<ApiResponse<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id, true) 
            ?? throw new ApiException("User not found");
        user.IsActive = false;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ApiResponse<bool> { Data = true };
    }
}
