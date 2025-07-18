using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using RateMe.Api.MainApi.Clients;
using RateMe.Models.ClientModels;
using RateMe.Models.JsonFileModels;
using RateMe.Repositories;
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
        base.OnStartup(e);
            
        SetProjectDirectory();
            
        using SubjectsContext db = new();
        db.Database.Migrate();
            
        Config config = JsonFileHelper.GetConfig();
        OpenNextWin(config);
    }
        
    private static async void OpenNextWin(Config? config)
    {
        if (config == null || !config.IsSubjectsLoaded)
        {
            DataCollection dataCollectionWin = new DataCollection();
            dataCollectionWin.Show();
            return;
        }

        SyllabusModel syllabus = JsonFileHelper.GetSyllabus();

        BaseClient client = new();
        bool isRemoteAlive = await client.IsRemoteAlive();
        
        GradesWin gradesWin = new(syllabus, isRemoteAlive);
        gradesWin.Show();
    }
        
    private static void SetProjectDirectory()
    {
        string defaultPath = Directory.GetCurrentDirectory();
        string[] defaultPathArr = defaultPath.Split(Path.DirectorySeparatorChar);
        string projectPath = string.Join(Path.DirectorySeparatorChar, defaultPathArr[..^3]);  // TODO: get via while() loop
        Directory.SetCurrentDirectory(projectPath);
    }
}