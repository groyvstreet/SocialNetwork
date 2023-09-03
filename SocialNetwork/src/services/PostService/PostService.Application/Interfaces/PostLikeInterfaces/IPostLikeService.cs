using PostService.Application.DTOs.PostLikeDTOs;

namespace PostService.Application.Interfaces.PostLikeInterfaces
{
    public interface IPostLikeService
    {
        Task<GetPostLikeDTO> AddPostLikeAsync(AddRemovePostLikeDTO addRemovePostLikeDTO);

        Task RemovePostLikeAsync(AddRemovePostLikeDTO addRemovePostLikeDTO);
    }
}
