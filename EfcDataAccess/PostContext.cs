using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess;

public class PostContext : DbContext
{
	public DbSet<User>? Users { get; set; }
	public DbSet<Post>? Posts { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite("Data Source = /Users/jakubkuka/RiderProjects/Assigment3-EFC/EfcDataAccess/Post.db");
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Post>().HasKey(post => post.Id);
		modelBuilder.Entity<User>().HasKey(user => user.Id);
	}
}
