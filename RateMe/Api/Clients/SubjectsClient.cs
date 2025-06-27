using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using RateMeShared.Dto;

namespace RateMe.Api.Clients;

public class SubjectsClient : BaseClient
{
    public SubjectsClient() : base()
    { }

    public async Task<List<SubjectId>?> PushSubjectsByUserId(SubjectsByUserId subjects)
    {
        string userIdStr = subjects.UserId.ToString();
        using HttpResponseMessage response = await TheHttpClient.PostAsJsonAsync($"api/users/{userIdStr}/subjects", subjects);
        
        string content = await response.Content.ReadAsStringAsync();
        return ReflectOnPushSubjectsResponse(response, content);
    }
    
    public async Task UpdateSubjects(List<PlainSubject> dto)
    {
        using HttpResponseMessage response = await TheHttpClient.PutAsJsonAsync("api/subjects", dto);
        string content = await response.Content.ReadAsStringAsync();
        
        // To be continued...
    }

    public async Task RemoveSubjectsByKeys(List<int> keys)
    {
        using HttpResponseMessage response = await TheHttpClient.PostAsJsonAsync("api/subjects/delete", keys);
        string content = await response.Content.ReadAsStringAsync();

        if (response.StatusCode is not (HttpStatusCode.NoContent or HttpStatusCode.OK))
        {
            MessageBox.Show($"Failed to remove remote data: {content}");
        }
    }
    
    private static List<SubjectId>? ReflectOnPushSubjectsResponse(HttpResponseMessage response, string content)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
            {
                SubjectsIds? subjIds = JsonSerializer.Deserialize<SubjectsIds>(content, options: CaseInsensitiveOptions);
                return subjIds?.Subjects;
            }
            case HttpStatusCode.NotFound:
            {
                MessageBox.Show(content);
                return null;
            }
            default:
            {
                MessageBox.Show($"Failed to push subjects to remote bd: {content}");
                return null;
            }
        }
    }
}