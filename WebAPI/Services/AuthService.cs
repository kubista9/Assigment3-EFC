using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Models;

namespace WebAPI.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _client = new();
    private readonly IUserDao _userDao;
    public Action<ClaimsPrincipal> OnAuthStateChanged { get; set; } = null!;
    public static string? Jwt { get; private set; } = "";

    public AuthService(IUserDao userDao)
    {
	    _userDao = userDao;
    }

    public async Task<User> ValidateUser(string username, string password)
    {

	    User? user = await _userDao.GetByUsernameAsync(username);

	    if (username == null)
	    {
		    throw new Exception("User not found");
	    }

	    if (!user.Password.Equals(password))
	    {
		    throw new Exception("Password doesn't match");
	    }

	    return user;
    }

    public Task RegisterUser(User user)
    {
        if (string.IsNullOrEmpty(user.Username))
        {
            throw new ValidationException("Username cannot be null");
        }

        if (string.IsNullOrEmpty(user.Password))
        {
            throw new ValidationException("Password cannot be null");
        }

        return Task.CompletedTask;
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        string payload = jwt.Split('.')[1];
        byte[] jsonBytes = ParseBase64WithoutPadding(payload);
        Dictionary<string, object>? keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }
    /*NOT USING */
    public async Task LoginAsync(string? username, string? password)
    {
        UserCreationDto userCreationDto = new(username, password)
        {
            Username = username,
            Password = password
        };

        string userAsJson = JsonSerializer.Serialize(userCreationDto);
        StringContent content = new(userAsJson, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _client.PostAsync("http://localhost:5162/auth/login", content);
        string responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseContent);
        }

        string token = responseContent;
        Jwt = token;
        ClaimsPrincipal principal = CreateClaimsPrincipal();
        OnAuthStateChanged.Invoke(principal);
    }

    private static ClaimsPrincipal CreateClaimsPrincipal()
    {
        if (string.IsNullOrEmpty(Jwt))
        {
            return new ClaimsPrincipal();
        }

        IEnumerable<Claim> claims = ParseClaimsFromJwt(Jwt);
        ClaimsIdentity identity = new(claims, "jwt");
        ClaimsPrincipal principal = new(identity);
        return principal;
    }
}
