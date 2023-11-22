namespace Domain.DTOs;

public class UserCreationDto
{
    public string? Username { get; init; }
    public string? Password { get; init; }

    public UserCreationDto(string? username, string? password)
    {
        Username = username;
        Password = password;
    }
}
