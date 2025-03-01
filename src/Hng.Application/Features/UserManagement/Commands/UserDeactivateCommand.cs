using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hng.Application.Features.UserManagement.Commands
{
    public class UserDeactivateCommand : IRequest<UserDeactivateResponse>
    {
        public Guid UserId { get; set; }
        public string? Reason { get; set; }
        public Guid RequesterId { get; set; }  // The user making the request

        public UserDeactivateCommand(Guid userId, string? reason, Guid requesterId)
        {
            UserId = userId;
            Reason = reason;
            RequesterId = requesterId;
        }
    }

    public class UserDeactivateResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public int StatusCode { get; set; }
    }
}
