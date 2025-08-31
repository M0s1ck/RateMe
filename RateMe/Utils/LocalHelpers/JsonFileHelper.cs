using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows;
using RateMe.Models.ClientModels;
using RateMe.Models.JsonFileModels;

namespace RateMe.Utils.LocalHelpers;

public static class JsonFileHelper
{
    #region static consts
    private static readonly string DataDir = Directory.GetCurrentDirectory();
    private static readonly string SyllabusJsonPath = Path.Combine(DataDir, "syllabus.json");
    private static readonly string ConfigJsonPath = Path.Combine(DataDir, "config.json");
    private static readonly string UserJsonPath = Path.Combine(DataDir, "user.json");
    private static readonly JsonSerializerOptions JsonOptions = new()
    { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic) };
    private const string ApiLocalHost = "http://localhost:8080";
    private const string S3ServLocalHost = "http://localhost:8800";
    #endregion

    public static SyllabusModel GetSyllabus()
    {
        if (!File.Exists(SyllabusJsonPath))
        {
            SyllabusModel defaultSm = new SyllabusModel();
            WriteSyllabus(defaultSm);
            return defaultSm;
        }
        
        string jsonContent = File.ReadAllText(SyllabusJsonPath);
        SyllabusModel? syllabus = JsonSerializer.Deserialize<SyllabusModel>(jsonContent);

        if (syllabus == null)
        {
            MessageBox.Show("Couldn't deserialize Data\\syllabus.json");
            return new SyllabusModel();
        }

        return syllabus;
    }


    public static Config GetConfig()
    {
        string jsonContent = File.ReadAllText(ConfigJsonPath);
        Config? config = JsonSerializer.Deserialize<Config>(jsonContent);

        if (config == null)
        {
            throw new IOException("Couldn't deserialize Data\\config.json");
        }

        return config;
    }

    public static void WriteDefaultConfig()
    {
        if (File.Exists(ConfigJsonPath)) 
        {
            return;
        }
        
        Config defaultConf = new Config() { IsSubjectsLoaded = true, ApiUrl = ApiLocalHost, S3Url = S3ServLocalHost };
        SaveConfig(defaultConf);
    }


    public static User? GetUserOrNull()
    {
        if (!File.Exists(UserJsonPath))
        {
            return null;
        }
        
        string jsonContent = File.ReadAllText(UserJsonPath);
        User? user = JsonSerializer.Deserialize<User>(jsonContent);

        if (user == null)
        {
            throw new IOException("Couldn't deserialize Data\\user.json");
        }

        return user;
    }
    

    public static void SaveSyllabus(SyllabusModel syllabus)
    {
        string jsonString = JsonSerializer.Serialize(syllabus, JsonOptions);
        File.WriteAllTextAsync(SyllabusJsonPath, jsonString);
    }

    public static void SaveConfig(Config config)
    {
        string jsonString = JsonSerializer.Serialize(config, JsonOptions);
        File.WriteAllTextAsync(ConfigJsonPath, jsonString);
    }

    public static void SaveUser(User user)
    {
        string jsonString = JsonSerializer.Serialize(user, JsonOptions);
        File.WriteAllTextAsync(UserJsonPath, jsonString);
    }

    public static void RemoveUser()
    {
        File.Delete(UserJsonPath);
    }
    
    private static void WriteSyllabus(SyllabusModel sm)
    {
        string jsonString = JsonSerializer.Serialize(sm, JsonOptions);
        File.WriteAllTextAsync(SyllabusJsonPath, jsonString);
    }
}