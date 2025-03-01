using Hng.Application.Features.UserManagement.Commands;
using Hng.Application.Features.UserManagement.Dtos;
using Hng.Application.Features.UserManagement.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Hng.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/users")]
public class UserController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        var query = new GetUserByIdQuery(id);
        var response = await _mediator.Send(query);
        return response is null
            ? NotFound(new
            {
                message = "User not found",
                is_successful = false,
                status_code = 404
            })
            : Ok(response);
    }

    [HttpGet("")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _mediator.Send(new GetUsersQuery());
        return Ok(users);
    }

    [HttpPut("organisations/{organisationId:guid}")]
    public async Task<IActionResult> SwitchUserOrganisation(
        Guid organisationId,
        [FromBody] SwitchOrganisationRequestDto request)
    {
        var command = new SwitchOrganisationCommand
        {
            OrganisationId = organisationId,
            IsActive = request.IsActive
        };

        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPatch("deactivate/{userId}")]
    [Authorize]
    public async Task<IActionResult> DeactivateUser([FromRoute] Guid userId, [FromBody] DeactivateUserDto request)
    {
        // Extract RequesterId from JWT token claims
        var requesterIdStr = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid")?.Value;

        // Debugging Log
        Console.WriteLine($"RequesterId received from token: {requesterIdStr}");

        // Validate RequesterId
        if (string.IsNullOrEmpty(requesterIdStr) || !Guid.TryParse(requesterIdStr, out Guid requesterId))
        {
            Console.WriteLine("Invalid token or RequesterId is not a valid GUID.");
            return Unauthorized(new { message = "Invalid token or requester ID is not a valid GUID." });
        }

        // Debugging Log Before Command Execution
        Console.WriteLine($"Processing deactivation for UserId: {userId} by RequesterId: {requesterId}");

        // Execute the command
        var command = new UserDeactivateCommand(userId, request.Reason, requesterId);
        var response = await _mediator.Send(command);

        // Debugging Log After Command Execution
        Console.WriteLine($"Deactivation Response: {response.Message}, Success: {response.Success}, StatusCode: {response.StatusCode}");

        return StatusCode(response.StatusCode, response);
    }


    

}



