using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using RateMe.DataUtils.LocalDbModels;

namespace RateMe
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            SetProjectDirectory();
            using SubjectsContext db = new SubjectsContext();
            db.Database.Migrate();
        }
        
        private static void SetProjectDirectory()
        {
            string defaultPath = Directory.GetCurrentDirectory();
            string[] defaultPathArr = defaultPath.Split(System.IO.Path.DirectorySeparatorChar);
            string projectPath = string.Join(System.IO.Path.DirectorySeparatorChar, defaultPathArr[..^3]);
            Directory.SetCurrentDirectory(projectPath);
        }
    }
}
