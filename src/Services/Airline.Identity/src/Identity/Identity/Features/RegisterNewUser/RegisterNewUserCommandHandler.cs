using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Domain;
using BuildingBlocks.EventBus.Messages;
using BuildingBlocks.Outbox;
using Identity.Data;
using Identity.Identity.Dtos;
using Identity.Identity.Exceptions;
using Identity.Identity.Models;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Identity.Features.RegisterNewUser;

public class RegisterNewUserCommandHandler : IRequestHandler<RegisterNewUserCommand, RegisterNewUserResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IdentityContext _identityContext;
    private readonly IEventProcessor _eventProcessor;

    public RegisterNewUserCommandHandler(UserManager<ApplicationUser> userManager,
        IdentityContext identityContext,
        IEventProcessor eventProcessor)
    {
        _userManager = userManager;
        _identityContext = identityContext;
        _eventProcessor = eventProcessor;
    }

    public async Task<RegisterNewUserResponseDto> Handle(RegisterNewUserCommand command,
        CancellationToken cancellationToken)
    {
        var applicationUser = new ApplicationUser
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            UserName = command.Username,
            Email = command.Email,
            PasswordHash = command.Password,
            PassPortNumber = command.PassportNumber
        };

        var identityResult = await _userManager.CreateAsync(applicationUser, command.Password);
        var roleResult = await _userManager.AddToRoleAsync(applicationUser, Constants.Role.User);

        if (identityResult.Succeeded == false)
            throw new RegisterIdentityUserException(string.Join(',', identityResult.Errors.Select(e => e.Description)));

        if (roleResult.Succeeded == false)
            throw new RegisterIdentityUserException(string.Join(',', roleResult.Errors.Select(e => e.Description)));

        await _eventProcessor.PublishAsync(
            new UserCreated(applicationUser.Id, applicationUser.FirstName + " " + applicationUser.LastName,
                applicationUser.PassPortNumber), cancellationToken);

        return new RegisterNewUserResponseDto
        {
            Id = applicationUser.Id,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            Username = applicationUser.UserName,
            PassportNumber = applicationUser.PassPortNumber
        };
    }
}
