using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Resources;
using RateMe.Models.JsonFileModels;

namespace RateMe.Models.InterfaceModels;

public class Curriculums : ObservableCollection<string>
{
    static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonSerializerOptions IndentOptions = new() { WriteIndented = true };

    private const string BuildFilePath = "pack://application:,,,/Assets/curriculums.json";
    private static readonly string RunTimePath = Path.Combine(Directory.GetCurrentDirectory(), "curriculums.json"); 

    public Curriculums()
    {
        AddToCollectionBuildIn();
            
        if (!File.Exists(RunTimePath))
        {
            CurriculumsModel empty = new() { Curriculums = [] };
            string emptyJson = JsonSerializer.Serialize(empty, IndentOptions);
            File.WriteAllText(RunTimePath, emptyJson);
        }

        string runtimeJson = File.ReadAllText(RunTimePath);
        AddToCollectionJsonContent(runtimeJson);
    }

    private void AddToCollectionBuildIn()
    {
        Uri resourceUri = new(BuildFilePath, UriKind.Absolute);
        StreamResourceInfo? resourceInfo = Application.GetResourceStream(resourceUri);
            
        if (resourceInfo == null)
        {
            return;
        }
            
        using StreamReader reader = new StreamReader(resourceInfo.Stream);
        string jsonContent = reader.ReadToEnd();
            
        AddToCollectionJsonContent(jsonContent);
    }

    private void AddToCollectionJsonContent(string jsonContent)
    {
        CurriculumsModel? buildInCursModel = JsonSerializer.Deserialize<CurriculumsModel>(jsonContent, Options);

        if (buildInCursModel == null)
        {
            return;
        } 
            
        foreach (string op in buildInCursModel.Curriculums)
        {
            Add(op);
        }
    }
}