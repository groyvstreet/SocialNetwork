using AutoMapper;
using IdentityService.BLL.DTOs.RoleDTOs;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.Interfaces;
using IdentityService.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.BLL.Services
{
    public class RoleService : IRoleService
    {
        private readonly IMapper mapper;
        private readonly IRoleRepository roleRepository;

        public RoleService(IMapper mapper,
                           IRoleRepository roleRepository)
        {
            this.mapper = mapper;
            this.roleRepository = roleRepository;
        }

        public async Task<List<GetRoleDTO>> GetRolesAsync()
        {
            var roles = await roleRepository.GetRolesAsync();
            var getRoleDTOs = roles.Select(mapper.Map<GetRoleDTO>).ToList();

            return getRoleDTOs;
        }

        public async Task<GetRoleDTO> GetRoleByIdAsync(string id)
        {
            var role = await roleRepository.GetRoleByIdAsync(id);

            if (role is null)
            {
                throw new NotFoundException($"no such role with id = {id}");
            }

            var getRoleDTO = mapper.Map<GetRoleDTO>(role);

            return getRoleDTO;
        }

        public async Task<GetRoleDTO> GetRoleByNameAsync(string name)
        {
            var role = await roleRepository.GetRoleByNameAsync(name);

            if (role is null)
            {
                throw new NotFoundException($"no such role with name = {name}");
            }

            var getRoleDTO = mapper.Map<GetRoleDTO>(role);

            return getRoleDTO;
        }

        public async Task<GetRoleDTO> AddRoleAsync(AddRoleDTO addRoleDTO)
        {
            addRoleDTO.Name = addRoleDTO.Name.ToLower();
            var role = await roleRepository.GetRoleByNameAsync(addRoleDTO.Name);

            if (role is not null)
            {
                throw new AlreadyExistsException($"role with name = {addRoleDTO.Name} already exists");
            }

            role = mapper.Map<IdentityRole>(addRoleDTO);
            await roleRepository.AddRoleAsync(role);
            var getRoleDTO = mapper.Map<GetRoleDTO>(role);

            return getRoleDTO;
        }

        public async Task<GetRoleDTO> UpdateRoleAsync(UpdateRoleDTO updateRoleDTO)
        {
            updateRoleDTO.Name = updateRoleDTO.Name.ToLower();
            var role = await roleRepository.GetRoleByNameAsync(updateRoleDTO.Name);

            if (role is not null)
            {
                throw new AlreadyExistsException($"role with name = {updateRoleDTO.Name} already exists");
            }

            role = await roleRepository.GetRoleByIdAsync(updateRoleDTO.Id);

            if (role is null)
            {
                throw new NotFoundException($"no such role with id = {updateRoleDTO.Id}");
            }

            role.Name = updateRoleDTO.Name;
            await roleRepository.UpdateRoleAsync(role);
            var getRoleDTO = mapper.Map<GetRoleDTO>(role);

            return getRoleDTO;
        }

        public async Task RemoveRoleByIdAsync(string id)
        {
            var role = await roleRepository.GetRoleByIdAsync(id);

            if (role is null)
            {
                throw new NotFoundException($"no such role with id = {id}");
            }

            await roleRepository.RemoveRoleAsync(role);
        }
    }
}
