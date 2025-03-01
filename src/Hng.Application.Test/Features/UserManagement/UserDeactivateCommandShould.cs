using System;
using System.Threading;
using System.Threading.Tasks;
using Hng.Application.Features.UserManagement.Commands;
using Hng.Application.Features.UserManagement.Handlers;
using Hng.Domain.Entities;
using Hng.Infrastructure.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class UserDeactivateCommandHandlerTests
{
    private readonly Mock<IRepository<User>> _userRepositoryMock;
    private readonly Mock<ILogger<UserDeactivateCommandHandler>> _loggerMock;
    private readonly UserDeactivateCommandHandler _handler;

    public UserDeactivateCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IRepository<User>>();
        _loggerMock = new Mock<ILogger<UserDeactivateCommandHandler>>();
        _handler = new UserDeactivateCommandHandler(_userRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_UserExists_ShouldDeactivateSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var user = new User { Id = userId, IsActive = true };

        _userRepositoryMock.Setup(repo => repo.GetAsync(userId))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(repo => repo.UpdateAsync(user))
            .Returns(Task.CompletedTask);
        _userRepositoryMock.Setup(repo => repo.SaveChanges())
            .Returns(Task.CompletedTask);

        var command = new UserDeactivateCommand(userId, null, requesterId);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.Equal("User deactivated successfully", response.Message);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        Assert.False(user.IsActive);
        _userRepositoryMock.Verify(repo => repo.UpdateAsync(user), Times.Once);
        _userRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();

        _userRepositoryMock.Setup(repo => repo.GetAsync(userId))
            .ReturnsAsync((User)null);

        var command = new UserDeactivateCommand(userId, null, requesterId);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("User not found", response.Message);
        Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Handle_ExceptionThrown_ShouldReturnInternalServerError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();

        _userRepositoryMock.Setup(repo => repo.GetAsync(userId))
            .ThrowsAsync(new Exception("Database error"));

        var command = new UserDeactivateCommand(userId, null, requesterId);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("An error occurred while processing your request", response.Message);
        Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
    }
}
