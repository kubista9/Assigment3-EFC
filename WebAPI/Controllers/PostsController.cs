using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostLogic _postLogic;

    public PostsController(IPostLogic postLogic)
    {
        _postLogic = postLogic;
    }

    [HttpPost]
    public async Task<ActionResult<Post>> CreateAsync([FromBody] PostCreationDto dto)
    {
        try
        {
            Post post = await _postLogic.CreateAsync(dto);
            return Created($"/posts/{post.Id}", $"Post with title {post.Title} from {post.Owner.Username}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> GetAsync([FromQuery] string? username, int? userId,
        string? titleContains)
    {
        try
        {
            SearchPostParametersDto parameters = new(username, userId, titleContains);
            IEnumerable<Post> posts = await _postLogic.GetAsync(parameters);
            return Ok(posts);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500,e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostBasicDto>> GetById([FromRoute] int id)
    {
	    try
	    {
		    PostBasicDto result = await _postLogic.GetByIdAsync(id);
		    return Ok(result);
	    }
	    catch (Exception e)
	    {
		    Console.WriteLine(e);
		    return StatusCode(500, e.Message);
	    }
    }

    [HttpPatch]
    public async Task<ActionResult> UpdateAsync([FromBody] PostUpdateDto dto)
    {
        try
        {
            await _postLogic.UpdateAsync(dto);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] int id)
    {
        try
        {
            await _postLogic.DeleteAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }


}
