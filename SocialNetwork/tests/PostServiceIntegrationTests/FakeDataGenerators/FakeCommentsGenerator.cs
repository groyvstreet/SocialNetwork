using Bogus;
using PostService.Domain.Entities;

namespace PostServiceIntegrationTests.FakeDataGenerators
{
    public class FakeCommentsGenerator
    {
        public readonly List<Comment> Comments = new();

        private readonly int _amountOfComments = 10;

        public void InitializeData(List<User> users, List<Post> posts)
        {
            foreach (var user in users)
            {
                foreach (var post in posts)
                {
                    var commentGenerator = GetCommentGenerator(user.Id, post.Id);
                    var fakeComments = commentGenerator.Generate(_amountOfComments);
                    Comments.AddRange(fakeComments);
                }
            }
        }

        private Faker<Comment> GetCommentGenerator(Guid userId, Guid postId)
        {
            return new Faker<Comment>()
                .RuleFor(post => post.Id, _ => Guid.NewGuid())
                .RuleFor(post => post.DateTime, _ => DateTimeOffset.UtcNow)
                .RuleFor(post => post.Text, faker => faker.Lorem.Text())
                .RuleFor(post => post.LikeCount, _ => 10u)
                .RuleFor(post => post.UserId, _ => userId)
                .RuleFor(post => post.PostId, _ => postId);
        }
    }
}
