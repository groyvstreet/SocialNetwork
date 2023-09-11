﻿using Microsoft.EntityFrameworkCore;
using PostService.Domain.Entities;

namespace PostService.Infrastructure.Data
{
    public class DataContext : DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<PostsUserProfile> PostsUserProfiles { get; set; }

        public DbSet<CommentsUserProfile> CommentsUserProfiles { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}
