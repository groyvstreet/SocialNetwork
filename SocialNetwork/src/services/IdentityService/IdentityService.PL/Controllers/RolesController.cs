using IdentityService.BLL.DTOs.RoleDTOs;
using IdentityService.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService roleService;

        public RolesController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRolesAsync()
        {
            var roles = await roleService.GetRolesAsync();

            return Ok(roles);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetRoleByIdAsync(string id)
        {
            var role = await roleService.GetRoleByIdAsync(id);

            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> AddRoleAsync(AddRoleDTO addRoleDTO)
        {
            var role = await roleService.AddRoleAsync(addRoleDTO);

            return Ok(role);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRoleAsync(UpdateRoleDTO updateRoleDTO)
        {
            var role = await roleService.UpdateRoleAsync(updateRoleDTO);

            return Ok(role);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> RemoveRoleByIdAsync(string id)
        {
            await roleService.RemoveRoleByIdAsync(id);

            return Ok();
        }
    }
}
