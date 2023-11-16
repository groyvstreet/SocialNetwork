using PostService.Infrastructure.Data;

namespace PostServiceIntegrationTests.Controllers.PostLikesControllerTests
{
    public abstract class PostLikesControllerTests : ControllerTests
    {
        protected override void InitializeDatabase(DataContext dataContext)
        {
            _fakeUsersGenerator.InitializeData();
            dataContext.AddRange(_fakeUsersGenerator.Users);

            _fakePostsGenerator.InitializeData(_fakeUsersGenerator.Users);
            dataContext.AddRange(_fakePostsGenerator.Posts);

            _fakePostLikesGenerator.InitializeData(_fakeUsersGenerator.Users.TakeLast(1).ToList(), _fakePostsGenerator.Posts);
            dataContext.AddRange(_fakePostLikesGenerator.PostLikes);

            dataContext.SaveChanges();
        }
    }
}
