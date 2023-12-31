using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfcDataAccess.DAOs;

public class UserEfcDao : IUserDao
{
	private readonly PostContext _context;

	public UserEfcDao(PostContext context)
	{
		_context = context;
	}
	public async Task<User> CreateAsync(User user)
	{
		EntityEntry<User> newUser = await _context.Users.AddAsync(user);
		await _context.SaveChangesAsync();
		return newUser.Entity;
	}

	public async Task<User?> GetByUsernameAsync(string userName)
	{
		User? existing = await _context.Users.FirstOrDefaultAsync(u =>
			u.Username.ToLower().Equals(userName.ToLower())
		);
		return existing;
	}

	public async Task<IEnumerable<User>> GetAsync(SearchUserParametersDto searchParameters)
	{
		IQueryable<User> usersQuery = _context.Users.AsQueryable();
		if (searchParameters.UsernameContains != null)
		{
			usersQuery = usersQuery.Where(u => u.Username.ToLower().Contains(searchParameters.UsernameContains.ToLower()));
		}

		IEnumerable<User> result = await usersQuery.ToListAsync();
		return result;
	}

	public async Task<User?> GetByIdAsync(int id)
	{
		User? user = await _context.Users.FindAsync(id);
		return user;
	}
}
