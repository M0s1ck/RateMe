using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using RateMeShared.Dto;

namespace RateMe.Api.MainApi.Clients;

public class SubjectsClient : BaseClient
{
    private const string UrlTemplate = "api/users/{0}/subjects/";

    public SubjectsClient(int userId)
    {
        TheHttpClient = new HttpClient();
        string relativePath = string.Format(UrlTemplate, userId);
        TheHttpClient.BaseAddress = new Uri(BaseUri, relativePath);
    }
    
    
    public async Task<SubjectDto[]?> GetAllSubjects()
    {
        using HttpResponseMessage response = await TheHttpClient.GetAsync(string.Empty);
        string content = await response.Content.ReadAsStringAsync();
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            MessageBox.Show($"Failed to get subjects: {content}");
        }
        
        SubjectDto[]? subjDtos = JsonSerializer.Deserialize<SubjectDto[]>(content, options: CaseInsensitiveOptions);
        return subjDtos;
    }

    
    public async Task<List<SubjectId>?> PushSubjects(SubjectDto[] subjects)
    {
        using HttpResponseMessage response = await TheHttpClient.PostAsJsonAsync(string.Empty, subjects);
        
        string content = await response.Content.ReadAsStringAsync();
        return ReflectOnPushSubjectsResponse(response, content);
    }
    
    
    public async Task UpdateSubjects(List<PlainSubject> dto)
    {
        using HttpResponseMessage response = await TheHttpClient.PutAsJsonAsync(string.Empty, dto);
        string content = await response.Content.ReadAsStringAsync();

        if (response.StatusCode != HttpStatusCode.NoContent)
        {
            MessageBox.Show(content);
        }
    }

    
    public async Task RemoveSubjectsByKeys(List<int> keys)
    {
        using HttpResponseMessage response = await TheHttpClient.PostAsJsonAsync("delete", keys);
        string content = await response.Content.ReadAsStringAsync();

        if (response.StatusCode != HttpStatusCode.NoContent)
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