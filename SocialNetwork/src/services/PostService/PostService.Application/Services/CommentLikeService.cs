using AutoMapper;
using PostService.Application.DTOs.CommentLikeDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;

namespace PostService.Application.Services
{
    public class CommentLikeService : ICommentLikeService
    {
        private readonly IMapper _mapper;
        private readonly ICommentLikeRepository _commentLikeRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICacheRepository<Comment> _commentCacheRepository;
        private readonly ICacheRepository<User> _userCacheRepository;
        private readonly ICacheRepository<CommentLike> _commentLikeCacheRepository;

        public CommentLikeService(IMapper mapper,
                                  ICommentLikeRepository commentLikeRepository,
                                  ICommentRepository commentRepository,
                                  IUserRepository userRepository,
                                  ICacheRepository<Comment> commentCacheRepository,
                                  ICacheRepository<User> userCacheRepository,
                                  ICacheRepository<CommentLike> commentLikeCacheRepository)
        {
            _mapper = mapper;
            _commentLikeRepository = commentLikeRepository;
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _userCacheRepository = userCacheRepository;
            _commentCacheRepository = commentCacheRepository;
            _commentLikeCacheRepository = commentLikeCacheRepository;
        }

        public async Task<GetCommentLikeDTO> AddCommentLikeAsync(AddRemoveCommentLikeDTO addCommentLikeDTO, Guid authenticatedUserId)
        {
            if (addCommentLikeDTO.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            var comment = await _commentCacheRepository.GetAsync(addCommentLikeDTO.CommentId.ToString());

            if (comment is null)
            {
                comment = await _commentRepository.GetFirstOrDefaultByAsync(c => c.Id == addCommentLikeDTO.CommentId);

                if (comment is null)
                {
                    throw new NotFoundException($"no such comment with id = {addCommentLikeDTO.CommentId}");
                }
            }
            else
            {
                _commentRepository.Update(comment);
            }

            var user = await _userCacheRepository.GetAsync(addCommentLikeDTO.UserId.ToString());

            if (user is null)
            {
                user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == addCommentLikeDTO.UserId);

                if (user is null)
                {
                    throw new NotFoundException($"no such user with id = {addCommentLikeDTO.UserId}");
                }

                await _userCacheRepository.SetAsync(user.Id.ToString(), user);
            }

            var commentLike = await _commentLikeCacheRepository.GetAsync($"{addCommentLikeDTO.CommentId}&{addCommentLikeDTO.UserId}");

            if (commentLike is null)
            {
                commentLike = await _commentLikeRepository.GetFirstOrDefaultByAsync(cl =>
                    cl.CommentId == addCommentLikeDTO.CommentId && cl.UserId == addCommentLikeDTO.UserId);

                if (commentLike is not null)
                {
                    await _commentLikeCacheRepository.SetAsync($"{addCommentLikeDTO.CommentId}&{addCommentLikeDTO.UserId}", commentLike);

                    throw new AlreadyExistsException($"comment like with commentId = {addCommentLikeDTO.CommentId} and userId = {addCommentLikeDTO.UserId} already exists");
                }
            }
            else
            {
                throw new AlreadyExistsException($"comment like with commentId = {addCommentLikeDTO.CommentId} and userId = {addCommentLikeDTO.UserId} already exists");
            }

            commentLike = _mapper.Map<CommentLike>(addCommentLikeDTO);
            await _commentLikeRepository.AddAsync(commentLike);
            await _commentLikeRepository.SaveChangesAsync();
            var getCommentsUserDTO = _mapper.Map<GetCommentLikeDTO>(commentLike);

            comment.LikeCount++;
            await _commentRepository.SaveChangesAsync();

            await _commentCacheRepository.SetAsync(comment.Id.ToString(), comment);

            return getCommentsUserDTO;
        }

        public async Task RemoveCommentLikeAsync(AddRemoveCommentLikeDTO addRemoveCommentLikeDTO, Guid authenticatedUserId)
        {
            if (addRemoveCommentLikeDTO.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            var commentLike = await _commentLikeCacheRepository.GetAsync($"{addRemoveCommentLikeDTO.CommentId}&{addRemoveCommentLikeDTO.UserId}");

            if (commentLike is null)
            {
                commentLike = await _commentLikeRepository.GetFirstOrDefaultByAsync(cl =>
                    cl.CommentId == addRemoveCommentLikeDTO.CommentId && cl.UserId == addRemoveCommentLikeDTO.UserId);

                if (commentLike is null)
                {
                    throw new NotFoundException($"no such comment like with commentId = {addRemoveCommentLikeDTO.CommentId} and userId = {addRemoveCommentLikeDTO.UserId}");
                }
            }

            _commentLikeRepository.Remove(commentLike);
            await _commentLikeRepository.SaveChangesAsync();

            await _commentLikeCacheRepository.RemoveAsync($"{addRemoveCommentLikeDTO.CommentId}&{addRemoveCommentLikeDTO.UserId}");

            var comment = await _commentCacheRepository.GetAsync(addRemoveCommentLikeDTO.CommentId.ToString());

            if (comment is null)
            {
                comment = await _commentRepository.GetFirstOrDefaultByAsync(c => c.Id == commentLike.CommentId);
            }
            else
            {
                _commentRepository.Update(comment);
            }

            comment!.LikeCount--;
            await _commentRepository.SaveChangesAsync();
        }
    }
}
