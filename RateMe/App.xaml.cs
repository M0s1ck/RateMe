using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using RateMe.Api.MainApi.Clients;
using RateMe.Api.S3ServiceApi;
using RateMe.Models.ClientModels;
using RateMe.Models.JsonFileModels;
using RateMe.Repositories;
using RateMe.Services;
using RateMe.Utils.LocalHelpers;
using RateMe.View.Windows;

namespace RateMe;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        Console.WriteLine("RateMe App");
        base.OnStartup(e);
            
        SetProjectDirectory();
        
        Console.WriteLine("Starting SQLite migrations");
        
        using SubjectsContext db = new();
        db.Database.Migrate();
        
        Console.WriteLine("Migrations were applied");
            
        Config config = JsonFileHelper.GetConfig();
        OpenNextWin(config);
    }
        
    private static async void OpenNextWin(Config? config)
    {
        if (config == null || !config.IsSubjectsLoaded)
        {
            DataCollection dataCollectionWin = new();
            dataCollectionWin.Show();
            return;
        }

        GradesWin gradesWin = await BuildGradesWin();
        Console.WriteLine("Opening grades win");
        gradesWin.Show();
    }

    private static async Task<GradesWin> BuildGradesWin()
    {
        BaseClient client = new();
        bool isRemoteAlive = await client.IsRemoteAlive();
        
        ObservableCollection<Subject> subjects = [];
        SubjectsService subjService = new(subjects, isRemoteAlive);
        ElementsService elemService = new(subjects, isRemoteAlive);
        
        PictureClient picClient = new();
        bool isS3ServiceAlive = await picClient.IsS3ServiceAlive();
        PictureService picService = new(picClient, isS3ServiceAlive);

        UserService userService = new(subjService, elemService, picService, isRemoteAlive); 
        
        GradesWin gradesWin = new(subjects, subjService, elemService, userService, picService);
        return gradesWin;
    }
        
    private static void SetProjectDirectory()  // TODO: test in real environment 
    {
        const string dataDirName = "Data";
        string defaultPath = Directory.GetCurrentDirectory();
        string[] dirsArr = defaultPath.Split(Path.DirectorySeparatorChar);

        for (int i = dirsArr.Length - 1; i > 0; --i)
        {
            string dir = string.Join(Path.DirectorySeparatorChar, dirsArr[..(i + 1)]);
            string dataPath = dir + Path.DirectorySeparatorChar + dataDirName;
            
            if (Directory.Exists(dataPath))
            {
                Directory.SetCurrentDirectory(dataPath);
                return;
            }
        }
        
        string newDataDir = defaultPath + Path.DirectorySeparatorChar + dataDirName;
        Directory.CreateDirectory(newDataDir);
        Directory.SetCurrentDirectory(newDataDir);
        JsonFileHelper.WriteDefaultConfig();
    }
}