using Bogus;
using PostService.Domain.Entities;

namespace PostServiceIntegrationTests.FakeDataGenerators
{
    public class FakeCommentLikesGenerator
    {
        public readonly List<CommentLike> CommentLikes = new();

        private readonly int _amountOfCommentLikes = 1;

        public void InitializeData(List<User> users, List<Comment> comments)
        {
            foreach (var user in users)
            {
                foreach (var comment in comments)
                {
                    var CommentLikeGenerator = GetCommentLikeGenerator(user.Id, comment.Id);
                    var fakeCommentLikes = CommentLikeGenerator.Generate(_amountOfCommentLikes);
                    CommentLikes.AddRange(fakeCommentLikes);
                }
            }
        }

        private Faker<CommentLike> GetCommentLikeGenerator(Guid userId, Guid commentId)
        {
            return new Faker<CommentLike>()
                .RuleFor(post => post.Id, _ => Guid.NewGuid())
                .RuleFor(post => post.UserId, _ => userId)
                .RuleFor(post => post.CommentId, _ => commentId);
        }
    }
}
