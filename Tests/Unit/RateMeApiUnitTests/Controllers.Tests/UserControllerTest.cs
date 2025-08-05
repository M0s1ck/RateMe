using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RateMeApiServer.Common;
using RateMeApiServer.Controllers;
using RateMeApiServer.Services;
using RateMeShared.Dto;
using Xunit;

namespace RateMeApiUnitTests.Controllers.Tests;

public class UserControllerTest
{
    [Theory]
    [InlineData(DbInteractionStatus.NotFound, typeof(NotFoundObjectResult))]
    [InlineData(DbInteractionStatus.WrongData, typeof(UnauthorizedObjectResult))]
    [InlineData(DbInteractionStatus.Success, typeof(OkObjectResult))]
    public async Task SignIn_ReturnsExpected(DbInteractionStatus dbStatus, Type expectedResType)
    {
        var mockService = new Mock<IUserService>();
        mockService.Setup(s => s.AuthUserAsync(It.IsAny<AuthRequest>()))
            .ReturnsAsync(new DbInteractionResult<UserFullDto>(null, dbStatus));
        
        UsersController userController = new UsersController(mockService.Object);
        AuthRequest request = new AuthRequest() { Email = "email", Password = "wrong" };
        
        // Act
        IActionResult result = await userController.SignIn(request);
        
        // Assert
        result.Should().BeOfType(expectedResType);
    }
    
    [Fact]
    public async Task SignIn_Ok()
    {
        // Arrange
        UserFullDto responseDto = new UserFullDto()
        {
            Email = "email", 
            Password = "pass" // others by default
        };
        
        var mockService = new Mock<IUserService>();
        mockService.Setup(s => s.AuthUserAsync(It.IsAny<AuthRequest>()))
            .ReturnsAsync(new DbInteractionResult<UserFullDto>(responseDto, DbInteractionStatus.Success));
        
        UsersController userController = new UsersController(mockService.Object);
        AuthRequest request = new AuthRequest() { Email = "email", Password = "pass" };
        
        // Act
        IActionResult result = await userController.SignIn(request);
        
        // Assert
        result.Should().BeOfType<OkObjectResult>();
        OkObjectResult? ok = result as OkObjectResult;
        ok!.Value.Should().BeEquivalentTo(responseDto);
    }

    [Fact]
    public async Task SignUp_Fail()  // To check if it fails in CI
    {
        UserDto existingUser = new UserDto()
        {
            Email = "exists@me",
            Password = "pass"
        };
        
        var mockService = new Mock<IUserService>();
        mockService.Setup(s => s.AddUserAsync(It.IsAny<UserDto>()))
            .ReturnsAsync(new DbInteractionResult<int>(0, DbInteractionStatus.Conflict));
        
        UsersController userController = new UsersController(mockService.Object);
        
        // Act
        IActionResult result = await userController.SignUp(existingUser);
        
        // Assert
        result.Should().BeOfType<OkObjectResult>();  // Here it fails
    }
}