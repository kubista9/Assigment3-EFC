using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Models;

namespace FileData.DAOs;

public class PostFileDao : IPostDao
{
    private readonly FileContext _context;

    public PostFileDao(FileContext context)
    {
        _context = context;
    }

    public Task<Post> CreateAsync(Post post)
    {
        int id = 1;
        if (_context.Posts.Any())
        {
            id = _context.Posts.Max(p => p.Id);
            id++;
        }

        post.Id = id;
        _context.Posts.Add(post);
        _context.SaveChanges();
        return Task.FromResult(post);
    }

    public Task<IEnumerable<Post>> GetAsync(SearchPostParametersDto searchParameters)
    {
        IEnumerable<Post> posts = _context.Posts.AsEnumerable();
        if (!string.IsNullOrEmpty(searchParameters.Username))
        {
            posts = _context.Posts.Where(post =>
                post.Owner.Username.Equals(searchParameters.Username, StringComparison.OrdinalIgnoreCase));
        }
        if (searchParameters.UserId != null)
        {
            posts = posts.Where(p => p.Owner.Id == searchParameters.UserId);
        }
        if (!string.IsNullOrEmpty(searchParameters.TitleContains))
        {
            posts = posts.Where(p =>
                p.Title.Contains(searchParameters.TitleContains, StringComparison.OrdinalIgnoreCase));
        }
        return Task.FromResult(posts);
    }

    public Task<Post?> GetByIdAsync(int id)
    {
        Post? existing = _context.Posts.FirstOrDefault(p =>
            p.Id == id
        );
        return Task.FromResult(existing);
    }

    public Task UpdateAsync(Post toUpdate)
    {
        Post? existing = _context.Posts.FirstOrDefault(post => post.Id == toUpdate.Id);
        if (existing == null)
        {
            throw new Exception($"PostId {toUpdate.Id} doest exist | UpdateAsync -> PostFileDao");
        }

        _context.Posts.Remove(existing);
        _context.Posts.Add(toUpdate);
        _context.SaveChanges();
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Post? existing = _context.Posts.FirstOrDefault(post => post.Id == id);
        if (existing == null)
        {
            throw new Exception($"PostId {id} doesnt exist | DeleteAsync -> PostFileDao");
        }

        _context.Posts.Remove(existing);
        _context.SaveChanges();
        return Task.CompletedTask;
    }
}
