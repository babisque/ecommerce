using System.Security.Claims;
using Authorization.API.Controllers;
using Authorization.DTO.Users;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Authorization.Tests;

public class UserControllerTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userManagerMock = GetMockUserManager<IdentityUser>();
        _controller = new UserController(_userManagerMock.Object);
    }

    [Fact]
    public async void Get_ReturnsOkWithUser()
    {
        // Arrange
        var user = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "email@test.com",
            UserName = "Test123"
        };

        _userManagerMock.Setup(um => um.FindByNameAsync("Test123"))
            .ReturnsAsync(user);
        
        // Act
        var result = await _controller.GetUserByUsername("Test123");
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<IdentityUser>(okResult.Value);
    }

    [Fact]
    public async void Get_NonExistingUserName_ReturnsNotFound()
    {
        // Arrange
        _userManagerMock.Setup(um => um.FindByNameAsync("Test"))
            .ReturnsAsync(new IdentityUser());
        
        // Act
        var result = await _controller.GetUserByUsername("Test");
        
        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async void Post_ValidUser_ReturnsCreatedResult()
    {
        // Arrange
        var req = new UserPostReq
        {
            Email = "test@example.com",
            Password = "Password123!",
            Roles = new List<string> { "User" },
            FirstName = "Foo",
            LastName = "Bar"
        };

        var identityUser = new IdentityUser
        {
            Email = req.Email,
            UserName = "test"
        };

        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(um => um.AddToRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(um => um.AddClaimsAsync(It.IsAny<IdentityUser>(), It.IsAny<IEnumerable<Claim>>()))
            .ReturnsAsync(IdentityResult.Success);
        
        // Act
        var result = await _controller.Post(req);
        
        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        var location = createdResult.Location;

        Assert.StartsWith("/", location);
        Assert.Matches(@"^/[\da-f]{8}-([\da-f]{4}-){3}[\da-f]{12}$", location);
    }

    [Fact]
    public async void Post_CreateUserFailed_ReturnsBadRequest()
    {
        // Arrange
        var req = new UserPostReq
        {
            Email = "test@example.com",
            Password = "Password123!",
            Roles = new List<string> { "User" },
            FirstName = "Foo",
            LastName = "Bar"
        };

        var identityResult = IdentityResult.Failed(new IdentityError { Description = "Error" });

        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(identityResult);

        // Act
        var result = await _controller.Post(req);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<List<IdentityError>>(badRequestResult.Value);
        Assert.Single(errors);
        Assert.Equal("Error", errors[0].Description);
    }
    
    [Fact]
    public async void DeleteUserByUsername_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var username = "nonexistentuser";

        _userManagerMock.Setup(um => um.FindByNameAsync(username))
            .ReturnsAsync((IdentityUser)null);

        // Act
        var result = await _controller.DeleteUserByUsername(username);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"User {username} not found.", notFoundResult.Value);
    }
    
    [Fact]
    public async void DeleteUserByUsername_UserFound_DeletionSucceeds_ReturnsNoContent()
    {
        // Arrange
        var username = "existinguser";
        var user = new IdentityUser { UserName = username };

        _userManagerMock.Setup(um => um.FindByNameAsync(username))
            .ReturnsAsync(user);

        _userManagerMock.Setup(um => um.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.DeleteUserByUsername(username);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
    
    [Fact]
    public async void DeleteUserByUsername_UserFound_DeletionFails_ReturnsBadRequest()
    {
        // Arrange
        var username = "existinguser";
        var user = new IdentityUser { UserName = username };

        var identityResult = IdentityResult.Failed(new IdentityError { Description = "Error" });

        _userManagerMock.Setup(um => um.FindByNameAsync(username))
            .ReturnsAsync(user);

        _userManagerMock.Setup(um => um.DeleteAsync(user))
            .ReturnsAsync(identityResult);

        // Act
        var result = await _controller.DeleteUserByUsername(username);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<List<IdentityError>>(badRequestResult.Value);
        Assert.Single(errors);
        Assert.Equal("Error", errors[0].Description);
    }
    
    [Fact]
    public async void UpdateUserByUsername_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var username = "nonexistentuser";
        var req = new UserPutReq();

        _userManagerMock.Setup(um => um.FindByNameAsync(username))
                        .ReturnsAsync((IdentityUser)null);

        // Act
        var result = await _controller.UpdateUserByUsername(username, req);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"User {username} not found.", notFoundResult.Value);
    }

    [Fact]
    public async void UpdateUserByUsername_SuccessfulUpdate_ReturnsOk()
    {
        // Arrange
        var username = "existinguser";
        var user = new IdentityUser { UserName = username, Email = "old@example.com" };
        var req = new UserPutReq
        {
            Email = "new@example.com",
            Password = "NewPassword123!",
            FirstName = "NewFirstName",
            LastName = "NewLastName",
            Roles = new List<string> { "Admin" }
        };

        var existingClaims = new List<Claim>
        {
            new Claim("FirstName", "OldFirstName"),
            new Claim("LastName", "OldLastName"),
            new Claim("FullName", "OldFirstName OldLastName")
        };

        var existingRoles = new List<string> { "User" };

        // Mock UserManager dependencies
        _userManagerMock.Setup(um => um.FindByNameAsync(username))
            .ReturnsAsync(user);

        _userManagerMock.Setup(um => um.GetClaimsAsync(user))
            .ReturnsAsync(existingClaims);

        _userManagerMock.Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(existingRoles);

        _userManagerMock.Setup(um => um.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Create a mock for IPasswordHasher<IdentityUser>
        var passwordHasherMock = new Mock<IPasswordHasher<IdentityUser>>();

        // Set up the HashPassword method to return the expected hashed password
        passwordHasherMock.Setup(ph => ph.HashPassword(user, req.Password))
            .Returns("HashedNewPassword");

        // Inject the mocked IPasswordHasher into UserManager
        _userManagerMock.Object.PasswordHasher = passwordHasherMock.Object;

        // Act
        var result = await _controller.UpdateUserByUsername(username, req);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal($"User {username} updated successfully.", okResult.Value);
    }
    
    [Fact]
    public async void UpdateUserByUsername_NoChanges_ReturnsOk()
    {
        // Arrange
        var username = "existinguser";
        var user = new IdentityUser { UserName = username, Email = "test@example.com" };
        var req = new UserPutReq(); // No changes provided

        _userManagerMock.Setup(um => um.FindByNameAsync(username))
                        .ReturnsAsync(user);

        _userManagerMock.Setup(um => um.GetClaimsAsync(user))
                        .ReturnsAsync(new List<Claim>());

        _userManagerMock.Setup(um => um.GetRolesAsync(user))
                        .ReturnsAsync(new List<string>());

        _userManagerMock.Setup(um => um.UpdateAsync(user))
                        .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.UpdateUserByUsername(username, req);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal($"User {username} updated successfully.", okResult.Value);
    }

    [Fact]
    public async void UpdateUserByUsername_PartialUpdates_ReturnsOk()
    {
        // Arrange
        var username = "existinguser";
        var user = new IdentityUser { UserName = username, Email = "old@example.com" };
        var req = new UserPutReq
        {
            Email = "new@example.com", // Only changing email
        };

        var existingClaims = new List<Claim>
        {
            new Claim("FirstName", "OldFirstName"),
            new Claim("LastName", "OldLastName"),
            new Claim("FullName", "OldFirstName OldLastName")
        };

        var existingRoles = new List<string> { "User" };

        _userManagerMock.Setup(um => um.FindByNameAsync(username))
                        .ReturnsAsync(user);

        _userManagerMock.Setup(um => um.GetClaimsAsync(user))
                        .ReturnsAsync(existingClaims);

        _userManagerMock.Setup(um => um.GetRolesAsync(user))
                        .ReturnsAsync(existingRoles);

        _userManagerMock.Setup(um => um.UpdateAsync(user))
                        .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.UpdateUserByUsername(username, req);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal($"User {username} updated successfully.", okResult.Value);
    }

    private static Mock<UserManager<TUser>> GetMockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        return mgr;
    }
}