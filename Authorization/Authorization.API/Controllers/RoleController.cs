using Authorization.DTO.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.API.Controllers;

[ApiController]
[Route("[controller]")]
public class RoleController : ControllerBase
{
    private RoleManager<IdentityRole> _roleManager;

    public RoleController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    [HttpPost]
    public async Task<ActionResult> Post(RolePostReq req)
    {
        var role = new IdentityRole
        {
            Name = req.Name
        };

        if (await _roleManager.RoleExistsAsync(role.Name))
            return BadRequest($"The role {role.Name} already exists.");

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
            return BadRequest(result.Errors.First());

        return Created($"/{role.Id}", role.Id);
    }
}