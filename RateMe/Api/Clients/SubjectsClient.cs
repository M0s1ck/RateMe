using System.Net.Http;
using System.Net.Http.Json;
using RateMe.Models.DtoModels;

namespace RateMe.Api.Clients;

public class SubjectsClient : BaseClient
{
    public SubjectsClient() : base() { }

    public async Task PushSubjects(SubjectsByUserId subjects)
    {
        string userIdStr = subjects.UserId.ToString();
        using HttpResponseMessage response = await TheHttpClient.PostAsJsonAsync($"api/User/{userIdStr}", subjects);
    }
        
        
}