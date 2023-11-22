namespace Domain.DTOs;

public class PostUpdateDto
{
    public int Id { get; }
    public int OwnerId { get; init; }
    public string? Title { get; init; }
    public string? Body { get; init; }

    public PostUpdateDto(int id)
    {
        Id = id;
    }
}
