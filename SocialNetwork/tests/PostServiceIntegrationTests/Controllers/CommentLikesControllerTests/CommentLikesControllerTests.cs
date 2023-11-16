using PostService.Infrastructure.Data;

namespace PostServiceIntegrationTests.Controllers.CommentLikesControllerTests
{
    public abstract class CommentLikesControllerTests : ControllerTests
    {
        protected override void InitializeDatabase(DataContext dataContext)
        {
            _fakeUsersGenerator.InitializeData();
            dataContext.AddRange(_fakeUsersGenerator.Users);

            _fakePostsGenerator.InitializeData(_fakeUsersGenerator.Users);
            dataContext.AddRange(_fakePostsGenerator.Posts);

            _fakeCommentsGenerator.InitializeData(_fakeUsersGenerator.Users, _fakePostsGenerator.Posts);
            dataContext.AddRange(_fakeCommentsGenerator.Comments);

            _fakeCommentLikesGenerator.InitializeData(_fakeUsersGenerator.Users.TakeLast(1).ToList(), _fakeCommentsGenerator.Comments);
            dataContext.AddRange(_fakeCommentLikesGenerator.CommentLikes);

            dataContext.SaveChanges();
        }
    }
}
