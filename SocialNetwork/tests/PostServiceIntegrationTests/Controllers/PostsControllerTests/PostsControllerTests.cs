using PostService.Infrastructure.Data;

namespace PostServiceIntegrationTests.Controllers.PostsControllerTests
{
    public abstract class PostsControllerTests : ControllerTests
    {
        protected override void InitializeDatabase(DataContext dataContext)
        {
            _fakeUsersGenerator.InitializeData();
            dataContext.AddRange(_fakeUsersGenerator.Users);

            _fakePostsGenerator.InitializeData(_fakeUsersGenerator.Users);
            dataContext.AddRange(_fakePostsGenerator.Posts);

            _fakeCommentsGenerator.InitializeData(_fakeUsersGenerator.Users, _fakePostsGenerator.Posts);
            dataContext.AddRange(_fakeCommentsGenerator.Comments);

            _fakePostLikesGenerator.InitializeData(_fakeUsersGenerator.Users, _fakePostsGenerator.Posts);
            dataContext.AddRange(_fakePostLikesGenerator.PostLikes);

            dataContext.SaveChanges();
        }
    }
}
