using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfcDataAccess.DAOs;

public class PostEfcDao : IPostDao
{
	private readonly PostContext _context;

	public PostEfcDao(PostContext context)
	{
		_context = context;
	}
	public async Task<Post> CreateAsync(Post post)
	{
		EntityEntry<Post> added = await _context.Posts.AddAsync(post);
		await _context.SaveChangesAsync();
		return added.Entity;
	}

	public async Task<IEnumerable<Post>> GetAsync(SearchPostParametersDto searchParams)
	{
		IQueryable<Post> query = _context.Posts.Include(post => post.Owner).AsQueryable();

		if (!string.IsNullOrEmpty(searchParams.Username))
		{
			// we know username is unique, so just fetch the first
			query = query.Where(post => post.Owner.Username.ToLower().Equals(searchParams.Username.ToLower()));
		}
		if (searchParams.UserId != null)
		{
			query = query.Where(post  => post.Owner.Id == searchParams.UserId);
		}
		if (!string.IsNullOrEmpty(searchParams.TitleContains))
		{
			query = query.Where(post => post.Title.ToLower().Contains(searchParams.TitleContains.ToLower()));
		}

		List<Post> result = await query.ToListAsync();
		return result;
	}

	public async Task UpdateAsync(Post post)
	{
		_context.Posts.Update(post);
		await _context.SaveChangesAsync();
	}

	public async Task<Post> GetByIdAsync(int postId)
	{
		Post? found = await _context.Posts
			.Include(post => post.Owner)
			.SingleOrDefaultAsync(post => post.Id == postId);
		return found;
	}

	public async Task DeleteAsync(int id)
	{
		Post? existing = await GetByIdAsync(id);
		if (existing == null)
		{
			throw new Exception($"Post with id {id} not found");
		}

		_context.Posts.Remove(existing);
		await _context.SaveChangesAsync();
	}
}
