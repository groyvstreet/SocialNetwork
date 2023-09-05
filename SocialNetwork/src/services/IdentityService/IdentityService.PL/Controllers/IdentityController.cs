using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService identityService;

        public IdentityController(IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        [HttpPost]
        [Route("Signup")]
        public async Task<IActionResult> SignUpAsync(AddUserDTO addUserDTO)
        {
            var user = await identityService.SignUp(addUserDTO);

            return Ok(user);
        }
    }
}
