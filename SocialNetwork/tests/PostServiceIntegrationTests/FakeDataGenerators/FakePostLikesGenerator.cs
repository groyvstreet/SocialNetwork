using Bogus;
using PostService.Domain.Entities;

namespace PostServiceIntegrationTests.FakeDataGenerators
{
    public class FakePostLikesGenerator
    {
        public readonly List<PostLike> PostLikes = new();

        private readonly int _amountOfPostLikes = 1;

        public void InitializeData(List<User> users, List<Post> posts)
        {
            foreach (var user in users)
            {
                foreach (var post in posts)
                {
                    var postLikeGenerator = GetPostLikeGenerator(user.Id, post.Id);
                    var fakePostLikes = postLikeGenerator.Generate(_amountOfPostLikes);
                    PostLikes.AddRange(fakePostLikes);
                }
            }
        }

        private Faker<PostLike> GetPostLikeGenerator(Guid userId, Guid postId)
        {
            return new Faker<PostLike>()
                .RuleFor(post => post.Id, _ => Guid.NewGuid())
                .RuleFor(post => post.UserId, _ => userId)
                .RuleFor(post => post.PostId, _ => postId);
        }
    }
}
