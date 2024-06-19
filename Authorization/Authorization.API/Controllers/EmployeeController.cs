using System.Security.Claims;
using Authorization.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.API.Controllers;

[ApiController]
[Route("/[controller]")]
public class EmployeeController : ControllerBase
{
    private UserManager<IdentityUser> _userManager;
    private RoleManager<IdentityRole> _roleManager;

    public EmployeeController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
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
            return BadRequest(result.Errors.First());

        var claimResult = await _userManager.AddClaimAsync(user, new Claim("Name", req.Name));

        if (!claimResult.Succeeded)
            return BadRequest(result.Errors.First());

        return Created($"/{user.Id}", user.Id);
    }
}