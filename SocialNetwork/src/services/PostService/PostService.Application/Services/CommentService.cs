using PostService.Application.DTOs.CommentDTOs;
using PostService.Application.Interfaces;

namespace PostService.Application.Services
{
    public class CommentService : ICommentService
    {
        public Task<List<GetCommentDTO>> GetCommentsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GetCommentDTO?> GetCommentByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task AddCommentAsync(AddCommentDTO addCommentDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCommentAsync(UpdateCommentDTO updateCommentDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveCommentByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
