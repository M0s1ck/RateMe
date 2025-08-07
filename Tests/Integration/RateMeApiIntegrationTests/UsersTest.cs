using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using RateMeShared.Dto;
using Xunit;

namespace RateMeApiIntegrationTests;

public class UsersTest
{
    private static string _uriPath = "http://localhost:8080/api/users/"; // 5200 for dev, 8080 for docker
    private HttpClient _client = new HttpClient() { BaseAddress = new Uri(_uriPath) };
    
    private static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task AuthPipeline()
    {
        //Arrange
        const string email = "tobe@added";
        const string pass = "pass";
        const string name = "name";
        const string surname = "surname";

        UserDto usr = new UserDto()
        {
            Email = email,
            Password = pass,
            Name = name,
            Surname = surname,
        };
        
        // Act
        HttpResponseMessage createdResponse = await _client.PostAsJsonAsync("signup", usr);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        
        using JsonDocument json = await JsonDocument.ParseAsync(await createdResponse.Content.ReadAsStreamAsync());
        int userId = json.RootElement.GetProperty("id").GetInt32();
        
        // Act 2
        using HttpResponseMessage getResponse = await _client.GetAsync(userId.ToString());
        
        // Assert 2
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        
        string getMsg = await getResponse.Content.ReadAsStringAsync();
        UserFullDto? receivedUser = JsonSerializer.Deserialize<UserFullDto>(getMsg, options: CaseInsensitiveOptions);
        
        Assert.NotNull(receivedUser);
        
        Assert.Equal(receivedUser.Email, usr.Email);
        Assert.Equal(receivedUser.Password, usr.Password);
        Assert.Equal(receivedUser.Name, usr.Name);
        Assert.Equal(receivedUser.Surname, usr.Surname);
        
        // Extending existing user 
        
        // Arrange 2.5
        UserFullDto user = new UserFullDto() 
        {
            Id = userId,
            Email = email,
            Password = pass,
            Name = name,
            Surname = surname,
            Curriculum = "lala",
            Quote = "Hello there",
            Year = 2028
        };
        
        // Act 2.5
        using HttpResponseMessage extendResponse = await _client.PatchAsJsonAsync(userId.ToString(), user);
        
        // Assert 2.5
        Assert.Equal(HttpStatusCode.OK, extendResponse.StatusCode);
        
        
        // Act 2.69
        using HttpResponseMessage getExtendedResponse = await _client.GetAsync(userId.ToString());
        
        // Assert 2.69
        Assert.Equal(HttpStatusCode.OK, getExtendedResponse.StatusCode);
        
        string extendedMsg = await getExtendedResponse.Content.ReadAsStringAsync();
        UserFullDto? receivedFullUser = JsonSerializer.Deserialize<UserFullDto>(extendedMsg, options: CaseInsensitiveOptions);
        
        Assert.NotNull(receivedFullUser);
        
        Assert.Equal(user.Email, receivedFullUser.Email);
        Assert.Equal(user.Password, receivedFullUser.Password);
        Assert.Equal(user.Name, receivedFullUser.Name);
        Assert.Equal(user.Surname, receivedFullUser.Surname);
        Assert.Equal(user.Year, receivedFullUser.Year);
        Assert.Equal(user.Curriculum, receivedFullUser.Curriculum);
        Assert.Equal(user.Quote, receivedFullUser.Quote);
        
        
        // Arrange 3
        AuthRequest authGoodRequest = new AuthRequest()
        {
            Email = email,
            Password = pass
        };
        
        // Act 3
        using HttpResponseMessage response = await _client.PostAsJsonAsync("auth", authGoodRequest);
        
        // Assert 3
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        string authMsg = await response.Content.ReadAsStringAsync();
        UserFullDto? authedUser = JsonSerializer.Deserialize<UserFullDto>(authMsg, options: CaseInsensitiveOptions);
        
        Assert.NotNull(authedUser);
        
        Assert.Equal(user.Email, authedUser.Email);
        Assert.Equal(user.Password, authedUser.Password);
        Assert.Equal(user.Name, authedUser.Name);
        Assert.Equal(user.Surname, authedUser.Surname);
        Assert.Equal(user.Curriculum, authedUser.Curriculum);
        Assert.Equal(user.Quote, authedUser.Quote);
        Assert.Equal(user.Year, authedUser.Year);
        
        
        // Arrange 4
        const string newEmail = "newEmail";
        const string newPass = "newPass";
        
        UserFullDto updatedUser = new UserFullDto() 
        {
            Id = userId,
            Email = newEmail,
            Password = newPass,
            Name = "name1",
            Surname = "surname1",
            Curriculum = "lala1",
            Quote = "Hello there1",
            Year = 2029
        };
        
        // Act 4
        using HttpResponseMessage updResponse = await _client.PatchAsJsonAsync(userId.ToString(), updatedUser);
        
        // Assert 4
        Assert.Equal(HttpStatusCode.OK, updResponse.StatusCode);
        
        
        // Act 5
        using HttpResponseMessage getUpdResponse = await _client.GetAsync(userId.ToString());
        
        // Assert 5
        Assert.Equal(HttpStatusCode.OK, getUpdResponse.StatusCode);
        
        string getUpdMsg = await getUpdResponse.Content.ReadAsStringAsync();
        UserFullDto? receivedUpdUser = JsonSerializer.Deserialize<UserFullDto>(getUpdMsg, options: CaseInsensitiveOptions);
        
        Assert.NotNull(receivedUpdUser);
        
        Assert.Equal(updatedUser.Email, receivedUpdUser.Email);
        Assert.Equal(updatedUser.Password, receivedUpdUser.Password);
        Assert.Equal(updatedUser.Name, receivedUpdUser.Name);
        Assert.Equal(updatedUser.Surname, receivedUpdUser.Surname);
        Assert.Equal(updatedUser.Curriculum, receivedUpdUser.Curriculum);
        Assert.Equal(updatedUser.Quote, receivedUpdUser.Quote);
        Assert.Equal(updatedUser.Year, receivedUpdUser.Year);
        
        
        // Arrange 6
        AuthRequest authNewGoodRequest = new AuthRequest()
        {
            Email = newEmail,
            Password = newPass
        };
        
        // Act 6
        using HttpResponseMessage newAuthResponse = await _client.PostAsJsonAsync("auth", authNewGoodRequest);
        
        // Assert 6
        Assert.Equal(HttpStatusCode.OK, newAuthResponse.StatusCode);
        
        
        // Act 7
        using HttpResponseMessage deleteResponse = await _client.DeleteAsync(userId.ToString());
        
        // Assert 7
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    
    [Fact]
    public async Task Auth_ReturnsExpected()
    {
        // Arrange: sign up user
        const string email = "email@added";
        const string pass = "pass";
        const string name = "name";
        const string surname = "surname";

        UserDto usr = new UserDto()
        {
            Email = email,
            Password = pass,
            Name = name,
            Surname = surname,
        };
        
        HttpResponseMessage createdResponse = await _client.PostAsJsonAsync("signup", usr);
        
        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        
        using JsonDocument json = await JsonDocument.ParseAsync(await createdResponse.Content.ReadAsStreamAsync());
        int userId = json.RootElement.GetProperty("id").GetInt32();
        
        
        // Test wrong pass 
        AuthRequest authWrongPassRequest = new AuthRequest()
        {
            Email = email,
            Password = "wrong"
        };
        
        using HttpResponseMessage wrongPassResponse = await _client.PostAsJsonAsync("auth", authWrongPassRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, wrongPassResponse.StatusCode);
        
        
        // Test email taken when sign up
        UserDto anotherUser = new UserDto()
        {
            Email = email,
            Password = "somePass",
            Name = "name1",
            Surname = "surname1",
        };
        
        HttpResponseMessage emailTakenResponse = await _client.PostAsJsonAsync("signup", anotherUser);
        
        Assert.Equal(HttpStatusCode.Conflict, emailTakenResponse.StatusCode);
        
        // Remove created user
        using HttpResponseMessage deleteResponse = await _client.DeleteAsync(userId.ToString());
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task Get_NotFound()
    {
        const int nonExistingId = 1_000_000;
        using HttpResponseMessage extendResponse = await _client.GetAsync(nonExistingId.ToString());
        Assert.Equal(HttpStatusCode.NotFound, extendResponse.StatusCode);
    }
    
    [Fact]
    public async Task Auth_NotFound() // Failing test to check CI
    {
        const string nonExistingEmail = "lalalalala@gmail.com";
        
        AuthRequest authNonExistingEmailRequest = new AuthRequest()
        {
            Email = nonExistingEmail,
            Password = "pass"
        };
        
        using HttpResponseMessage wrongPassResponse = await _client.PostAsJsonAsync("auth", authNonExistingEmailRequest);

        Assert.Equal(HttpStatusCode.OK, wrongPassResponse.StatusCode);
    }
}