using System.Security.Claims;
using Authorization.DTO.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.API.Controllers;

[ApiController]
[Route("/[controller]")]
public class UserController : ControllerBase
{
    private UserManager<IdentityUser> _userManager;

    public UserController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<ActionResult> Post(UserPostReq req)
    {
        var username = req.Email;
        username = username.Split('@')[0];
        var user = new IdentityUser
        {
            Email = req.Email,
            UserName = username
        };
        
        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.ToList());

        var roleResult = await _userManager.AddToRolesAsync(user, req.Roles);
        if (!roleResult.Succeeded)
            return BadRequest(roleResult.Errors.ToList());

        var claims = new List<Claim>
        {
            new Claim("FirstName", req.FirstName),
            new Claim("LastName", req.LastName),
            new Claim("FullName", $"{req.FirstName} {req.LastName}")
        };
        var claimResult = await _userManager.AddClaimsAsync(user, claims);
        if (!claimResult.Succeeded)
            return BadRequest(result.Errors.ToList());

        return Created($"/{user.Id}", user.Id);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult> GetUserByUsername([FromRoute] string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return NotFound($"User {username} not found");
        
        return Ok(user);
    }

    [HttpDelete("{username}")]
    public async Task<ActionResult> DeleteUserByUsername([FromRoute] string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return NotFound($"User {username} not found.");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors.ToList());

        return NoContent();
    }

    [HttpPut("{username}")]
    public async Task<ActionResult> UpdateUserByUsername([FromRoute] string username, UserPutReq req)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return NotFound($"User {username} not found.");

        var claims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        user.Email = req.Email ?? user.Email;
        user.UserName = req.Email != null ? req.Email.Split('@')[0] : user.UserName;
        user.PasswordHash = req.Password != null
            ? _userManager.PasswordHasher.HashPassword(user, req.Password)
            : user.PasswordHash;

        var firstNameClaim = claims.FirstOrDefault(c => c.Type == "FirstName");
        var lastNameClaim = claims.FirstOrDefault(c => c.Type == "LastName");
        var fullNameClaim = claims.FirstOrDefault(c => c.Type == "FullName");

        if (req.FirstName != null)
        {
            if (firstNameClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, firstNameClaim);
            }
            await _userManager.AddClaimAsync(user, new Claim("FirstName", req.FirstName));
        }

        if (req.LastName != null)
        {
            if (lastNameClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, lastNameClaim);
            }
            await _userManager.AddClaimAsync(user, new Claim("LastName", req.LastName));
        }

        if (req.FirstName != null || req.LastName != null)
        {
            var fullName = $"{req.FirstName ?? firstNameClaim?.Value} {req.LastName ?? lastNameClaim?.Value}".Trim();
            if (fullNameClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, fullNameClaim);
            }
            await _userManager.AddClaimAsync(user, new Claim("FullName", fullName));
        }
        
        if (req.Roles.Any())
        {
            var existingRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in existingRoles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            foreach (var role in req.Roles)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok($"User {username} updated successfully.");
    }
}