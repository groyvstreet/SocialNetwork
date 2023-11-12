using Bogus;
using PostService.Domain.Entities;

namespace PostServiceIntegrationTests.FakeDataGenerators
{
    public class FakePostsGenerator
    {
        public readonly List<Post> Posts = new();

        private readonly int _amountOfPosts = 10;

        public void InitializeData(List<User> users)
        {
            foreach (var user in users)
            {
                var postGenerator = GetPostGenerator(user.Id);
                var fakePosts = postGenerator.Generate(_amountOfPosts);
                Posts.AddRange(fakePosts);
            }
        }

        private Faker<Post> GetPostGenerator(Guid userId)
        {
            return new Faker<Post>()
                .RuleFor(post => post.Id, _ => Guid.NewGuid())
                .RuleFor(post => post.DateTime, _ => DateTimeOffset.UtcNow)
                .RuleFor(post => post.Text, faker => faker.Lorem.Text())
                .RuleFor(post => post.CommentCount, _ => 10u)
                .RuleFor(post => post.LikeCount, _ => 10u)
                .RuleFor(post => post.RepostCount, _ => 10u)
                .RuleFor(post => post.UserId, _ => userId);
        }
    }
}
