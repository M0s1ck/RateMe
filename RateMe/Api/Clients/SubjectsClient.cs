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
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            SubjectsIds? subjIds = JsonSerializer.Deserialize<SubjectsIds>(content, options: CaseInsensitiveOptions);
            return subjIds?.Subjects;
        }
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            MessageBox.Show(content);
            return null;
        }
        
        MessageBox.Show($"Failed to push subjects to remote bd: {content}");
        return null;
    }
        
        
}