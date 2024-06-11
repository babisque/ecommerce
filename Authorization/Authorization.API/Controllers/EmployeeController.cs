using Authorization.DTO;
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
            UserName = req.Email 
        };
        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
            return BadRequest(result.Errors.First());

        return Created($"/{user.Id}", user.Id);
    }
}