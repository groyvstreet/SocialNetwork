﻿using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Domain.Entities;
using PostService.Infrastructure.Data;

namespace PostService.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly DataContext context;

        public PostRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await context.Posts.AsNoTracking().ToListAsync();
        }

        public async Task<Post?> GetPostByIdAsync(Guid id)
        {
            return await context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Post>> GetPostsByUserIdAsync(Guid userId)
        {
            return await context.Posts.AsNoTracking().Where(p => p.UserId == userId).ToListAsync();
        }

        public async Task<Post> AddPostAsync(Post post)
        {
            context.Posts.Add(post);
            await context.SaveChangesAsync();

            return post;
        }

        public async Task<Post> UpdatePostAsync(Post post)
        {
            context.Posts.Update(post);
            await context.SaveChangesAsync();

            return post;
        }

        public async Task RemovePostAsync(Post post)
        {
            context.Posts.Remove(post);
            await context.SaveChangesAsync();
        }
    }
}