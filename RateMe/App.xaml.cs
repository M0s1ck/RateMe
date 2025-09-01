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
    protected async override void OnStartup(StartupEventArgs e)
    {
        Console.WriteLine("RateMe App");
        base.OnStartup(e);
            
        SetProjectDirectory();
        
        Console.WriteLine("Starting SQLite migrations");

        await using SubjectsContext db = new();
        await db.Database.MigrateAsync();
        
        Console.WriteLine("Migrations were applied");
            
        Config config = JsonFileHelper.GetConfig();
        await BuildNextWin(config);
    }

    private static async Task BuildNextWin(Config? config)
    {
        BaseClient client = new();
        PictureClient picClient = new();
        
        Task<bool> remoteAliveTask = client.IsRemoteAlive();
        Task<bool> s3ServiceAliveTask = picClient.IsS3ServiceAlive();

        await Task.WhenAll(remoteAliveTask, s3ServiceAliveTask);
        
        bool isRemoteAlive = await remoteAliveTask;
        bool isS3ServiceAlive = await s3ServiceAliveTask;
        
        ObservableCollection<Subject> subjects = [];
        SubjectsService subjService = new(subjects, isRemoteAlive);
        ElementsService elemService = new(subjects, isRemoteAlive);
        
        PictureService picService = new(picClient, isS3ServiceAlive);

        UserService userService = new(subjService, elemService, picService, isRemoteAlive);
        
        if (config == null || !config.IsSubjectsLoaded)
        {
            DataCollection dataCollectionWin = new(isRemoteAlive, isS3ServiceAlive);
            dataCollectionWin.Show();
        }
        else
        {
            GradesWin gradesWin = new(subjects, subjService, elemService, userService, picService);
            gradesWin.Show();
        }
    }
        
    private static void SetProjectDirectory()
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