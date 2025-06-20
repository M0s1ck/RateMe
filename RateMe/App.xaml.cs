using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using RateMe.Models.ClientModels;
using RateMe.Models.JsonModels;
using RateMe.Repositories;
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
            
        Config config = JsonModelsHandler.GetConfig();
        OpenNextWin(config);
    }
        
    private static void OpenNextWin(Config? config)
    {
        if (config == null || !config.IsSubjectsLoaded)
        {
            DataCollection dataCollectionWin = new DataCollection();
            dataCollectionWin.Show();
            return;
        }

        SyllabusModel syllabus = JsonModelsHandler.GetSyllabus();
        GradesWin gradesWin = new(syllabus);
        gradesWin.Show();
    }
        
    private static void SetProjectDirectory()
    {
        string defaultPath = Directory.GetCurrentDirectory();
        string[] defaultPathArr = defaultPath.Split(Path.DirectorySeparatorChar);
        string projectPath = string.Join(Path.DirectorySeparatorChar, defaultPathArr[..^3]);
        Directory.SetCurrentDirectory(projectPath);
    }
}