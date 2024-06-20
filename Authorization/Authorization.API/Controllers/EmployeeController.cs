using System.Security.Claims;
using Authorization.DTO;
using Authorization.DTO.Employees;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.API.Controllers;

[ApiController]
[Route("/[controller]")]
public class EmployeeController : ControllerBase
{
    private UserManager<IdentityUser> _userManager;

    public EmployeeController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<ActionResult> Post(EmployeePostReq req)
    {
        var user = new IdentityUser
        {
            Email = req.Email,
            UserName = req.Email,
        };
        
        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.ToList());

        var roleResult = await _userManager.AddToRoleAsync(user, req.RoleName);
        if (!roleResult.Succeeded)
            return BadRequest(roleResult.Errors.ToList());
        
        var claimResult = await _userManager.AddClaimAsync(user, new Claim("Name", req.Name));
        if (!claimResult.Succeeded)
            return BadRequest(result.Errors.ToList());

        return Created($"/{user.Id}", user.Id);
    }
}