using Hng.Application.Features.UserManagement.Commands;
using Hng.Domain.Entities;
using Hng.Infrastructure.Repository.Interface;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hng.Application.Features.UserManagement.Handlers
{
    public class UserDeactivateCommandHandler : IRequestHandler<UserDeactivateCommand, UserDeactivateResponse>
    {
        private readonly IRepository<User> _userRepository;

        private readonly ILogger<UserDeactivateCommandHandler> _logger;

        public UserDeactivateCommandHandler(IRepository<User> userRepository, ILogger<UserDeactivateCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserDeactivateResponse> Handle(UserDeactivateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Step 1: Check if the user exists
                var user = await _userRepository.GetAsync(request.UserId);

                if (user == null)
                {
                    return new UserDeactivateResponse
                    {
                        Message = "User not found",
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                // ✅ Step 3: Mark the user as inactive
                user.IsActive = false;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChanges();

                _logger.LogInformation($"User {user.Id} has been deactivated by {request.RequesterId}");

                return new UserDeactivateResponse
                {
                    Message = "User deactivated successfully",
                    Success = true,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deactivating user");
                return new UserDeactivateResponse
                {
                    Message = "An error occurred while processing your request",
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }

}
