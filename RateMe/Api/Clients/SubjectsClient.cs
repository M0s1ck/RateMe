using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Documents;
using RateMe.Models.ClientModels;
using RateMe.Models.JsonModels;
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

    public async Task RemoveSubjectsByKeys(PlainKeys keysObj)
    {
        using HttpResponseMessage response = await TheHttpClient.PostAsJsonAsync("api/users/0/subjects/delete", keysObj);
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