using Microsoft.AspNetCore.Mvc;
using PostService.Application.Interfaces.UserProfileInterfaces;

namespace PostService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private readonly IUserProfileService userProfileService;

        public UserProfilesController(IUserProfileService userProfileService)
        {
            this.userProfileService = userProfileService;
        }

        [HttpGet]
        [Route("/api/Comments/{id}/Likes/UserProfiles")]
        public async Task<IActionResult> GetUserProfilesLikedByCommentIdIdAsync(Guid id)
        {
            var userProfiles = await userProfileService.GetUserProfilesLikedByCommentIdAsync(id);

            return Ok(userProfiles);
        }
    }
}
