using Domain.Models;

namespace FileData;

public class DataContainer
{
	public ICollection<User> Users { get; set; } = new List<User>();
	public ICollection<Post> Posts { get; set; } = new List<Post>();
}
