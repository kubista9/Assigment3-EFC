using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Models;

namespace Application.Logic;

public class PostLogic : IPostLogic
{
    private readonly IPostDao _postDao;
    private readonly IUserDao _userDao;

    public PostLogic(IPostDao postDao, IUserDao userDao)
    {
        _postDao = postDao;
        _userDao = userDao;
    }

    public async Task<Post> CreateAsync(PostCreationDto dto)
    {
        User? user = await _userDao.GetByIdAsync(dto.OwnerId);
        if (user == null)
        {
            throw new Exception($"UserId: {dto.OwnerId} doesnt exist | CreateAsync -> PostLogic");
        }

        Post post = new Post(dto.OwnerId, dto.Title, dto.Body);
        ValidatePost(post);
        Post created = await _postDao.CreateAsync(post);
        created.Owner.Password = null;
        return created;
    }

    public Task<IEnumerable<Post>> GetAsync(SearchPostParametersDto searchParameters)
    {
        return _postDao.GetAsync(searchParameters);
    }

    public async Task UpdateAsync(PostUpdateDto dto)
    {
        Post? existing = await _postDao.GetByIdAsync(dto.Id);
        if (existing == null)
        {
            throw new Exception($"PostId {dto.Id} doesnt exist | UpdateAsync -> PostLogic");
        }

        User? user = null;
        if (dto.OwnerId != null)
        {
            user = await _userDao.GetByIdAsync(dto.OwnerId);
            if (user == null)
            {
                throw new Exception($"UserId {dto.OwnerId} doesnt exist | UpdateAsync -> PostLogic");
            }
        }

        Post updated = new(dto.OwnerId, dto.Title, dto.Body)
        {
	        Id = existing.Id
        };
        ValidatePost(updated);
        await _postDao.UpdateAsync(updated);
    }

    public async Task DeleteAsync(int id)
    {
        Post? post = await _postDao.GetByIdAsync(id);
        if (post == null)
        {
            throw new Exception($"PostId {id} doesnt exist | DeleteAsync -> PostLogic");
        }
        await _postDao.DeleteAsync(id);
    }

    public async Task<PostBasicDto> GetByIdAsync(int id)
    {
        Post? post = await _postDao.GetByIdAsync(id);
        if (post == null)
        {
            throw new Exception($"PostId{id} doesnt exist | GetByIdAsync -> PostLogic");
        }

        return new PostBasicDto(post.Id, post.Owner.Username, post.Title, post.Body);
    }

    private void ValidatePost(Post post)
    {
        if (string.IsNullOrEmpty(post.Title))
        {
            throw new Exception("Title cannot be empty.");
        }

        if (string.IsNullOrEmpty(post.Body))
        {
            throw new Exception("Body cannot be empty.");
        }
    }
}
